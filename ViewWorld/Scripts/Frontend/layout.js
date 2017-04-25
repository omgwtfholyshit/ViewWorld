$(function () {
    var layout = {
        init: function () {
            this.api = {
                getUserProfile: '/Account/GetUserInfo',
            },
            this.authorised = function () {
                var _this = this;
                return !this.rightMenu.find('.nav-item').text().indexOf('登录') == 0;
            };
            this.isSmallDevice = browser.versions().smallDevice;
            this.nav = $('.nav');
            this.navMenu = this.nav.find('.nav-menu');
            this.leftMenu = this.nav.find('.nav-menu.left-menu');
            this.rightMenu = this.nav.find('.nav-menu.right-menu');
            this.menuBtn = $('.menu-toggle');
            this.rightMenuEvents();
            this.toggleMenu();
            this.navigate();
            this.resize();
            this.setActivePage();
            this.getUserProfile();
        },
        rightMenuEvents: function () {
            var _this = this;
            _this.rightMenu.delegate('.log-out','click', function () {
                $.post("/Account/LogOff", null, function (data, status) {
                    if (data.status == 200) {
                        location.href = "/";
                    } else {
                        $.tip(".message-container", "服务器无响应", "服务器暂无响应,请稍候再试.", "negative", 4)
                    }
                });
            })
        },
        toggleMenu: function (e) {
            var _this = this;
            _this.menuBtn.click(function (e) {
                if (_this.isSmallDevice) {
                    _this.menuBtn.toggleClass('active');
                    _this.menuBtn.hasClass('active') ? _this.navMenu.css('display', 'block') : _this.navMenu.css('display', 'none');
                } else {
                    _this.menuBtn.removeClass('active');
                    _this.navMenu.css('display', 'inline-block');
                }
            })
        },
        navigate: function () {
            var _this = this;
            _this.navMenu.find('.nav-item').click(function (e) {
                $target = $(e.target);
                if (typeof $target.data('url') !== 'undefined')
                    location.href = $(e.target).data('url');
            });
            _this.nav.on('blur', function () {
                _this.resize();
            })
        },
        resize: function () {
            var _this = this;
            _this.menuBtn.removeClass('active');
            _this.isSmallDevice ? this.navMenu.css('display', 'none') : this.navMenu.css('display', 'inline-block');
        },
        setActivePage: function () {
            var _this = this, text = '', $li;
            switch (location.pathname.toLowerCase()) {
                case '/':
                case '/home/index':
                    text = '首页';
                    break;
                default:
                    break;
            }
            _this.leftMenu.find('.nav-item').each(function (index, element) {
                $li = $(element);
                if($li.text().trim() == text)
                    return $li.addClass('active')
            })
        },
        getUserProfile: function () {
            var _this = this;
            if (_this.authorised()) {
                $.get(_this.api.getUserProfile, null, function (data) {
                    if (data.status == 200) {
                        _this.rightMenu.find('.nickname').html(data.data.Nickname);
                        _this.rightMenu.find('.avatar.image').attr('src', data.data.Avatar);
                        _this.rightMenu.find('.compact.menu').transition({
                            animation: 'fade in',
                            displayType : 'inline-flex'
                        });
                    } else {
                        $.tip(".message-container", data.message, "请重新登陆,4秒后为您跳转....", "negative", 4);
                        setTimeout(function () {
                            location.href = "../Account/Login?returnUrl=" + location.pathname;
                        }, 4000)
                    }
                })
            }
        }
    }
    window.onresize = function () {
        layout.isSmallDevice = browser.versions().smallDevice;
        layout.resize();
    }
    layout.init();
})