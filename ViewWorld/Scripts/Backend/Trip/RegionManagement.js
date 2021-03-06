﻿$(function () {
    var $table = $('#dataTable tbody'), $modalCatMenu = $('.region-editor .dropdown .menu'), $modalCatDropDown = $('.region-editor .dropdown'), globalVar = { Id: "", ParentRegionId: "", }, openedCategory = new Array();
    var api = {
        modalCategory: '/Trip/ListRegionsAPI',
        tablePartial: '/Trip/_PartialRegionTable',
        addRegion: '/Trip/AddRegion',
        deleteRegion: '/Trip/DeleteRegion',
        changeRegion: '/Trip/UpdateRegion',
    }
    function BindEvents() {
        $('#addRegion').on('click', function () {
            var $modal = $('.region-editor.add');
            if ($modal.hasClass('modifying')) {
                $modal.removeClass('modifying');
                $(".region-editor .ui.form")[0].reset();
            }
            $('.region-editor .ui.form .checkbox').checkbox('check');
            $modal.modal('show');
            LoadModalCategory();
        })
        $('#clear').on('click', function () {
            $('.header-left input').val('');
            openedCategory.splice(0);
            BuildTable();
        })
        $('.header-left .search.icon').on('click', function (e) {
            BuildTable($(e.target).siblings().val());
        })
        $('.header-left input').on('keyup', function () {
            $('.header-left .search.icon').click();
        })
        InitFormValidator();
        $('#submitForm').on('click', function () {
            $('.region-editor .ui.form').form('validate form');
        })
        $table.delegate('i.caret', 'click', function (e) {
            $target = $(e.target);
            if ($target.hasClass('open')) {
                $target.removeClass('open')
                $('.' + $target.closest('tr').data('id')).transition('fly up')
                openedCategory.splice(openedCategory.findIndex(function (element) { return element == '.' + $target.closest('tr').data('id');}), 1);
            } else {
                $target.addClass('open')
                $('.' + $target.closest('tr').data('id')).transition('fly down')
                openedCategory.push('.' + $target.closest('tr').data('id'));
            }
        }).delegate('button.delete', 'click', function (e) {
            var modal = $('.region-editor.confirm.modal'),$dataSource=$(e.target).closest('tr');
            modal.find('.content span').html($dataSource.data('name'));
            globalVar.Id = $dataSource.data('id');
            globalVar.ParentRegionId = $dataSource.data('parent');
            modal.modal('show');
        }).delegate('button.modify', 'click', function (e) {
            //LoadModalCategory();
            var $dataSource = $(e.target).closest('tr'), $form = $(".region-editor .ui.form");
            FillForm($dataSource, $form);
            $('.region-editor.add').addClass('modifying').modal('show')
        })
        $('.region-editor.confirm.modal .button.positive').on('click', function (e) {
            $.get(api.deleteRegion, { id: globalVar.Id, parentId: globalVar.ParentRegionId }, function (result) {
                if (result.Success) {
                    $.tip(".message-container", "删除成功", "分类已删除", "positive", 4);
                    BuildTable();
                } else {
                    $.tip(".message-container", "删除失败", result.Message, "negative", 4);
                }
            })
        })
    }
    function BuildTable(keyword) {
        var loadingHtml = '<tr class="center aligned"><td colspan="6" class="ui loading segment" height="150px"></td></tr>';
        if (typeof keyword == "undefiend")
            keyword = '';

        if (!$table.hasClass('loading')) {
            $.ajax({
                url: api.tablePartial,
                method: 'get',
                beforeSend: function () {
                    $table.addClass('loading');
                    $table.html(loadingHtml);
                },
                data: {
                    keyword: keyword,
                },
                success: function (data) {
                    $table.removeClass('loading');
                    $table.hide().html(data).transition('fade in');
                    $.each(openedCategory, function (index, ele) {
                        $(ele).transition('fade in');
                    })
                },
                error: function (data) { $table.find('td.loading').removeClass('loading').html("服务器超时，请稍后重试！"); $.tip(".message-container", "载入失败", "服务器超时，请稍后重试！", "negative", 4); }
            })
        }
    }
    function LoadModalCategory(callback) {
        $.ajax({
            url: api.modalCategory,
            success: function (response) {
                console.log(response);
                $modalCatDropDown.dropdown({
                    apiSettings: {
                        response: response
                    }
                }).removeClass('loading');
                if (typeof callback == "function")
                    callback();
            }
        });
    }
    function InitFormValidator() {
        $('.region-editor .ui.form').form({
            on : 'blur',
            fields: {
                Name: {
                    identifier: 'Name',
                    rules: [{ type: 'empty', prompt : '分类名称不能为空'}]
                },
                SortOrder: {
                    identifier: 'SortOrder',
                    rules: [{ type: 'regExp[^\\d+$]', prompt: '排序必须为非负整数' }]
                },
                EnglishName: {
                    identifier: 'EnglishName',
                    rules: [{ type: 'regExp[^[a-zA-Z ]+$]', prompt: '英文名称仅限英文及空格' }]
                },
                Initial: {
                    identifier: 'Initial',
                    rules: [{ type: 'regExp[^[a-zA-Z]$]', prompt: '分类标识仅限英文A-Z' }]
                }
            },
            onSuccess: function (event, fields) {
                return SubmitForm($(this));
            },
            onFailure: function (formErrors, fields) {
                console.log(formErrors)
            }
            
        })
    }
    function FillForm($dataSource,$form) {
        $form.find('input[name="Name"]').val($dataSource.data('name'));
        $form.find('input[name="SortOrder"]').val($dataSource.data('sort'));
        $form.find('input[name="EnglishName"]').val($dataSource.data('englishname'));
        $form.find('input[name="Initial"]').val($dataSource.data('initial'));
        globalVar.Id = $dataSource.data('id')
        globalVar.ParentRegionId = $dataSource.data('parent');
        if ($dataSource.data('isdisplay') == "True") {
            $form.find('.checkbox').checkbox('check');
        } else {
            $form.find('.checkbox').checkbox('uncheck');
        }
        if (typeof $dataSource.data('parentname') == "undefined" || $dataSource.data('parentname') == "-1") {
            $modalCatDropDown.find('input').click().after(function () { $modalCatDropDown.dropdown("set selected", "无") })
        } else {
            $modalCatDropDown.find('input').click();
            setTimeout(function () { $modalCatDropDown.dropdown("set selected", $dataSource.data('parentname')) },500);
        }
    }
    function SubmitForm($form) {
        var $inputs = $form.find('.parent,input:visible, textarea:visible,input[name=IsVisible]').not('.search'),$button = $('#submitForm');
        var model = {}, $input, outcome;
        $inputs.each(function (index, input) {
            $input = $(input);
            if ($input.data('db-key') == "IsVisible") {
                model[$input.data('db-key')] = $input.parent().checkbox('is checked');
            } else {
                model[$input.data('db-key')] = $input.val().trim();
            }
        });
        if (!$button.hasClass('loading')) {
            if ($('.region-editor.add').hasClass('modifying')) {
                model['Id'] = globalVar.Id
                $.ajax({
                    url: api.changeRegion,
                    method: 'post',
                    beforeSend: function () {
                        $button.addClass('loading');
                    },
                    data: {
                        model: model,
                        prevParentId: globalVar.ParentRegionId,
                        __RequestVerificationToken: $form.find('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (data) {
                        if (data.Success) {
                            $form[0].reset();
                            $button.removeClass('loading');
                            $.tip(".message-container", "操作成功", "分类已更新成功", "positive", 4);
                            BuildTable();
                        } else {
                            $button.removeClass('loading');
                            $.tip(".message-container", "保存失败", data.Message, "negative", 4);
                            return false;
                        }

                    },
                    error: function (data) { $button.removeClass('loading'); $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); }
                })
            } else {
                $.ajax({
                    url: api.addRegion,
                    method: 'post',
                    beforeSend: function () {
                        $button.addClass('loading');
                    },
                    data: {
                        model: model,
                        __RequestVerificationToken: $form.find('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (data) {
                        if (data.Success) {
                            $form[0].reset();
                            $button.removeClass('loading');
                            $.tip(".message-container", "操作成功", "新分类添加成功", "positive", 4);
                            BuildTable();
                            $('.ui.modal.region-editor').modal('refresh');
                            return false;
                        } else {
                            $button.removeClass('loading');
                            $.tip(".message-container", "保存失败", data.Message, "negative", 4);
                        }

                    },
                    error: function (data) { $button.removeClass('loading'); $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); }
                })
            }
            
        }
    }
    function InitPage() {
        $('.header-left').transition('fade in').removeClass('invisible');
        BuildTable();
        BindEvents();
        LoadModalCategory();
    }
    InitPage();
})