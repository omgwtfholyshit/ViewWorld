$(function () {
    var page = {
        firstRequest: true,
        api: {
            updateOrder: "/Order/UpdateOrder",
            listSales: '/Account/ListUsersByRoles',
            listProviders: '/Order/ListProviders',
            getTrip: '/Trip/RetrieveTripArrangementByProductId'
        },
        orderData: {
            contactName: '',
            contactNumber: '',
            itemName: '',
            providerName: '',
            salesId: '',
            salesName: '',
            price: 0,
            status: 0,
            commenceDate:'',
            finishDate:'',
            orderDetail: new Array(),
        },
        tripData:{},
        dropdowns:{
            statusDropdown: $('.status.dropdown'),
            salesDropdown: $('.sales.dropdown'),
        },
        detailContainer: {
            formContainer: $('.roomdetail-container'),
            roomSelectModal: $('#roomSelectModal'),
            roomInfoContainer: $('#roomSelectModal .rooms-container'),
        },
        validUser: orderVar.isMyOrder == "True" || orderVar.role == "管理员",
        loadOrderData: function (callback) {
            var _this = this;
            for (var property in _this.orderData) {
                if (property != "orderDetail")
                    _this.orderData[property] = $('.form').find('input[name=' + property + ']').val();
                //console.log(property + " | " + $('.form').find('input[name=' + property + ']').val())
            }
            _this.orderData.salesName = $('.sales.dropdown .text').text().trim();
            if (_this.firstRequest) {
                _this.orderData.orderDetail = JSON.parse($.htmlDecode(orderVar.detail));
                _this.renderRoomList(10, _this.orderData.orderDetail.length);
                _this.firstRequest = false;
            }
            callback && callback();
        },
        renderOrderDetails:function(){
            var _this = this, roomDetailModel = { roomNumber: '', roomDesc: '' };
            _this.detailContainer.formContainer.find('.list').html("");
            $.each(_this.orderData.orderDetail, function (index, element) {
                roomDetailModel.roomNumber = "房间" + (index + 1);
                roomDetailModel.roomDesc = element.adults + "位成人" + element.children + "位儿童";
                _this.detailContainer.formContainer.find('.list').loadTemplate('#roomDetailTmpl', roomDetailModel, { append: 'true' });
            })
        },
        initDropdown: function () {
            var _this = this;
            _this.dropdowns.statusDropdown.dropdown("set selected", orderVar.status);
            $.get(_this.api.listSales, { roles: "管理员,销售" })
            .done(function (data) {
                var menuItem = '';
                $.each(data, function (index, element) {
                    menuItem += '<div class=item data-value="' + element.id + '">' + element.nickname + '</div>';
                })
                _this.dropdowns.salesDropdown.find('.menu').html(menuItem);
                _this.dropdowns.salesDropdown.dropdown("set selected", orderVar.salesId);
                _this.orderData.salesName = _this.dropdowns.salesDropdown.find('.text').text();
            })
            .fail(function () {
                $.tip(".message-container", "获取人员信息失败", "服务器超时，请稍后重试！", "negative", 4)
            })
            if (!_this.validUser) {
                _this.dropdowns.statusDropdown.addClass('disabled');
            }
            if (orderVar.role != "管理员") {
                _this.dropdowns.salesDropdown.addClass('disabled');
            }
                
        },
        getTripById: function ($button,id) {
            var _this = this,$secModal = $('.second.trip.modal');
            if (!$button.hasClass('loading')) {
                $button.addClass('loading');
                $.get(_this.api.getTrip, { productId: id })
                 .done(function (data) {
                     if (data.Success) {
                         var html = "";
                         html += '<p>行程名称: ' + data.Entity.CommonInfo.Name + '</p>';
                         html += '<p>产品编号: ' + data.Entity.ProductId + '</p><br />';
                         html += '<a href="/Finder/TripDetail?ProductId="' + data.Entity.ProductId + ' target="_blank">查看详情</a>'
                         $secModal.find('.content').html(html);
                         $secModal.modal('show');
                         _this.tripData = data.Entity;
                          $button.removeClass('loading');
                      } else {
                          $.tip(".message-container", "获取行程失败", data.Message, "negative", 4);
                          $button.removeClass('loading');
                          return false;
                      }
                 })
                .fail(function (xhr) {
                    $.tip(".message-container", "获取行程失败", "服务器超时，请稍后重试！", "negative", 4);
                    $button.removeClass('loading');
                })
            }
            
        },
        renderRoomList: function (totalRooms, orderedRooms) {
            var _this = this, roomClass = '';
            _this.detailContainer.roomInfoContainer.html("");
            $('#RoomTotal input[name=totalrooms]').val(orderedRooms);
            for (var i = 1; i <= totalRooms; i++) {
                i <= orderedRooms ? roomClass = '' : roomClass = 'hidden';
                _this.detailContainer.roomInfoContainer.loadTemplate('#roomTmpl', { 'roomNumber': "房间 " + i, 'roomClass': roomClass }, { 'append': true });
            }
            _this.detailContainer.roomSelectModal
            .delegate('#RoomTotal .spinner.label', 'click', function (e) {
                var $target = $(e.currentTarget), $input = $target.siblings('input'), roomCount = +$input.val();
                $target.hasClass('plus') ? roomCount++ : roomCount--;
                roomCount < 0 ? roomCount = 0 : roomCount;
                roomCount > 10 ? roomCount = 10 : roomCount;
                _this.detailContainer.roomInfoContainer.find('.room-detail').each(function (index, element) {
                    index < roomCount ? $(element).removeClass('hidden') : $(element).addClass('hidden');
                })
                $input.val(roomCount);
                _this.detailContainer.roomSelectModal.modal('refresh');
            })
            .delegate('.positive.button', 'click', function (e) {
                var rooms = new Array(), adultsCount = 0, childrenCount = 0;
                _this.detailContainer.roomInfoContainer.find('.room-detail').not('.hidden').each(function (index, element) {
                    var $element = $(element);
                    var ppr = new _this.PeoplePerRoom(+$element.find('input[name=adults]').val(), +$element.find('input[name=children]').val());
                    adultsCount += ppr.adults;
                    childrenCount += ppr.children;
                    rooms.push(ppr);
                })
                _this.orderData.orderDetail = rooms;
                _this.renderOrderDetails();
                _this.detailContainer.roomSelectModal.modal('hide');
            })
            .delegate('.grey.button', 'click', function (e) {
                _this.detailContainer.roomSelectModal.modal('hide');
            })
            _this.detailContainer.roomInfoContainer
            .delegate('input[name=totalrooms]', 'change', function (e) {
                var $target = $(e.target), roomCount = +$target.val();
                roomCount < 0 ? roomCount = 0 : roomCount;
                roomCount > 10 ? roomCount = 10 : roomCount;
                $target.val(roomCount);
                _this.detailContainer.roomInfoContainer.find('.room-detail').each(function (index, element) {
                    index < roomCount ? $(element).removeClass('hidden') : $(element).addClass('hidden');
                })
            })
            .delegate('.spinner.label', 'click', function (e) {
                var $target = $(e.currentTarget), $input = $target.siblings('input'), $otherInput = $target.parents('td').siblings().find('input');
                var adultsCount = 0, childrenCount = 0, isAdults = false, coefficient = -1, description = "";
                $target.hasClass('plus') ? coefficient = 1 : coefficient;
                if ($input.attr('name') == 'adults') {
                    adultsCount = +$input.val() + coefficient;
                    childrenCount = +$otherInput.val();
                    isAdults = true;
                } else {
                    adultsCount = +$otherInput.val();
                    childrenCount = +$input.val() + coefficient;
                }
                adultsCount < 1 ? adultsCount = 1 : adultsCount;
                adultsCount > 4 ? adultsCount = 4 : adultsCount;
                childrenCount < 0 ? childrenCount = 0 : childrenCount;
                childrenCount > 3 ? childrenCount = 3 : childrenCount;
                if (isAdults) {
                    if (adultsCount + childrenCount > 4) {
                        $.tip(".error-messages", "提示", "每个房间最多住四个人，并且必须包括一名成人", "warning", 2);
                        $input.val(adultsCount); $otherInput.val(4 - adultsCount);
                    } else {
                        $input.val(adultsCount); $otherInput.val(childrenCount);
                    }
                }
                else {
                    if (adultsCount + childrenCount > 4) {
                        $.tip(".error-messages", "提示", "每个房间最多住四个人，并且必须包括一名成人", "warning", 2);
                        $input.val(childrenCount); $otherInput.val(4 - childrenCount);
                    } else {
                        $input.val(childrenCount); $otherInput.val(adultsCount);
                    }
                }

            })
        },
        submitOrder:function($button,order){
            var _this = this;
            if (!$button.hasClass('loading')) {
                if (_this.validUser) {
                    $.ajax({
                        url: _this.api.updateOrder,
                        method: 'post',
                        beforeSend: function () {
                            $target.addClass('loading');
                        },
                        data: {
                            order: order,
                            __RequestVerificationToken: $('.ui.form input[name="__RequestVerificationToken"]').val(),
                        },
                        success: function (data) {
                            if (data.Success) {
                                $.tip(".message-container", "订单保存成功", "订单数据已更新，请刷新页面核查。", "positive", 4); 
                            } else {
                                $.tip(".message-container", "订单保存失败", data.Message, "negative", 4); 
                            }
                            $button.removeClass('loading');
                        },
                        error: function (data) { $.tip(".message-container", "订单保存失败", "服务器超时，请稍后重试！", "negative", 4); $button.removeClass('loading'); }
                    });
                }
            }
        },
        bindEvents: function () {
            var _this = this;
            $('.order-management')
            .delegate('#updateOrders', 'click', function (e) {
                $target = $(e.target);
                _this.loadOrderData();
                //console.log(orderVar)
                var businessOrder = new _this.BusinessOrder(orderVar.Id, orderVar.itemId, _this.orderData.itemName, _this.orderData.providerName, _this.orderData.contactName, _this.orderData.contactNumber, _this.orderData.salesId, _this.orderData.salesName, _this.orderData.price, _this.orderData.status, _this.orderData.commenceDate, _this.orderData.finishDate, _this.orderData.orderDetail);
                _this.submitOrder($target, businessOrder);
            })
            .delegate('#editTrip', 'click', function (e) {
                $('.first.trip.modal').modal('show');
            })
            .delegate('#editDetail', 'click', function (e) {
                _this.detailContainer.roomSelectModal.modal('show');
            })
            $('.first.trip.modal .positive.button').on('click', function (e) {
                var $modal = $(e.target).parents('.modal'),id = $modal.find('input[name=productId]').val();
                _this.getTripById($(e.target), id);
                return false;
            })
            $('.second.trip.modal .positive.button').on('click', function (e) {
                _this.orderData.itemName = _this.tripData.CommonInfo.Name;
                _this.orderData.providerName = _this.tripData.CommonInfo.ProviderName;
                orderVar.itemId = _this.tripData.Id;
                $('input[name=itemName]').val(_this.orderData.itemName);
                $('input[name=providerName]').val(_this.orderData.providerName);
            })

        },
        init: function () {
            var _this = this;
            _this.initDropdown();
            _this.loadOrderData(_this.renderOrderDetails.bind(_this));
            
            _this.bindEvents();
        },
        BusinessOrder: function (Id, ItemId, ItemName, ProviderName, ContactName, ContactNumber, SalesId, SalesName, Price, Status, CommenceDate, FinishDate, OrderDetail) {
            this.Id = Id;
            this.ItemId = ItemId;
            this.ItemName = ItemName;
            this.ProviderName = ProviderName;
            this.ContactName = ContactName;
            this.ContactNumber = ContactNumber;
            this.SalesId = SalesId;
            this.SalesName = SalesName;
            this.Price = Price;
            this.Status = Status;
            this.CommenceDate = CommenceDate;
            this.FinishDate = FinishDate;
            this.OrderDetail = JSON.stringify(OrderDetail);
            
        },
        PeoplePerRoom: function (adults, children) {
            this.adults = adults;
            this.children = children;
        }
    }
    page.init();
})