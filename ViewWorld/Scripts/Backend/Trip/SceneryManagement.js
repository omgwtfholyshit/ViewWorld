$(function () {
    var $table = $('#dataTable tbody'), $modalCatMenu = $('.scenery-editor .dropdown .menu'), $modalCatDropDown = $('.scenery-editor .dropdown'), globalVar = { Id: "", ParentSceneryId: "", }, openedCategory = new Array();
    var api = {
        modalCategory: '/Trip/ListRegionsAPI?displaySubRegions=true',
        tablePartial: '/Trip/_PartialSceneryTable',
        addScenery: '/Trip/AddScenery',
        deleteScenery: '/Trip/DeleteScenery',
        changeScenery: '/Trip/UpdateScenery',
    }
    function BindEvents() {
        $('#addScenery').on('click', function () {
            var $modal = $('.scenery-editor.add');
            if ($modal.hasClass('modifying')) {
                $modal.removeClass('modifying');
                $(".scenery-editor .ui.form")[0].reset();
            }
            $('.scenery-editor .ui.form .checkbox').checkbox('check');
            $modal.modal('show');
        })
        $('#clear').on('click', function () {
            $('.header-left input').val('');
            openedCategory.splice(0);
            BuildTable();
        })
        $('.header-left .search.icon').on('click', function (e) {
            BuildTable($(e.target).siblings().val());
        })
        $('.header-left input').on('keyup', function (e) {
            $('.header-left .search.icon').click();
        })
        InitFormValidator();
        $('#submitForm').on('click', function () {
            $('.scenery-editor .ui.form').form('validate form');
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
            var modal = $('.scenery-editor.confirm.modal'),$dataSource=$(e.target).closest('tr');
            modal.find('.content span').html($dataSource.data('name'));
            globalVar.Id = $dataSource.data('id');
            globalVar.ParentSceneryId = $dataSource.data('parent');
            modal.modal('show');
        }).delegate('button.modify', 'click', function (e) {
            var $dataSource = $(e.target).closest('tr'), $form = $(".scenery-editor .ui.form");
            FillForm($dataSource, $form);
            $('.scenery-editor.add').addClass('modifying').modal('show')
        })
        $('.scenery-editor.confirm.modal .button.positive').on('click', function (e) {
            $.get(api.deleteScenery, { id: globalVar.Id, parentId: globalVar.ParentSceneryId }, function (result) {
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
        var loadingHtml = '<tr class="center aligned"><td colspan="9" class="ui loading segment" height="150px"></td></tr>';
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
    function LoadModalCategory() {
        $modalCatDropDown.dropdown({
            apiSettings: {
                url: api.modalCategory,
                saveRemoteData: false,
            },
        })
    }
    function InitFormValidator() {
        $('.scenery-editor .ui.form').form({
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
        globalVar.ParentSceneryId = $dataSource.data('parent');
        if ($dataSource.data('isdisplay') == "True") {
            $form.find('.checkbox').checkbox('check');
        } else {
            $form.find('.checkbox').checkbox('uncheck');
        }
        if (typeof $dataSource.data('parentname') == "undefined" || $dataSource.data('parentname') == "-1") {
            $modalCatDropDown.find('input').click().after(function () { $modalCatDropDown.dropdown("set selected", "无") })
        } else {
            $modalCatDropDown.find('input').click().after(function () { $modalCatDropDown.dropdown("set selected", $dataSource.data('parentname')) })
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
            if ($('.scenery-editor.add').hasClass('modifying')) {
                model['Id'] = globalVar.Id
                $.ajax({
                    url: api.changeScenery,
                    method: 'post',
                    beforeSend: function () {
                        $button.addClass('loading');
                    },
                    data: {
                        model: model,
                        prevParentId: globalVar.ParentSceneryId,
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
                    url: api.addScenery,
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
                            $('.ui.modal.scenery-editor').modal('refresh');
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