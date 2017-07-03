$(function () {
    var firstRequest = true, paginationVar = { PageCount: 0, PageIndex: 1, PageSize: 3 };
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
        filter: $('.filter'),
        filterLabels:["所属区域","出发城市","结束城市","行程天数","游玩主题"],
        search: $('.filter-container .search-container'),
        sort: $('.sort'),
        result: $('.results-container'),
        pagination: $('.pagination.menu'),
        api: { renderCities: '/Finder/RenderCityPartial', getTrips: '/Finder/GetTripsBySearchModel' },
        searchModel: { keyword: '', Region: '', DepartureCity: '', ArrivalCity: '', Days: 0, Theme :''},
        init: function () {
            var _this = this;
            _this.initCityTab();
            _this.cnCityContainer.load(_this.api.renderCities + "?isCnCity=true");
            _this.fnCityContainer.load(_this.api.renderCities);
            _this.bindCitySelectorEvents();
            _this.bindSearchEvents();
            _this.bindFilterEvents();
            _this.bindSortEvents();
            _this.bindPaginationEvents();
            _this.bindResultEvents();
            _this.initCityInfo();
            _this.setCityHistory();
            _this.initSearch();
            _this.loadTrips(paginationVar.PageIndex, _this.buildPagination.bind(_this));
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
        bindSearchEvents: function () {
            var _this = this, $input = _this.search.find('input'), $button = _this.search.find('.button');
            $input.on('keypress', function (e) {
                if (e.keyCode == 13) {
                    $button.click();
                }
            })
            $button.click(function () {
                _this.searchModel.keyword = $input.val();
                _this.loadTrips(1, _this.buildPagination.bind(_this));
            })
        },
        bindFilterEvents: function () {
            var _this = this, modelname = '';
            _this.filter.delegate('.filter-tab .tab-item,.filter-content .content-item span', 'click', function (e) {
                var $target = $(e.target);
                $target.siblings().removeClass('active');
                $target.addClass('active');
                modelname = $target.parents('.content-item').data('modelname');
                if ($target.text() == '不限') {
                    _this.searchModel[modelname] = '';
                } else {
                    if (modelname == 'Days') {
                        _this.searchModel[modelname] = parseInt($target.text());
                    } else if (modelname == 'Theme') {
                        _this.searchModel[modelname] = $target.data('theme');
                    } else {
                        _this.searchModel[modelname] = $target.text();
                    }
                }
                _this.loadTrips(1, _this.buildPagination.bind(_this));
            })
            .delegate('.filter-content .content-item .toggleOptions', 'click', function (e) {
                var $target = $(e.currentTarget);
                if ($target.hasClass('off')) {
                    $target.removeClass('off').addClass('on');
                    $target.html('收起<i class="caret up icon"></i>');
                    $target.parent('.content-item').addClass('open');
                } else {
                    $target.removeClass('on').addClass('off');
                    $target.html('展开<i class="caret down icon"></i>');
                    $target.parent('.content-item').removeClass('open');
                }
            })
            $('.filter-container .divider span').click(function (e) {
                $icon = $(e.target).find('i');
                if ($icon.hasClass('down')) {
                    $icon.removeClass('down').addClass('up');
                } else {
                    $icon.removeClass('up').addClass('down');
                }
                _this.filter.transition('slide');
            })
        },
        bindSortEvents:function(){
            var _this = this;
            _this.sort.find('li').click(function (e) {
                var $target = $(e.target);
                $target.siblings().removeClass('active');
                $target.addClass('active');
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
        initSearch: function () {
            var _this = this;
            _this.search.find('.dropdown').dropdown();
        },
        loadTrips: function (pageNumber,callback) {
            var _this = this;
            typeof pageNumber == "number" && +pageNumber > 0 ? pageNumber : pageNumber = 1;
            if (firstRequest) {
                _this.searchModel.DepartureCity = _this.cityInfo.find('em').text();
                _this.searchModel.Region = decodeURIComponent($.getQueryStringByName('region'));
                _this.searchModel.keyword = decodeURIComponent($.getQueryStringByName('keyword'));
                _this.search.find('input').val(decodeURIComponent($.getQueryStringByName('keyword')));
                firstRequest = false;
            }
            $.ajax({
                url: _this.api.getTrips,
                method: 'POST',
                data: {
                    model: _this.searchModel,
                    pageNum: pageNumber
                },
                success: function (result) {
                    paginationVar.PageCount = result.data.PageCount;
                    paginationVar.PageIndex = result.data.PageIndex;
                    paginationVar.PageSize = result.data.PageSize;
                    _this.buildFilter(result.data.Data);
                    callback && callback();
                    _this.loadImage();
                },
                error: function (xhr) {
                    $.tip(".message-container", "操作失败", "服务器超时，请稍后重试！", "negative", 4);
                }

            });
        },
        loadImage: function () {
            this.result.find('img').visibility({
                type: 'image',
                transition: 'fade in',
                duration: 300
            })
        },
        buildFilter: function (tripData) {
            console.log(tripData);
            var _this = this, $filterTab = _this.filter.find('.filter-tab'), $filterContent = _this.filter.find('.filter-content'), $content = $('.results-container .items');
            var regionTags = departTags = arrivalTags = dayTags = themeTags = '<span>不限</span>';
            var departCities, arrivalCities, dayArray = new Array(), temp, className = '', cityStr = '',
                weekday = [["1", "周一"], ["2", "周二"], ["3", "周三"], ["4", "周四"], ["5", "周五"], ["6", "周六"], ["7", "周日"]],
                //主题。a.亲子游 b.自然风光 c.主题公园 d.都市名城 e.冒险之旅 f.毕业旅行 g.蜜月之旅 h.时尚购物 i.商务之旅 j.父母游 k.假期特惠 l.自由行 m.新年特惠 n.特色游
                labels = [["a", "亲子游"], ["b", "自然风光"], ["c", "主题公园"], ["d", "都市名城"], ["e", "冒险之旅"], ["f", "毕业旅行"], ["g", "蜜月之旅"], ["h", "时尚购物"], ["i", "商务之旅"], ["j", "父母游"], ["k", "假期特惠"], ["l", "自由行"], ["m", "新年特惠"], ["n", "特色游"]];
            try {
                $content.html("");
                if (typeof tripData != 'undefined' && tripData.length > 0) {
                    $.each(tripData, function (index, element) {
                        regionTags.indexOf(element.CommonInfo.RegionName) > 0 ? regionTags : regionTags += '<span>' + element.CommonInfo.RegionName + '</span>';
                        departCities = getCity(element.ProductInfo.DepartingCity);
                        arrivalCities = getCity(element.ProductInfo.ArrivingCity);
                        $.each(departCities, function (i, e) {
                            temp = e.split(',')[1];
                            departTags.indexOf(temp) >= 0 ? departTags : departTags += '<span>' + temp + '</span>';
                            cityStr += temp + ',';
                        })
                        $.each(arrivalCities, function (i, e) {
                            temp = e.split(',')[1];
                            arrivalTags.indexOf(temp) >= 0 ? arrivalTags : arrivalTags += '<span>' + temp + '</span>';
                        })
                        dayArray.indexOf(element.ProductInfo.TotalDays) >= 0 ? dayArray : dayArray.push(+element.ProductInfo.TotalDays);
                        var dayStr = '逢', labelHtml = '';
                        $.each(weekday, function (i, e) {
                            if (element.CommonInfo.AvailableDates.indexOf(e[0]) > -1)
                                dayStr += e[1] + ',';
                        })
                        $.each(labels, function (i, e) {
                            if (element.CommonInfo.Theme.indexOf(e[0]) > -1)
                                labelHtml += '<div class="ui teal label">' + e[1] + '</div>';
                        })
                        element.CommonInfo.FrontCover == null ? element.CommonInfo.FrontCover = { FileLocation: '' } : element.CommonInfo.FrontCover;
                        $content.loadTemplate('#resultContentTmpl', new contentItem(element.CommonInfo.Name, element.CommonInfo.Keyword, element.CommonInfo.FrontCover.FileLocation.replace('~', ''), "$" + element.CommonInfo.LowestPrice, cityStr.substr(0, cityStr.length - 1) + "出发", dayStr.substr(0, dayStr.length - 1) + "出行", labelHtml, element.ProductId), { 'append': true });
                        temp = cityStr = '';
                    })
                    regionTags.split(',').length >= 5 ? className = '' : className = 'hidden';
                    _this.searchModel.Region == '' ? regionTags = regionTags.replace('>不限', ' class="active">不限') : regionTags = regionTags.replace('>' + _this.searchModel.Region, ' class="active">' + _this.searchModel.Region);
                    $filterContent.loadTemplate("#filterContentTmpl", new filterItem("Region",_this.filterLabels[0], regionTags, className), { 'append': false });
                    departTags.length >= 5 ? className = '' : className = 'hidden';
                    _this.searchModel.DepartureCity == '' ? departTags = departTags.replace('>不限', ' class="active">不限') : departTags = departTags.replace('>' + _this.searchModel.DepartureCity, ' class="active">' + _this.searchModel.DepartureCity);
                    $filterContent.loadTemplate("#filterContentTmpl", new filterItem("DepartureCity", _this.filterLabels[1], departTags, className), { 'append': true });
                    arrivalTags.length >= 5 ? className = '' : className = 'hidden';
                    _this.searchModel.ArrivalCity == '' ? arrivalTags = arrivalTags.replace('>不限', ' class="active">不限') : arrivalTags = arrivalTags.replace('>' + _this.searchModel.ArrivalCity, ' class="active">' + _this.searchModel.ArrivalCity);
                    $filterContent.loadTemplate("#filterContentTmpl", new filterItem("ArrivalCity", _this.filterLabels[2], arrivalTags, className), { 'append': true });
                    dayArray.sort(function (a, b) { return a - b; });
                    for (var i = 0; i < dayArray.length; i++) {
                        _this.searchModel.Days == dayArray[i] ? dayTags += '<span class="active">' + dayArray[i] + '日</span>' : dayTags += '<span>' + dayArray[i] + '日</span>';
                    }
                    if (dayTags.indexOf('active') == -1)
                        dayTags = dayTags.replace('>不限', ' class="active">不限');
                    dayArray.length >= 5 ? className = '' : className = 'hidden';
                    $filterContent.loadTemplate("#filterContentTmpl", new filterItem("Days", _this.filterLabels[3], dayTags, className), { 'append': true });
                    themeTags += '<span data-theme="a">亲子游</span><span data-theme="b">自然风光</span><span data-theme="c">主题公园</span><span data-theme="d">都市名城</span><span data-theme="e">冒险之旅</span><span data-theme="f">毕业旅行</span><span data-theme="g">蜜月之旅</span><span data-theme="h">时尚购物</span><span data-theme="i">商务之旅</span><span data-theme="j">父母游</span><span data-theme="k">假期特惠</span><span data-theme="l">自由行</span><span data-theme="m">新年特惠</span><span data-theme="n">特色游</span>';
                    _this.searchModel.Theme == '' ? themeTags = themeTags.replace('>不限', ' class="active">不限') : themeTags = themeTags.replace(_this.searchModel.Theme + '"', _this.searchModel.Theme + '" class="active"');
                    $filterContent.loadTemplate("#filterContentTmpl", new filterItem("Theme", _this.filterLabels[4], themeTags), { 'append': true });
                }
                else {
                    $.tip(".message-container", "没有行程", "您选择的城市暂时没有行程，请看看其他城市的行程吧！", "negative", 4);
                }
            } catch (ex) {
                console.log(ex);
            }
        },
        buildPagination:function(){
            var _this = this, tmpl = '';
            if (paginationVar.PageIndex >= paginationVar.PageCount) {
                _this.pagination.addClass('hidden');
            } else {
                tmpl = '<a class="icon item"><i class="left chevron icon"></i></a>';
                for (var i = 1; i <= paginationVar.PageCount; i++) {
                    if (i == paginationVar.PageIndex) {
                        tmpl += '<a class="item active">' + i + '</a>';
                    } else {
                        tmpl += '<a class="item">' + i + '</a>';
                    }
                }
                tmpl += '<a class="icon item"><i class="right chevron icon"></i></a>';
                _this.pagination.html(tmpl).removeClass('hidden');
            }
            
        },
        bindPaginationEvents: function () {
            var _this = this, $target;
            _this.pagination.delegate('a.item', 'click', function (e) {
                $target = $(e.currentTarget);
                if ($target.hasClass('icon')) {
                    if ($target.find('i').hasClass('left')) {
                        if (paginationVar.PageIndex <= 1) {
                            return;
                        } else {
                            paginationVar.PageIndex--;
                        }
                    }
                    else if ($target.find('i').hasClass('right')) {
                        if (paginationVar.PageIndex >= paginationVar.PageCount) {
                            return;
                        } else {
                            paginationVar.PageIndex++;
                        }
                    }
                }
                else {
                    paginationVar.PageIndex = parseInt($target.text())
                }
                $target.siblings().removeClass('active');
                _this.pagination.children().eq(paginationVar.PageIndex).addClass('active');
                _this.loadTrips(paginationVar.PageIndex);
            })
        },
        bindResultEvents: function () {
            var _this = this;
            _this.result
            .delegate('.item .header,.item .detail-button', 'click', function (e) {
                var productId = $(e.target).parents('.item').data('productid');
                window.open('/Finder/TripDetail?ProductId=' + productId);
            })
        },
    }
    page.init();
    var filterItem = function (modelname, name, tags, className) {
        this.modelname = modelname;
        this.name = name;
        this.tags = tags;
        typeof className !== 'string' ? this.className = 'toggleOptions off ' : this.className = 'toggleOptions off ' + className;
    }
    var contentItem = function (title, keyword, image, price, departcity, availabledates, labels, productId) {
        this.title = title;
        this.keyword = keyword;
        this.image = image;
        this.price = price;
        this.departcity = departcity;
        this.availabledates = availabledates;
        this.labels = labels;
        this.productId = productId;
    }
    function getCity(cityStr) {
        return cityStr.split('|');
    }
})
