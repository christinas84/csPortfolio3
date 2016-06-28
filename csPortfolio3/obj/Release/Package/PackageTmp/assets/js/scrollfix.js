$(document).ready(function () {
    $("#msg-submitted").click(function () {
        var anchor = document.getElementById('@Model.Contact');
        anchor.scrollIntoView(true);
        });
});