$(function () {
    var api = {
        getTripList: '/Trip/RenderTripArrangementByKeyword',
        toggleTrip: '/Trip/ToggleTripArrangement',
        frontPageDisplay: '/Trip/DisplayTripOnFrontPage',
        copyTrip: '/Trip/CopyTripArrangementById',
        deleteTrip: '/Trip/DeleteTripArrangementById'
    }, globalVar = { tripId: '', token: $('input[name=__RequestVerificationToken]').val() };
    var $table = $('#dataTable tbody');
    function BuildTable(keyword) {
        var loadingHtml = '<tr class="center aligned"><td colspan="9" class="ui loading segment" height="150px"></td></tr>';
        if (typeof keyword == "undefined")
            keyword = '';

        if (!$table.hasClass('loading')) {
            $.ajax({
                url: api.getTripList,
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
                    $table.find('.circular.button').popup({ on: 'hover' });
                },
                error: function (data) { $table.find('td.loading').removeClass('loading').html("服务器超时，请稍后重试！"); $.tip(".message-container", "载入失败", "服务器超时，请稍后重试！", "negative", 4); }
            })
        }
    }
    function BindEvents() {
        $('#addTrip').on('click', function () {
            window.open('/Page/EditTripManagement');
        })
        $('#clear').on('click', function () {
            $('.header-left input').val('');
            BuildTable();
        })
        $table.delegate('button.circular', 'click', function (e) {
            var $button = $(e.currentTarget)
                , tripId = $button.parents('tr').attr('id');
            var content = $button.data('content');
            switch (content) {
                case '编辑':
                    window.open('/Page/EditTripManagement?tripId=' + tripId);
                    break;
                case '复制':
                    if (!$button.hasClass('loading')) {
                        $button.addClass('loading');
                        $.post(api.copyTrip, { tripId: tripId, __RequestVerificationToken: globalVar.token }).done(function (result) {
                            if (result.Success) {
                                window.open('/Page/EditTripManagement?tripId=' + result.Message);
                            } else {
                                $.tip(".message-container", "复制失败", result.Message, "negative", 4);
                            }
                            $button.removeClass('loading');
                        }).fail(function () {
                            $.tip(".message-container", "复制失败", "服务器超时，请稍后重试！", "negative", 4);
                            $button.removeClass('loading');
                        })
                    }
                    break;
                case '删除':
                    var tripName = $button.parents('tr').find('.three.wide').text();
                    globalVar.tripId = tripId;
                    $modal = $('.confirm.modal');$modal.find('.red').text(tripName);
                    $modal.modal('show');
                    break;
                case '发布线路':
                case '隐藏线路':
                    if (!$button.hasClass('loading')) {
                        $button.addClass('loading');
                        $.post(api.toggleTrip, { tripId: tripId, __RequestVerificationToken: globalVar.token }).done(function (result) {
                            if (result.Success) {
                                $button.siblings('.hidden.route').removeClass('hidden');
                                $button.addClass('hidden')
                            } else {
                                $.tip(".message-container", "设置失败", result.Message, "negative", 4);
                            }
                            $button.removeClass('loading');
                            
                        }).fail(function () {
                            $.tip(".message-container", "设置失败", "服务器超时，请稍后重试！", "negative", 4);
                            $button.removeClass('loading');
                        })
                    }
                    break;
                case '首页显示':
                case '首页隐藏':
                    if (!$button.hasClass('loading')) {
                        $button.addClass('loading');
                        $.post(api.frontPageDisplay, { tripId: tripId, __RequestVerificationToken: globalVar.token }).done(function (result) {
                            if (result.Success) {
                                $button.siblings('.hidden.frontpage').removeClass('hidden');
                                $button.addClass('hidden')
                            } else {
                                $.tip(".message-container", "设置失败", result.Message, "negative", 4);
                            }
                            $button.removeClass('loading');

                        }).fail(function () {
                            $.tip(".message-container", "设置失败", "服务器超时，请稍后重试！", "negative", 4);
                            $button.removeClass('loading');
                        })
                    }
                default:
                    break;
            }
        })
        $('.confirm.modal').delegate('.button.positive', 'click', function (e) {
            $.post(api.deleteTrip, { tripId: globalVar.tripId, __RequestVerificationToken: globalVar.token }).done(function (result) {
                if (result.Success) {
                    $.tip(".message-container", "删除成功", "选中线路已删除", "positive", 4);
                    $('#' + globalVar.tripId).remove();
                } else {
                    $.tip(".message-container", "删除失败", result.Message, "negative", 4);
                }
            }).fail(function () {
                $.tip(".message-container", "删除失败", "服务器超时，请稍后重试！", "negative", 4);
            });
        })
        $('.header-left .search.icon').on('click', function (e) {
            BuildTable($(e.target).siblings().val());
        })
        $('.header-left input').on('keypress', function (e) {
            if(e.keyCode == '13')
                $('.header-left .search.icon').click();
        })
    }
    function InitPage() {
        $('.header-left').transition('fade in').removeClass('invisible');
        BuildTable();
        BindEvents();
    }
    InitPage();
})