window.preventDefaultDragOver = (element) => {
    element.addEventListener('dragover', (event) => {
        event.preventDefault();
    });
};

window.addEventsToFigures = () => {
    document.querySelectorAll('.getItem').forEach(draggableElemnt => {
        draggableElemnt.addEventListener('dragstart', function (event) {
            const svg = draggableElemnt.querySelector('svg');
            const imgX = svg.width.baseVal.value / 2;
            const imgY = svg.height.baseVal.value / 2;
            event.dataTransfer.setDragImage(svg, imgX, imgY);
        });
        
    });
}

window.getDivText = (element) => {
    if (!element) {
        console.warn("Element reference is null in JS!");
        return "";
    }
    return element.innerText || element.textContent;
};

window.triggerBlur = (element) => {
    if (element) {
        element.blur();
    } else {
        console.warn("Element reference is null in JS!");
    }
};


let mouseMoveHandler = null;
let mouseUpHandler = null;

window.addResizeListeners = (dotNetRef) => {
    mouseMoveHandler = (e) => {
        dotNetRef.invokeMethodAsync("HandleResize", e.clientX, e.clientY);
    };
    mouseUpHandler = async () => {
        await dotNetRef.invokeMethodAsync("StopResize");
    };

    document.addEventListener("mousemove", mouseMoveHandler);
    document.addEventListener("mouseup", mouseUpHandler);
};

window.removeResizeListeners = () => {
    if (mouseMoveHandler) {
        document.removeEventListener("mousemove", mouseMoveHandler);
    }
    if (mouseUpHandler) {
        document.removeEventListener("mouseup", mouseUpHandler);
    }
};
