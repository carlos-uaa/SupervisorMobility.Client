window.preventDefaultDragOver = (element) => {
    element.addEventListener('dragover', (event) => {
        event.preventDefault();
    });
};


window.addEventsToFigures = () => {
    document.querySelectorAll('.data-figure').forEach(draggableElement => {
        // Añade esta línea para prevenir la acción táctil predeterminada
        draggableElement.style.touchAction = 'none';

        draggableElement.addEventListener('dragstart', function (event) {
            const svg = draggableElement.querySelector('svg');
            const imgX = svg.width.baseVal.value / 2;
            const imgY = svg.height.baseVal.value / 2;
            event.dataTransfer.setDragImage(svg, imgX, imgY);
            const figureType = draggableElement.getAttribute('data-figure-type');
            // Almacena el figureType globalmente
            window.draggedFigureType = figureType;
        });

        // Prevenir el comportamiento predeterminado en dispositivos táctiles
        draggableElement.addEventListener('touchstart', function (event) {
            event.preventDefault();
            const touch = event.touches[0];
            const svg = draggableElement.querySelector('svg');
            const imgX = svg.width.baseVal.value / 2;
            const imgY = svg.height.baseVal.value / 2;
            const touchX = touch.clientX - imgX;
            const touchY = touch.clientY - imgY;
            draggableElement.style.position = 'absolute';
            draggableElement.style.left = `${touchX}px`;
            draggableElement.style.top = `${touchY}px`;
        }, { passive: false });

        draggableElement.addEventListener('touchmove', function (event) {
            event.preventDefault();
            const touch = event.touches[0];
            const svg = draggableElement.querySelector('svg');
            const imgX = svg.width.baseVal.value / 2;
            const imgY = svg.height.baseVal.value / 2;
            const touchX = touch.clientX - imgX;
            const touchY = touch.clientY - imgY;
            draggableElement.style.left = `${touchX}px`;
            draggableElement.style.top = `${touchY}px`;
        }, { passive: false });

        draggableElement.addEventListener('touchend', function (event) {
            event.preventDefault();
            // Aquí puedes agregar lógica adicional para manejar el final del arrastre
        }, { passive: false });
    });
};

window.getDraggedFigureType = function () {
    // Asumiendo que has almacenado el tipo de figura durante el evento de 'dragstart'
    return window.draggedFigureType;
};

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
