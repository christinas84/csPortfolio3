$(document).ready(function () {
    $('#go').click(function fact() {
        var n = $('#fact1').val();
        var m = 1;
        while (n) {
            m = m * n;
            n--;
        }
        var el = document.getElementById('result');
        $('#result').append("Factorial of given number :" + +m);

        $('#reset1').click(function () {
            $('#result').empty();
            $('#fact1').val("");
        });
    });
})