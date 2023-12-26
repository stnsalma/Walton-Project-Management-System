function FlashMessage(flashLabel, message) {
    if (flashLabel == 'success') {
        $('.saveCoursebtn').attr('disabled', 'disable');
    }
    var $flash = $('<div id="flash" style="display:none;">');
    $flash.html(message);
    $flash.toggleClass('flash');
    $flash.toggleClass('flash-' + flashLabel);
    $('body').find('.FlashMessage').prepend($flash);
    $flash.slideDown('slow');
    $flash.delay(2000).slideToggle('highlight');
    $($flash).click(function () { $(this).slideToggle("highlight");; });
    if (flashLabel == 'success') {
        setTimeout((function () {
            window.location.reload();
        }), 1500);
    }
}