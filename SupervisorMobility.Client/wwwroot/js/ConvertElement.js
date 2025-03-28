function captureElementAsImageAndSend(elementId) {
    const element = document.getElementById(elementId).querySelector(".diagram-canvas .diagram-svg-layer");
    if (!element) return null;

    return html2canvas(element).then(canvas => {
        return canvas.toDataURL("image/png"); // Returns base64 string
    });
}

function captureSvgLayerAsImage() {
    const svgLayer = document.querySelector("#CanvContainer .diagram-canvas .diagram-svg-layer");
    if (!svgLayer) return null;

    const clonedSvg = svgLayer.cloneNode(true);
    clonedSvg.querySelectorAll(".flow-port").forEach(el => el.remove());
    clonedSvg.querySelectorAll(".port-hidden").forEach(el => el.remove());

    const serializer = new XMLSerializer();
    const svgString = serializer.serializeToString(clonedSvg);

    let { width, height } = getComputedDimensions(svgLayer);

    const canvas = document.createElement("canvas");
    canvas.width = width;
    canvas.height = height;
    const ctx = canvas.getContext("2d");
    const img = new Image();

    const svgBlob = new Blob([svgString], { type: "image/svg+xml;charset=utf-8" });
    const url = URL.createObjectURL(svgBlob);

    return new Promise((resolve) => {
        img.onload = function () {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.drawImage(img, 0, 0, width, height);
            URL.revokeObjectURL(url);
            resolve(canvas.toDataURL("image/png"));
        };
        img.src = url;
    });
}

function getComputedDimensions(svgElement) {
    const computedStyle = window.getComputedStyle(svgElement);
    const width = parseFloat(computedStyle.width);
    const height = parseFloat(computedStyle.height);

    return { width, height };
}