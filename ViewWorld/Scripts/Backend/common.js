(function () {
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
    function bindEvents() {
        $('.message-container').delegate('.message .close', 'click', function () {
            $(this).closest('.message').transition('fade');
        })
    }
    $.extend({ tip: tip, checkEmail: checkEmail, checkMobile: checkMobile });
    bindEvents();
})(window, document, jQuery);