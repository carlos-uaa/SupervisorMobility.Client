let draggedImageId = null;
let draggedImageElement = null;
let initialTouchX = 0;
let initialTouchY = 0;
let previewImageElement = null;

window.setupCanvas = function (canvasRef, dotNetObjectRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");

    canvas.addEventListener("dragover", (e) => e.preventDefault());

    canvas.addEventListener("drop", async function (e) {
        e.preventDefault();
        if (draggedImageElement) {
            const x = e.offsetX;
            const y = e.offsetY;
            ctx.drawImage(draggedImageElement, x, y, draggedImageElement.width / 2, draggedImageElement.height / 2);

            removeSelection(draggedImageElement);
            removePreviewImage();
            draggedImageElement = null;

            await dotNetObjectRef.invokeMethodAsync('OnImageDropped');
        } else {
            console.error("Image element not found");
        }
    });

    canvas.addEventListener("touchstart", function (e) {
        if (e.touches.length === 1 && draggedImageElement) {
            const touch = e.touches[0];
            initialTouchX = touch.clientX;
            initialTouchY = touch.clientY;
        }
    });

    canvas.addEventListener("touchmove", function (e) {
        e.preventDefault();
        if (previewImageElement) {
            const touch = e.touches[0];
            const x = touch.clientX - previewImageElement.width / 2;
            const y = touch.clientY - previewImageElement.height / 2;
            previewImageElement.style.left = `${x}px`;
            previewImageElement.style.top = `${y}px`;
        }
    });

    canvas.addEventListener("touchend", async function (e) {
        if (draggedImageElement) {
            const touch = e.changedTouches[0];
            const rect = canvas.getBoundingClientRect();
            const x = touch.clientX - rect.left;
            const y = touch.clientY - rect.top;
            ctx.drawImage(draggedImageElement, x, y, draggedImageElement.width / 2, draggedImageElement.height / 2);

            removeSelection(draggedImageElement);
            removePreviewImage();
            draggedImageElement = null;

            await dotNetObjectRef.invokeMethodAsync('OnImageDropped');
        }
    });
};

window.onDragStartJs = function (imageId) {
    draggedImageId = imageId;
    draggedImageElement = document.getElementById(imageId);

    if (draggedImageElement) {
        // Añadir borde para indicar selección
        draggedImageElement.style.border = '2px solid blue';

        // Crear imagen de vista previa
        createPreviewImage(draggedImageElement);
    }
};

function createPreviewImage(imageElement) {
    previewImageElement = imageElement.cloneNode(true);
    previewImageElement.style.position = 'absolute';
    previewImageElement.style.zIndex = '1000';
    previewImageElement.style.pointerEvents = 'none'; // Evitar interferencias con otros eventos
    document.body.appendChild(previewImageElement);
}

function removePreviewImage() {
    if (previewImageElement) {
        previewImageElement.remove();
        previewImageElement = null;
    }
}

function removeSelection(imageElement) {
    if (imageElement) {
        imageElement.style.border = '';
    }
}

window.loadImageFromFile = function (file) {
    return new Promise((resolve, reject) => {
        if (file instanceof Blob) {
            const reader = new FileReader();
            reader.onload = function (e) {
                resolve(e.target.result);
            };
            reader.onerror = function (e) {
                reject(e);
            };
            reader.readAsDataURL(file);
        } else {
            reject("Passed parameter is not a Blob or File");
        }
    });
};

window.generateImageFromCanvas = function (canvasRef) {
    const canvas = canvasRef;
    const dataUrl = canvas.toDataURL("image/png");
    const imgElement = document.getElementById('generatedImage');
    imgElement.src = dataUrl;
    imgElement.style.display = 'block';
};

window.addImageToCanvas = function (dataUrl, canvasRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");

    const img = new Image();
    img.src = dataUrl;
    img.onload = function () {
        const maxWidth = window.innerWidth * 0.6;
        const scale = maxWidth / img.width;
        const width = img.width * scale;
        const height = img.height * scale;

        canvas.width = width;
        canvas.height = height;
        ctx.drawImage(img, 0, 0, width, height);
    };
};

window.clearCanvas = function (canvasRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);
};

function getCanvasImage(canvas) {
    return canvas.toDataURL("image/png");
}

function resizeCanvas(canvasRef, width, height) {
    const canvas = canvasRef;
    canvas.width = width;
    canvas.height = height;
}
