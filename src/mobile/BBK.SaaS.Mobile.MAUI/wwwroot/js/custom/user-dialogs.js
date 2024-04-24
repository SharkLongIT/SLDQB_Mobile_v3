
// Class Definition
var BlazorUserDialogService = function () {
    const defaultSelector = "body";
    var block = function (selector) {
        if (!selector || selector === "") {
            selector = defaultSelector;
        }

        var element = document.querySelector(selector);
        var blockUI = KTBlockUI.getInstance(element);
        if (blockUI) {
            if (!blockUI.isBlocked()) {
                blockUI.block();
            }
        } else {
            blockUI = new KTBlockUI(element, {
                overlayClass: "bg-body",
            });

            if (blockUI) {
                blockUI.block();
            }
        }
    }

    var unBlock = function (selector) {
        if (!selector || selector === "") {
            selector = defaultSelector;
        }

        var element = document.querySelector(selector);
        var blockUI = KTBlockUI.getInstance(element);
        if (blockUI && blockUI.isBlocked()) {
            blockUI.release();
        }
    }

    var alertSuccess = function (text, confirmButtonText) {
        Swal.fire({
            text: text,
            icon: "success",
            buttonsStyling: false,
            confirmButtonText: confirmButtonText,
            customClass: {
                confirmButton: "btn btn-success btn-white-text"
            },
            background: "white",
            iconHtml: '<i style="color: #50cd89;"></i>'

        });
    }

    var alertInfo = function (text, confirmButtonText) {
        Swal.fire({
            text: text,
            icon: "info",
            buttonsStyling: false,
            confirmButtonText: confirmButtonText,
            customClass: {
                confirmButton: "btn btn-primary btn-white-text"
            },
            background: "white"
        });
    }

    var alertError = function (text, confirmButtonText) {
        Swal.fire({
            text: text,
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: confirmButtonText,
            customClass: {
                confirmButton: "btn btn-danger btn-white-text"
            },
            background: "white"
        });
    }

    var alertWarn = function (text, confirmButtonText) {
        Swal.fire({
            text: text,
            icon: "warning",
            buttonsStyling: false,
            confirmButtonText: confirmButtonText,
            customClass: {
                confirmButton: "btn btn-warning"
            },
            background: "white",
            iconHtml: '<i class="fa fa-exclamation-circle" style="color: #f1bc00;"></i>'
        });
    }

    var confirm = async function (text, title, confirmButtonText, cancelButtonText) {
        let promise = new Promise((resolve, reject) => {
            Swal.fire({
                title: title,
                text: text,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: confirmButtonText,
                cancelButtonText: cancelButtonText
            }
            background: "white").then((result) => {
                resolve(result.isConfirmed);
            });
        });

        let result = await promise;
        return result;
    }

    var prompt = async function (text, title, confirmButtonText, showCancelButton, cancelButtonText, allowOutsideClick) {
        const { value: result } = await Swal.fire({
            title: title,
            input: 'text',
            inputLabel: text,
            showCancelButton: showCancelButton,
            confirmButtonText: confirmButtonText,
            cancelButtonText: cancelButtonText,
            allowOutsideClick: allowOutsideClick
        })

        return result;
    }

    // Public Functions
    return {
        block: block,
        unBlock: unBlock,
        alertSuccess: alertSuccess,
        alertInfo: alertInfo,
        alertError: alertError,
        alertWarn: alertWarn,
        confirm: confirm,
        prompt: prompt
    };
}();