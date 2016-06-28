$(document).ready(function () {
    $('#submit1').click(function () {
        revString = "";
        inpString = document.getElementById("text1").value;
        i = inpString.length;

        for (var j = i; j >= 0; j--) {
            revString = revString + inpString.charAt(j);
        }
        if (inpString === revString) {
            document.getElementById("answer").innerHTML = inpString + " is a palindrome";
        } else {
            document.getElementById("answer").innerHTML = inpString + " is not a palindrome";
        }
    })
    $('#reset').click(function () {
        $('#text1').val("");
        $('#answer').empty();
    });
});