window.initCanvas = (canvasId) => {
    var canvas = document.getElementById(canvasId);
    if (canvas) {
        var context = canvas.getContext('2d');
        context.strokeStyle = 'black';
        context.lineWidth = 5;
    } else {
        console.error("Canvas with the provided ID was not found.");
    }
}

window.draw = (canvasId, offsetX, offsetY, isDrawing, prevX, prevY) => {
    var canvas = document.getElementById(canvasId);
    if (canvas) {
        var context = canvas.getContext('2d');
        if (!isDrawing) {
            prevX = offsetX;
            prevY = offsetY;
            isDrawing = true;
        } else {
            context.beginPath();
            context.moveTo(prevX, prevY);
            context.lineTo(offsetX, offsetY);
            context.stroke();
            prevX = offsetX;
            prevY = offsetY;
        }
    } else {
        console.error("Canvas with the provided ID was not found.");
    }
}

window.endDrawing = () => {
    isDrawing = false;
}

window.clearCanvas = (canvasId) => {
    var canvas = document.getElementById(canvasId);
    if (canvas) {
        var context = canvas.getContext('2d');
        context.clearRect(0, 0, canvas.width, canvas.height);
    } else {
        console.error("Canvas with the provided ID was not found.");
    }
}

window.getOffsetLeft = (canvasId) => {
    var element = document.getElementById(canvasId);
    if (element) {
        return element.offsetLeft;
    } else {
        console.error("Element with the provided ID was not found.");
        return 0;
    }
}

window.getOffsetTop = (canvasId) => {
    var element = document.getElementById(canvasId);
    if (element) {
        return element.offsetTop;
    } else {
        console.error("Element with the provided ID was not found.");
        return 0;
    }
}


window.generateSignature = (canvasId) => {
    var canvas = document.getElementById(canvasId);
    if (canvas) {
        var dataUrl = canvas.toDataURL();
        return dataUrl;
    } else {
        console.error("Canvas with the provided ID was not found.");
        return "";
    }
}