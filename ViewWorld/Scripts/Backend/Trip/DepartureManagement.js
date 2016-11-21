; $(function () {
    function bindEvents() {
        $('.button.add-departure').on('click', function () {
            $('.ui.modal.add-departure-modal').modal('show');
            $('.ui.dropdown.provider-list').dropdown({
                apiSettings: {
                    url: '/Provider/GetAll',
                    saveRemoteData: false
                },
            });
        });
        $('.button#submit-form').on('click', function (e) {
            e.preventDefault();
            $('.ui.form#add-departure').form('validate form');
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

    function initFormValidator() {
        $('.ui.form#add-departure').form({
            inline: true,
            on    : 'blur',
            fields: {
                landmark: {
                    identifier: 'landmark',
                    rules: [
                        {
                            type: 'empty',
                            prompt: '地点不能为空'
                        }
                    ]
                },
                address: {
                    identifier: 'address',
                    rules: [
                        {
                            type: 'empty',
                            prompt: '地址不能为空'
                        }
                    ]
                },
                depart_time: {
                    identifier: 'depart_time',
                    rules: [
                        {
                            type: 'empty',
                            prompt: '出发时间不能为空'
                        }
                    ]
                },
                provider: {
                    identifier: 'provider',
                    rules: [
                        {
                            tyep: 'empty',
                            prompt: '供应商不能为空'
                        }
                    ]
                }
            },
            onSuccess: function (e, fields) {
                return submitForm($(this));
            },
            onFailure: function (errors, fields) {
                console.log(errors);
            }
        });
    }

    function submitForm($form) {
        var $submitButton = $form.find('#submit-form');
        if ($submitButton.hasClass('loading')) {
            return;
        }
        $submitButton.addClass('loading');
        var $inputs = $form.find('input');
        var data = {};

    }
    
    function initPage() {
        initFormValidator();
        bindEvents();
    }

    initPage();
});