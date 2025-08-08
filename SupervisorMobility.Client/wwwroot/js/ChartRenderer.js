let chartInstances = {}

window.renderPieChart = (canvasId, chartData) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
    }

    const total = chartData.total ?? chartData.datasets[0].data.reduce((a, b) => a + b, 0);

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'pie',
        data: chartData,
        options: {
            responsive: false,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const value = context.raw;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((value / total) * 100).toFixed(1);
                            return `${context.label}: ${value} (${percentage}%)`;
                        }
                    }
                },
                legend: {
                    onClick: null,
                    labels: {
                        generateLabels: function (chart) {
                            const dataset = chart.data.datasets[0];
                            const total = dataset.data.reduce((a, b) => a + b, 0);
                            return chart.data.labels.map((label, i) => {
                                const value = dataset.data[i];
                                const percentage = ((value / total) * 100).toFixed(1);
                                return {
                                    text: `${label} (${value} - ${percentage}%)`,
                                    fillStyle: dataset.backgroundColor[i],
                                    strokeStyle: dataset.borderColor ? dataset.borderColor[i] : '#fff',
                                    hidden: isNaN(value) || chart.getDatasetMeta(0).data[i].hidden,
                                    index: i
                                };
                            });
                        }
                    }
                },
                datalabels: {
                    color: '#fff',
                    formatter: (value) => {
                        const percent = ((value / total) * 100).toFixed(1);
                        return percent + '%';
                    },
                    anchor: 'center',
                    align: 'center',
                    offset: 10,
                    font: {
                        weight: 'bold',
                        size: 14
                    },
                    display: function (context) {
                        const value = context.dataset.data[context.dataIndex];
                        const total = context.dataset.data.reduce((a, b) => a + b, 0);
                        const percentage = value / total;

                        // Only show label if slice is >= 5% (adjust as needed)
                        return percentage >= 0.05;
                    }
                }
            }
        },
        plugins: [ChartDataLabels]
    });
};

window.renderStackedBarChart = (canvasId, chartData) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
    }
    chartInstances[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: chartData,
        options: {
            responsive: false,
            plugins: {
                legend: {
                    onClick: null,
                    position: 'top',
                    labels: {
                        generateLabels: function (chart) {
                            return chart.data.datasets.map((dataset, i) => {
                                const total = dataset.data.reduce((sum, val) => sum + (Number(val) || 0), 0);
                                return {
                                    text: `${dataset.label} (${total})`,
                                    fillStyle: dataset.backgroundColor,
                                    hidden: !chart.isDatasetVisible(i),
                                    datasetIndex: i
                                };
                            });
                        }
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: function (context) {
                            // Only show if value is non-zero
                            const value = context.raw;

                            // Hide tooltips for values that are 0 or null
                            if (!value || value === 0) {
                                return null;
                            }

                            // Custom label renaming
                            const label = context.dataset.label;
                            return label + ': ' + value;
                        }
                    }
                },
                datalabels: {
                    color: '#fff', // Text color
                    anchor: 'center', // Where it attaches inside the bar
                    align: 'center',  // Position inside the bar
                    formatter: (value) => {
                        // Only show labels if value > 0
                        return value > 0 ? value : '';
                    },
                    font: {
                        weight: 'bold'
                    }
                }
            },
            interaction: {
                mode: 'index',
                intersect: false
            },
            scales: {
                x: {
                    stacked: true
                },
                y: {
                    stacked: true,
                    beginAtZero: true
                }
            }
        }
    });
}