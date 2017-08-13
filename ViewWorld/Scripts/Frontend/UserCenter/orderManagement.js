$(function () {
    var page = {
        api: {
            renderOrderList:"/User/RenderOrderManagementPartial",
        },
        pageVar: {
            pageNum: 1,
            orderId: '',
            status: '行程已确认',
            type: '旅行团订单',
        },
        orderContainer:$('.order-management'),
        orderListContainer: $('.order-list'),
        renderOrderList: function () {
            var _this = this;
            $.ajax({
                url: _this.api.renderOrderList,
                method: 'get',
                beforeSend: function () {
                    _this.orderListContainer.html('<div class="ui segment" style="height:10rem"><div class="ui active inverted dimmer" ><div class="ui text loader">Loading</div></div><p></p></div>');
                },
                data: {
                    orderId: _this.pageVar.orderId,
                    status: _this.pageVar.status,
                    type: _this.pageVar.type,
                    pageNum: _this.pageVar.pageNum,
                },
                success: function (data) {
                    _this.orderListContainer.html(data);
                    _this.loadImage();
                },
                error: function (data) { $.tip(".message-container", "载入失败", "服务器超时，请稍后重试！", "negative", 4); }
            });
        },
        loadImage: function () {
            this.orderListContainer.find('img').visibility({
                type: 'image',
                transition: 'fade in',
                duration: 300
            })
        },
        bindFilterEvents: function () {
            var _this = this, $target;
            _this.orderContainer.find('.filter-container .label').click(function (e) {
                $target = $(e.target);
                _this.pageVar.status = $target.data('status');
                $target.siblings().each(function (index,element) {
                    var $element = $(element);
                    if ($element.hasClass('teal'))
                        $element.removeClass('teal').addClass('grey');
                })
                $target.removeClass('grey').addClass('teal');
                _this.renderOrderList();
            })
        },
        bindPaginationEvents: function () {
            var _this = this;
            _this.orderContainer
                .delegate('.item:first-child', 'click', function (e) {
                    $target = $(e.currentTarget);
                    if (!$target.hasClass('disabled')) {
                        if (_this.pageVar.pageNum > 1)
                            _this.pageVar.pageNum -= 1;
                    }
                    _this.renderOrderList();
                })
                .delegate('.item:last-child', 'click', function (e) {
                    $target = $(e.currentTarget);
                    if (!$target.hasClass('disabled')) {
                        _this.pageVar.pageNum += 1;
                    }
                    _this.renderOrderList();
                })
                .delegate('.item:not(:first-child,:last-child)', 'click', function (e) {
                    $target = $(e.currentTarget);
                    _this.pageVar.pageNum = +$target.text().trim();
                    _this.renderOrderList();
                })
            
        },
        init: function () {
            var _this = this;
            _this.renderOrderList();
            _this.bindFilterEvents();
            _this.bindPaginationEvents();
        }
    }
    window.page = page;
    page.init();
})