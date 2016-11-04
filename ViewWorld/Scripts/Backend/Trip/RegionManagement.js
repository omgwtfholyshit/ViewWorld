$(function () {
    function BindEvents() {
        $('#addRegion').on('click', function () {
            $('.region-editor').modal('show')
        })
    }
    function LoadCategory(callback) {
        
    }
    function InitPage() {
        BindEvents();
    }
    InitPage();
})