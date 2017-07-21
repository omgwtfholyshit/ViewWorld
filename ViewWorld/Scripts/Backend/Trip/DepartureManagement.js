; $(function () {
    var api = {
        addPoint: '/Trip/AddStartingPoint',
        editPoint: '/Trip/UpdateStartingPoint',
        deletePoint: '/Trip/DeleteStartingPointById'
    }, token = $('input[name=__RequestVerificationToken]').val(), $form = $('.ui.form#add-departure'), status = '',pointId='';
    function bindEvents() {
        $('.button.add-departure').on('click', function () {
            status = 'add';
            $('.ui.modal.add-departure-modal').modal('show');            
        });
        $('.button#submit-form').on('click', function (e) {
            e.preventDefault();
            if ($form.form('validate form')) {
                submitForm($form);
            }
                
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
        $('.header-left .search.icon').on('click', function (e) {
            location.replace("/Page/DepartureManagement?keyword=" + $(e.target).siblings().val());
        })
        $('.header-left input').on('keypress', function (e) {
            if (e.keyCode == '13')
                $('.header-left .search.icon').click();
        })
        $('#clear').on('click', function () {
            $('.header-left input').val('');
            $('.header-left .search.icon').click();
        })
    }

    function editDeparture(e) {
        var $button = $(e.currentTarget), $tr = $button.closest('tr'), availabledates = $tr.data('availabledays'), providerId = $tr.data('providerid');
        status = 'edit',pointId = $tr.attr('id');
        $form.find('input[name=landmark]').val($($tr.find('td')[0]).text().trim());
        $form.find('input[name=address]').val($($tr.find('td')[1]).text().trim());
        $form.find('input[name=depart_time]').val($($tr.find('td')[3]).text().trim());
        $form.find('.checkbox.availabledates').each(function (index, element) {
            $element = $(element);
            if (availabledates.indexOf($element.find('input').val()) != -1)
                $element.checkbox('check');
        })
        $form.find('.provider-list input.search').click();
        setTimeout(function () {
            console.log(providerId);
            $form.find('.provider-list').dropdown("set selected", providerId);
        }, 500);
        $('.ui.modal.add-departure-modal').modal('show');
    }

    function deleteDeparture(e) {
        var $this = $(e.target).closest('.button.delete');
        if ($this.hasClass('loading')) {
            return;
        }
        $this.addClass('loading');
        if ($this.closest('tr').length > 0) {
            $.ajax({
                url: api.deletePoint,
                method: 'post',
                data: {
                    pointId: $this.closest('tr').attr('id'),
                    __RequestVerificationToken: token
                },
                success: function (response) {
                    if (response.Success) {
                        $.tip('.message-container', '删除成功', '', 'positive', 4);
                        $this.closest('tr').remove();
                    } else {
                        $this.removeClass('loading');
                        $.tip(".message-container", "删除失败", response.message, "negative", 4);
                    }
                },
                error: function (data) {
                    $this.removeClass('loading');
                    $.tip(".message-container", "操作失败", "服务器超时，请稍后重试！", "negative", 4);
                }
            });
        }
    }
    function renderProviderList() {
        $.ajax({
            url: '/Provider/GetAll',
            success: function (response) {
                $('.ui.dropdown.provider-list').dropdown({
                    apiSettings: {
                        response: response
                    }
                }).removeClass('loading');

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
                            type: 'empty',
                            prompt: '供应商不能为空'
                        }
                    ]
                }
            },
            onSuccess: function (e, fields) {
            },
            onFailure: function (errors, fields) {
                console.log(errors);
            }
        });
    }

    function submitForm($form, url) {
       
        var $submitButton = $form.find('#submit-form'), availabledates = '';
        if (!$submitButton.hasClass('loading')) {
            var $inputs = $form.find('input:visible, textarea:visible').not('.search'), text = '';
            var model = {};
            $('.checkbox.availabledates').each(function (index, element) {
                var $element = $(element);
                if ($element.checkbox('is checked')) {
                    availabledates += $element.find('input[name=availabledates]').val() + ',';
                }
            })
            if (availabledates == '') {
                return $.tip(".message-container", "请选择出发日期", "出发日期不能为空", "negative", 4);
            }
            $inputs.each(function (index, input) {
                if (typeof $(input).data('db-key')!='undefined')
                    model[$(input).data('db-key')] = $(input).val().trim();
            });
            switch (status) {
                case 'add':
                    url = api.addPoint;
                    break;
                case 'edit':
                    url = api.editPoint;
                    model.Id = pointId;
                    break;
                default:
                    return $.tip(".message-container", "提交失败", "状态获取错误，请刷新页面重试", "negative", 4);
            }
            $provider = $('.provider-list');
            model.ProviderId = $provider.find('input[name=provider]').val();
            text = $provider.find('.text').text();
            model.ProviderName = text.split('----')[0];
            model.ProviderAlias = text.split('----')[1];
            model.AvailableDays = availabledates;
            $submitButton.addClass('loading');
            console.log(url);
            $.post(url, { point: model, __RequestVerificationToken: token }).done(function (result) {
                $submitButton.removeClass('loading');
                setTimeout(function () {
                    window.location.reload();
                }, 1000);
                $.tip(".message-container", "操作成功", "出发地已保存,请等待页面刷新", "positive", 4);
            }).fail(function (xhr) {
                $submitButton.removeClass('loading');
                $.tip(".message-container", "提交失败", "服务器超时，请稍后重试！", "negative", 4);
            })
        } 
    }
    
    function initPage() {
        
        $('.header-left').transition('fade in').removeClass('invisible');
        initFormValidator();
        bindEvents();
        renderProviderList();
    }

    initPage();
});