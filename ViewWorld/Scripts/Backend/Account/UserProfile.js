$(function () {
    function bindEvents() {
        $('.user-role').popup({
            on: 'hover'
        })
        $('.user-profile .ui.card .image').dimmer({
            on: 'hover' 
        });
        $('#DOB').calendar({
            ampm: false,
            type: 'date',
            formatter: {
                date: function (date, settings) {
                    if (!date) return '';
                    var day = date.getDate();
                    var month = settings.text.monthsShort[date.getMonth()];
                    var year = date.getFullYear();
                    return year + '年' + month + day + '日';
                }
            }
        });
        $('.user-profile .edit-avatar').on('click', function () {
            $('.avatar-editor').modal('show');
        })
        $('.user-profile .ui.form').form({
            fields: {
                nickname: "empty",
                
            }
        })
    }
    function initAvatarEditor(logo_url) {
        logo_url = "/Images/IMG_0602.JPG";
        var options = {
            id: 'swf',
            src_url: logo_url,
            upload_url: '/Account/UploadUserAvatar', //上传接口
            method: 'post', //传递到上传接口中的查询参数的提交方式。更改该值时，请注意更改上传接口中的查询参数的接收方式
            src_upload: 0, //是否上传原图片的选项，有以下值：0-不上传；1-上传；2-显示复选框由用户选择
            avatar_box_border_width: 0,
            avatar_sizes: '200*200',
            tab_visible: false,
            avatar_intro: "预览",
            button_cancel_text: "更改图片",
            avatar_sizes_desc: '200*200'
        }
        var editor = new fullAvatarEditor(['*/FullAvatarEditor.swf'], ['*/expressInstall.swf'], 'editor', 380, 600, options, function (data) {
            console.log(data);
            editor.call('loadPic', data.content.avatarUrls[0]);
            //if (data.code == 5) {

            //    window.location.reload();
            //}
        });

    }
    function initPage() {
        initAvatarEditor();
        bindEvents();
    }
    initPage();
})