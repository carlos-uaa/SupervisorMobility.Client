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
let isArrowSelected = false;
let isArrowheadSelected = false;
let drawColor = "#000000";
let arrowColor = "#000000";
let arrowheadColor = "#000000";
let drawPathData = [];
let drawings = [];
let actionHistory = [];

let movableArrows = [];
let arrowStartPoint = null;
let previewArrowEndPoint = null;
let dotNetObjectRef = null;

let isTextSelected = false;
let textStartPoint = null;
let movableTexts = [];
let textInputElement = null;

let isDraggingStart = false;
let isDraggingEnd = false;
let isEditingArrow = false;
let selectedArrow = null;

window.setupCanvas = function (canvasRef, dotNetRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");
    dotNetObjectRef = dotNetRef;

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

    canvas.addEventListener("mousedown", async function (e) {
        const rect = canvas.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;



        const clickedText = movableTexts.find(text =>
            x >= text.x &&
            x <= text.x + ctx.measureText(text.text).width &&
            y >= text.y - 16 && // Altura del texto estimada
            y <= text.y
        );

        const clickedArrow = movableArrows.find(arrow => {
            const near = isPointNearArrow(x, y, arrow);
            return near.isNearStart || near.isNearEnd || near.isNearLine;
        });



        if (!clickedArrow) {
            selectedArrow = null;
            isEditingArrow = false;
            redrawCanvas(ctx, canvas); 
        }

        if (isEditingArrow) {
            const rect = canvas.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            // Detectar si se hizo clic en el punto inicial o final
            const near = isPointNearArrow(x, y, selectedArrow);
            if (near.isNearStart) {
                isDraggingStart = true;
            } else if (near.isNearEnd) {
                isDraggingEnd = true;
            }
        }

        if (!isArrowSelected && !isArrowheadSelected) {
            const clickedArrow = movableArrows.find(arrow => {
                const near = isPointNearArrow(x, y, arrow);
                return near.isNearStart || near.isNearEnd || near.isNearLine;
            });

            if (clickedArrow) {
                selectedArrow = clickedArrow;
                redrawCanvas(ctx, canvas);

                drawControlPoint(ctx, clickedArrow.start.x, clickedArrow.start.y);
                drawControlPoint(ctx, clickedArrow.end.x, clickedArrow.end.y);

                isEditingArrow = true;
                return;
            }
        }


        if (clickedText && isTextSelected) {
            isTextSelected = false; 
            await dotNetObjectRef.invokeMethodAsync('DeselectText');

            textStartPoint = { x: clickedText.x, y: clickedText.y };

            if (!textInputElement) {
                textInputElement = document.createElement("textarea");
                textInputElement.value = clickedText.text; // Mostrar el texto actual
                textInputElement.style.position = "absolute";
                textInputElement.style.left = `${rect.left + clickedText.x - 5}px`;
                textInputElement.style.top = `${rect.top + clickedText.y - 22}px`;
                textInputElement.style.zIndex = 1000;
                textInputElement.style.fontSize = "16px";
                textInputElement.style.lineHeight = "1.5";
                textInputElement.style.border = "1px dashed gray";
                textInputElement.style.resize = "none";
                textInputElement.style.padding = "4px";
                textInputElement.style.backgroundColor = "transparent";

                redrawCanvas(ctx, canvas, clickedText);
                // Habilitar movimiento del textarea
                let isDragging = false;
                let offsetX = 0;
                let offsetY = 0;

                textInputElement.addEventListener("mousedown", function (e) {
                    isDragging = true;
                    offsetX = e.clientX - textInputElement.offsetLeft;
                    offsetY = e.clientY - textInputElement.offsetTop;
                });

                document.addEventListener("mousemove", function (e) {
                    if (isDragging) {
                        textInputElement.style.left = `${e.clientX - offsetX}px`;
                        textInputElement.style.top = `${e.clientY - offsetY}px`;
                    }
                });

                document.addEventListener("mouseup", function () {
                    if (isDragging) {
                        isDragging = false;
                    }
                });


                textInputElement.addEventListener("blur", function () {
                    if (textInputElement.value.trim() !== "") {
                        // Actualizar el texto existente
                        clickedText.x = parseFloat(textInputElement.style.left) - rect.left + 5; // Ajustar para el borde
                        clickedText.y = parseFloat(textInputElement.style.top) - rect.top + 22; // Ajustar para la altura
                        clickedText.text = textInputElement.value;
                        redrawCanvas(ctx, canvas); // Redibujar el canvas
                    }
                    textInputElement.remove();
                    textInputElement = null;
                });

                textInputElement.addEventListener("keydown", function (event) {
                    if (event.key === "Enter") {
                        event.preventDefault();
                        textInputElement.blur(); // Finalizar edición
                    }
                });

                actionHistory.push({ type: "editText" });
                await dotNetObjectRef.invokeMethodAsync('OnTextEdited');

                document.body.appendChild(textInputElement);
                setTimeout(() => {
                    textInputElement.focus();
                }, 0);
                isTextSelected = true; 
                await dotNetObjectRef.invokeMethodAsync('SelectText');

            }
        } else if (isTextSelected) {

            if (!textInputElement) {
                textStartPoint = { x, y };
                textInputElement = document.createElement("textarea");
                textInputElement.style.position = "absolute";
                textInputElement.style.left = `${e.clientX}px`;
                textInputElement.style.top = `${e.clientY}px`;
                textInputElement.style.zIndex = 1000;
                textInputElement.style.fontSize = "16px";
                textInputElement.style.lineHeight = "1.5";
                textInputElement.style.border = "1px dashed gray";
                textInputElement.style.resize = "none";
                textInputElement.style.padding = "4px";
                textInputElement.style.backgroundColor = "transparent";

                textInputElement.addEventListener("blur", function () {
                    if (textInputElement.value.trim() !== "") {
                        movableTexts.push({
                            text: textInputElement.value,
                            x: textStartPoint.x + 5,
                            y: textStartPoint.y + 22,
                            color: "#000", // Ajusta el color si es necesario
                        });
                        redrawCanvas(ctx, canvas);
                    }
                    textInputElement.remove();
                    textInputElement = null;
                });

                textInputElement.addEventListener("keydown", function (event) {
                    if (event.key === "Enter") {
                        event.preventDefault();
                        textInputElement.blur();
                    }
                });

                actionHistory.push({ type: "addText" });
                await dotNetObjectRef.invokeMethodAsync('OnTextAdded');

                document.body.appendChild(textInputElement);
                setTimeout(() => {
                    textInputElement.focus();
                }, 0);
            }
        }

        else if (isArrowheadSelected) {
            if (!arrowStartPoint) {
                arrowStartPoint = { x, y };
            } else {
                const arrowEndPoint = { x, y };

                movableArrows.push({
                    start: arrowStartPoint,
                    end: arrowEndPoint,
                    color: arrowheadColor,
                    hasHead: true, 
                });

                arrowStartPoint = null;
                previewArrowEndPoint = null;

                actionHistory.push({ type: "addArrowhead" });

                await dotNetObjectRef.invokeMethodAsync('OnArrowheadAdded');

                redrawCanvas(ctx, canvas);
            }
            return;

        }

        else if (isArrowSelected) {
            if (!arrowStartPoint) {
                arrowStartPoint = { x, y };
            } else {
                const arrowEndPoint = { x, y };
                movableArrows.push({
                    start: arrowStartPoint,
                    end: arrowEndPoint,
                    color: arrowColor,
                });

                arrowStartPoint = null;
                previewArrowEndPoint = null;

                actionHistory.push({ type: "addArrow" });

                await dotNetObjectRef.invokeMethodAsync('OnArrowAdded');
                redrawCanvas(ctx, canvas);
            }
            return;

        } else if (isPencilSelected) {
            isDrawing = true;
            ctx.strokeStyle = drawColor;
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.moveTo(e.offsetX, e.offsetY);
            drawPathData = [[e.offsetX, e.offsetY]];
        } else {

            redrawCanvas(ctx, canvas);
            selectedImage = movableImages.find(img => isPointInImage(x, y, img));

            if (selectedImage) {

                selectedIndex = selectedImage.imageId;
                offsetX = x - selectedImage.x;
                offsetY = y - selectedImage.y;
                canvas.style.cursor = 'move';

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

        if (isEditingArrow && (isDraggingStart || isDraggingEnd)) {
            const rect = canvas.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            if (isDraggingStart) {
                selectedArrow.start.x = x;
                selectedArrow.start.y = y;
            } else if (isDraggingEnd) {
                selectedArrow.end.x = x;
                selectedArrow.end.y = y;
            }

            redrawCanvas(ctx, canvas);

            drawControlPoint(ctx, selectedArrow.start.x, selectedArrow.start.y);
            drawControlPoint(ctx, selectedArrow.end.x, selectedArrow.end.y);
        }
        if ((isArrowSelected || isArrowheadSelected) && arrowStartPoint) {
            const rect = canvas.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            previewArrowEndPoint = { x, y };

            redrawCanvas(ctx, canvas);
            drawPreviewArrow(ctx, arrowStartPoint, previewArrowEndPoint, arrowColor, isArrowheadSelected);

        }
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

        if (isEditingArrow) {
            isDraggingStart = false;
            isDraggingEnd = false;

        }
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
                    const aspectRatio = selectedImage.originalWidth / selectedImage.originalHeight;
                    if (selectedImage.width / selectedImage.height !== aspectRatio) {
                        selectedImage.height = selectedImage.width / aspectRatio;
                    }
                    redrawCanvas(ctx, canvas);
                    dotNetObjectRef.invokeMethodAsync('UpdateSelectedImage', selectedImage.element.id);
                    dotNetObjectRef.invokeMethodAsync('UpdateSelectedRotateSlider', (selectedImage.rotation * 180 / Math.PI) + 180);
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
            }
        }
    });

};

window.togglePencilState = function (state) {

    isPencilSelected = state;
};

window.toggleArrowState = function (state) {
    isArrowSelected = state;
};

window.toggleArrowheadState = function (state) {
    isArrowheadSelected = state;
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
        const maxWidth = window.innerWidth * 0.95;
        const maxHeight = window.innerHeight * 0.68;


        let width, height;

        if (originalWidth / originalHeight > maxWidth / maxHeight) {
            width = maxWidth;
            height = (originalHeight / originalWidth) * maxWidth;
        } else {
            height = maxHeight;
            width = (originalWidth / originalHeight) * maxHeight;
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
        const originalWidth = img.width;
        const originalHeight = img.height;

        const maxWidth = window.innerWidth * 0.95;
        const maxHeight = window.innerHeight * 0.68;

        let width, height;

        if (originalWidth / originalHeight > maxWidth / maxHeight) {
            width = maxWidth;
            height = (originalHeight / originalWidth) * maxWidth;
        } else {
            height = maxHeight;
            width = (originalWidth / originalHeight) * maxHeight;
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

function getImageSrcById(id) {
    const imageElement = document.getElementById(id);
    return imageElement ? imageElement.src : null;
}

window.undoLastAction = function (canvasRef) {
    if (actionHistory.length > 0) {
        const lastAction = actionHistory.pop();

        if (lastAction.type === "addImage" && movableImages.length > 0) {
            movableImages.pop();
        } else if (lastAction.type === "addDrawing" && drawings.length > 0) {
            drawings.pop();
        } else if ((lastAction.type === "addArrow" || lastAction.type === "addArrowhead") && movableArrows.length > 0) {
            movableArrows.pop();
        } else if ((lastAction.type === "addText" || lastAction.type === "editText") && movableTexts.length > 0) {
            movableTexts.pop();
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

        const imageIndex = movableImages.findIndex(img => img.imageId == selectedIndex);
        if (imageIndex !== -1) {
            movableImages[imageIndex] = image;
        }
        redrawCanvas(ctx, canvas);
    }
};


function redrawCanvas(ctx, canvas, excludeText = null) {
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

    movableArrows.forEach(arrow => {
        ctx.strokeStyle = arrow.color;
        ctx.lineWidth = 2;

        ctx.beginPath();
        ctx.moveTo(arrow.start.x, arrow.start.y);
        ctx.lineTo(arrow.end.x, arrow.end.y);
        ctx.stroke();

        if (arrow.hasHead) {
            drawArrowhead(ctx, arrow.start, arrow.end, arrow.color);
        }

        if (arrow === selectedArrow) {
            drawControlPoint(ctx, arrow.start.x, arrow.start.y);
            drawControlPoint(ctx, arrow.end.x, arrow.end.y);
        }
    });

    if (arrowStartPoint && previewArrowEndPoint) {
        ctx.strokeStyle = isArrowheadSelected ? arrowheadColor : arrowColor;
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(arrowStartPoint.x, arrowStartPoint.y);
        ctx.lineTo(previewArrowEndPoint.x, previewArrowEndPoint.y);
        ctx.stroke();
        ctx.setLineDash([]); 
    }


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

    movableTexts.forEach(textItem => {
        if (textItem !== excludeText) {
            ctx.fillStyle = textItem.color;
            ctx.font = "16px Arial";
            ctx.fillText(textItem.text, textItem.x, textItem.y);
        }
    });

}

function drawPreviewArrow(ctx, start, end, color, withHead) {
    ctx.strokeStyle = color;
    ctx.lineWidth = 2;

    ctx.beginPath();
    ctx.moveTo(start.x, start.y);
    ctx.lineTo(end.x, end.y);
    ctx.stroke();

    if (withHead) {
        drawArrowhead(ctx, start, end, color);
    }
}

function drawArrowhead(ctx, start, end, color) {
    const angle = Math.atan2(end.y - start.y, end.x - start.x);
    const headLength = 10; 

    ctx.fillStyle = color;

    ctx.beginPath();
    ctx.moveTo(end.x, end.y);
    ctx.lineTo(
        end.x - headLength * Math.cos(angle - Math.PI / 6),
        end.y - headLength * Math.sin(angle - Math.PI / 6)
    );
    ctx.lineTo(
        end.x - headLength * Math.cos(angle + Math.PI / 6),
        end.y - headLength * Math.sin(angle + Math.PI / 6)
    );
    ctx.closePath();
    ctx.fill();
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

window.toggleTextState = function (state) {
    isTextSelected = state;
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

window.setArrowColor = function (color) {
    arrowColor = color;
}

window.setArrowheadColor = function (color) {
    arrowheadColor = color;
}

function isPointNearArrow(x, y, arrow, tolerance = 10) {
    const { start, end } = arrow;

    const isNearStart = Math.hypot(x - start.x, y - start.y) <= tolerance;
    const isNearEnd = Math.hypot(x - end.x, y - end.y) <= tolerance;

    const isNearLine = isPointNearLine(x, y, start.x, start.y, end.x, end.y, tolerance);


    return { isNearStart, isNearEnd, isNearLine };
}

function isPointNearLine(px, py, x1, y1, x2, y2, tolerance) {
    const A = px - x1;
    const B = py - y1;
    const C = x2 - x1;
    const D = y2 - y1;

    const dot = A * C + B * D;
    const lenSq = C * C + D * D;
    let param = -1;

    if (lenSq !== 0) {
        param = dot / lenSq;
    }

    let nearestX, nearestY;

    if (param < 0) {
        nearestX = x1;
        nearestY = y1;
    } else if (param > 1) {
        nearestX = x2;
        nearestY = y2;
    } else {
        nearestX = x1 + param * C;
        nearestY = y1 + param * D;
    }

    // Distancia del punto al punto más cercano en la línea
    const dx = px - nearestX;
    const dy = py - nearestY;
    return Math.hypot(dx, dy) <= tolerance;
}

function drawControlPoint(ctx, x, y) {
    ctx.beginPath();
    ctx.arc(x, y, 5, 0, 2 * Math.PI);
    ctx.fillStyle = "#5fa0e7";
    ctx.fill();
    ctx.closePath();
}

document.addEventListener("mousedown", async function (e) {

    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    const isOutsideCanvas =
        e.clientX < rect.left || e.clientX > rect.right ||
        e.clientY < rect.top || e.clientY > rect.bottom;



    if (isOutsideCanvas) {
        isArrowSelected = false;
        isArrowheadSelected = false;
        isPencilSelected = false;
        previewArrowheadPoint = null;
        arrowStartPoint = null;


        redrawCanvas(canvas.getContext("2d"), canvas);
        await dotNetObjectRef.invokeMethodAsync('SetPencilState', false);
        await dotNetObjectRef.invokeMethodAsync('DeselectArrow');
        await dotNetObjectRef.invokeMethodAsync('DeselectText');
    }
});


