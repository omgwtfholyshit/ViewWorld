$(function () {
    var page = {
        slider: $('.slider'),
        calendar: $('#bookingCalendar'),
        calendarHeader: $('#calendarHeader'),
        smallCalendar: $('.toggleCalendar'),
        weekArray: ['周日', '周一', '周二', '周三', '周四', '周五', '周六', '周日'],
        tripData: JSON.parse(decodeURIComponent(calendarData)),
        roomInfoContainer: $('#roomSelectModal .rooms-container'),
        roomSelectionModal: $('#roomSelectModal'),
        sceneryModal: $('#sceneryModal'),
        scenerySwiperInstance: '',
        stickyBar: {
            body: $('.body-wrapper'),
            navMenu: $('.nav'),//导航
            navHeight: $('.nav').height(),//导航高度
            detailNav: $('#detailNav'),//详情导航
            detailNavStickyHeight: $('#detailNav')[0].offsetTop - $('.nav').height(),//sticky when scrollHeight more than this
            detailHeaders: $('.horizontal.divider.header'),
        },
        bookingSection: $('.booking-section'),
        productDetail: $('#detailContainer'),
        tripSettings:{
            rooms: new Array(),
            planId: '',
            departtime: '',
            tripId: ProductInfo.TripId,
            productId: ProductInfo.ProductId,
            productName: ProductInfo.ProductName,
            price: 0,
            currencyType: '',
            timeStamp: '',
        },
        api: {
            calculatePriceUrl: '/Finder/CalculateTripPrice', getSceneryDetail: '/Finder/GetSceneryDetail',
            addToCollection: '/User/AddToCollection', checkIfcollected: '/User/CheckIfItemCollected', removeFromCollection: '/User/RemoveFromCollection',
            addToOrder: '/User/AddToOrder'
        },
        init: function () {
            var _this = this;
            _this.initSlider();
            _this.initCalendar();
            _this.renderRoomList(10);
            _this.initDropDown();
            _this.initCollection();
            _this.bindEvents();
        },
        initSlider: function () {
            var _this = this;
            var galleryTop = new Swiper(_this.slider.find('.gallery-top.main-slider'), {
                nextButton: '.swiper-button-next',
                prevButton: '.swiper-button-prev',
                spaceBetween: 10,
            });
            var galleryThumbs = new Swiper(_this.slider.find('.gallery-thumbs.main-slider'), {
                spaceBetween: 10,
                centeredSlides: true,
                slidesPerView: 'auto',
                touchRatio: 0.2,
                slideToClickedSlide: true
            });
            
            galleryTop.params.control = galleryThumbs;
            galleryThumbs.params.control = galleryTop;
        },
        initCalendar: function () {
            var _this = this, timeStr = "", tripData = new Array();
            $.each(_this.tripData, function (index, element) {
                tripData[index] ="最低" + element.split('_')[0];
            })
            //console.log(tripData)

            //Big Calendar
            var calendarObj = _this.calendar.calendario({
                weeks: _this.weekArray,
                weekabbrs: ['日', '一', '二', '三', '四', '五', '六'],
                months: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
                monthabbrs: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
                caldata: tripData,
                minDate: new Date(),
                maxDate: function () {
                    var today = new Date(), maxDate = new Date();
                    maxDate.setFullYear(today.getFullYear() + 1);
                    return maxDate;
                }(),
                onDayClick: function ($el, $content, dateProperties) {
                    removeActiveClass(_this.calendar);
                    if (!$el.hasClass('fc-disabled')&&$el.hasClass('fc-content')) {
                        $el.addClass('fc-today');
                        _this.smallCalendar.calendar("set date", new Date(dateProperties.year + '-' + dateProperties.month + '-' + dateProperties.day), false, false);
                        timeStr = dateProperties.year + "年" + dateProperties.month + "月" + dateProperties.day + "日 " + dateProperties.weekdayname;
                        $('input[name=departtime]').val(timeStr);
                        _this.tripSettings.departtime = dateProperties.year + '-' + dateProperties.month + '-' + dateProperties.day;
                        _this.calculatePrice($('.booking-container .horizontal.statistics'));
                    } else {
                        $.tip(".message-container", "暂无行程", "选择的日期没有行程,换个日子试试看吧.", "negative", 2);
                    }
                    return false;
                }
            });
            _this.calendarHeader
            .delegate('#custom-prev', 'click', function () {
                removeActiveClass(_this.calendar);
                calendarObj.gotoPreviousMonth(updateMonthAndYear);
            })
            .delegate('#custom-next', 'click', function () {
                removeActiveClass(_this.calendar);
                calendarObj.gotoNextMonth(updateMonthAndYear);
            })
            updateMonthAndYear();
            function updateMonthAndYear() {
                _this.calendarHeader.find('#custom-month').text(calendarObj.getMonthName());
                _this.calendarHeader.find('#custom-year').text(calendarObj.getYear());
            }
            //Small Calendar
            _this.smallCalendar.calendar({
                ampm: false,
                type: 'date',
                minDate: new Date(),
                maxDate: function () {
                    var today = new Date(), maxDate = new Date();
                    maxDate.setFullYear(today.getFullYear() + 1);
                    return maxDate;
                }(),
                onChange: function (date, text) {
                    var validDate = (text !== "false");
                    if (validDate) {
                        _this.tripSettings.departtime = date.toSimpleDateString();
                        _this.calculatePrice($('.booking-container .horizontal.statistics'))
                    } else {
                        $.tip(".message-container", "暂无行程", "选择的日期没有行程,换个日子试试看吧.", "negative", 2);
                    }
                    return validDate;
                },
                formatter: {
                    date: function (date, settings) {
                        if (!date) return '';
                        var day = date.getDate();
                        var month = settings.text.monthsShort[date.getMonth()];
                        var year = date.getFullYear();
                        var cell = calendarObj.getCell(date);
                        if (typeof _this.tripData[date.toMMddyyyyString()] == "string") {
                            calendarObj.goto(date.getMonth(), date.getFullYear(), function () {
                                var $cell = calendarObj.getCell(date).parent('.fc-content');
                                removeActiveClass(_this.calendar);
                                if (!$cell.hasClass('fc-disabled')) {
                                    $cell.addClass('fc-today');
                                }
                            });
                            updateMonthAndYear();
                            return year + '年' + month + day + '日 ' + _this.weekArray[date.getDay()];
                        }
                        return false;
                    }
                }
            });
        },
        renderRoomList: function (totalRooms) {
            var _this = this, $priceContainer = $('.booking-container .horizontal.statistics'), $detailContainer = $('.roomdetail-container .ui.list');
            _this.roomInfoContainer.html("");
            for (var i = 1; i <= totalRooms; i++) {
                _this.roomInfoContainer.loadTemplate('#roomTmpl', { 'roomNumber': "房间 " + i }, { 'append': true });
            }
            _this.roomSelectionModal
            .delegate('#RoomTotal .spinner.label', 'click', function (e) {
                var $target = $(e.currentTarget), $input = $target.siblings('input'), roomCount = +$input.val();
                $target.hasClass('plus') ? roomCount++ : roomCount--;
                roomCount < 0 ? roomCount = 0 : roomCount;
                roomCount > 10 ? roomCount = 10 : roomCount;
                _this.roomInfoContainer.find('.room-detail').each(function (index, element) {
                    index < roomCount ? $(element).removeClass('hidden') : $(element).addClass('hidden');
                })
                $input.val(roomCount);
                _this.roomSelectionModal.modal('refresh');
            })
            .delegate('.positive.button', 'click', function (e) {
                var rooms = new Array(), planId,
                    departtime = $('input[name=departtime]').val().split(' ')[0].replace('年', '-').replace('月', '-').replace('日', '');
                if (departtime == 'false')
                    return $.tip(".message-container", "当前日期不可用", "请您先选择一个日期！若无日期可用，请联系客服为您确认", "negative", 4);
                planId = _this.tripData[(new Date(departtime)).toMMddyyyyString()].split('_')[1], adultsCount = 0, childrenCount = 0;
                $detailContainer.html("");
                _this.roomInfoContainer.find('.room-detail').not('.hidden').each(function (index, element) {
                    var $element = $(element);
                    var ppr = new PeoplePerRoom(+$element.find('input[name=adults]').val(), +$element.find('input[name=children]').val());
                    adultsCount += ppr.adults;
                    childrenCount += ppr.children;
                    rooms.push(ppr);
                    //console.log(ppr);

                    $detailContainer.loadTemplate('#roomDetailTmpl', { 'roomNumber': "房间 " + (index + 1), 'roomDesc': ppr.adults + "位成人" + ppr.children + "位儿童" }, { 'append': true })
                })
                if (rooms.length == 0) {
                    return $.tip(".message-container", "获取价格失败", "先选个房间吧！", "negative", 4);
                }
                _this.tripSettings.rooms = rooms;
                _this.tripSettings.planId = planId;
                _this.tripSettings.departtime = departtime;
                _this.calculatePrice($priceContainer);
                
            })
            .delegate('.grey.button', 'click', function (e) {
                _this.roomSelectionModal.modal('hide');
            })
            _this.roomInfoContainer
            .delegate('input[name=totalrooms]', 'change', function (e) {
                var $target = $(e.target), roomCount = +$target.val();
                roomCount < 0 ? roomCount = 0 : roomCount;
                roomCount > 10 ? roomCount = 10 : roomCount;
                $target.val(roomCount);
                _this.roomInfoContainer.find('.room-detail').each(function (index, element) {
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
        initDropDown: function () {
            var _this = this;
            $('.room-numbers.dropdown').dropdown({
                onChange: function (value, text, $choice) {
                    if (typeof +value != "undefined") {
                        _this.roomInfoContainer.find('.room-detail').each(function (index, element) {
                            index < +value ? $(element).removeClass('hidden') : $(element).addClass('hidden');
                        })
                        _this.roomSelectionModal.find('#RoomTotal input[name=totalrooms]').val(+value);
                    }
                },
                onHide: function () {
                    _this.roomSelectionModal.modal('show');
                    _this.roomSelectionModal.modal('refresh');
                }
            });
        },
        initCollection: function () {
            var _this = this;
            if (loginHelper.isLoggedIn) {
                $.get(_this.api.checkIfcollected, { itemId: ProductInfo.TripId })
                    .done(function (collected) {
                        _this.updateCollectionStatus(collected.data);
                    }).fail(function () { $.tip(".message-container", "获取收藏状态失败", "服务器超时，请稍后重试！", "negative", 4) })
            }
        },
        updateCollectionStatus: function (collected) {
            var $collected = $('#collect'), $navBarCollect = $('#detailNav .right.menu .collect');
            if (collected) {
                $collected.removeClass('teal').addClass('grey').html('<i class="bookmark icon"></i>已收藏');
                $navBarCollect.text("已收藏行程");
            } else {
                $collected.removeClass('grey').addClass('teal').html('<i class="bookmark icon"></i>收藏');
                $navBarCollect.text("收藏行程");
            }
            
        },
        onScrollEvents: function () {
            var _this = page.stickyBar, navWidth = window.innerWidth < $('#detailContainer').width() ? navWidth = window.innerWidth : navWidth = $('#detailContainer').width();
            //console.log('scrollTop: ' + _this.body.scrollTop() + ' | navStickyHeight: ' + _this.detailNavStickyHeight);
            if (_this.body.scrollTop() > _this.detailNavStickyHeight) {
                _this.detailNav.css({ 'position': 'absolute', 'top': _this.navHeight, 'margin': 0, 'width': navWidth + 30});
            } else {
                _this.detailNav.css({ 'position': 'relative', 'margin': '1rem 0', 'top': 0, 'width': 'auto' });
            }
            _this.detailHeaders.each(function (index, element) {
                var $element = $(element);
                if ($element.offset().top > 0 && $element.offset().top < 120) {
                    $(_this.detailNav.find('>.item')[index]).addClass('active').siblings().removeClass('active');
                }
            })
        },
        calculatePrice: function ($priceContainer) {
            var _this = this;
            if (_this.tripSettings.rooms.length == 0)
                return;
            $.ajax({
                url: _this.api.calculatePriceUrl,
                method: 'post',
                beforeSend: function () {
                    
                },
                data: {
                    rooms: _this.tripSettings.rooms,
                    departDate: _this.tripSettings.departtime,
                    tripId: ProductInfo.TripId,
                    planId: _this.tripSettings.planId,
                    //__RequestVerificationToken: $('.ui.form input[name="__RequestVerificationToken"]').val(),
                },
                success: function (data) {
                    if (data.status == 200) {
                        $priceContainer.find('.value').html(data.data.replace('|'," "));
                        $priceContainer.find('.label').text("共计" + adultsCount + "位成人" + childrenCount + "位儿童");
                        switch(data.data.split('|')[0]){
                            case "USD$":
                                _this.tripSettings.currencyType = "美元";
                                break;
                            case "AUD$":
                                _this.tripSettings.currencyType = "澳元";
                            case "EUR€":
                                _this.tripSettings.currencyType = "欧元";
                            default:
                                _this.tripSettings.currencyType = "人民币";
                        }
                        _this.tripSettings.price = data.data.split('|')[1];
                    } else {
                        $.tip(".message-container", "获取价格失败", data.message, "negative", 4);
                    }
                    _this.roomSelectionModal.modal('hide');
                },
                error: function (data) { _this.roomSelectionModal.modal('hide'); $.tip(".message-container", "获取价格失败", "服务器超时，请稍后重试！", "negative", 4); return false; }
            });
        },
        submitOrder: function ($target, order) {
            var _this = this, token = $('input[name="__RequestVerificationToken"').val();
            $.ajax({
                url: _this.api.addToOrder,
                method: 'post',
                beforeSend: function () {
                    $target.addClass('loading');
                },
                data: {
                    order: order, __RequestVerificationToken: token
                },
                success: function (data) {
                    if (data.Success) {
                        $('.second.contact.modal').modal('show');
                    } else {
                        $.tip(".message-container", "提交失败", data.Message, "warning", 4);
                    }
                    $target.removeClass('loading');
                },
                error: function (data) {
                    $.tip(".message-container", "预定失败", "服务器超时，请稍后重试！", "negative", 4);
                    $target.removeClass('loading');
                }
            });
        },
        bindEvents: function () {
            var _this = this;
            _this.productDetail
            .delegate('.product-detail .scenery', 'click', function (e) {
                var id = $(e.target).data('id');
                $.get(_this.api.getSceneryDetail, { sceneryId: id }).done(function (data) {
                    //console.log(data);
                    var photoHtml = '';
                    if (data.status == 200) {
                        if (data.data.Photos.length > 0) {
                            $.each(data.data.Photos, function (index, element) {
                                photoHtml += '<div class="swiper-slide" style="background:url(' + element + ') no-repeat;background-size:cover"></div>'
                            })
                        } else {
                            photoHtml = '<div class="swiper-slide" style="background:url(/Images/Logo/logo_352-172.svg) no-repeat;background-size:contain"></div>';
                        }
                    }
                    _this.sceneryModal.find('.swiper-wrapper').html(photoHtml);
                    _this.sceneryModal.find('.description').html('<div class="ui header">' + data.data.Name + '</div >' + $.htmlDecode(data.data.Description))
                    setTimeout(function () {
                        _this.scenerySwiperInstance = new Swiper(_this.sceneryModal.find('.gallery-top.modal-slider'), {
                            nextButton: '.swiper-button-next',
                            prevButton: '.swiper-button-prev',
                            spaceBetween: 10,
                        });
                    }, 100);

                    _this.sceneryModal.modal({ onHidden: function () { _this.scenerySwiperInstance.destroy(true, true) } }).modal('show').modal('refresh');
                }).fail(function () { $.tip(".message-container", "服务器超时", "服务器没有响应，请稍后再试.", "negative", 2); })
            });
            _this.bookingSection
            .delegate('#reserve', 'click', function (e) {
                if (_this.tripSettings.rooms.length > 0 && _this.tripSettings.departtime != false) {
                    _this.tripSettings.timeStamp = new Date().getTime();
                    $.setCookie('tripSettings', JSON.stringify(_this.tripSettings), 2);
                    if (loginHelper.isLoggedIn) {
                        $('.first.contact.modal').modal('show');
                       
                    } else {
                        window.location.href = '/Account/Login?returnUrl=' + location.pathname + location.search;
                    }
                } else {
                    $.tip(".message-container", "预定信息不全", "人数和日期缺一不可哦！", "warning", 2);
                }
               
            })
            .delegate('#collect', 'click', function (e) {
                var $target = $(e.target), token = $('input[name="__RequestVerificationToken"').val();
                if (loginHelper.isLoggedIn && !$target.hasClass('loading')) {
                    if ($target.hasClass('grey')) {
                        $.ajax({
                            url: _this.api.removeFromCollection,
                            method: 'post',
                            beforeSend: function () {
                                $target.addClass('loading');
                            },
                            data: {
                                itemId: ProductInfo.TripId, __RequestVerificationToken: token
                            },
                            success: function (data) {
                                _this.updateCollectionStatus(!data.Success);
                                $target.removeClass('loading');
                            },
                            error: function (data) {
                                $.tip(".message-container", "删除收藏失败", "服务器超时，请稍后重试！", "negative", 4);
                                $target.removeClass('loading');
                            }
                        });
                    } else if($target.hasClass('teal')) {
                        $.ajax({
                            url: _this.api.addToCollection,
                            method: 'post',
                            beforeSend: function () {
                                $target.addClass('loading');
                            },
                            data: {
                                itemId: ProductInfo.TripId, itemName: ProductInfo.ProductName,
                                type: '旅行团订单', __RequestVerificationToken: token
                            },
                            success: function (data) {
                                _this.updateCollectionStatus(data.Success);
                                $target.removeClass('loading');
                            },
                            error: function (data) {
                                $.tip(".message-container", "收藏失败", "服务器超时，请稍后重试！", "negative", 4);
                                $target.removeClass('loading');
                            }
                        });
                    }
                } else {
                    window.location.href = '/Account/Login?returnUrl=' + location.pathname + location.search;
                }
            })
            _this.stickyBar.detailNav
                .delegate('>.item', 'click', function (e) {
                    var $target = $(e.target), index = $target.index(), headerPosition = _this.stickyBar.detailNavStickyHeight - _this.stickyBar.detailNav.height() + 4;
                    $target.addClass('active').siblings().removeClass('active');
                    headerPosition += _this.stickyBar.detailHeaders[index].offsetTop;
                    _this.stickyBar.body.stop().animate({ scrollTop: headerPosition }, 300);
                })
                .delegate('.collect', 'click', function (e) {
                    return $('#collect').click();
                })
                .delegate('.reserve', 'click', function () {
                    return $('#reserve').click();
                })
            _this.stickyBar.body.scroll(_this.onScrollEvents);
            $('.first.contact.modal .positive.button').on('click', function (e) {
                var $modal = $(e.target).parents('.modal');
                var name = $modal.find('input[name=name]').val(), phone = $modal.find('input[name=phone]').val();
                var departTime = new Date(_this.tripSettings.departtime);
                var finishTime = departTime.setDate(departTime.getDate() + ProductInfo.TotalDays);
                var order = new Order(ProductInfo.TripId, ProductInfo.ProductName, name, phone, ProductInfo.ProviderName, _this.tripSettings.departtime, new Date(finishTime).toSimpleDateString(), "旅行团订单", JSON.stringify(_this.tripSettings.rooms), _this.tripSettings.price, _this.tripSettings.currencyType);
                //console.log(order)
                _this.submitOrder($(e.target), order);
                return false;
            })
        }
    }
    page.init();
    function removeActiveClass($calendar) {
        $calendar.find('.fc-today').each(function (index, element) {
            $(element).removeClass('fc-today');
        })
    }
    function PeoplePerRoom(adults, children) {
        this.adults = adults;
        this.children = children;
    }
    function Order(ItemId, ItemName, ContactName, ContactNumber, ProviderName, CommenceDate, FinishDate, Type, OrderDetail, Price, CurrencyType) {
        this.ItemId = ItemId;
        this.ItemName = ItemName;
        this.ContactName = ContactName;
        this.ContactNumber = ContactNumber;
        this.ProviderName = ProviderName;
        this.CommenceDate = CommenceDate;
        this.FinishDate = FinishDate;
        this.Type = Type;
        this.OrderDetail = OrderDetail;
        this.Price = Price;
        this.CurrencyType = CurrencyType;
    }
})