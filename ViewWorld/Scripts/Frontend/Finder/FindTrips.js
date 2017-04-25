$(function () {
    var page = {
        cityInfo: $('.cityinfo'),
        cnCityContainer: $('#chineseCities'),
        fnCityContainer: $('#foreignCities'),
        citySelection: $('.city-selection'),
        cityTab: $('.city-tab'),
        cityHistory: function () {
            if (localStorage.getItem('cityHistory') == null) {
                return new Array();
            } else {
                var tempArray = localStorage.getItem('cityHistory').toString().split(',');
                tempArray.pop();
                return tempArray;
            }
        }(),
        api: { renderCities: '/Finder/RenderCityPartial' },
        searchModel: { keyword: '', Region: '', DepartureCity: '', ArrivalCity: '', Days: 0, MinPrice: 0, MaxPrice: 0, Sceneries :''},
        init: function () {
            var _this = this;
            _this.initCityTab();
            _this.cnCityContainer.load(_this.api.renderCities + "?isCnCity=true");
            _this.fnCityContainer.load(_this.api.renderCities);
            _this.bindCitySelectorEvents();
            _this.initCityInfo();
            _this.setCityHistory();
        },
        initCityTab: function () {
            $('.city-tab .menu .item').tab();
        },
        bindCitySelectorEvents: function () {
            var _this = this, $anchor, text = '';
            _this.citySelection
            .delegate('.city-anchor a', 'click', function (e) {
                $anchor = $(e.target);
                $anchor.siblings().removeClass('selected');
                $anchor.addClass('selected');
            })
            .delegate('.city-item a,.recent-city .sub.header span', 'click', function (e) {
                text = $(e.target).text();
                _this.cityInfo.find('em').text(text);
                _this.setCityHistory(text);
                _this.cityInfo.click();
            })
            .delegate('.recent-city .remove.icon', 'click', function (e) {
                _this.cityInfo.removeClass('active');
                _this.citySelection.css({ 'display': 'none' });
            })
            .delegate('.recent-city .remove-history', 'click', function () {
                localStorage.setItem('cityHistory', null);
                _this.cityHistory.splice(0, _this.cityHistory.length);
                _this.setCityHistory();
            })
        },
        initCityInfo: function () {
            var _this = this, $cityInfo;
            _this.cityInfo.click(function (e) {
                if (_this.cityInfo.hasClass('active')) {
                    _this.cityInfo.removeClass('active');
                    _this.citySelection.css({ 'display': 'none' });
                } else {
                    _this.cityInfo.addClass('active');
                    _this.citySelection.css({ 'display': 'flex' });
                }
            })
        },
        setCityHistory: function (history) {
            var _this = this, historyStr = '', html = '';
            if (typeof history != 'undefined' && history != '' && _this.cityHistory.indexOf(history) == -1) {
                if (_this.cityHistory.length >= 5) {
                    _this.cityHistory.shift();
                }
                _this.cityHistory.push(history);
            }
            $.each(_this.cityHistory, function (index, element) {
                historyStr = historyStr + element + ',';
                html += '<span>' + element + '</span>';
            })
            localStorage.setItem('cityHistory', historyStr);
            _this.citySelection.find('.recent-city .sub.header').html(html);
        },
    }
    page.init();
})
