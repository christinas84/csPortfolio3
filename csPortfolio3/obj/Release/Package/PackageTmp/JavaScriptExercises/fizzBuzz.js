$(document).ready(function () {
    $('.fizzBuzzBtn').click(function () {

        var newNumber = $('.inputBox').val()
        var num = +newNumber
        $('.inputBox').val('');
        $('.fizzbuzz').empty();

        if (num !== NaN && num >= 1 && num <= 100 && num % 1 == 0) {
            var i = 1
            while (i <= num) {
                if ((i % 3 == 0) && (i % 5 == 0)) {
                    $('.fizzbuzz').append('<li>FizzBuzz</li>');
                } else if (i % 3 == 0) {
                    $('.fizzbuzz').append('<li>Fizz</li>');
                } else if (i % 5 == 0) {
                    $('.fizzbuzz').append('<li>Buzz</li>');
                } else {
                    $('.fizzbuzz').append('<li>' + i + '</li>');
                }
                i++
            }
        } else {
            alert("Please choose a number between 1 and 100!");
        }
    });

    $('.clearList').click(function (event) {
        $('.fizzbuzz').empty();
    });

});