$(function () {
    function BindEvents() {
        $('#addRegion').on('click', function () {
            $('.region-editor').modal('show')
        })
    }
    function InitPage() {
        BindEvents();
    }
    InitPage();
})