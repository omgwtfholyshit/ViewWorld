$(function () {
    var particles = true,
        particleDensity,
        options = {
            effectWeight: 1,
            outerBuffer: 1.08,
            elementDepth: 220
        };
    function InitAnimation() {
        $("#background").logosDistort(options);
        if (particles) {
            particleDensity = window.outerWidth * 7.5;
            if (particleDensity < 13000) {
                particleDensity = 13000;
            } else if (particleDensity > 20000) {
                particleDensity = 20000;
            }
            return $('#particle-target').particleground({
                dotColor: '#1ec5ee',
                lineColor: '#0a4e90',
                density: particleDensity.toFixed(0),
                parallax: false
            });
        }
    }
    function ValidateInput(username, password) {
        var errorMessage = '', status = true;
        //检测用户名
        if (username == '' || username == '输入邮箱或手机号登录') {
            status = false;
            errorMessage += '<li>用户名不能为空</li>'
        } else {
            if (!isNaN(username)) {
                if (!$.checkMobile(username)) {
                    status = false;
                    errorMessage += '<li>手机号错误</li>'
                }
            } else {
                if (!$.checkEmail(username)) {
                    status = false;
                    errorMessage += '<li>邮箱错误</li>';
                }
            }
        }
        //检测密码
        if (password == '') {
            status = false;
            errorMessage += '<li>密码不能为空</li>';
        }
        return data = { Status: status, Message: errorMessage };
    }
    function InitCaptcha(callback) {
        var $captcha = $('#CaptchaDiv');
        if ($captcha.data('required') == "True") {
            if ($captcha.hasClass('hidden')) {
                $captcha.removeClass('hidden')
            }
            callback();
        } 
    }
    function SubmitForm(username, password, verificationCode, rememberMe) {
        var loginModel = {
            UserName: username,
            Password: password,
            VerificationCode: verificationCode,
            RememberMe: rememberMe
        }, token = $('.ui.form input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/Account/UserLogin",
            method: 'post',
            data: {
                model: loginModel,
                __RequestVerificationToken: token,
            },
            success: function (data) {
                if (data.status == 200) {
                    location.href = data.data;
                } else if (data.status == 301) {
                    $('#CaptchaDiv').data('required',"True");
                    InitCaptcha(ResetCaptcha);
                    $('#error-message').css({ display: 'block' });
                    $('#error-message ul').html(data.message);
                } else {
                    $('#error-message').css({ display: 'block' });
                    $('#error-message ul').html(data.message);
                }

            },
            error: function (data) { $.tip("服务器超时，请稍后重试！"); }
        });
    }
    function ResetCaptcha(requestUrl) {
        if (typeof requestUrl == "undefined" || requestUrl.length < 10) {
            requestUrl = "../Account/GetLoginCaptcha"
        }
        requestUrl += '?' + Math.round(Math.random() * 10000);
        $('#captchaImage').attr('src', requestUrl);
    }
    function bindEvents() {
        $(document).keydown(function (event) {
            if (event.keyCode == 13 && ($("#password").is(":focus") || $('#verificationCode').is(':focus'))) {
                $("#login").click();
            }
        })
        $('#login').on('click', function (event) {
            var username = $('#username').val(), password = $('#password').val(), verificationCode = $('#verificationCode').val(),
            rememberMe = $('#rememberMe').prop('checked'), $errorMessage = $('#error-message ul');
            var result = ValidateInput(username,password);
            if (!result.Status) {
                $errorMessage.html(result.Message);
                $('#error-message').css({ display: 'block' });
                return false;
            } else {
                $('#error-message').css({ display: 'none' });
                SubmitForm(username, password, verificationCode, rememberMe);
            }
        });
        $('#captchaImage').on('click', function (event) { ResetCaptcha();})
    }
    function InitPage() {
        
        bindEvents();
        InitAnimation();
        InitCaptcha(ResetCaptcha);
    }
    InitPage();
})