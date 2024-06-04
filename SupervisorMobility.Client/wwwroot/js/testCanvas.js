window.testCanvas = {
    initializeCanvas: (canvasId) => {
        const canvas = document.getElementById(canvasId);
        const context = canvas.getContext('2d');

        let isDrawing = false;
        let lastX = 0;
        let LastY = 0;

        function draw(e) {
            if (!isDrawing) return;
            const rect = canvas.getBoundingClientRect();
            context.strokeStyle = '#000'; // Set stroke color
            context.lineWidth = 5; // Set line width
            context.lineCap = 'round'; // Set line cap style

            let x = e.clientX - rect.left;
            let y = e.clientY - rect.top;

            context.beginPath();
            context.moveTo(lastX, lastY);
            context.lineTo(x, y);
            context.stroke();

            [lastX, lastY] = [x, y];
        }

        function drawTouch(e) {
            if (!isDrawing) return;
            document.body.style.overflow = 'hidden';
            const rect = canvas.getBoundingClientRect();
            const touch = e.touches[0];
            context.strokeStyle = '#000'; // Set stroke color
            context.lineWidth = 5; // Set line width
            context.lineCap = 'round'; // Set line cap style

            let x = touch.clientX - rect.left;
            let y = touch.clientY - rect.top;

            context.beginPath();
            context.moveTo(lastX, lastY);
            context.lineTo(x, y);
            context.stroke();

            [lastX, lastY] = [x, y];
        }

        canvas.addEventListener('mousedown', (e) => {
            isDrawing = true;
            const rect = canvas.getBoundingClientRect();
            [lastX, lastY] = [e.clientX - rect.left, e.clientY - rect.top];
        });

        canvas.addEventListener('mousemove', draw);
        canvas.addEventListener('mouseup', () => isDrawing = false);
        canvas.addEventListener('mouseout', () => isDrawing = false);

        canvas.addEventListener('touchstart', (e) => {
            isDrawing = true;
            const rect = canvas.getBoundingClientRect();
            [lastX, lastY] = [e.touches[0].clientX - rect.left, e.touches[0].clientY - rect.top];
        });

        canvas.addEventListener('touchmove', drawTouch);
        canvas.addEventListener('touchend', () => { isDrawing = false; document.body.style.overflow = ''; });
        canvas.addEventListener('touchleave', ()=> isDrawing = false);
        // Set up event listeners (e.g., mousemove, mousedown, mouseup)
        // Handle drawing logic here
    },
    drawLine: (context, x1, y1, x2, y2) => {
        context.beginPath();
        context.moveTo(x1, y1);
        context.lineTo(x2, y2);
        context.stroke();
    },
    // Add other drawing functions as needed
};