$(function () {
    var particles = true,
        particleDensity,
        options = {
            effectWeight: 1,
            outerBuffer: 1.08,
            elementDepth: 220
        };
    function initAnimation() {
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
    function SubmitForm() {
        var username = $('#username').val(), $errorMessage = $('#error-message');
        if (!$.checkEmail(username)) {
            $errorMessage.html('<ul class="list"><li>邮箱错误</li></ul>');
            return false;
        } else if (!$.checkMobile(username)) {
            $errorMessage.html('<ul class="list"><li>手机号错误</li></ul>');
            return false;
        } else {
            return false;
        }
    }
    function bindEvents() {
        $('#login').on('click', function (event) {
            var username = $('#username').val(), password = $('#password').val(), $errorMessage = $('#error-message ul');
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
                        errorMessage +='<li>邮箱错误</li>';
                    }
                }
            }
            //检测密码
            if (password == '') {
                status = false;
                errorMessage += '<li>密码不能为空</li>';
            }
            if (!status) {
                $errorMessage.html(errorMessage);
                $('#error-message').css({ display: 'block' });
                return false;
            } else {
                $('#error-message').css({ display: 'none' });
                return false;
            }
        });
    }
    function initPage() {
        
        bindEvents();
        initAnimation();
    }
    initPage();
})