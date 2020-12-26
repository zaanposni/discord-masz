window.handleRequestError = function handleRequestError(errMsg, title="API call failed") {
    console.log(errMsg);
    if (errMsg.responseJSON) {
        if ('errors' in errMsg.responseJSON) {
            for (const [key, value] of Object.entries(errMsg.responseJSON.errors)) {
                toastr.error(value, "Invalid " + key);
            }
        } else {
            if ('title' in errMsg.responseJSON) {
                toastr.error(errMsg.responseJSON.title, title);
            } else {
                toastr.error(errMsg.status + ': ' + errMsg.responseText, title);
            }
        }
    } else {
        toastr.error(errMsg.status + ': ' + errMsg.responseText, title);
    }
}
