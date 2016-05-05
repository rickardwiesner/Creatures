$(document).ready(function () {
    $('div.dropdown').hover(function () {
        $('.dropdown-menu-hover', this).fadeIn('fast');

    }, function () {
        $('.dropdown-menu-hover', this).fadeOut('fast');
    });
});