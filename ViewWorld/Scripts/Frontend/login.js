$(function () {
    var login = {
        init: function () {
            this.mode = 'login';
            this.api = {
                login: '/Account/UserLogin', register: '/Account/Register',
                getMobileCode: '/Account/GetMobileVerificationCode', getEmailCode: '/Account/GetEmailVerificationCode'
            };
            this.loginForm = $('.local-signin');
            this.registerForm = $('.local-register');
            this.token = $('input[name=__RequestVerificationToken]').val();
            this.termOfUse = $('.term-of-use');
            this.captchaBtn = $('#captchaImage');
            this.captchaDiv = $('#captchaDiv');
            this.verificationDiv = $('#VerificationCodeDiv');
            this.getVerificationBtn = $('#getCode');
            this.submitBtn = $('.submit-form.button');
            this.switchModeBtn = $('.switch-mode');
            this.switchMode();
            this.initCaptcha();
            this.resetCaptcha();
            this.initFormValidator();
            this.getVerificatetionCode();
            this.submit();
            this.onKeyDown();
        },
        switchMode: function () {
            var _this = this;
            _this.switchModeBtn.click(function (e) {
                e.preventDefault();
                if (_this.mode == 'login') {
                    _this.mode = 'register';
                    _this.loginForm.hide();
                    _this.registerForm.show();
                    _this.termOfUse.show();
                } else {
                    _this.mode = 'login';
                    _this.loginForm.show();
                    _this.registerForm.hide();
                    _this.termOfUse.hide();
                }
            })
        },
        initCaptcha: function () {
            var _this = this;
            if (_this.captchaDiv.data('required') == "True") {
                _this.captchaDiv.show();
            }
        },
        resetCaptcha: function (requestUrl) {
            var _this = this;
            if (typeof requestUrl == "undefined" || requestUrl.length < 10) {
                requestUrl = "../Account/GetLoginCaptcha"
            }
            requestUrl += '?' + Math.round(Math.random() * 10000);
            _this.captchaBtn.attr('src', requestUrl);
            _this.captchaBtn.click(function () {
                _this.resetCaptcha();
            })
        },
        initFormValidator: function () {
            var _this = this;
            _this.loginForm.find('.ui.form').form({
                on: 'blur',
                keyboardShortcuts:false,
                fields: {
                    UserName: {
                        rules: [{
                            type: 'empty',
                            prompt: '用户名不能为空'
                        }]
                    },
                    Password: {
                        rules: [{
                            type: 'empty',
                            prompt: '密码不能为空'
                        },{
                            type: 'minLength[6]',
                            prompt: '密码必须大于 {ruleValue} 位'
                        }
                        ]
                    },
                },
                onSuccess: function (event, fields) {
                    return true;
                },
                onFailure: function (formErrors, fields) {
                    return false;
                }
            })
            _this.registerForm.find('.ui.form').form({
                on: 'blur',
                keyboardShortcuts: false,
                fields: {
                    UserName: {
                        rules: [{
                            type: 'empty',
                            prompt: '用户名不能为空'
                        }]
                    },
                    Password: {
                        rules: [{
                            type: 'empty',
                            prompt: '密码不能为空'
                        }, {
                            type: 'minLength[6]',
                            prompt: '密码必须大于 {ruleValue} 位'
                        }
                        ]
                    },
                    VerificationCode: {
                        rules: [{
                            type: 'empty',
                            prompt: '验证码不能为空'
                        }, {
                            type: 'exactLength[4]',
                            prompt: '验证码是 {ruleValue} 位哦'
                        }
                        ]
                    }
                },
                onSuccess: function (event, fields) {
                    return true;
                },
                onFailure: function (formErrors, fields) {
                    return false;
                }
            })
        },
        getVerificatetionCode: function (e) {
            var _this = this, timer = 60, requestUrl = '';
            _this.getVerificationBtn.click(function (e) {
                e.preventDefault();
                var username = _this.registerForm.find('input[name=UserName]').val();
                if ($.checkEmail(username)) {
                    requestUrl = _this.api.getEmailCode;
                } else if ($.checkMobile(username)) {
                    requestUrl = _this.api.getMobileCode;
                } else {
                    _this.registerForm.find('.error.message').show();
                    _this.registerForm.find('.error.message ul').html("用户名必须为有效邮箱或手机");
                }
                if (!_this.getVerificationBtn.hasClass('loading')) {
                    $.ajax({
                        url: requestUrl,
                        method: 'get',
                        beforeSend: function () {
                            _this.getVerificationBtn.addClass('loading');
                        },
                        data: {
                            userName: username
                        },
                        success: function (data) {
                            if (data.status == 200) {
                                _this.getVerificationBtn.attr('disabled', true).text("重新获取(60)");
                                var setTime = setInterval(function () {
                                    timer--;
                                    if (timer < 10) {
                                        timer = "0" + timer;
                                    }
                                    _this.getVerificationBtn.text("重新获取(" + timer + ")");
                                    if (timer <= 0) {
                                        clearInterval(setTime);
                                        timer = 60;
                                        _this.getVerificationBtn.attr('disabled', false).text("获取验证码");
                                    }
                                }, 1000);

                            } else {
                                _this.registerForm.find('.error.message').show();
                                _this.registerForm.find('.error.message ul').html(data.message);
                            }
                            _this.getVerificationBtn.removeClass('loading');

                        },
                        error: function (data) { $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); _this.getVerificationBtn.removeClass('loading'); }
                    });
                }
            })
            
        },
        submit: function () {
            var _this = this;
            _this.submitBtn.click(function (e) {
                var $button = $(e.target);
                e.preventDefault();
                var model = {}, url = '', $input;
                if (!$button.hasClass('loading')) {
                    switch (_this.mode) {
                        case 'login':
                            url = _this.api.login;
                            if (!_this.loginForm.find('.ui.form').form("validate form")) {
                                return;
                            }
                            _this.loginForm.find('input').not('input[name=__RequestVerificationToken]').each(function (index, element) {
                                $input = $(element);
                                if ($input.attr('name') == 'RememberMe') {
                                    model[$input.attr('name')] = $input.parent('.checkbox').checkbox('is checked');
                                } else {
                                    model[$input.attr('name')] = $input.val();
                                }

                            })
                            break;
                        case 'register':
                            url = _this.api.register;
                            if (!_this.registerForm.find('.ui.form').form("validate form")) {
                                return;
                            }
                            _this.registerForm.find('input').each(function (index, element) {
                                $input = $(element);
                                model[$input.attr('name')] = $input.val();
                            })
                            break;
                        default:
                            break;
                    }
                    $.ajax({
                        url: url,
                        method: 'post',
                        beforeSend: function () {
                            $button.addClass('loading');
                        },
                        data: {
                            model: model,
                            returnUrl: decodeURIComponent($.getQueryStringByName("ReturnUrl")),
                            __RequestVerificationToken: _this.token,
                        },
                        success: function (data) {
                            if (_this.mode == 'login') {
                                if (data.status == 200) {
                                    location.href = data.data;
                                } else if (data.status == 301) {
                                    _this.captchaDiv.data('required', "True");
                                    _this.initCaptcha();
                                    _this.resetCaptcha();
                                    _this.loginForm.find('.error.message').show();
                                    _this.loginForm.find('.error.message ul').html(data.message);
                                } else {
                                    _this.resetCaptcha();
                                    _this.loginForm.find('.error.message').show();
                                    _this.loginForm.find('.error.message ul').html(data.message);
                                }
                            } else if (_this.mode == 'register') {
                                if (data.status == 200) {
                                    location.href = data.data;
                                } else {
                                    _this.registerForm.find('.error.message').show();
                                    _this.registerForm.find('.error.message ul').html(data.message);
                                }
                            }
                            $button.removeClass('loading');

                        },
                        error: function (data) { $.tip(".message-container", "保存失败", "服务器超时，请稍后重试！", "negative", 4); $button.removeClass('loading'); }
                    });
                }
            })
            return false;
        },
        onKeyDown: function () {
            var _this = this;
            $(document).keydown(function (event) {
                if (event.keyCode == 13 && $("input").is(":focus")) {
                    switch (_this.mode) {
                        case 'login':
                            _this.loginForm.find('.submit-form.button').click();
                            break;
                        case 'register':
                            _this.registerForm.find('.submit-form.button').click();
                            break;
                        default:
                            break;
                    }
                }
            })
        }
    }
    login.init();
})