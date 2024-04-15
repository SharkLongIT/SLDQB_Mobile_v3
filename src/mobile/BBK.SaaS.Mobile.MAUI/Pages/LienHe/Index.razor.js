document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();
    var form = document.getElementById("form-send");
    if ($("#fullname").val() == "" || $("#fullname").val() == null) {
        return;
    }
    if ($("#email").val() == "" || $("#email").val() == null) {
        return;
    }
    if ($("#phone").val() == "" || $("#phone").val() == null) {
        return;
    }
    if ($("#description").val() == "" || $("#description").val() == null) {
        return;
    }
    form.reset();
});