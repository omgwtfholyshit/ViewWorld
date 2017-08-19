$(function () {
    var page = {
        api: {
            renderCollectionList: "/User/RenderCollectionPartial",
            removeCollectionItem: '/User/RemoveFromCollection',
        },
        pageVar: {
            pageNum: 1,
            orderId: '',
            type: '旅行团订单',
        },
        collectionContainer:$('.collection-management'),
        collectionListContainer: $('.collection-list'),
        renderCollectionList: function () {
            var _this = this;
            $.ajax({
                url: _this.api.renderCollectionList,
                method: 'get',
                beforeSend: function () {
                    _this.collectionListContainer.html('<div class="ui segment" style="height:10rem"><div class="ui active inverted dimmer" ><div class="ui text loader">Loading</div></div><p></p></div>');
                },
                data: {
                    type: _this.pageVar.type,
                    pageNum: _this.pageVar.pageNum,
                },
                success: function (data) {
                    _this.collectionListContainer.html(data);
                    _this.loadImage();
                },
                error: function (data) { $.tip(".message-container", "载入失败", "服务器超时，请稍后重试！", "negative", 4); }
            });
        },
        loadImage: function () {
            this.collectionListContainer.find('img').visibility({
                type: 'image',
                transition: 'fade in',
                duration: 300
            })
        },
        bindFilterEvents: function () {
            var _this = this, $target;
            _this.collectionContainer.find('.filter-container .label').click(function (e) {
                $target = $(e.target);
                _this.pageVar.status = $target.data('status');
                $target.siblings().each(function (index,element) {
                    var $element = $(element);
                    if ($element.hasClass('teal'))
                        $element.removeClass('teal').addClass('grey');
                })
                $target.removeClass('grey').addClass('teal');
                _this.pageNum = 1;
                _this.renderCollectionList();
            })
        },
        bindPaginationEvents: function () {
            var _this = this;
            var $paginationContainer = _this.collectionListContainer;
            $paginationContainer
                .delegate('.pagination.menu .item:first-child', 'click', function (e) {
                    $target = $(e.currentTarget);
                    if (!$target.hasClass('disabled')) {
                        if (_this.pageVar.pageNum > 1)
                            _this.pageVar.pageNum -= 1;
                    }
                    _this.renderCollectionList();
                })
                .delegate('.pagination.menu .item:last-child', 'click', function (e) {
                    $target = $(e.currentTarget);
                    if (!$target.hasClass('disabled')) {
                        _this.pageVar.pageNum += 1;
                    }
                    _this.renderCollectionList();
                })
                .delegate('.pagination.menu .item:not(:first-child,:last-child)', 'click', function (e) {
                    $target = $(e.currentTarget);
                    _this.pageVar.pageNum = +$target.text().trim();
                    _this.renderCollectionList();
                })
            
        },
        bindListEvents: function () {
            var _this = this;
            _this.collectionListContainer
                .delegate('.teal.button', 'click', function (e) {
                    var tripId = $(e.target).parents('.item').data('tripid');
                    if (typeof tripId == 'number')
                        window.open('/Finder/TripDetail?ProductId=' + tripId);
                })
                .delegate('.red.button', 'click', function (e) {
                    var id = $(e.target).parents('.item').data('id');
                    if (typeof id == 'string')
                        _this.deleteCollectionItem(id);
                })
        },
        deleteCollectionItem: function (id) {
            var _this = this;
            $.post(_this.api.removeCollectionItem, { itemId: id, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() })
                .done(function () {
                    _this.renderCollectionList();
                })
                .fail(function () {
                    $.tip(".message-container", "删除收藏失败", "服务器超时，请稍后重试！", "negative", 4);
                })
        },
        init: function () {
            var _this = this;
            _this.renderCollectionList();
            _this.bindPaginationEvents();
            _this.bindListEvents();
            //_this.bindFilterEvents();
            
        }
    }
    page.init();
})