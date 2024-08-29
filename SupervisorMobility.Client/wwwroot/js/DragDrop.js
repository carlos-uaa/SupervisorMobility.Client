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

let isDrawing = false;
let isPencilSelected = false;
let drawColor = "#000000";
let drawPathData = [];
let drawings = [];
let actionHistory = [];

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
                originalWidth: draggedImageElement.width,
                originalHeight: draggedImageElement.height,
                imageId: imageIndex++,
                rotation: 0
            });
            actionHistory.push({ type: "addImage" });

            removeSelection(draggedImageElement);
            removePreviewImage();
            draggedImageElement = null;

            isPencilSelected = false;
            await dotNetObjectRef.invokeMethodAsync('SetPencilState', isPencilSelected);
            await dotNetObjectRef.invokeMethodAsync('OnImageDropped');
        } else {
            console.error("Image element not found");
        }
    });

    canvas.addEventListener("mousedown", function (e) {
        if (isPencilSelected) {
            isDrawing = true;
            ctx.strokeStyle = drawColor;
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.moveTo(e.offsetX, e.offsetY);
            drawPathData = [[e.offsetX, e.offsetY]];

        } else {
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
                const aspectRatio = selectedImage.originalWidth / selectedImage.originalHeight;
                if (selectedImage.width / selectedImage.height !== aspectRatio) {
                    selectedImage.height = selectedImage.width / aspectRatio;
                }
                redrawCanvas(ctx, canvas);


                dotNetObjectRef.invokeMethodAsync('UpdateSelectedImage', selectedImage.element.id);
                dotNetObjectRef.invokeMethodAsync('UpdateSelectedSizeSlider', selectedImage.width);
                dotNetObjectRef.invokeMethodAsync('UpdateSelectedRotateSlider', (selectedImage.rotation * 180 / Math.PI) + 180);
            } else {
                dotNetObjectRef.invokeMethodAsync('DeselectImage');
            }
        }
    });

    canvas.addEventListener("mousemove", function (e) {
        if (isPencilSelected && isDrawing) {
            ctx.lineTo(e.offsetX, e.offsetY);
            ctx.stroke();
            drawPathData.push([e.offsetX, e.offsetY]);

        } else if (selectedImage) {
            const rect = canvas.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            selectedImage.x = x - offsetX;
            selectedImage.y = y - offsetY;

            const aspectRatio = selectedImage.originalWidth / selectedImage.originalHeight;
            if (selectedImage.width / selectedImage.height !== aspectRatio) {
                selectedImage.height = selectedImage.width / aspectRatio;
            }
            redrawCanvas(ctx, canvas);
        }
    });

    canvas.addEventListener("mouseup", async function () {
        if (isPencilSelected) {
            isDrawing = false;
            drawings.push({ path: drawPathData.slice(), color: drawColor });
            actionHistory.push({ type: "addDrawing" });
            await dotNetObjectRef.invokeMethodAsync('OnDrawingAdded');

            redrawCanvas(ctx, canvas);
        } else {
            selectedImage = null;
            canvas.style.cursor = 'default';
        }
    });

    canvas.addEventListener("touchstart", function (e) {
        if (isPencilSelected) {
            if (e.touches.length === 1) {
                isDrawing = true;
                const touch = e.touches[0];
                const rect = canvas.getBoundingClientRect();
                const x = touch.clientX - rect.left;
                const y = touch.clientY - rect.top;

                ctx.strokeStyle = drawColor;
                ctx.lineWidth = 2;
                ctx.beginPath();
                ctx.moveTo(x, y);
                drawPathData = [[e.offsetX, e.offsetY]];
            }
        } else {
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
                    dotNetObjectRef.invokeMethodAsync('UpdateSelectedImage', selectedImage.imageId);

                } else if (draggedImageElement) {
                    initialTouchX = touch.clientX;
                    initialTouchY = touch.clientY;
                    createPreviewImage(draggedImageElement);

                    previewImageElement.style.left = `${initialTouchX - previewImageElement.width / 2}px`;
                    previewImageElement.style.top = `${initialTouchY - previewImageElement.height / 2}px`;
                }
            }
        }
    });

    canvas.addEventListener("touchmove", function (e) {
        e.preventDefault();
        if (isPencilSelected && isDrawing) {
            const touch = e.touches[0];
            const rect = canvas.getBoundingClientRect();
            const x = touch.clientX - rect.left;
            const y = touch.clientY - rect.top;

            ctx.lineTo(x, y);
            ctx.stroke();
            drawPathData.push([x, y]);

        } else if (selectedImage) {
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
        if (isPencilSelected && isDrawing) {
            drawings.push({ path: drawPathData.slice(), color: drawColor });
            actionHistory.push({ type: "addDrawing" });
            await dotNetObjectRef.invokeMethodAsync('OnDrawingAdded');

            redrawCanvas(ctx, canvas);
            isDrawing = false;
        } else {
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
                    imageId: imageIndex++,
                    rotation: 0
                });
                actionHistory.push({ type: "addImage" });

                removeSelection(draggedImageElement);
                removePreviewImage();
                draggedImageElement = null;

                await dotNetObjectRef.invokeMethodAsync('OnImageDropped');
            }
        }
    });

};

    window.togglePencilState = function (state) {
        isPencilSelected = state;
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
    drawings = [];
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

window.undoLastAction = function (canvasRef) {
    if (actionHistory.length > 0) {
        const lastAction = actionHistory.pop();

        if (lastAction.type === "addImage" && movableImages.length > 0) {
            movableImages.pop();
        } else if (lastAction.type === "addDrawing" && drawings.length > 0) {
            drawings.pop();
        }

        const canvas = canvasRef;
        const ctx = canvas.getContext("2d");
        redrawCanvas(ctx, canvas);
    }
};

window.updateImageSize = function (canvasRef, newSize) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");
    const image = movableImages.find(img => img.imageId == selectedIndex);

    if (image) {
        const aspectRatio = image.originalWidth / image.originalHeight;

        if (image.originalWidth > image.originalHeight) {
            image.width = newSize;
            image.height = newSize / aspectRatio;
        } else {
            image.height = newSize;
            image.width = newSize * aspectRatio;
        }

        redrawCanvas(ctx, canvas);
    }
};


function redrawCanvas(ctx, canvas) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (fixedImage) {
        ctx.drawImage(fixedImage.element, fixedImage.x, fixedImage.y, fixedImage.width, fixedImage.height);
    }
    drawings.forEach(drawing => {
        ctx.strokeStyle = drawing.color;
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(drawing.path[0][0], drawing.path[0][1]);
        for (let i = 1; i < drawing.path.length; i++) {
            ctx.lineTo(drawing.path[i][0], drawing.path[i][1]);
        }
        ctx.stroke();
    });


    movableImages.forEach(img => {
        ctx.save();
        ctx.translate(img.x + img.width / 2, img.y + img.height / 2);
        ctx.rotate(img.rotation);
        ctx.drawImage(img.element, -img.width / 2, -img.height / 2, img.width, img.height);
        if (img === selectedImage) {
            ctx.strokeStyle = 'blue';
            ctx.lineWidth = 1;
            ctx.strokeRect(-img.width / 2, -img.height / 2, img.width, img.height);
        }
        ctx.restore();
    });
}

function drawPath(ctx) {
    if (drawPathData.length > 0) {
        ctx.beginPath();
        ctx.moveTo(drawPathData[0][0], drawPathData[0][1]);
        for (let i = 1; i < drawPathData.length; i++) {
            ctx.lineTo(drawPathData[i][0], drawPathData[i][1]);
        }
        ctx.stroke();
    }
}


window.rotateImage = function (canvasRef, newRotation) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");
    const image = movableImages.find(img => img.imageId === selectedIndex);

    if (image) {
        const degrees = newRotation - 180;
        const radians = degrees * Math.PI / 180;

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

        image.rotation = radians;
        updateImagePositionAfterRotation(image);
    }
};


function updateImagePositionAfterRotation(image) {
    const centerX = image.x + image.width / 2;
    const centerY = image.y + image.height / 2;

    image.x = centerX - image.width / 2;
    image.y = centerY - image.height / 2;
}




window.setPencilColor = function (color) {
    drawColor = color;
}
