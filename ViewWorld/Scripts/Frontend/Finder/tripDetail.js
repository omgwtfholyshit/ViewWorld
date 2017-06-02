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
        api: { calculatePriceUrl: '/Finder/CalculateTripPrice' },
        init: function () {
            var _this = this;
            _this.initSlider();
            _this.initCalendar();
            _this.renderRoomList(10);
            $('.room-numbers.dropdown').dropdown({
                onChange: function (value, text, $choice) {
                    if (typeof +value != "undefined") {
                        _this.roomInfoContainer.find('.room-detail').each(function (index, element) {
                            index < +value ? $(element).removeClass('hidden') : $(element).addClass('hidden');
                        })
                        _this.roomSelectionModal.modal('show');
                        _this.roomSelectionModal.find('#RoomTotal input[name=totalrooms]').val(+value);
                    }
                }
            });
        },
        initSlider: function () {
            var _this = this;
            var galleryTop = new Swiper(_this.slider.find('.gallery-top'), {
                nextButton: '.swiper-button-next',
                prevButton: '.swiper-button-prev',
                spaceBetween: 10,
            });
            var galleryThumbs = new Swiper(_this.slider.find('.gallery-thumbs'), {
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
                tripData[index] = element.split('_')[0];
            })
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
                    validDate ? validDate : $.tip(".message-container", "暂无行程", "选择的日期没有行程,换个日子试试看吧.", "negative", 2);
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
            var _this = this, $priceContainer = $('.booking-container .horizontal.statistics')
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
            })
            .delegate('.positive.button', 'click', function (e) {
                var rooms = new Array(), departtime=$('input[name=departtime]').val().split(' ')[0], tripId, planId;
                _this.roomInfoContainer.find('.room-detail').not('.hidden').each(function (index, element) {
                    var $element = $(element);
                    rooms.push(new PeoplePerRoom($element.find('input[name=adults]').val(), $element.find('input[name=adults]').val()));
                })
                /*
                $.ajax({
                    url: _this.api.calculatePriceUrl,
                    method: 'post',
                    beforeSend: function () {
                        
                    },
                    data: {
                        rooms: rooms,
                        departtime: departtime,
                        tripId: tripId,
                        planId: planId,
                        __RequestVerificationToken: $('.ui.form input[name="__RequestVerificationToken"]').val(),
                    },
                    success: function (data) {
                        if (data.status == 200) {
                            $priceContainer.find('.value').html(data.data)
                        } else {
                            $.tip(".message-container", "获取价格失败", data.message, "negative", 4);
                        }
                        _this.roomSelectionModal.modal('hide');
                    },
                    error: function (data) { _this.roomSelectionModal.modal('hide'); $.tip(".message-container", "获取价格失败", "服务器超时，请稍后重试！", "negative", 4); return false; }
                });
                */
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
                var adultsCount = 0, childrenCount = 0, isAdults = false, coefficient = -1;
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
                        $input.val(adultsCount);$otherInput.val(4 - adultsCount);
                    } else {
                        $input.val(adultsCount); $otherInput.val(childrenCount);
                    }
                } else {
                    if (adultsCount + childrenCount > 4) {
                        $.tip(".error-messages", "提示", "每个房间最多住四个人，并且必须包括一名成人", "warning", 2);
                        $input.val(childrenCount); $otherInput.val(4 - childrenCount);
                    } else {
                        $input.val(childrenCount); $otherInput.val(adultsCount);
                    }
                }
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
})