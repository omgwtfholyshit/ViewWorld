$(function () {
    var coefficient = 27.06, $menuItem = $('.ui.menu.layout-nav .item'), $contentWrapper = $('.content-wrapper');

    function bindEvents() {
        $('.layout-nav .ui.dropdown.item').dropdown({ 'on': 'hover' });
        $('.log-out').on('click', function () {
            $.post("/Account/LogOff", null, function (data, status) {
                if (data.status == 200) {
                    location.href = "/";
                } else {
                    $.tip(".message-container", "服务器无响应", "服务器暂无响应,请稍候再试.", "negative", 4)
                }

            });
        })

    }
    function adjustLayout() {
        var result = 100 - (1920 - window.innerWidth) / coefficient;
        if (result <= 66) result = 66;
        $menuItem.stop().animate({ 'font-size': result + "%" })
        $contentWrapper.css({ 'height': (window.innerHeight / 14) - 9 + "rem" });
    }
    function initPage() {
        bindEvents()
        adjustLayout()
        window.onresize = adjustLayout;
        //$('.ui.dimmer').dimmer('toggle');
        //setTimeout(function () { $('.ui.dimmer').dimmer('hide'); }, 2000);
    }
    initPage()
})