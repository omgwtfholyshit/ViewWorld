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
        $('.user-profile .change-mobile').on('click', function (e) {
            e.preventDefault();
            ResetCaptcha();
            $('.mobile-editor').modal('show');
        })
        $('.user-profile .get-code').on('click', function (e) {
            e.preventDefault();
            var phone = $(this).siblings().val();
            if ($.checkMobile(phone)) {
                $.get("/Account/GetMobileVerificationCode", {"mobileNumber":phone}, function (data) {
                    console.log(data);
                    if (data.status == 200) {
                        $.tip(".message-container", "发送成功", "验证码已发送,请注意查收", "positive", 4);
                        CountDown(60, "重新发送", CountDownCallback);
                    } else {
                        $.tip(".message-container", "获取验证码失败", data.message, "negative", 4);
                    }
                   
                })
            } else {
                $.tip(".message-container", "获取验证码失败", "您输入的手机号有误,请修改后再试.", "negative", 4);
            }
            
        })
        $('#captchaImage').on('click', function () {
            ResetCaptcha();
        })
        $('#submitForm').on('click', function (e) {
            e.preventDefault();
            UpdateUserInfo()
        })
        $('#submitMobile').on('click', function (e) {
            e.preventDefault();
            return UpdateUserMobile();
        })
    }
    function CountDown(timer, message, callback) {
        if (typeof +timer == "number") {
            if (timer == 0) {
                callback();
            } else {
                $('.get-code').html(message + "(" + timer + ")").attr('disabled', true);
                timer--;
                setTimeout(function () {
                    CountDown(timer, message, callback);
                }, 1000);
            }
        }
    }
    function CountDownCallback() {
        $('.get-code').html("获取验证码").attr('disabled', false);
    }
    function UpdateUserMobile() {
        var userInfo = {
            Mobile: $(".get-code").siblings().val(),
            VerificationCode: $('#mobileCode').val(),
            SetAsUserName: false,
        }, $submitMobile = $('#submitMobile');
        if (!$.checkMobile(userInfo.Mobile)) {
            $.tip(".message-container", "提交失败", "您输入的手机号有误,请修改后再试.", "negative", 4);
            return false;
        }
        if (userInfo.VerificationCode.length != 4) {
            $.tip(".message-container", "提交失败", "您输入验证码有误,请修改后再试.", "negative", 4);
            return false;
        }
        if (!$submitMobile.hasClass('loading')) {
            $.ajax({
                url: "/Account/UpdateUserMobile",
                method: 'post',
                beforeSend: function () {
                    $submitMobile.addClass('loading');
                },
                data: {
                    model: userInfo,
                    __RequestVerificationToken: $('.ui.form input[name="__RequestVerificationToken"]').val(),
                },
                success: function (data) {
                    if (data.status == 200) {
                        $submitMobile.removeClass('loading')
                        $('.change-mobile').siblings().val(userInfo.Mobile);
                        $.tip(".message-container", "保存成功", "您的手机号已更新", "positive", 4);
                    } else {
                        $submitMobile.removeClass('loading')
                        $.tip(".message-container", "保存失败", data.message, "negative", 4);
                        return false;
                    }

                },
                error: function (data) { $submitMobile.removeClass('loading'); $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); return false; }
            });
        } else {
            return false;
        }
    }
    function UpdateUserInfo() {
        var userInfo = {
            NickName: $('input[name=nickname]').val(),
            Sex: $('input:radio[name=sex]:checked').val(),
            DOB: $('input[name=dob]').val()
        }, $submit = $('#submitForm');
        if (userInfo.NickName == '' || userInfo.NickName.length > 20)
        {
            $.tip(".message-container", "昵称错误", "昵称不能为空或者超过20个字符哦~", "negative", 4);
            return;
        }
        if (userInfo.DOB == '') {
            $.tip(".message-container", "出生日期错误", "出生日期不能为空哦~", "negative", 4);
            return;
        }
        if (!$submit.hasClass('loading')) {
            $.ajax({
                url: "/Account/UpdateUserInfo",
                method: 'post',
                beforeSend: function () {
                    $('#submitForm').addClass('loading');
                },
                data: {
                    model: userInfo,
                    __RequestVerificationToken: $('.ui.form input[name="__RequestVerificationToken"]').val(),
                },
                success: function (data) {
                    if (data.status == 200) {
                        location.reload();
                    } else {
                        $submit.removeClass('loading');
                        $.tip(".message-container", "保存失败", data.message, "negative", 4);
                    }

                },
                error: function (data) { $submit.removeClass('loading'); $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); }
            });
        }
        
    }
    function GetUserSex() {
        var sex = 0, //0 = M, 1 = F , 2 = Unknown
            $userRole = $('.user-role');
        if ($userRole.hasClass('woman')) {
            sex = 1
        } else if ($userRole.hasClass('genderless')) {
            sex = 2;
        }
        $('input:radio[name=sex]')[sex].checked = true;
    }
    function initAvatarEditor(logo_url) {
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
            if (data.code == 5) {

                window.location.reload();
            }
        });
        $('#upload').on('click', function () {
            editor.call('upload');
        })
    }
    function ResetCaptcha(requestUrl) {
        if (typeof requestUrl == "undefined" || requestUrl.length < 10) {
            requestUrl = "../Account/GetMobileCaptcha"
        }
        requestUrl += '?' + Math.round(Math.random() * 10000);
        $('#captchaImage').attr('src', requestUrl);
    }
    function initPage() {
        initAvatarEditor($('.user-image').attr('src'));
        bindEvents();
        GetUserSex();
    }
    initPage();
})