$(function () {
    var page = {
        api: { renderTable: '/Order/RenderOrderTable', assginOrder:'/Order/AssignOrderToSales' },
        searchOpt: { keyword: '', mineOnly: false,},
        paginationVar: { PageCount: 0, PageIndex: 1, PageSize: 3 },
        pagination: $('.pagination.menu'),
        tableContainer: $('table tbody'),
        orderPage: $('.order-management'),
        btnTmpl:{
            getOrderBtn: '<button class="ui labeled blue icon button get-order"><i class="hand pointer icon"></i>抢单</button>',
            modifyBtn: '<button class="ui labeled blue icon button modify"><i class="edit icon"></i>修改</button>',
            viewBtn:'<button class="ui labeled blue icon button view"><i class="unhide icon"></i>查看</button>'
        },
        renderTable: function (url,callback) {
            var _this = this;
            $.get(url, {
                keyword: _this.searchOpt.keyword,
                mineOnly: _this.searchOpt.mineOnly,
                pageNum:_this.paginationVar.PageIndex
            })
            .done(function (data) {
                _this.paginationVar.PageCount = data.data.PageCount;
                _this.paginationVar.PageIndex = data.data.PageIndex;
                _this.paginationVar.PageSize = data.data.PageSize;
                _this.tableContainer.html("");
                $.each(data.data.Data, function (index, element) {
                    if (element.SalesId == null || element.SalesId == "") {
                        element.Operations = _this.btnTmpl.getOrderBtn;
                    } else {
                        element.Operations = _this.btnTmpl.viewBtn;
                    }
                    _this.tableContainer.loadTemplate('#tbodyTmpl', element, { append: true })
                })
                callback && callback();
            })
            .fail(function (xhr) {
            })
        },
        updateRow: function (orderId,name) {
            var $operation = $('#' + orderId).find('td:last'), $personInCharge = $('#' + orderId).find('td:nth-last-child(2)');
            $personInCharge.text(name);
            $operation.html(this.btnTmpl.viewBtn);
        },
        buildPagination: function () {
            var _this = this, tmpl = '';
            if (_this.paginationVar.PageIndex >= _this.paginationVar.PageCount) {
                _this.pagination.parents('tfoot').addClass('hidden');
                _this.pagination.addClass('hidden');
            } else {
                tmpl = '<a class="icon item"><i class="left chevron icon"></i></a>';
                for (var i = 1; i <= _this.paginationVar.PageCount; i++) {
                    if (i == _this.paginationVar.PageIndex) {
                        tmpl += '<a class="item active">' + i + '</a>';
                    } else {
                        tmpl += '<a class="item">' + i + '</a>';
                    }
                }
                tmpl += '<a class="icon item"><i class="right chevron icon"></i></a>';
                _this.pagination.parents('tfoot').removeClass('hidden');
                _this.pagination.html(tmpl).removeClass('hidden');
            }

        },
        bindEvents: function () {
            var _this = this;
            _this.orderPage
                .delegate('.table-container .get-order.button', 'click', function (e) {
                    var $target = $(e.target), orderId = $target.parents('tr').attr('id');
                    if (!$target.hasClass('loading')) {
                        $.ajax({
                            url: _this.api.assginOrder,
                            method: 'post',
                            beforeSend: function () {
                                $target.addClass('loading')
                            },
                            data: {
                                salesId: userInfo.myId,
                                orderId: orderId,
                                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                            },
                            success: function (data) {
                                if (data.Success) {
                                    _this.updateRow(orderId, data.Message);
                                } else {
                                    $.tip(".message-container", "抢单失败", data.Message, "negative", 4);
                                }
                                $target.removeClass('loading');
                            },
                            error: function (data) {
                                $target.removeClass('loading');
                                $.tip(".message-container", "抢单失败", "服务器超时，请稍后重试！", "negative", 4);
                            }
                        });
                    }
                    
                })
                .delegate('.table-container .view.button', 'click', function (e) {
                    var $target = $(e.target), orderId = $target.parents('tr').attr('id');
                    window.open("/Order/OrderDetail?id=" + orderId);
                })
                .delegate('#myOrders', 'click', function (e) {
                    var $target = $(e.target);
                    if($target.hasClass('teal')){
                        _this.searchOpt.mineOnly = false;
                        $target.removeClass('teal');
                    }else{
                        _this.searchOpt.mineOnly = true
                        $target.addClass('teal');
                    }
                    _this.renderTable(_this.api.renderTable, _this.buildPagination.bind(_this));
                })
                .delegate('#clear', 'click', function (e) {
                    _this.searchOpt.keyword = "";
                    _this.searchOpt.mineOnly = false;
                    _this.paginationVar.PageIndex = 1;
                    $('#myOrders').removeClass('teal');
                    _this.renderTable(_this.api.renderTable, _this.buildPagination.bind(_this));
                })

            $('.header-left .search.icon').on('click', function (e) {
                _this.searchOpt.keyword = $(e.target).siblings('input').val();
                _this.paginationVar.PageIndex = 1;
                _this.renderTable(_this.api.renderTable, _this.buildPagination.bind(_this));
            })
            $('.header-left input').on('keyup', function (e) {
                if(e.keyCode == 13)
                    $('.header-left .search.icon').click();
            })
        },
        init: function () {
            var _this = this;
            _this.renderTable(_this.api.renderTable,_this.buildPagination.bind(_this));
            _this.bindEvents();
            $('.header-left').transition('fade in').removeClass('invisible');
        }
    }
    page.init();
})