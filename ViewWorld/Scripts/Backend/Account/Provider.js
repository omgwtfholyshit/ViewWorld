;$(function () {
    function bindEvents () {
        $('.button.add-provider').on('click', function () {
            $('.ui.modal.add-provider-modal').modal('show');
        });
        $('.button#submit-form').on('click', function (e) {
            e.preventDefault();
            submitForm(e);
        });
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
        $inputs.each (function (index, input) {
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
                        $this.removeClass('loading');
                        $.tip(".message-container", "操作成功", "供应商已保存", "positive", 4);
                    } else {
                        $this.removeClass('loading');
                        $.tip(".message-container", "保存失败", data.message, "negative", 4);
                    }

                },
                error: function (data) { $this.removeClass('loading'); $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); }
            })
        }
    }
    
    function initPage () {
        bindEvents();
    }

    initPage();
});