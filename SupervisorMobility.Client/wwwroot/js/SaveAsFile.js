function saveAsFile(filename, byteBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64' + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

window.triggerFileDownloadAndWaitForConfirmation = (fileName, fileURL) => {
    // Llame a la función de descarga de archivos existente
    downloadFile(fileName, fileURL);

    // Devuelve una promesa que se resuelve después de recibir la confirmación de descarga
    return new Promise((resolve, reject) => {
        // Establezca un intervalo para verificar si se recibió la confirmación de descarga
        const intervalId = setInterval(() => {
            // Si la confirmación de descarga está presente en el cuerpo de la página
            if (document.body.textContent.includes("File downloaded successfully")) {
                // Elimine el intervalo y resuelva la promesa con la cadena de confirmación
                clearInterval(intervalId);
                resolve("File downloaded successfully");
            }
        }, 500);
    });
}

window.triggerFileDownload = (fileName, url) => {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}

window.downloadFile = async (fileName, url) => {
    try {
        const response = await fetch(url);
        const blob = await response.blob();
        const objectUrl = URL.createObjectURL(blob);
        const anchorElement = document.createElement('a');
        anchorElement.href = objectUrl;
        anchorElement.download = fileName ?? '';
        anchorElement.click();
        anchorElement.remove();

    } catch (error) {
        console.log(`Error en la descarga del archivo: ${error}`);
    }
}
