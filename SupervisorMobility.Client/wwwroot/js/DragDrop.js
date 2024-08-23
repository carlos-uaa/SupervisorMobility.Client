let draggedImageId = null;
let draggedImageElement = null;
let initialTouchX = 0;
let initialTouchY = 0;
let previewImageElement = null;
let offsetX = 0;
let offsetY = 0;
let movableImages = [];
let fixedImage = null;
let selectedImage = null; 
let imageIndex = 0;
let selectedIndex = 0;

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
                height: draggedImageElement.height,
                imageId: imageIndex++
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
        redrawCanvas(ctx, canvas);
        selectedImage = movableImages.find(img => isPointInImage(x, y, img));
        if (selectedImage) {
            selectedIndex = selectedImage.imageId;

            offsetX = x - selectedImage.x;
            offsetY = y - selectedImage.y;
            canvas.style.cursor = 'move';
            redrawCanvas(ctx, canvas);

            dotNetObjectRef.invokeMethodAsync('UpdateSelectedImage', selectedImage.element.id);
            dotNetObjectRef.invokeMethodAsync('UpdateSelectedSlider', selectedImage.width);
        }
        else {
            dotNetObjectRef.invokeMethodAsync('DeselectImage');
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
        if (e.touches.length === 1) {
            const touch = e.touches[0];
            const rect = canvas.getBoundingClientRect();
            const x = touch.clientX - rect.left;
            const y = touch.clientY - rect.top;

            selectedImage = movableImages.find(img => isPointInImage(x, y, img));
            if (selectedImage) {
                offsetX = x - selectedImage.x;
                offsetY = y - selectedImage.y;
                canvas.style.cursor = 'move';
            } else if (draggedImageElement) {
                initialTouchX = touch.clientX;
                initialTouchY = touch.clientY;
                createPreviewImage(draggedImageElement);

                previewImageElement.style.left = `${initialTouchX - previewImageElement.width / 2}px`;
                previewImageElement.style.top = `${initialTouchY - previewImageElement.height / 2}px`;
            }
        }
    });

    canvas.addEventListener("touchmove", function (e) {
        e.preventDefault();
        if (selectedImage) {
            const touch = e.touches[0];
            const rect = canvas.getBoundingClientRect();
            const x = touch.clientX - rect.left;
            const y = touch.clientY - rect.top;

            selectedImage.x = x - offsetX;
            selectedImage.y = y - offsetY;
            redrawCanvas(ctx, canvas);
        } else if (previewImageElement) {
            const touch = e.touches[0];
            const x = touch.clientX - previewImageElement.width / 2;
            const y = touch.clientY - previewImageElement.height / 2;
            previewImageElement.style.left = `${x}px`;
            previewImageElement.style.top = `${y}px`;
        }
    }, { passive: false });

    canvas.addEventListener("touchend", async function (e) {
        if (selectedImage) {
            selectedImage = null;
            canvas.style.cursor = 'default';
        } else if (draggedImageElement) {
            const touch = e.changedTouches[0];
            const rect = canvas.getBoundingClientRect();
            const x = touch.clientX - rect.left - (draggedImageElement.width * 1.05) / 2;
            const y = touch.clientY - rect.top - (draggedImageElement.height * 1.05) / 2;
            const width = draggedImageElement.width * 1.05;
            const height = draggedImageElement.height * 1.05;
            ctx.drawImage(draggedImageElement, x, y, width, height);

            movableImages.push({
                element: draggedImageElement,
                x: x,
                y: y,
                width: width,
                height: height,
                imageId: imageIndex++

            });

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
        const maxHeight = window.innerHeight * 0.75;

        let width, height;

        if (img.width > maxWidth) {
            const scale = maxWidth / img.width;
            width = img.width * scale;
            height = img.height * scale;
        } else {
            width = img.width;
            height = img.height;
        }

        if (height > maxHeight) {
            const scale = maxHeight / height;
            width = width * scale;
            height = height * scale;
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
    const ctx = canvas.getContext("2d");
    redrawCanvas(ctx, canvas)

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
        const maxHeight = window.innerHeight * 0.75;

        let width, height;

        if (img.width > maxWidth) {
            const scale = maxWidth / img.width;
            width = img.width * scale;
            height = img.height * scale;
        } else {
            width = img.width;
            height = img.height;
        }

        if (height > maxHeight) {
            const scale = maxHeight / height;
            width = width * scale;
            height = height * scale;
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
    movableImages = [];
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

        if (img === selectedImage) {
            ctx.strokeStyle = 'blue';
            ctx.lineWidth = 1;
            ctx.strokeRect(img.x, img.y, img.width, img.height);
        }
    });
}

window.undoLastAction = function (canvasRef) {
    if (movableImages.length > 0) {
        movableImages.pop();
        const canvas = canvasRef;
        const ctx = canvas.getContext("2d");
        redrawCanvas(ctx, canvas)
    }
};

window.updateImageSize = function (canvasRef, newSize) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");
    const image = movableImages.find(img => img.imageId == selectedIndex);
    if (image) {
        image.width = newSize;
        image.height = newSize;
        redrawCanvas(ctx, canvas)

    }
};


window.rotateImage = function (canvasRef, newRotation) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");
    const image = movableImages.find(img => img.imageId === selectedIndex); // Encuentra la imagen seleccionada

    if (image) {
        const radians = newRotation * Math.PI / 180;
        ctx.save();
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        redrawCanvas(ctx, canvas);
        ctx.translate(image.x + image.width / 2, image.y + image.height / 2);
        ctx.rotate(radians);
        ctx.drawImage(
            image.element,
            -image.width / 2,
            -image.height / 2,
            image.width,
            image.height
        );
        ctx.restore();
    }
};
