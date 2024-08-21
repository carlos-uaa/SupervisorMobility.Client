let draggedImageId = null;
let draggedImageElement = null;
let initialTouchX = 0;
let initialTouchY = 0;
let previewImageElement = null;
let offsetX = 0;
let offsetY = 0;
let movableImages = [];
let fixedImage = null;



window.setupCanvas = function (canvasRef, dotNetObjectRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");

    canvas.addEventListener("dragover", (e) => e.preventDefault());

    canvas.addEventListener("drop", async function (e) {
        e.preventDefault();
        if (draggedImageElement) {
            const x = e.offsetX - draggedImageElement.width / 2;
            const y = e.offsetY - draggedImageElement.height / 2;
            ctx.drawImage(draggedImageElement, x, y, draggedImageElement.width, draggedImageElement.height);

            movableImages.push({
                element: draggedImageElement,
                x: x,
                y: y,
                width: draggedImageElement.width,
                height: draggedImageElement.height
            });

            removeSelection(draggedImageElement);
            removePreviewImage();
            draggedImageElement = null;

            await dotNetObjectRef.invokeMethodAsync('OnImageDropped');
        } else {
            console.error("Image element not found");
        }
    });

    canvas.addEventListener("mousedown", function (e) {
        const rect = canvas.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;

        selectedImage = movableImages.find(img => isPointInImage(x, y, img));
        if (selectedImage) {
            offsetX = x - selectedImage.x;
            offsetY = y - selectedImage.y;
            canvas.style.cursor = 'move';
        }
    });

    canvas.addEventListener("mousemove", function (e) {
        if (selectedImage) {
            const rect = canvas.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            selectedImage.x = x - offsetX;
            selectedImage.y = y - offsetY;
            redrawCanvas(ctx, canvas);
        }
    });

    canvas.addEventListener("mouseup", function () {
        selectedImage = null;
        canvas.style.cursor = 'default';
    });

    canvas.addEventListener("touchstart", function (e) {
        if (e.touches.length === 1 && draggedImageElement) {
            const touch = e.touches[0];
            initialTouchX = touch.clientX;
            initialTouchY = touch.clientY;
        }
        createPreviewImage(draggedImageElement);

        previewImageElement.style.left = `${initialTouchX - previewImageElement.width / 2}px`;
        previewImageElement.style.top = `${initialTouchY - previewImageElement.height / 2}px`;
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
    }, { passive: false });

    canvas.addEventListener("touchend", async function (e) {
        if (draggedImageElement) {
            const touch = e.changedTouches[0];
            const rect = canvas.getBoundingClientRect();
            const x = touch.clientX - rect.left - (draggedImageElement.width * 1.05) / 2;
            const y = touch.clientY - rect.top - (draggedImageElement.height * 1.05) / 2;
            const width = draggedImageElement.width * 1.05;
            const height = draggedImageElement.height * 1.05;
            ctx.drawImage(draggedImageElement, x, y, width, height);

            removeSelection(draggedImageElement);
            removePreviewImage();
            draggedImageElement = null;

            await dotNetObjectRef.invokeMethodAsync('OnImageDropped');
        }
    });
};

window.onDragStartJs = function (imageId) {
    if (draggedImageElement) {
        removeSelection(draggedImageElement);
    }

    draggedImageId = imageId;
    draggedImageElement = document.getElementById(imageId);

    if (draggedImageElement) {
        draggedImageElement.style.border = '2px solid blue';
        createPreviewImage(draggedImageElement);
    }
};

window.setFixedImage = function (dataUrl, canvasRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");

    const img = new Image();
    img.src = dataUrl;
    img.onload = function () {
        const maxWidth = window.innerWidth * 0.6;

        let width, height;
        if (img.width > maxWidth) {
            const scale = maxWidth / img.width;
            width = img.width * scale;
            height = img.height * scale;
        } else {
            width = img.width;
            height = img.height;
        }

        fixedImage = {
            element: img,
            x: 0,
            y: 0,
            width: width,
            height: height
        };

        redrawCanvas(ctx, canvas);
    };
};

function createPreviewImage(imageElement) {
    if (imageElement) { 
        previewImageElement = imageElement.cloneNode(true);
        previewImageElement.style.position = 'absolute';
        previewImageElement.style.zIndex = '1000';
        previewImageElement.style.pointerEvents = 'none';
        document.body.appendChild(previewImageElement);
    }
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

        let width, height;
        if (img.width > maxWidth) {
            const scale = maxWidth / img.width;
            width = img.width * scale;
            height = img.height * scale;
        } else {
            width = img.width;
            height = img.height;
        }

        canvas.width = width;
        canvas.height = height;

        fixedImage = {
            element: img,
            x: 0,
            y: 0,
            width: width,
            height: height
        };

        ctx.drawImage(fixedImage.element, fixedImage.x, fixedImage.y, fixedImage.width, fixedImage.height);
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

function isPointInImage(x, y, image) {
    return x >= image.x && x <= image.x + image.width &&
        y >= image.y && y <= image.y + image.height;
}

function redrawCanvas(ctx, canvas) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (fixedImage) {
        ctx.drawImage(fixedImage.element, fixedImage.x, fixedImage.y, fixedImage.width, fixedImage.height);
    }

    movableImages.forEach(img => {
        ctx.drawImage(img.element, img.x, img.y, img.width, img.height);
    });
}

window.undoLastAction = function (canvasRef) {
    if (movableImages.length > 0) {
        movableImages.pop();
    }
};
