$(function () {
    var $table = $('#dataTable tbody'), $modalCatMenu = $('.scenery-editor .dropdown .menu'), $modalCatDropDown = $('.scenery-editor .dropdown'), $sceneryPhotos = $('#photoList'),
        globalVar = { Id: "", fileUpload: "",imageUrl:"" ,prevButton:""};
    var api = {
        modalCategory: '/Trip/ListRegionsAPI?displaySubRegions=true',
        tablePartial: '/Trip/_PartialSceneryTable',
        addScenery: '/Trip/AddScenery',
        deleteScenery: '/Trip/DeleteScenery',
        changeScenery: '/Trip/UpdateScenery',
        photos: '/Trip/ListSceneryPhotos',
        deletePhoto: '/Trip/DeleteSceneryPhotoByFileName',
    }
    function BindEvents() {
        $('#addScenery').on('click', function () {
            var $modal = $('.scenery-editor.add');
            if ($modal.hasClass('modifying')) {
                $modal.removeClass('modifying');
                $(".scenery-editor .ui.form")[0].reset();
            }
            $('#photoList').html("");
            $('#photoListContainer').parent().hide();
            CleanUploadData();
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
        $('.scenery-editor.confirm.modal').delegate('.positive.button', 'click', function (e) {
            var $modal = $(e.delegateTarget);
            if ($modal.hasClass('delete-photo')) {
                $.ajax({
                    url: api.deletePhoto,
                    method: 'delete',
                    data: {
                        sceneryId: globalVar.Id,
                        fileName:globalVar.imageUrl
                    },
                    success: function (data) {
                        $modal.removeClass('delete-photo');
                        if (data.Success) {
                            LoadSceneryPhtotos();
                            $(globalVar.prevButton).click();
                        }
                            
                        
                    },
                    error: function (data) {  $.tip(".message-container", "删除图片失败", "服务器超时，请稍后重试！", "negative", 4); }
                })
            } else if ($modal.hasClass('delete-scenery')) {
                $.ajax({
                    url: api.deleteScenery,
                    method: 'delete',
                    data: {
                        id: globalVar.Id,
                    },
                    success: function (data) {
                        $modal.removeClass('delete-scenery');
                        BuildTable();
                    },
                    error: function (data) { $.tip(".message-container", "删除图片失败", "服务器超时，请稍后重试！", "negative", 4); }
                })
            }
        }).delegate('.negative.button', 'click', function (e) {
            var $modal = $(e.delegateTarget);
            if ($modal.hasClass('delete-photo')) {
                $modal.removeClass('delete-photo');
                $(globalVar.prevButton).click();
            } else if($modal.hasClass('delete-scenery')){
                $modal.removeClass('delete-scenery');
            }
        })
        $('#photoList').delegate('.trash.outline.icon', 'click', function (e) {
            var $modal = $('.scenery-editor.confirm.modal');
            $modal.addClass('delete-photo');
            var urlArray = $(e.target).closest('li').css('backgroundImage').split('/');
            var imageUrl = urlArray[urlArray.length - 1].replace('")', '');
            globalVar.imageUrl = imageUrl;
            $modal.find('.content span').html(imageUrl);
            $modal.modal('show');
        });
        InitFormValidator();
        $('#submitForm').on('click', function () {
            $('.scenery-editor .ui.form').form('validate form');
            if (!$('.scenery-editor.add').hasClass('modifying'))
            {
                $('.scenery-editor.add').modal('refresh');
                return false;
            }
                
        })
        $table.delegate('button.delete', 'click', function (e) {
            var $modal = $('.scenery-editor.confirm.modal'), $dataSource = $(e.target).closest('tr');
            $modal.addClass('delete-scenery');
            $modal.find('.content span').html($dataSource.data('name'));
            globalVar.Id = $dataSource.data('id');
            $modal.modal('show');
        }).delegate('button.modify', 'click', function (e) {
            var $dataSource = $(e.target).closest('tr'), $form = $(".scenery-editor .ui.form");
            globalVar.Id = $dataSource.data('id');
            globalVar.prevButton = e.target;
            var uploadUrl = '/Trip/UploadSceneryPhotos?sceneryId=' + globalVar.Id;
            $('#fileUpload').fileupload({ url: uploadUrl });
            LoadSceneryPhtotos();
            var photoDisplayStatus = $('#photoListContainer').parent().css('display');
            if (photoDisplayStatus == 'none')
                $('#photoListContainer').parent().show();
            FillForm($dataSource, $form);
            $('.scenery-editor.add').addClass('modifying').modal('refresh').modal('show');
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
                
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                var originFiles = data.originalFiles;
                var processingId = 0;
                if (originFiles.length > 1) {
                    var fileName = data.files[0].name;
                    $.each(originFiles, function (index, ele) {
                        if (ele.name == fileName)
                            processingId = ele.uniqueId;
                    })
                    
                } else {
                    processingId = data.originalFiles[0].uniqueId;
                }
                $('#' + processingId).find('.single-progress-text span').text(progress + '%');
                $('#' + processingId).find('.single-progress').css({ 'margin-left': progress + '%' });
                if(progress == 100)
                    $('#' + processingId).addClass('upload-success');
            },
            progressall: function (e, data) {
                //console.log(e);
                //console.log(data);
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#progress .progress-bar').css(
                    'width',
                    progress + '%'
                );
                if (progress == 100)
                    LoadSceneryPhtotos();
            },

        }).bind('fileuploadadd', function (e, data) {
            var count = 0;
            $.each(data.files, function (index, file) {
                loadImage(file, function (img) {
                    var li = document.createElement('li');
                    var uniqueId = img.src.split('/')[3];
                    var progressBar = '<div class="progress-container" id=' + uniqueId + '><div class="single-progress"></div><div class="single-progress-text"><span>0%</span></div>';
                    li.className = 'sceneryPhoto';
                    $(li).append(img).append(progressBar);
                    file.uniqueId = uniqueId;
                    $('#pendingList').append(li);
                    $('.scenery-editor.add').modal('refresh');
                }, {
                    maxWidth: 200
                });
                if (++count === data.files.length) {
                    data.submit();
                }
            })
        })
    }
    function CleanUploadData() {
        $('#pendingList').html("");
        $('#progress .progress-bar').css(
                    'width',
                    0 + '%'
                );
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
        })
    }
    function LoadSceneryPhtotos() {
        $.ajax({
            url: api.photos,
            method: 'get',
            beforeSend: function () {
            },
            data: {
                sceneryId: globalVar.Id,
            },
            success: function (data) {
                var html = "";
                if (data.status == 200) {
                    if (data.data.length > 0) {
                        $.each(data.data, function (index, element) {
                            html += '<li class="sceneryPhoto" style="background:url(' + element + ')no-repeat;background-position: center;background-size: contain;"><span><i class="trash outline icon"></i></span></li>';
                        })
                    }
                }
                var dtd = $.Deferred();
                var writeHtml = function (dtd) {
                    var task = function () {
                        $sceneryPhotos.html(html);
                        dtd.resolve();
                    }
                    setTimeout(task, 200);
                    return dtd;
                }
                $.when(writeHtml(dtd)).done(function () {
                    CleanUploadData();
                    $('.scenery-editor.add').modal('refresh');
                })
            },
            error: function (data) {
                console.log(data);
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
                return false;
            }

        })
    }
    function FillForm($dataSource,$form) {
        $form.find('input[name="Name"]').val($dataSource.data('name'));
        $form.find('input[name="SortOrder"]').val($dataSource.data('sort'));
        $form.find('input[name="EnglishName"]').val($dataSource.data('englishname'));
        $form.find('input[name="Initial"]').val($dataSource.data('initial'));
        $form.find('input[name="Address"]').val($dataSource.data('address'));
        if (typeof $dataSource.data('regionid') == "undefined") {
            $modalCatDropDown.find('input').click().after(function () { $modalCatDropDown.dropdown("set selected", "无") })
        } else {
            $modalCatDropDown.find('input').click().after(function () { $modalCatDropDown.dropdown("set selected", $dataSource.data('regionid')) })
        }
    }
    function SubmitForm($form) {
        var $inputs = $form.find('.parent,input:visible, textarea:visible,input[name=IsVisible]').not('.search'),$button = $('#submitForm');
        var model = {}, $input, outcome;
        $inputs.each(function (index, input) {
            $input = $(input);
            model[$input.data('db-key')] = $input.val().trim();
            
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
                            globalVar.Id = data.Message;
                            var uploadUrl = '/Trip/UploadSceneryPhotos?sceneryId=' + globalVar.Id;
                            $('#fileUpload').fileupload({ url: uploadUrl });
                            $('#photoListContainer').parent().show();
                            $('.scenery-editor.add').addClass('modifying');
                            $button.removeClass('loading');
                            $.tip(".message-container", "操作成功", "新分类添加成功,您可以继续上传图片", "positive", 4);
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
        InitFileUpload();
        LoadModalCategory();
        
    }
    InitPage();
})