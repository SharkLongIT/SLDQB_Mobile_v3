$(document).ready(function () {
    $(".card-header").click(function () {
        var icon = $(this).find("i");
        icon.toggleClass("fa-angle-down fa-angle-up");
        $(this).nextAll(".card-body, .card-footer").slideToggle();
    });
});
$(document).ready(function () {
    $(".filter-icon").click(function () {
        $(this).nextAll(".card1").slideToggle();
    });
});
$(document).ready(function () {
    $(".btn-search").click(function () {
        $(".card1").addClass("d-none");
    });
});
$(document).ready(function () {
    $(".filter-icon").click(function () {
        $(".card1").removeClass("d-none");
    });
});