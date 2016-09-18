(function () {
    function tip(selector, title, content, type, dismiss) {
        var id = Math.round(Math.random() * 10000);
        html = '<div class = "ui message ' + type + '" id = "' + id + '" >';
        html += '<i class="close icon"></i>';
        html += '<div class="header">' + title + '</div>';
        html += '<p>' + content + '</p></div>';
        $(selector).append(html);
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
    $.extend({ tip: tip });
})(window, document, jQuery);