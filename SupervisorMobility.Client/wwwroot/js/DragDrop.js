let draggedImageId = null;

window.setupCanvas = function (canvasRef, dotNetObjectRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");

    canvas.addEventListener("dragover", (e) => e.preventDefault());

    canvas.addEventListener("drop", async function (e) {
        e.preventDefault();
        const img = document.getElementById(draggedImageId);
        if (img) {
            const x = e.offsetX;
            const y = e.offsetY;
            ctx.drawImage(img, x, y, img.width / 2, img.height / 2);

            await dotNetObjectRef.invokeMethodAsync('OnImageDropped');
        } else {
            console.error("Image element not found");
        }
    });
};


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


window.addImageToCanvas = function (dataUrl, canvasRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");

    const img = new Image();
    img.src = dataUrl;
    img.onload = function () {
        canvas.width = img.width;
        canvas.height = img.height;
        ctx.drawImage(img, 0, 0, img.width, img.height);
    };
};



window.onDragStartJs = function (imageId) {
    draggedImageId = imageId;
};

function resizeCanvas(canvasRef, width, height) {
    const canvas = canvasRef;
    canvas.width = width;
    canvas.height = height;
}


function getCanvasImage(canvas) {
    return canvas.toDataURL("image/png");
}

window.clearCanvas = function (canvasRef) {
    const canvas = canvasRef;
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);
};
