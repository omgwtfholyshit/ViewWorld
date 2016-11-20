; $(function () {
    function bindEvents() {
        $('.button.add-departure').on('click', function () {
            $('.ui.modal.add-departure-modal').modal('show');
            $('.ui.dropdown.provider-list').dropdown({
                apiSettings: {
                    mockResponse: {
                        success: true,
                        results: [
                            { name: "test", value: 1 },
                            { name: "test2", value: 2 }
                        ]
                    }
                }
            });
        });
        $('.button#submit-form').on('click', function (e) {
            e.preventDefault();
            submitForm(e);
        });
        $('.button.edit').on('click', function (e) {
            e.preventDefault();
            editDeparture(e);
        });
        $('.button.delete').on('click', function (e) {
            e.preventDefault();
            var r = window.confirm("请确认删除操作");
            if (r) {
                deleteDeparture(e);
            }
        });
        $('.button.save').on('click', function (e) {
            e.preventDefault();
            var r = window.confirm("确认保存");
            if (r) {
                saveDeparture(e);
            }
        });
    }
    

    function initPage() {
        bindEvents();
    }

    initPage();
});