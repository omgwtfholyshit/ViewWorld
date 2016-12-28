; $(function () {
    function bindEvents() {
        $('.button.add-provider').on('click', function () {
            $('.ui.modal.add-provider-modal').modal('show');
        });
        $('.button#submit-form').on('click', function (e) {
            e.preventDefault();
            submitForm(e);
        });
        $('.button.edit').on('click', function (e) {
            e.preventDefault();
            editProvider(e);
        });
        $('.button.delete').on('click', function (e) {
            e.preventDefault();
            var r = window.confirm("请确认删除操作");
            if (r) {
                deleteProvider(e);
            }
        });
        $('.button.save').on('click', function (e) {
            e.preventDefault();
            var r = window.confirm("确认保存");
            if (r) {
                saveProvider(e);
            }
        });
    }

    function saveProvider(e) {
        var $this = $(e.target).closest('.button.save');
        if ($this.hasClass('loading')) {
            return;
        }
        $this.addClass('loading');

        var $cells = $this.closest('tr').find('.editable-cell');
        
        var model = {
            Id: $this.closest('tr').data('provider-id')
        };
        $cells.each(function (index, cell) {
            model[$(cell).data('db-key')] = $(cell).text().trim();
        });

        $.ajax({
            url: '/Provider/EditProvider',
            method: 'POST',
            data: {
                model: model,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"').val()
            },
            success: function (response) {
                if (response.status == 200) {
                    $.tip('.message-container', '修改成功', '', 'positive', 4);
                    $this.removeClass('loading')
                         .addClass('hidden')
                         .siblings('.button.edit')
                            .removeClass('loading hidden');
                    $cells.attr('contenteditable', false);
                } else {
                    $this.removeClass('loading');
                    $.tip(".message-container", "修改失败", response.message, "negative", 4);
                }
            },
            error: function (error) {
                $this.removeClass('loading')
                         .addClass('hidden')
                         .siblings('.button.edit')
                            .removeClass('loading hidden');
                $.tip(".message-container", "操作失败", "服务器超时，请稍后重试！", "negative", 4);
            }

        });
    }

    function editProvider(e) {

        // todo: edit provider
        var $this = $(e.target).closest('.button.edit');
        if ($this.hasClass('loading')) {
            return;
        }
        $this.addClass('loading');

        if ($this.closest('tr').length > 0) {
            var $row = $this.closest('tr');
            $row.find('.editable-cell').attr('contenteditable', true);
            $($row.find('.editable-cell')[0]).focus();
            $this.addClass('hidden').siblings('.button.save').removeClass('hidden');
        }
    }

    function deleteProvider(e) {
        var $this = $(e.target).closest('.button.delete');
        if ($this.hasClass('loading')) {
            return;
        }
        $this.addClass('loading');
        if ($this.closest('tr').length > 0) {
            $.ajax({
                url: "/Provider/DeleteProvider",
                method: 'post',
                data: {
                    id: $this.closest('tr').data('provider-id'),
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"').val()
                },
                success: function (response) {
                    if (response.status == 200) {
                        $.tip('.message-container', '删除成功', '', 'positive', 4);                        
                        $this.closest('tr').addClass('archived-row');
                        $this.addClass('disabled').removeClass('red loading');
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

    function validator($form, $inputs) {
        var error = false;

        $inputs.each(function () {
            var $this = $(this);
            if ($this.closest('.field').hasClass('required') && $this.val().trim().length == 0) {
                error = true;
                $form.find('.ui.error.message ui').append(
                    $('<li/>').text('请填写' + $this.siblings('label').text())
                );
                $this.closest('.field').addClass('error');
            }
        });

        if (error) {
            $form.addClass('error');
            $('.ui.modal.add-provider-modal').modal('refresh');
        }
        return !error;
    }

    function submitForm(e) {
        var $form = $(e.target).closest('.ui.form');
        var $this = $(e.target);
        $form.removeClass('error');
        $form.find('.field').removeClass('error');
        $form.find('.ui.error.message ui').empty();
        var $inputs = $form.find('input:visible, textarea:visible');
        var valid = validator($form, $inputs);
        if (!valid) {
            return;
        }
        var model = {};
        $inputs.each(function (index, input) {
            model[$(input).data('db-key')] = $(input).val().trim();
        });
        if (!$this.hasClass('loading')) {
            $.ajax({
                url: "/Provider/AddProvider",
                method: 'post',
                beforeSend: function () {
                    $this.addClass('loading');
                },
                data: {
                    model: model,
                    __RequestVerificationToken: $form.find('input[name="__RequestVerificationToken"]').val()
                },
                success: function (data) {
                    if (data.status == 200) {
                        $form[0].reset();
                        $this.removeClass('loading');
                        $.tip(".message-container", "操作成功", "供应商已保存,请等待页面刷新", "positive", 4);
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    } else {
                        $this.removeClass('loading');
                        $.tip(".message-container", "保存失败", data.message, "negative", 4);
                    }

                },
                error: function (data) { $this.removeClass('loading'); $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); }
            })
        }
    }

    function initPage() {
        bindEvents();
    }

    initPage();
});