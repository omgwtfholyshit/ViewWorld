$(function () {
    function bindEvents() {
        $('.user-role').popup({
            on: 'hover'
        })
        $('#DOB').calendar({
            ampm: false,
            type: 'date',
            formatter: {
                date: function (date, settings) {
                    if (!date) return '';
                    var day = date.getDate();
                    var month = settings.text.monthsShort[date.getMonth()];
                    var year = date.getFullYear();
                    return year + '年' + month + day + '日';
                }
            }
        });
    }
    function initPage() {
        bindEvents();
    }
    initPage();
})