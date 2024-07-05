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

window.triggerFileDownloadAndWaitForConfirmation = async (fileName, fileBytes) => {
    try {
        const blob = new Blob([fileBytes], { type: 'application/octet-stream' });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.style.display = 'none';
        anchor.href = url;
        anchor.download = fileName;
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
        URL.revokeObjectURL(url);
        return "File downloaded successfully";
    } catch (error) {
        console.error(`Error during file download: ${error}`);
        return "Error during file download";
    }
};

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
