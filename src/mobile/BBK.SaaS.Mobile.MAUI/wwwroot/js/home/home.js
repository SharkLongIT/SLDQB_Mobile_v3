var date = new Date();
var currentHour = date.getHours();

// Chọn thẻ p trong HTML
var greetingElement = document.getElementById("greeting");

// Áp dụng giờ hiện tại vào nội dung của thẻ p
if (currentHour >= 18) {
    greetingElement.innerHTML = "Chúc bạn buổi tối vui vẻ";
} else if (currentHour >= 12) {
    greetingElement.innerHTML = "Chúc bạn buổi chiều vui vẻ";
} else if (currentHour >= 6) {
    greetingElement.innerHTML = "Chúc bạn buổi sáng tốt lành";
} else {
    greetingElement.innerHTML = "Chúc bạn buổi tối vui vẻ";
};

$("#btn-search").on("click", function (e, m) {
    $("#cancel").removeClass("d-none");
});
$("#cancel").on("click", function (e, m) {
    $("#cancel").addClass("d-none");
});
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