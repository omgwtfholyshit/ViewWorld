$(function () {
    var $table = $('#dataTable tbody'), $modalCatMenu = $('.scenery-editor .dropdown .menu'), $modalCatDropDown = $('.scenery-editor .dropdown'), $sceneryPhotos = $('#photoList'),
        globalVar = {
            Id: "", fileUpload: "", ParentSceneryId: "",
            reset: function () {
                var _this = this;
                this.Id = "";
                this.ParentSceneryId = "";
            }
        };
    var E = window.wangEditor, editor = new E('#descEditor');
    editor.create();
    var api = {
        modalCategory: '/Trip/ListRegionsAPI?displaySubRegions=true',
        tablePartial: '/Trip/_PartialSceneryTable',
        addScenery: '/Trip/AddScenery',
        deleteScenery: '/Trip/DeleteScenery',
        changeScenery: '/Trip/UpdateScenery',
        photos: '/Trip/ListSceneryPhotos',
        deletePhotos: '/Trip/DeleteSceneryPhotoByFileName',
        getSceneryDesc:'/Trip/GetSceneryDescription'
    }
    function BindEvents() {
        $('#addScenery').on('click', function () {
            var $modal = $('.scenery-editor.add');
            if ($modal.hasClass('modifying')) {
                $modal.removeClass('modifying');
                $(".scenery-editor .ui.form")[0].reset();
                globalVar.reset();
                $sceneryPhotos.html("");
                editor.txt.clear();
            } 
            $('.scenery-editor .ui.form .checkbox').checkbox('check');
            $modal.modal('show');
        })
        $('#clear').on('click', function () {
            $('.header-left input').val('');
            BuildTable();
        })
        $('.header-left .search.icon').on('click', function (e) {
            BuildTable($(e.target).siblings().val());
        })
        $('.header-left input').on('keyup', function (e) {
            $('.header-left .search.icon').click();
        })
        InitFileUpload();
        InitFormValidator();
        $('#submitForm').on('click', function () {
            $('.scenery-editor .ui.form').form('validate form');
        })
        $table.delegate('button.delete', 'click', function (e) {
            var modal = $('.scenery-editor.confirm.modal'),$dataSource=$(e.target).closest('tr');
            modal.find('.content span').html($dataSource.data('name'));
            modal.modal('show');
        })
        .delegate('button.modify', 'click', function (e) {
            var $dataSource = $(e.target).closest('tr'), $form = $(".scenery-editor .ui.form");
            globalVar.Id = $dataSource.data('id');
            var uploadUrl = '/Trip/UploadSceneryPhotos?sceneryId=' + globalVar.Id;
            $('#fileUpload').fileupload({ url: uploadUrl });
            InitFileUpload();
            LoadSceneryPhotos();
            FillForm($dataSource, $form);
            $('.scenery-editor.add').addClass('modifying').modal('show');
            setTimeout(function () { $('.scenery-editor.add').modal('refresh'); },100)
            })
        $sceneryPhotos
            .delegate('.sceneryPhoto .delete.label', 'click', function (e) {
                var photoName = $(e.target).parents('li').data('photoname');
                DeleteSceneryPhoto(photoName);
            })
    }
    function InitFileUpload() {
        $('#fileUpload').fileupload({
            dataType: 'json',
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png|bmp)$/i,
            maxFileSize: 999000,
            disableImageResize: false,
            imageMaxWidth: 800,
            imageMaxHeight: 800,
            imageCrop: true,
            singleFileUploads: true,
            limitMultiFileUploads: 5,
            done: function (e, data) {
                //console.log(data.result);
                var result = data.result;
                var fileNames;
                if (result.status == 300) {
                    if (result.message.indexOf(',') != -1) {
                        fileNames = result.message.split(',');
                    }
                }
                $.each(data.files, function (index, file) {
                    if (typeof fileNames != "undefined" && fileNames.indexOf(file.name) != -1) {
                        $('<p/>').text(file.name + "上传失败").appendTo(data.context);
                    } else {
                        $('<p/>').text(file.name).appendTo(data.context);
                    }
                });
                LoadSceneryPhotos();
                //data.context.text('Upload finished.');
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                //console.log(data.files[0].name + "     " + progress);
                //console.log(data);
            },
            progressall: function (e, data) {
                //console.log(e);
                //console.log(data);
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#progress .progress-bar').css(
                    'width',
                    progress + '%'
                );
            },

        }).bind('fileuploadadd', function (e, data) {
            if (globalVar.Id == "") {
                $.tip(".message-container", "请先保存", "必须保存完后才能上传图片！", "warning", 4);
            } else {
                data.submit();
            }
            
        });
    }
    function BuildTable(keyword) {
        var loadingHtml = '<tr class="center aligned"><td colspan="9" class="ui loading segment" height="150px"></td></tr>';
        if (typeof keyword == "undefined")
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
            onChange: function (value, text, $choice) {
                globalVar.ParentSceneryId = value;
            }
        })
    }
    function LoadSceneryPhotos() {
        $.ajax({
            url: api.photos,
            method: 'get',
            beforeSend: function () {
               // console.log(globalVar.Id);
            },
            data: {
                sceneryId: globalVar.Id,
            },
            success: function (data) {
                //console.log(data);
                var html = "";
                if (data.status == 200) {
                    if (data.data.length > 0) {
                        $.each(data.data, function (index, element) {
                            html += '<li class="sceneryPhoto" data-photoName="' + element.split('/')[4] +'" style="background:url(' + element + ')no-repeat;background-position: center;background-size: cover;"><a class="ui red right corner delete label"><i class="trash outline icon" ></i ></a ></li>';
                        })
                    }
                }
               // html += '<li class="sceneryPhoto add-photo" style="background:url(/Images/DefaultImages/add.png) no-repeat;background-position: center;background-size: contain;"></li>';
                $sceneryPhotos.html(html);
                $('.scenery-editor.add').modal('refresh');
            },
            error: function (data) {
                //console.log(data);
            }
        })
    }
    function DeleteSceneryPhoto(photoName) {
        $.ajax({
            url: api.deletePhotos,
            method: 'post',
            beforeSend: function () {
                //console.log(globalVar.Id);
            },
            data: {
                sceneryId: globalVar.Id,
                fileName: photoName,
                __RequestVerificationToken: $('.scenery-editor .ui.form').find('input[name="__RequestVerificationToken"]').val()
            },
            success: function (data) {
                //console.log(data)
                if (data.Success) {
                    LoadSceneryPhotos();
                } else {
                    $.tip(".message-container", "删除图片失败", data.Message, "negative", 4);
                }
            },
            error: function (data) {
                $.tip(".message-container", "服务器没有响应", "服务器连接超时，请稍后再试", "negative", 4);
            }
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
    function FillForm($dataSource, $form) {
        $.get(api.getSceneryDesc, { sceneryId: globalVar.Id }).done(function (data) {
            if (data.status == 200) {
                //console.log(editor)
                editor.txt.html($.htmlDecode(data.data))
            }
        }).fail(function () {
            $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4);
            })
        $form.find('input[name="Name"]').val($dataSource.data('name'));
        $form.find('input[name="SortOrder"]').val($dataSource.data('sort'));
        $form.find('input[name="EnglishName"]').val($dataSource.data('englishname'));
        $form.find('input[name="Initial"]').val($dataSource.data('initial'));
        if ($dataSource.data('isdisplay') == "True") {
            $form.find('.checkbox').checkbox('check');
        } else {
            $form.find('.checkbox').checkbox('uncheck');
        }
        globalVar.ParentSceneryId = $dataSource.data('regionid');
        if (typeof globalVar.ParentSceneryId == "undefined" || globalVar.ParentSceneryId == "-1") {
            $modalCatDropDown.find('input').click();
            setTimeout(function () { $modalCatDropDown.dropdown("set selected", "无") },200)
        } else {
            $modalCatDropDown.find('input').click();
            setTimeout(function () { $modalCatDropDown.dropdown("set selected", globalVar.ParentSceneryId) }, 200)
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
        model.Description = $.htmlEncode(editor.txt.html());
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
                            $.tip(".message-container", "操作成功", "分类已更新成功", "positive", 4);
                            BuildTable();
                        } else {
                            $.tip(".message-container", "保存失败", data.Message, "negative", 4);
                            return false;
                        }
                        $button.removeClass('loading');
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