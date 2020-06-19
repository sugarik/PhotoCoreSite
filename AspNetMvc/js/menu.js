$(document).ready(function () {
    $('#open-menu-button').on('click', function () {
        $('body').css('overflow', 'hidden');
        $('#mobile-menu').css('display', 'block');
    });

    $('.exit').on('click', function () {
        $('body').css('overflow', 'auto');
        $('#mobile-menu').css('display', 'none');
    });
});