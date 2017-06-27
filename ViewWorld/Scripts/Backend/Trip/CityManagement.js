$(function () {
    var history = { Name: '', Initial: '', isChineseCity: true };
    function bindEvents() {
        $('#addCity').on('click', function () {
            $('.ui.modal.add-city-modal').modal('show');
        });
        $('.button#submit-form').on('click', function (e) {
            e.preventDefault();
            submitForm(e);
        })
        $('input[name=alias]').on('keypress', function (e) {
            if (e.keyCode == '13') {
                if ($(e.target).val() != '')
                    $('#submit-form').click();
            }
        })
        $('.ui.celled.table tbody').delegate('.button.edit', 'click', function (e) {
            e.preventDefault();
            editCity(e);
        }).delegate('.button.delete', 'click', function (e) {
            e.preventDefault();
            var r = window.confirm("请确认删除操作");
            if (r) {
                deleteCity(e);
            }
        }).delegate('.button.save', 'click', function (e) {
            e.preventDefault();
            var r = window.confirm("确认保存");
            if (r) {
                saveCity(e);
            } else {
                var $this = $(e.target);
                if ($this.closest('tr').length > 0) {
                    var $row = $this.closest('tr');
                    $row.find('.editable-cell').attr('contenteditable', false);
                    $($row.find('.editable-cell')[0]).focus();
                    $($row.find('.editable-cell')[0]).text(history.Name);
                    $($row.find('.editable-cell')[1]).text(history.Initial);
                    $this.closest('.button.save').addClass('hidden').siblings('.button.edit').removeClass('loading hidden');
                }
            }
        })
        $('.header-left .search.icon').on('click', function (e) {
            searchCity($(e.target).siblings().val());
        })
        $('.header-left input').on('keypress', function (e) {
            if (e.keyCode == 13)
                $('.header-left .search.icon').click();
        })
    }

    function saveCity(e) {
        var $this = $(e.target).closest('.button.save');
        if ($this.hasClass('loading')) {
            return;
        }
        $this.addClass('loading');
        var $cells = $this.closest('tr').find('.editable-cell');

        var model = {
            Id: $this.closest('tr').data('city-id'),
            IsChineseCity: $this.closest('tr').find('input[name=IsChineseCity]').prop('checked')
        };
        $cells.each(function (index, cell) {
            model[$(cell).data('db-key')] = $(cell).text().trim();
        });
        
       
        $.ajax({
            url: '/Trip/EditCity',
            method: 'POST',
            data: {
                model: model,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"').val()
            },
            success: function (response) {
                if (response.Success) {
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

    function editCity(e) {

        // todo: edit provider
        var $this = $(e.target).closest('.button.edit');
        if ($this.hasClass('loading')) {
            return;
        }
        $this.addClass('loading');

        if ($this.closest('tr').length > 0) {
            var $row = $this.closest('tr');
            $row.find('.editable-cell').attr('contenteditable', true);
            history.Name = $($row.find('.editable-cell')[0]).text().trim();
            history.Initial = $($row.find('.editable-cell')[1]).text().trim();
            $($row.find('.editable-cell')[0]).focus();
            $this.addClass('hidden').siblings('.button.save').removeClass('hidden');
            console.log(history);
        }
    }

    function deleteCity(e) {
        var $this = $(e.target).closest('.button.delete');
        if ($this.hasClass('loading')) {
            return;
        }
        $this.addClass('loading');
        if ($this.closest('tr').length > 0) {
            $.ajax({
                url: "/Trip/DeleteCity",
                method: 'post',
                data: {
                    id: $this.closest('tr').data('city-id'),
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"').val()
                },
                success: function (response) {
                    if (response.Success) {
                        $.tip('.message-container', '删除成功', '城市已删除,请等待页面刷新', 'positive', 4);
                        $this.removeClass('loading');
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
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
    function searchCity(keyword) {
        if (keyword == 'undefined')
            keyword = '';
        $.get('/Trip/SearchCityByKeyword', { keyword: keyword }).done(function (data) {
            if (data.status == 200) {
                var html = '';
                $.each(data.data, function (index, element) {
                    html += '<tr data-city-id="' + element.Id + '"><td><div class="editable-cell" data-db-key="name">' + element.Name + '</div>ID: ' + element.Id + ' </td><td><div class="editable-cell" data-db-key="initial">' + element.Initial + '</div></td><td><input type="checkbox"  checked="' + element.IsChineseCity + '"/></td><td><button class="ui blue icon button save hidden"><i class="icon save"></i></button><button class="ui blue icon button edit"><i class="icon edit"></i></button><button class="ui red icon button delete"><i class="icon delete"></i></button></td></tr>';
                })
                $('.ui.celled.table tbody').html(html);
            }
        }).fail(function (xhr) {
            console.log(xhr);
        })
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
            $('.ui.modal.add-city-modal').modal('refresh');
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
        var model = { Id: "" };
        $inputs.each(function (index, input) {
            model[$(input).data('db-key')] = $(input).val().trim().toUpperCase();
        });
        $('.checkbox.cncity').checkbox('is checked') ? model.IsChineseCity = true : model.IsChineseCity = false;
        if (!$this.hasClass('loading')) {
            $.ajax({
                url: "/Trip/AddCity",
                method: 'post',
                beforeSend: function () {
                    $this.addClass('loading');
                },
                data: {
                    model: model,
                    __RequestVerificationToken: $form.find('input[name="__RequestVerificationToken"]').val()
                },
                success: function (data) {
                    if (data.Success) {
                        //$form[0].reset();
                        //$form[0][1].focus();
                        $this.removeClass('loading');
                        $.tip(".message-container", "操作成功", "供应商已保存", "positive", 4);
                        searchCity();
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
        $('.header-left').transition('fade in').removeClass('invisible');
        bindEvents();
    }

    initPage();
});