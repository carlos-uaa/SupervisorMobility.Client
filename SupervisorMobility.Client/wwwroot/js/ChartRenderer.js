let chartInstances = {};

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
                            const pieTotal = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((value / pieTotal) * 100).toFixed(1);
                            return `${context.label}: ${value} (${percentage}%)`;
                        }
                    }
                },
                legend: {
                    onClick: null,
                    labels: {
                        generateLabels: function (chart) {
                            const dataset = chart.data.datasets[0];
                            const legendTotal = dataset.data.reduce((a, b) => a + b, 0);
                            return chart.data.labels.map((label, i) => {
                                const value = dataset.data[i];
                                const percentage = ((value / legendTotal) * 100).toFixed(1);
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
                // datalabels: {
                //     color: '#fff',
                //     formatter: (value) => {
                //         const percent = ((value / total) * 100).toFixed(1);
                //         return percent + '%';
                //     },
                //     anchor: 'center',
                //     align: 'center',
                //     offset: 10,
                //     font: {
                //         weight: 'bold',
                //         size: 14
                //     },
                //     display: function (context) {
                //         const value = context.dataset.data[context.dataIndex];
                //         const pieTotal = context.dataset.data.reduce((a, b) => a + b, 0);
                //         const percentage = value / pieTotal;
                //         return percentage >= 0.05;
                //     }
                // }
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

    const total = chartData.total ?? 0;
    const numberFormatter = new Intl.NumberFormat();

    const normalizedData = {
        ...chartData,
        datasets: (chartData.datasets ?? []).map((dataset) => ({
            ...dataset,
            borderSkipped: false,
            borderRadius: 8,
            maxBarThickness: 42,
            categoryPercentage: 0.7,
            barPercentage: 0.9
        }))
    };

    const hoverShadowPlugin = {
        id: 'hoverShadowPlugin',
        afterDatasetsDraw(chart) {
            const activeElements = chart.getActiveElements?.() ?? [];
            if (!activeElements.length) return;

            const chartCtx = chart.ctx;
            chartCtx.save();

            activeElements.forEach((item) => {
                const meta = chart.getDatasetMeta(item.datasetIndex);
                const bar = meta?.data?.[item.index];
                if (!bar) return;

                const { x, y, base, width } = bar.getProps(['x', 'y', 'base', 'width'], true);
                const height = Math.max(base - y, 0);

                chartCtx.shadowColor = 'rgba(15, 23, 42, 0.35)';
                chartCtx.shadowBlur = 16;
                chartCtx.shadowOffsetY = 6;
                chartCtx.fillStyle = 'rgba(0, 0, 0, 0)';
                chartCtx.fillRect(x - width / 2, y, width, height);
            });

            chartCtx.restore();
        }
    };

    chartInstances[canvasId] = new Chart(ctx, {
        type: 'bar',
        data: normalizedData,
        options: {
            responsive: true,
            maintainAspectRatio: true,
            aspectRatio: 1.45,
            resizeDelay: 180,
            animation: {
                duration: 650,
                easing: 'easeOutCubic'
            },
            plugins: {
                legend: {
                    onClick: null,
                    position: 'top',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'circle',
                        boxWidth: 8,
                        boxHeight: 8,
                        padding: 14,
                        generateLabels: function (chart) {
                            return chart.data.datasets.map((dataset, i) => {
                                const datasetTotal = dataset.data.reduce((sum, val) => sum + (Number(val) || 0), 0);
                                return {
                                    text: `${dataset.label} (${numberFormatter.format(datasetTotal)})`,
                                    fillStyle: dataset.backgroundColor,
                                    hidden: !chart.isDatasetVisible(i),
                                    datasetIndex: i
                                };
                            });
                        }
                    }
                },
                title: {
                    display: total > 0,
                    text: `Total: ${numberFormatter.format(total)}`,
                    align: 'end',
                    color: '#475569',
                    font: {
                        size: 12,
                        weight: '600'
                    },
                    padding: {
                        top: 2,
                        bottom: 8
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    displayColors: true,
                    backgroundColor: 'rgba(15, 23, 42, 0.92)',
                    padding: 10,
                    callbacks: {
                        title: function (contexts) {
                            return contexts?.[0]?.label ?? '';
                        },
                        label: function (context) {
                            const value = context.raw;
                            if (!value || value === 0) {
                                return null;
                            }

                            const label = context.dataset.label;
                            return `${label}: ${numberFormatter.format(value)}`;
                        },
                        footer: function (contexts) {
                            const subtotal = contexts.reduce((sum, item) => sum + (Number(item.raw) || 0), 0);
                            return `Subtotal: ${numberFormatter.format(subtotal)}`;
                        }
                    }
                },
                datalabels: {
                    color: '#fff',
                    anchor: 'center',
                    align: 'center',
                    formatter: (value) => {
                        return value > 0 ? numberFormatter.format(value) : '';
                    },
                    font: {
                        weight: '700',
                        size: 11
                    }
                }
            },
            interaction: {
                mode: 'index',
                intersect: false
            },
            scales: {
                x: {
                    stacked: true,
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        color: '#475569',
                        font: {
                            size: 11,
                            weight: '600'
                        }
                    }
                },
                y: {
                    stacked: true,
                    beginAtZero: true,
                    grace: '8%',
                    ticks: {
                        precision: 0,
                        color: '#64748b',
                        callback: function (value) {
                            return numberFormatter.format(value);
                        }
                    },
                    grid: {
                        display: false,
                        color: 'rgba(148, 163, 184, 0.25)',
                        drawBorder: false
                    }
                }
            }
        },
        plugins: [ChartDataLabels, hoverShadowPlugin]
    });
};