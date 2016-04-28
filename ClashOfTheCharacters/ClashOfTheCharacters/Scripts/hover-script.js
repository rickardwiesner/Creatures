$(document).ready(function () {
    $('div.dropdown').hover(function () {
        $('.dropdown-menu', this).fadeIn('fast');

    }, function () {
        $('.dropdown-menu', this).fadeOut('fast');
    });
});