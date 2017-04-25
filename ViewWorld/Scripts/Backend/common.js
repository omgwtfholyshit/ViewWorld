(function () {
    var broswer = {
        versions: function () {
            var u = navigator.userAgent, app = navigator.appVersion;
            return {     //移动终端浏览器版本信息
                trident: u.indexOf('Trident') > -1, //IE内核
                presto: u.indexOf('Presto') > -1, //opera内核
                webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
                gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
                mobile: !!u.match(/AppleWebKit.*Mobile.*/), //是否为移动终端
                ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或uc浏览器
                iPhone: u.indexOf('iPhone') > -1, //是否为iPhone或者QQHD浏览器
                iPad: u.indexOf('iPad') > -1, //是否iPad
                webApp: u.indexOf('Safari') == -1, //是否web应该程序，没有头部与底部
                smallDevice: window.innerWidth < 767 //窗口尺寸小于767px判定为小尺寸设备
            };
        },
        language: (navigator.browserLanguage || navigator.language).toLowerCase(),
        device: function () {
            var versions = this.versions();
            if (versions.mobile) {
                var ua = navigator.userAgent.toLowerCase();//获取判断用的对象
                if (ua.match(/MicroMessenger/i) == "micromessenger") {
                    return 'weixin';
                }
                if (ua.match(/WeiBo/i) == "weibo") {
                    return 'weibo';
                }
                if (ua.match(/QQ/i) == "qq") {
                    return 'qq';
                }
                if (versions.ios) {
                    return 'ios';
                }
                if (versions.android) {
                    return 'android';
                }
                return 'unknown';
            }
            return 'desktop';
        },
        checkBrowser:function(){
            if (navigator.userAgent.indexOf("MSIE 6.0") > 0 || navigator.userAgent.indexOf("MSIE 7.0") > 0 || navigator.userAgent.indexOf("MSIE 8.0") > 0 || navigator.userAgent.indexOf("MSIE 9.0") > 0) {
                alert("您的浏览器版本过低，为了保护您的个人安全及正常浏览本网站，建议您使用谷歌浏览器");

                window.open("https://www.baidu.com/s?ie=utf-8&f=3&rsv_bp=0&rsv_idx=1&tn=baidu&wd=%E8%B0%B7%E6%AD%8C%E6%B5%8F%E8%A7%88%E5%99%A8&rsv_pq=803e51460000430b&rsv_t=22d7eLk9jA1QOOunHmBKy7mcgXfU7GRDw7uiGYDlDd%2BbX4RGeCxdjdpFOdQ&rqlang=cn&rsv_enter=1&rsv_sug3=6&rsv_sug1=5&rsv_sug7=100&rsv_sug2=0&prefixsug=guge&rsp=1&inputT=2219&rsv_sug4=2219");
            }
        },
        init: function () {
            window.browser = this;
            browser.checkBrowser();
        }

    }
    function tip(selector, title, content, type, dismiss) {
        //type: Warning,Info,Positive,Negative 
        var id = Math.round(Math.random() * 10000);
        html = '<div class = "ui message hidden ' + type + '" id = "' + id + '" >';
        html += '<i class="close icon"></i>';
        html += '<div class="header">' + title + '</div>';
        html += '<p>' + content + '</p></div>';
        $(selector).append(html);
        $('#' + id).transition('vertical flip');
        if (typeof dismiss == 'number') {
            setTimeout(function () {
                $('#' + id).transition('fade');
            }, dismiss * 1000)
        }
    }

    function loadjscssfile(filename, filetype) {

        if (filetype == "js") {
            var fileref = document.createElement('script');
            fileref.setAttribute("type", "text/javascript");
            fileref.setAttribute("src", filename);
        } else if (filetype == "css") {

            var fileref = document.createElement('link');
            fileref.setAttribute("rel", "stylesheet");
            fileref.setAttribute("type", "text/css");
            fileref.setAttribute("href", filename);
        }
        if (typeof fileref != "undefined") {
            document.getElementsByTagName("head")[0].appendChild(fileref);
        }

    }
    function checkEmail(email) {
        return /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(email);
    }
    function checkMobile(mobile) {
        return /^0?(13|14|15|17|18)[0-9]{9}$/.test(mobile);
    }
    function validator($form, $inputs) {
        var error = false;

        $inputs.each(function () {
            var $this = $(this);
            if ($this.closest('.field').hasClass('required') && $this.val().trim().length == 0) {
                error = true;
                $form.find('.ui.error.message ui').append(
                    $('<li/>').text('请填写' + $this.siblings('label').text())
                );
                $this.closest('.field').addClass('error');
            }
        });

        if (error) {
            $form.addClass('error');
            $('.ui.modal.add-provider-modal').modal('refresh');
        }
        return !error;
    }
    //获取QueryString的数组

    function getQueryString() {

        var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));

        if (result == null) {

            return "";

        }

        for (var i = 0; i < result.length; i++) {

            result[i] = result[i].substring(1);

        }

        return result;

    }

    //根据QueryString参数名称获取值

    function getQueryStringByName(name) {

        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));

        if (result == null || result.length < 1) {

            return "";

        }

        return result[1];

    }

    //根据QueryString参数索引获取值

    function getQueryStringByIndex(index) {

        if (index == null) {

            return "";

        }

        var queryStringList = getQueryString();

        if (index >= queryStringList.length) {

            return "";

        }

        var result = queryStringList[index];

        var startIndex = result.indexOf("=") + 1;

        result = result.substring(startIndex);
        return result;

    }
    function htmlEncode(s) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(s));
        return div.innerHTML;
    }
    function htmlDecode(s) {
        var div = document.createElement('div');
        div.innerHTML = s;
        return div.innerText || div.textContent;
    }
    function uuid() {
        var s = [];
        var hexDigits = "0123456789abcdef";
        for (var i = 0; i < 36; i++) {
            s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
        }
        s[14] = "4"; // bits 12-15 of the time_hi_and_version field to 0010
        s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1); // bits 6-7 of the clock_seq_hi_and_reserved to 01
        s[8] = s[13] = s[18] = s[23] = "-";
        var uuid = s.join("");
        return uuid;
    }
    
    function ConvertJsonToDate(jsonDate) {
        if (typeof jsonDate != 'string') {
            return jsonDate;
        }
        return new Date(+jsonDate.replace('/Date(', '').replace(')/', ''));
    }
    function getCookie(c_name) {
        if (document.cookie.length > 0) {
            var c_start = document.cookie.indexOf(c_name + "=")
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1
                var c_end = document.cookie.indexOf(";", c_start)
                if (c_end == -1) c_end = document.cookie.length
                return unescape(document.cookie.substring(c_start, c_end))
            }
        }
        return ""
    }

    function setCookie(c_name, value, expiredays) {
        var exdate = new Date()
        exdate.setDate(exdate.getDate() + expiredays)
        document.cookie = c_name + "=" + escape(value) +
        ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
    }
    function bindEvents() {
        String.prototype.hashCode = function () {
            var hash = 0, i, chr;
            if (this.length === 0) return hash;
            for (i = 0; i < this.length; i++) {
                chr = this.charCodeAt(i);
                hash = ((hash << 5) - hash) + chr;
                hash |= 0; // Convert to 32bit integer
            }
            return hash;
        };
        Date.prototype.toSimpleDateString = function () {
            var _this = this;
            var year = _this.getFullYear();
            var month = _this.getMonth() + 1;
            var day = _this.getDate();
            return year + "-" + month + "-" + day;
        }
        $('.message-container').delegate('.message .close', 'click', function () {
            $(this).closest('.message').transition('fade');
        })
        broswer.init();
    }
    $.extend({
        tip: tip, checkEmail: checkEmail, checkMobile: checkMobile,
        getQueryString: getQueryString, getQueryStringByName: getQueryStringByName, getQueryStringByIndex: getQueryStringByIndex,
        htmlEncode: htmlEncode, htmlDecode: htmlDecode, uuid: uuid, ConvertJsonToDate: ConvertJsonToDate,setCookie:setCookie,getCookie:getCookie
    });
    bindEvents();
})(window, document, jQuery);