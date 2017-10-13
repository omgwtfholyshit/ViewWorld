$(function () {
    var E = window.wangEditor;
    var api = {
        regionDataUrl: '/Trip/ListRegionsAPI',
        tripDataUrl: '/Trip/GetTripArrangementById',
        partialDataUrl: '/Trip/UpdateTripPartial',
        addTripUrl: '/Trip/AddTripArrangement',
        updateTripUrl: '/Trip/UpdateTripArrangement',
        uploadPhotoUrl: '/Trip/UploadTripArrangementPhoto',
        setCoverUrl: '/Trip/SetFrontCoverById',
        deletePhotoUrl: '/Trip/DeletePhotoById',
        listCitiesUrl: '/Trip/SearchCityByKeyword',
        listSceneriesUrl: '/Trip/ListSceneriesAPI',
        listStartPointsUrl: '/Trip/ListStartingPointsAPI',
        listTypeUrl: '/Trip/ListTripTypeAPI',
        toggleTrip: '/Trip/ToggleTripArrangement',
    }, tmplOpt = { 'append': true }, token = $('input[name=__RequestVerificationToken]').val(), uploadArr = new Array(),
    introEditor = new E('#introduction'), includeEditor = new E('#include'), excludeEditor = new E('#exclude'),
    pIntroEditor = new E('#Intro'), pFeatureEditor = new E('#Feature'), scheduleEditor = new Array(), highlight = new Array(), publishable = true;
    introEditor.customConfig.zIndex = 1; includeEditor.customConfig.zIndex = 1; excludeEditor.customConfig.zIndex = 1; pIntroEditor.customConfig.zIndex = 1; pFeatureEditor.customConfig.zIndex = 1;
    introEditor.create(); includeEditor.create(); excludeEditor.create(); pIntroEditor.create(); pFeatureEditor.create();
    var CommonInfo = {
        Name:'',
        ProviderName: '',
        GroupId: '',//团号
        RegionId: '',
        RegionName: '',
        BookingRequired: false,
        TripType:'',
        Promotion: '',//优惠政策。a.特价 b.限时促销 c.热卖 d.买二送二 e.买二送一 f.推荐 g.积分优惠 h.免费接机
        Theme: '',//主题。a.亲子游 b.自然风光 c.主题公园 d.都市名城 e.冒险之旅 f.毕业旅行 g.蜜月之旅 h.时尚购物 i.商务之旅 j.父母游 k.假期特惠 l.自由行 m.新年特惠 n.特色游
        AvailableDates: '',
        CurrencyType: '',
        Price: 0,
        UsePoints: false,
        Points: 0,
        Introduction: '',
        Include: '',
        Exclude: '',
        SelfPayActivities: new Array(),
        Keyword: '',
        Description: '',
        Photos: new Array(),
        FrontCover: '',
        PendingPhotos: new Array(),
        LoadRegionData: function (url) {
            $.get(url, { displaySubRegions: true }).done(function (data) {
                var html = '';
                if (data.success) {
                    $.each(data.results, function (index, element) {
                        if (element.results != null) {
                            html += '<div class="item" data-value="' + element.value + '" data-initial="' + element.results + '">' + element.results + element.name + '</div>'
                        } else {
                            html += '<div class="item disabled" data-value="' + element.value + '">' + element.name + '</div>';
                        }
                        
                    })
                }
                $('#regionSelection .menu').html(html);
            }).fail(function (xhr) { $.tip(".message-container", "载入区域失败", "服务器超时，请稍后重试！", "negative", 4); });
        },
        LoadTypeData: function (url) {
            $.get(url).done(function (data) {
                var html = '';
                if (data.success) {
                    $.each(data.results, function (index, element) {
                        if (element != null) {
                            html += '<div class="item" data-value="' + element.value  + '">' + element.name + '</div>';
                        }
                    })
                    $('#typeSelection .menu').html(html);
                }
            }).fail(function (xhr) { $.tip(".message-container", "载入类型失败", "服务器超时，请稍后重试！", "negative", 4); });
            
        },
        SetCheckBoxForJs:function(){
            var promotion = theme = availabledates = '', usepoints = false;
            $('#commonInfo .checkbox').each(function (index, ele) {
                var $ele = $(ele),$input=$ele.find('input');
                if ($ele.checkbox('is checked')) {
                    switch($input.attr('name')){
                        case 'promotion':
                            promotion += $input.val() + ','; break;
                        case 'theme':
                            theme += $input.val() + ','; break;
                        case 'availabledates':
                            availabledates += $input.val() + ','; break;
                        case 'usepoints':
                            usepoints = true; break;
                        case 'booking':
                            CommonInfo.BookingRequired = true;break;
                    }
                } else {
                    if($input.attr('name') == 'booking')
                        CommonInfo.BookingRequired = false;
                }
            })
            CommonInfo.Theme = theme;
            CommonInfo.Promotion = promotion;
            CommonInfo.AvailableDates = availabledates;
            CommonInfo.UsePoints = usepoints;
            return;
        },
        SetFormDataForJs: function () {
            var $inputs = $('#commonInfo').find('input').not('.search,#fileUpload,input[type=checkbox],#selfpayActivities input,input[name=__RequestVerificationToken]');
            var $input, _this = this, key;
            $inputs.each(function (index, input) {
                $input = $(input), key = $input.data('db-key');
                if (key == 'TripType') {
                    _this[key] = $input.val().trim().replaceAll(',', '|');
                } else {
                    _this[key] = $input.val().trim();
                }
                
            });
            _this.RegionName = $('#regionSelection .text').text().split('----')[1];
            _this.Introduction = $.htmlEncode(introEditor.txt.html());
            _this.Include = $.htmlEncode(includeEditor.txt.html());
            _this.Exclude = $.htmlEncode(excludeEditor.txt.html());
        },
        SetSelfPayTableForJs: function () {
            var $trs = $('#selfpayActivities tbody tr'), $tr, name, price;
            CommonInfo.SelfPayActivities.splice(0, CommonInfo.SelfPayActivities.length);
            if ($trs.length > 0) {
                $trs.each(function (index, element) {
                    $tr = $(element);
                    name = $tr.find('.selfpay-name').val(), price = $tr.find('.selfpay-price').val();
                    if (name.length > 0 && price.length > 0) {
                        CommonInfo.SelfPayActivities.push(name + '|' + price);
                    }
                })
            }
        },
        SetCheckBoxForPage: function () {
            var _this = this;
            $('#commonInfo .checkbox').each(function (index, ele) {
                var $ele = $(ele), $input = $ele.find('input');
                switch ($input.attr('name')) {
                    case 'promotion':
                        if (_this.Promotion != null) {
                            if (_this.Promotion.indexOf($input.val()) != -1)
                                $input.parent().checkbox('check');
                        }
                        break;
                    case 'theme':
                        if (_this.Theme != null) {
                            if (_this.Theme.indexOf($input.val()) != -1)
                                $input.parent().checkbox('check');
                        }
                        break;
                    case 'availabledates':
                        if (_this.AvailableDates != null) {
                            if (_this.AvailableDates.indexOf($input.val()) != -1)
                                $input.parent().checkbox('check');
                        }
                        break;
                    case 'usepoints':
                        if (_this.UsePoints)
                            $input.parent().checkbox('check');
                        break;
                    case 'booking':
                        if (_this.BookingRequired)
                            $input.parent().checkbox('check');
                }
            })
        },
        SetFormDataForPage: function () {
            var $inputs = $('#commonInfo').find('input').not('.search,#fileUpload,input[type=checkbox],#selfpayActivities input,input[name=__RequestVerificationToken],.saved-photo input,#typeSelection input');
            var $input, outcome, _this = this;
            function PendingQueue() {
                var dfd = $.Deferred();
                $inputs.each(function (index, input) {
                    $input = $(input);
                    //console.log($input.data('db-key') + " :" + $input.val().trim());
                    $input.val(_this[$input.data('db-key')]);
                });
                setTimeout(function () {
                    dfd.resolve();
                }, 100);
                return dfd.promise();
            }
            PendingQueue().done(function () {
                $('#regionSelection').dropdown("set selected", CommonInfo.RegionId);
                $('#typeSelection').dropdown("set selected", CommonInfo.TripType.split('|'));
                $('#currency').dropdown("set selected", CommonInfo.CurrencyType);
            }).then(function () {
                introEditor.txt.html($.htmlDecode(_this.Introduction));
                includeEditor.txt.html($.htmlDecode(_this.Include));
                excludeEditor.txt.html($.htmlDecode(_this.Exclude));
            })
        },
        SetSelfPayTableForPage: function () {
            var _this = this, $table = $('#selfpayActivities tbody'), html = '', elementArray;
            $.each(_this.SelfPayActivities, function (index, ele) {
                elementArray = ele.split('|');
                html += '<tr><td><input type="text" class="selfpay-name" value="' + elementArray[0] + '" /></td><td><input type="text" class="selfpay-price" value="' + elementArray[1] + '" /></td><td><button class="ui red icon button delete"><i class="icon delete"></i></button><button class="ui primary icon button append"><i class="icon plus"></i></button></td></tr>'
            })
            $table.html(html);
        },
        SetPhotosForPage:function(){
            var _this = this, $container = $('#photoListContainer .saved-photo'), html = '';
            $container.html('');//clean up container
            $.each(_this.Photos, function (index, ele) {
                $container.loadTemplate('#savedTmpl', ele, tmplOpt)
                var img = document.createElement('img');
                img.src = ele.FileLocation;
                labelHtml = '<a class="ui red right corner label"><i class="newspaper icon" ></i ></a >';
                var $image = $('#' + ele.Id).find('.image');
                if (CommonInfo.FrontCover != null && CommonInfo.FrontCover.Id == ele.Id) {
                    $image.addClass('cover').append(labelHtml).append(img);
                    $image.find('.button.setCover').addClass('hidden');
                } else {
                    $image.append(img);
                }
            })
        },
        SyncJs: function () {
            var _this = this, object = {};
            _this.SetCheckBoxForJs();
            _this.SetFormDataForJs();
            _this.SetSelfPayTableForJs();
            UpdateObj(object,_this);
            return object;
            //console.log(this);
        },
        SyncPage: function () {
            var _this = this;
            _this.SetCheckBoxForPage();
            _this.SetFormDataForPage();
            _this.SetSelfPayTableForPage();
            _this.SetPhotosForPage();
            $CommonInfo = $('#commonInfo');
            $CommonInfo.find('input[name=productid]').val(Trip.ProductId);
        }
    },
    ProductInfo = {
        DepartingCity: '',
        ArrivingCity: '',
        FinishingCity: '',
        Sceneries: '',
        TotalDays: 1,
        Feature: '',
        Intro: '',
        LoadCityData: function (url) {
            $.get(url).done(function (data) {
                var html = '';
                if (data.status==200) {
                    $.each(data.data, function (index, element) {
                        html += '<div class="item" data-value="' + element.Id + '" data-initial="' + element.Initial + '">' + element.Name + '</div>'
                    })
                }
                $('.city-selection .menu').html(html);
            }).fail(function (xhr) { $.tip(".message-container", "载入城市数据失败", "服务器超时，请稍后重试！", "negative", 4); })
        },
        LoadSceneryData: function (url) {
            $.get(url).done(function (data) {
                var html = '';
                if (data.success) {
                    $.each(data.results, function (index, element) {
                        html += '<div class="item" data-value="' + element.value + '">' + element.name + '</div>'
                    })
                    localStorage.sceneries = html;
                }
                $('.scenery-selection .menu').html(html);
            }).fail(function (xhr) { $.tip(".message-container", "载入城市数据失败", "服务器超时，请稍后重试！", "negative", 4); })
        },
        SetFormDataForJs: function () {
            var $inputs = $('#productInfo').find('input').not('.search');
            var $input, _this = this;
            $inputs.each(function (index, input) {
                $input = $(input);
                if ($input.data('db-key') == 'DepartingCity' || $input.data('db-key') == 'ArrivingCity' || $input.data('db-key') == 'FinishingCity') {
                    var idArray = $input.val().trim().split(','), nameArray = $input.siblings('a'), dataStr = '';
                    for (var i = 0; i < idArray.length; i++) {
                        dataStr += idArray[i] + ',' + nameArray[i].innerText.split('----')[1] + '|';
                    }
                    dataStr = dataStr.substr(0, dataStr.length - 1);
                    _this[$input.data('db-key')] = dataStr;
                    
                } else if ($input.data('db-key') == 'Sceneries') {
                    //var idArray = $input.val().trim().split(','), nameArray = $input.siblings('a'), dataStr = '';
                    //for (var i = 0; i < idArray.length; i++) {
                    //    dataStr += idArray[i] + ',' + nameArray[i].innerText + '|';
                    //}
                    //_this[$input.data('db-key')] = dataStr;
                } else {
                    _this[$input.data('db-key')] = $input.val().trim();
                }
            });
            _this.Feature = $.htmlEncode(pFeatureEditor.txt.html());
            _this.Intro = $.htmlEncode(pIntroEditor.txt.html());
        },
        SetFormDataForPage: function () {
            var _this = this, departure = new Array(), arrival = new Array(), finish = new Array(),sceneries = new Array();
            $('#productInfo input[name=TotalDays]').val(ProductInfo.TotalDays);
            function PendingQueue() {
                var dfd = $.Deferred();
                $.each(ProductInfo.DepartingCity.split('|'), function (index, element) {
                    departure.push(element.split(',')[0]);
                })
                $.each(ProductInfo.ArrivingCity.split('|'), function (index, element) {
                    arrival.push(element.split(',')[0]);
                })
                $.each(ProductInfo.FinishingCity.split('|'), function (index, element) {
                    finish.push(element.split(',')[0]);
                })
                setTimeout(function () {
                    dfd.resolve();
                }, 150);
                return dfd.promise();
            }
            PendingQueue().done(function () {
                $('#departingCity').dropdown("set selected", departure);
                $('#arrivingCity').dropdown("set selected", arrival);
                $('#finishingCity').dropdown("set selected", finish);
            }).then(function () {
                pIntroEditor.txt.html($.htmlDecode(_this.Intro));
                pFeatureEditor.txt.html($.htmlDecode(_this.Feature))
            })
            //if (ProductInfo.Sceneries != null) {
            //    $.each(ProductInfo.Sceneries.split('|'), function (index, element) {
            //        sceneries.push(element.split(',')[0]);
            //    })
            //}
        },
        SyncJs: function () {
            var _this = this, object = {};
            _this.SetFormDataForJs();
            UpdateObj(object,_this);
            return object;
        },
        SyncPage: function () {
            return ProductInfo.SetFormDataForPage();
        }

    },
    Schedules = new Array(),
    TripProperty = {
        HotelPrices: new Array(),
        DepartingLocation: '',
        SelectableRoutes: new Array(),
        SelfChooseActivities:new Array(),
        LoadStartingPointsData: function (url) {
            $.get(url).done(function (data) {
                var html = '';
                if (data.status == 200) {
                    $.each(data.data, function (index, element) {
                        html += '<div class="item" data-value="' + element.value + '">' + element.name + '</div>'
                    })
                }
                $('.departure-selection .menu').html(html);
            }).fail(function (xhr) { $.tip(".message-container", "载入出发地数据失败", "服务器超时，请稍后重试！", "negative", 4); })
        },
        SetHotelUpgradeForJs: function () {
            var $trs = $('#hotelUpgrade tbody tr'), $input;
            TripProperty.HotelPrices.splice(0, TripProperty.HotelPrices.length);
            $trs.each(function (index, td) {
                var hotelPrice = new HotelPrice();
                $(td).find('input').each(function (ind, input) {
                    $input = $(input);
                    hotelPrice[$input.attr('name')] = $input.val();
                })
                if (hotelPrice.Name != '') {
                    TripProperty.HotelPrices.push(hotelPrice);
                }
            })
        },
        SetSelectableRouteForJs: function () {
            var $trs = $('#selectableRoutes tbody tr'), $input;
            TripProperty.SelectableRoutes.splice(0, TripProperty.SelectableRoutes.length);
            $trs.each(function (index, td) {
                var route = '';
                $(td).find('input').not('.search').each(function (ind, input) {
                    route = route + $(input).val() + '|';
                })
                route = route.substr(0, route.length - 1);
                if (route.indexOf('|') > 0 && route.length > 24) {
                    TripProperty.SelectableRoutes.push(route);
                }
            })
        },
        SetSelfChooseActivitiesForJs: function () {
            var $trs = $('#selfChooseActivities tbody tr'), $input;
            TripProperty.SelfChooseActivities.splice(0, TripProperty.SelfChooseActivities.length);
            $trs.each(function (index, td) {
                var activity = '', validInput = true, $input;
                $(td).find('input').not('.search').each(function (ind, input) {
                    $input = $(input);
                    if ($input.val().length == 0)
                        validInput = false;
                    activity = activity + $(input).val() + '|';
                })
                if (validInput) {
                    activity = activity.substr(0, activity.length - 1);
                    TripProperty.SelfChooseActivities.push(activity);
                }
            })
        },
        SetHotelUpgradeForPage: function () {
            var _this = this, $container = $('#hotelUpgrade tbody');
            if (_this.HotelPrices.length > 0) {
                $.each(_this.HotelPrices, function (index, element) {
                    $container.loadTemplate('#hotelPriceTmpl', element, tmplOpt);
                })
            } else {
                $container.loadTemplate('#hotelPriceTmpl', "", tmplOpt);
            }
            
        },
        SetSelectableRouteForPage: function () {
            var _this = this, $container = $('#selectableRoutes tbody'), data, model = {},$route;
            if (_this.SelectableRoutes.length > 0) {
                $.each(_this.SelectableRoutes, function (index, element) {
                    data = element.split('|');
                    model.routeId = $.uuid();
                    model.Name = data[0];
                    $container.loadTemplate('#routeTmpl', model, tmplOpt);
                    $route = $('#' + model.routeId);
                    $route.find('.menu').html(localStorage.sceneries);
                    $route.find('.dropdown').dropdown('set selected', data[1].split(','));
                })
            } else {
                $('#selectableRoutes .add.button').click();
            }
            
        },
        SetSelfChooseActivitiesForPage: function () {
            var _this = this, $container = $('#selfChooseActivities tbody'), data = '', html = '';
            if (_this.SelfChooseActivities.length > 0) {
                $.each(_this.SelfChooseActivities, function (index, element) {
                    data = element.split('|');
                    html += '<tr><td><input type="text" name="name" value=' + data[0] + ' /></td><td><input type="text" name="price" value=' + data[1].replaceAll(" ", "") + ' /></td><td><button class="ui red icon button delete"><i class="icon delete"></i></button></td></tr>';
                })
            }
            $('#selfChooseActivities tbody').html(html);
        },
        SyncJs: function () {
            var _this = this;
            _this.SetHotelUpgradeForJs();
            _this.DepartingLocation = $('#TripProperty .departure-selection input[name=DepartingLocation]').val();
            //_this.SetSelectableRouteForJs();
            _this.SetSelfChooseActivitiesForJs();
        },
        SyncPage: function () {
            var _this = this;
            _this.SetHotelUpgradeForPage();
            //_this.SetSelectableRouteForPage();
            _this.SetSelfChooseActivitiesForPage();
            if (typeof _this.DepartingLocation == 'string') {
                var locations = _this.DepartingLocation.split(',');
                $('#TripProperty .departure-selection').dropdown();
                setTimeout(function () {
                    $('#TripProperty .departure-selection').dropdown("set selected", locations);
                },200)
            }
        }
    },
    TripPlans = new Array(),
    Trip = {
        Id: '',
        IsVisible: false,
        IsDeleted: false,
        ProductId: '',
        Popularity: 0,
        TripOrdered: 0,
        SortOrder: 0,
        DisplayOnFrontPage: false,
        LoadServerData: function () {
            Trip.Id = $.getQueryStringByName('tripId');
            if (Trip.Id.length == 24) {
                $.get(api.tripDataUrl, { tripId: Trip.Id }).done(function (data) {
                    console.log(data);
                    if (data.Success) {
                        Trip.SyncLocalData(data, true);
                    } else {
                        Trip.Id = '';
                        publishable = false;
                        $.tip(".message-container", "载入数据失败", "服务器超时，或数据已损坏！！", "negative", 4);
                    }
                }).fail(function (xhr) { $.tip(".message-container", "载入数据失败", "服务器超时，请稍后重试！", "negative", 4); });
            } else {
                if (Trip.Id != '') {
                    publishable = false;
                    $.tip(".message-container", "载入数据失败", "TripId参数有误", "negative", 4);
                } else {
                    publishable = false;
                    Trip.UpdateSchedulesObj(new Array);
                    Trip.SetSchedulesForPage();
                    Trip.SetTripPlansForPage();
                }
            }
        },
        UpdateSchedulesObj: function (remoteData) {
            var _this = this;
            if (remoteData.length > 0) {
                $.each(remoteData, function (index, element) {
                    Schedules.push(new Schedule(element.Id, element.Day, element.Name, element.Description, element.Details, element.Meal, element.Accommodation, element.GroupPickUp, element.PickUp, element.Introduction));
                })
            } else {
                Schedules.push(new Schedule($.uuid()));
            }
        },
        UpdateTripPlansObj: function (remoteData) {
            var _this = this;
            if (remoteData.length > 0) {
                $.each(remoteData, function (index, element) {
                    if (element.Type == 0) {
                        element.Type = '天天发团'
                    } else if (element.Type == 1) {
                        element.Type = '指定日期发团'
                    } else if (element.Type == 2) {
                        element.Type = '定期发团'
                    }
                    TripPlans.push(new TripPlan(element.Id, element.Type, element.IsOneDayOnly, element.IsRoomDiffApplied, element.SelectedDates, element.WeekInfo, element.TripPrices, element.RaisePriceByPercentage, element.AdditionalPrice));
                })
            } else {
                TripPlans.push(new TripPlan($.uuid()));
            }
        },
        SetTripPlansForJs: function () {
            $('#generatorTable .add.button').click();
        },
        SetTripPlansForPage: function (activeIndex) {
            if (typeof activeIndex != 'number' || activeIndex > TripPlans.length || Number.isNaN(activeIndex) || activeIndex < 0)
            {
                activeIndex = 0;
            }
            var _this = this, $menuContainer = $('#TripPlans .ui.top.menu'), $form = $('#dateGenerator .ui.form'),
                $container = $('#displayTable tbody'), $tableBody = $('#generatorTable tbody'), menuHtml = '', obj = {}, $input;
            if (TripPlans.length == 0) {
                TripPlans.push(new TripPlan($.uuid()));
            }
            //清空显示区数据
            $container.html('');
            //写入显示区数据
            $.each(TripPlans, function (index, element) {
                index == activeIndex ? active = 'active' : active = '';
                //创建Menu
                menuHtml += '<div class="item-container" id="' + element.Id + '" data-index="' + index + '"><a class="item ' + active + '">发团计划<span>' + (index + 1) + '</span></a><div class="icon-container"><i class="plus icon"></i><i class="remove icon"></i></div></div>';
                $.each(element.TripPrices, function (ind,ele) {
                    obj = ele.BasePrice;
                    obj.index = ind;
                    obj.parentId = element.Id;
                    ele.TripDate.startsWith('/Date') ? obj.TripDate = ele.TripDate = $.ConvertJsonToDate(ele.TripDate).toSimpleDateString() : obj.TripDate = ele.TripDate;
                    $container.loadTemplate('#planTmpl', obj, tmplOpt);
                })
            })
            $menuContainer.html(menuHtml)
            //写入TripPlan生成器信息
            try{
                var activePlan = TripPlans[activeIndex];
                //console.log(activePlan)
                $form.find('.trip-type').dropdown('set selected', activePlan.Type);
                $form.find('.oneday-only').dropdown('set selected', activePlan.IsOneDayOnly);
                $form.find('.room-diff').dropdown('set selected', activePlan.IsRoomDiffApplied);
                if (activePlan.TripPrices.length > 0) {
                    $tableBody.find('input').each(function (index, input) {
                        $input = $(input);
                        $input.val(activePlan.TripPrices[0].BasePrice[$input.attr('name')]);
                    })
                }
                
                switch (activePlan.Type) {
                    case '天天发团':
                    case '定期发团':
                        var dateArray = activePlan.SelectedDates.split(',');
                        $form.find('input[name=startdate]').val(dateArray[0]);
                        $form.find('input[name=enddate]').val(dateArray[1]);
                        var weekInfo = activePlan.WeekInfo.split(',');
                        $form.find('.week.checkbox').each(function (index, element) {
                            $element = $(element);
                            if (weekInfo.indexOf($element.find('input[name=week]').val()) != -1) {
                                $element.checkbox('check');
                            }
                        })
                        break;
                    case '指定日期发团':
                        $form.find('input[name=selecteddates]').val(activePlan.SelectedDates);
                        break;
                    default:
                        $.tip('.message-container', "发团类型错误", "请检查您的发团类型！！", "negative", 4)
                        break;
                }
            } catch (ex) {
                console.log(ex);
            }
            
            
        },
        SetSchedulesForJs: function () {
            var _this = this, $tabs = $('#scheduleContainer .day.tab');
            $tabs.each(function (index, element) {
                //每天的schedule
                var correspondingSchedule = Schedules.find(function (schedule) { return schedule.Id == $(element).attr('id') });
                if (correspondingSchedule != null) {
                    var $inputs = $(element).find('input').not($('#' + correspondingSchedule.containerId).find('input')), $input;
                    $inputs.each(function (ind, ele) {
                        $input = $(ele);
                        correspondingSchedule[$input.data('db-key')] = $input.val().trim();
                    })
                    var $scheduleItems = $('#' + correspondingSchedule.containerId).find('.schedule-item');
                    $scheduleItems.each(function (i, e) {
                        var id = $(e).find('.weditor-default').attr('id'), $inputs = $(e).find('input').not('.search'),
                            item = correspondingSchedule.Details.find(function (item) { return item.Id == id });
                        if (item != 'undefined') {
                            $inputs.each(function (iterator, input) {
                                $input = $(input);
                                if ($input.data('db-key') == 'Sceneries') {
                                    if ($input.val() == "") {
                                        RemoveElementFromArray(correspondingSchedule.Details, item);
                                        return false;
                                    } else {
                                        var idArray = $input.val().trim().split(','), nameArray = $input.siblings('a'), dataStr = '';
                                        for (var i = 0; i < idArray.length; i++) {
                                            dataStr += idArray[i] + ',' + nameArray[i].innerText + '|';
                                        }
                                        item[$input.data('db-key')] = dataStr;
                                    }
                                } else {
                                    item[$input.data('db-key')] = $input.val().trim();
                                }
                                
                            })
                            if (item != null)
                                item.Arrangement = $.htmlEncode(scheduleEditor[id].txt.html());
                        }
                    })
                    //correspondingSchedule.Description = $.htmlEncode(UE.getEditor(correspondingSchedule.descId).getContent());
                    //correspondingSchedule.Introduction = $.htmlEncode(UE.getEditor(correspondingSchedule.introId).getContent());
                }
            })
            return Schedules;
        },
        SetSchedulesForPage: function () {
            var _this = this;
            var $scheduleContainer = $('#scheduleContainer');
            var $menuContainer = $('#scheduleList .ui.top.menu'), menuHtml = '', active = '', tabPath = '';
            $.each(Schedules, function (index, element) {
                //判定当前tab是否active。第一个载入的tab为active tab。
                index == 0 ? active = 'active' : active = '';
                element.class = "ui bottom attached tab segment day " + active;
                tabPath = "单日行程/" + element.Day;
                menuHtml += '<div class="item-container"><a class="item ' + active + '" data-tab="' + tabPath + '">第<span>' + element.Day + '</span>天</a><div class="icon-container"><i class="plus icon"></i><i class="remove icon"></i></div></div>';
                element.tabPath = tabPath;
                element.containerId = $.uuid();
                //element.descId = $.uuid();
                //element.introId = $.uuid();
                $scheduleContainer.loadTemplate('#scheduleTmpl', element, tmplOpt);
                //UE.getEditor(element.descId).ready(function (r) {
                //    this.setContent($.htmlDecode(element.Description))
                //})
                //UE.getEditor(element.introId).ready(function (r) {
                //    this.setContent($.htmlDecode(element.Description))
                //})
                $.each(element.Details, function (ind, ele) {
                    ele.Id = 's' + ele.Id;
                    var sceneries = new Array();
                    $('#' + element.containerId).loadTemplate('#scheduleItemTmpl', ele, tmplOpt);
                    var $sceneryDropdown = $('#' + ele.Id).parents('.inline.fields').siblings('.inline.field').find('.scenery-selection');
                    $sceneryDropdown.find('.menu').html(localStorage.sceneries);
                    if (ele.Sceneries != null) {
                        $.each(ele.Sceneries.split('|'), function (index, element) {
                            sceneries.push(element.split(',')[0]);
                        })
                        $sceneryDropdown.dropdown("set selected", sceneries);
                    }
                    var editor = new E('#' + ele.Id);
                    editor.customConfig.zIndex = 1;
                    editor.create();
                    scheduleEditor[ele.Id] = editor;
                    editor.txt.html($.htmlDecode(ele.Arrangement))
                    //UE.getEditor(ele.Id).ready(function () {
                    //    this.setContent($.htmlDecode(ele.Arrangement))
                    //})
                })
            })
           
            $menuContainer.html(menuHtml);
            BindTabs();
        },
        SetSaveButtonForPage: function (isVisible) {
            var $button = $('#saveAll');
            if (isVisible) {
                $button.html('<i class="icon refresh"></i>隐藏路线');
            } else {
                $button.html('<i class="icon refresh"></i>发布路线');
            }
        },
        SyncLocalData: function (data, writePage) {
            if (data.Success) {
                var _this = this;
                UpdateObjWithFunctions(CommonInfo, data.Entity.CommonInfo);
                UpdateObjWithFunctions(ProductInfo, data.Entity.ProductInfo);
                UpdateObjWithFunctions(TripProperty, data.Entity.TripProperty);
                _this.UpdateTripPlansObj(data.Entity.TripPlans);
                _this.UpdateSchedulesObj(data.Entity.Schedules);
                _this.Id = data.Entity.Id;
                _this.ProductId = data.Entity.ProductId;
                _this.IsDeleted = data.Entity.IsDeleted;
                _this.IsVisible = data.Entity.IsVisible;
                _this.Popularity = data.Entity.Popularity;
                _this.TripOrdered = data.Entity.TripOrdered;
                _this.SortOrder = data.Entity.SortOrder;
                _this.DisplayOnFrontPage = data.Entity.DisplayOnFrontPage;
            }
            if (writePage) {
                _this.SetSaveButtonForPage(_this.IsVisible);
                CommonInfo.SyncPage();
                ProductInfo.SyncPage();
                _this.SetSchedulesForPage();
                _this.SetTripPlansForPage();
                TripProperty.SyncPage();
            } else {
                console.log(_this);
            }
        },
        SyncRemoteData: function (type) {
            var _this = this;
            _this.CommonInfo = CommonInfo.SyncJs();
            var model = {}, $buttons = $('.toor-bar button');
            if (Trip.Id == '') {
                UpdateObj(model, _this);
                $.ajax({
                    url: api.addTripUrl,
                    method: 'post',
                    beforeSend: function () {
                        $buttons.addClass('loading');
                    },
                    data: {
                        entity: model,
                        __RequestVerificationToken: token
                    },
                    success: function (result) {
                        if (result.Success) {
                            Trip.Id = result.Message;
                            //CommonInfo.SyncPage();
                            var tripPath = location.pathname + "?tripid=" + Trip.Id;
                            location.replace(tripPath);
                        }
                        $buttons.removeClass('loading');
                    },
                    error: function (data) { $.tip(".message-container", "数据传输失败", "服务器超时，请稍后重试！", "negative", 4); $buttons.removeClass('loading'); }
                })
            } else {
                switch (type) {
                    case '通用信息':
                        model = CommonInfo.SyncJs();
                        break;
                    case '产品概要':
                        model = ProductInfo.SyncJs();
                        break;
                    case '单日行程':
                        model = Trip.SetSchedulesForJs();
                        break;
                    case '发团属性':
                        TripProperty.SyncJs();
                        model = TripProperty;
                        break;
                    case '发团计划':
                        //Trip.SetTripPlansForJs()
                        model = TripPlans;
                        break;
                }
                for (var p in model) {
                    if (typeof model[p] == 'array') {
                        model[p] = JSON.stringify(model[p]);
                    }
                }
                console.log(model);
                $.ajax({
                    url: api.partialDataUrl,
                    method: 'post',
                    beforeSend: function () {
                        $buttons.addClass('loading');
                    },
                    data: {
                        tripId:Trip.Id,
                        data: JSON.stringify(model),
                        type: type,
                        __RequestVerificationToken: token
                    },
                    success: function (result) {
                        if (result.Success) {
                            $.tip(".message-container", "保存成功", result.Message + " 保存成功！", "positive", 4);
                        } else {
                            publishable = false;
                            $.tip(".message-container", "保存失败", result.Message, "negative", 4);
                        }
                        $buttons.removeClass('loading');
                    },
                    error: function (data) { $.tip(".message-container", "数据传输失败", "服务器超时，请稍后重试！", "negative", 4); $buttons.removeClass('loading'); }
                })
            }
            
        },

    };
    var Schedule = function (Id, Day, Name, Description, Details, Meal, Accommodation, GroupPickUp, PickUp, Introduction) {
        if (this instanceof Schedule) {
            this.Id = Id;
            this.Day = typeof Day == 'undefined' ? Day = 1 : Day;
            this.Name = typeof Name == 'undefined' ? Name = '' : Name;
            this.Description = typeof Description == 'undefined' ? Description = '' : Description;
            if (typeof Details != 'object') { Details = new Array(); }
            this.Details = Details;
            this.Meal = typeof Meal == 'undefined' ? Meal = '' : Meal;
            this.Accommodation = typeof Accommodation == 'undefined' ? Accommodation = '' : Accommodation;
            this.GroupPickUp = typeof GroupPickUp == 'undefined' ? GroupPickUp = '' : GroupPickUp;
            this.PickUp = typeof PickUp == 'undefined' ? PickUp = '' : PickUp;
            this.Introduction = typeof Introduction == 'undefined' ? Introduction = '' : Introduction;
        } else {
            return new Schedule(Id, Day, Name, Description, Details, Meal, Accommodation, GroupPickUp, PickUp, Introduction);
        }
    },
    ScheduleItem = function (Id, Sceneries, ActivityTime, Arrangement, Memo) {
        if (this instanceof ScheduleItem) {
            this.Id = Id;
            this.Memo = Memo;
        } else {
            this.Sceneries = Sceneries;
            this.ActivityTime = ActivityTime;
            this.Arrangement = Arrangement;
            return new ScheduleItem(Sceneries, ActivityTime, Arrangement, Memo);
        }
    },
    PickUpInfo = function (IsFree, PickUpStartAt, PickUpEndAt, Price, Title) {
        if (this instanceof PickUpInfo) {
            this.IsFree = IsFree;
            this.PickUpStartAt = PickUpStartAt;
            this.PickUpEndAt = PickUpEndAt;
            this.Price = Price;
            this.Title = Title;
        } else {
            return new PickUpInfo(IsFree, PickUpStartAt, PickUpEndAt, Price, Title);
        }
    },
    HotelPrice = function (Name, SinglePrice, DoublePrice, TriplePrice, QuadplexPrice, ChildPrice, ShareRoomPrice) {
        if (this instanceof HotelPrice) {
            this.Name = typeof Name == 'undefined' ? Name = '' : Name;
            this.SinglePrice = SinglePrice;
            this.DoublePrice = DoublePrice;
            this.TriplePrice = TriplePrice;
            this.QuadplexPrice = QuadplexPrice;
            this.ChildPrice = ChildPrice;
            this.ShareRoomPrice = typeof ShareRoomPrice == 'undefined' ? ShareRoomPrice = '0' : ShareRoomPrice;
        } else {
            return new HotelPrice(Name, SinglePrice, DoublePrice, TriplePrice, QuadplexPrice, ChildPrice, ShareRoomPrice);
        }
    },
    TripPricesForSpecificDate = function (TripDate, BasePrice) {
        if (this instanceof TripPricesForSpecificDate) {
            this.TripDate = TripDate;
            this.BasePrice = BasePrice;
        } else {
            return new TripPricesForSpecificDate(TripDate, BasePrice);
        }
    },
    PhotoInfo = function (Id, Name, Description, FileLocation) {
        if (this instanceof PhotoInfo) {
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.FileLocation = FileLocation;
        } else {
            return new PhotoInfo(Name, Description, Description, FileLocation);
        }
    },
    TripPlan = function (Id, Type, IsOneDayOnly,IsRoomDiffApplied,SelectedDates,WeekInfo,TripPrices, RaisePriceByPercentage, AdditionalPrice) {
        this.Id = Id;
        this.Type = typeof Type == 'undefined' ? Type = '天天发团' : Type;
        this.IsOneDayOnly = typeof IsOneDayOnly == 'undefined' ? IsOneDayOnly = false : IsOneDayOnly;
        this.IsRoomDiffApplied = typeof IsRoomDiffApplied == 'undefined' ? IsRoomDiffApplied = false : IsRoomDiffApplied;
        this.SelectedDates = typeof SelectedDates == 'undefined' ? SelectedDates = '' : SelectedDates;
        this.WeekInfo = typeof WeekInfo == 'undefined' ? WeekInfo = '' : WeekInfo;
        this.TripPrices = typeof TripPrices == 'undefined' ? TripPrices = new Array() : TripPrices;
        this.RaisePriceByPercentage = typeof RaisePriceByPercentage == 'undefined' ? RaisePriceByPercentage = 0 : RaisePriceByPercentage;
        this.AdditionalPrice = typeof AdditionalPrice == 'undefined' ? AdditionalPrice = new HotelPrice('',0,0,0,0,0,0) : AdditionalPrice;
    };
    function BindTabs() {
        $('#tripContext .menu .item').tab({
            context: $('#tripContext'),
            onLoad: function (tabPath) {
                if (tabPath.indexOf('单日行程/') != -1) {
                    var $item = $('#scheduleList .menu');
                    $item.find('.item.active').removeClass('active');
                    $item.find('.item').each(function (index, element) {
                        var $ele = $(element);
                        if ($ele.data('tab') == tabPath)
                            $ele.addClass('active');
                    })
                }
                if (tabPath.indexOf('单日行程') != -1)
                    tabPath = '单日行程';
                $('#saveTab span').text(tabPath);
            }
        });
    }
    function BindCommonInfo() {
        $('#photoListContainer')
            .delegate('.content-editable.photo-desc', 'click', function (e) {
            $(e.target).attr('contenteditable', true);
            })
            .delegate('#pendingList .content-editable.photo-desc', 'blur', function (e) {
            var id = $(e.target).parents('.photo-item').attr('id');
            var photo = CommonInfo.PendingPhotos.find(function (element) {
                return element.Id == id;
            });
            photo.Description = $(e.target).text();
            })
            .delegate('.saved-photo .content-editable.photo-desc', 'blur', function (e) {
            var id = $(e.target).parents('.photo-item').attr('id');
            var photo = CommonInfo.Photos.find(function (element) {
                return element.Id == id;
            });
            photo.Description = $(e.target).text();
            })
            .delegate('.saved-photo .button.delete', 'click', function (e) {
            e.preventDefault();
            var id = $(e.target).parents('.photo-item').attr('id');
            $.ajax({
                url: api.deletePhotoUrl,
                method: 'post',
                beforeSend: function () {
                    $(e.target).addClass('loading');
                },
                data: {
                    tripId: Trip.Id,
                    photoId: id,
                    __RequestVerificationToken: token
                },
                success: function (result) {
                    if (result.Success) {
                        for (var i = 0; i <= CommonInfo.Photos.length - 1; i++) {
                            if (CommonInfo.Photos[i].Id == id) {
                                CommonInfo.Photos.splice(i, 1);
                            }
                        }
                        $('#' + id).remove();
                        $.tip(".message-container", "删除成功", "图片已删除", "positive", 4);
                    } else {
                        $.tip(".message-container", "删除失败", result.Message, "negative", 4);
                    }
                    $(e.target).removeClass('loading');
                },
                error: function (data) { $.tip(".message-container", "图片删除失败", "服务器超时，请稍后重试！", "negative", 4); $(e.target).removeClass('loading'); }
            })
            return false;
            })
            .delegate('.saved-photo .button.setCover', 'click', function (e) {
                e.preventDefault();
                var id = $(e.target).parents('.photo-item').attr('id');
                $.ajax({
                    url: api.setCoverUrl,
                    method: 'post',
                    beforeSend: function () {
                        $(e.target).addClass('loading');
                    },
                    data: {
                        tripId: Trip.Id,
                        photoId: id,
                        __RequestVerificationToken: token
                    },
                    success: function (result) {
                        if (result.Success) {
                            $('.image.cover').removeClass('.cover').find('a').remove();
                            var labelHtml = '<a class="ui red right corner label"><i class="newspaper icon" ></i ></a >';
                            var $target = $('#' + id).find('.image');
                            $target.addClass('cover').append(labelHtml);
                        }
                        $(e.target).removeClass('loading');
                    },
                    error: function (data) { $.tip(".message-container", "图片删除失败", "服务器超时，请稍后重试！", "negative", 4); $(e.target).removeClass('loading'); }
                })
                return false;
            })
            .delegate('.save-single.button', 'click', function (e) {
            e.preventDefault();
            $(e.target).addClass('loading');
            if (Trip.Id != '') {
                var photoId = $(e.target).parents('.photo-item').attr('id');
                $.each(uploadArr, function (index, element) {
                    if (element.uniqueId == photoId)
                        return element.submit();
                })
            } else {
                $('#saveTab').click();
            }

            return false;
        }).delegate('.cancel-single.button', 'click', function (e) {
            e.preventDefault();
            $(e.target).addClass('loading');
            var photoId = $(e.target).parents('.photo-item').attr('id');
            if ($(e.target).siblings('.save-single').hasClass('loading')) {
                $.each(uploadArr, function (index, element) {
                    if (element.uniqueId == photoId) {
                        return element.abort();
                    }

                })
                $(e.target).removeClass('loading');
            } else {
                $('#' + photoId).remove();
            }
            return false;
        }).delegate('.save-all.button', 'click', function (e) {
            if (uploadArr.length == 0) {
                $('#fileUpload').click();
            } else {
                if (Trip.Id != '') {
                    $('.save-single.button').not('.loading').click();
                } else {
                    $('#saveTab').click();
                }
            }
            return false;
        }).delegate('.cancel-all.button', 'click', function (e) {
            $('.cancel-single.button').click();
            return false;
        })
        $('#currency').dropdown('set selected', TripPlan.CurrencyType);
        $('#selfpayActivities').delegate('.button.delete', 'click', function (e) {
            $(e.target).parents('tr').remove();
            return false;
        }).delegate('.button.add,.button.append', 'click', function (e) {
            var tmpl = '<tr><td><input type="text" class="selfpay-name" /></td><td><input type="text" class="selfpay-price" /></td><td><button class="ui red icon button delete"><i class="icon delete"></i></button><button class="ui primary icon button append"><i class="icon plus"></i></button></td></tr>';
            var $target = $(e.currentTarget);
            if ($target.hasClass('add')) {
                $('#selfpayActivities tbody').append(tmpl);
            } else if ($target.hasClass('append')) {
                $target.parents('tr').after(tmpl);
            }

            return false;
        })
    }
    function BindTripProperty() {
        $('#TripProperty')
            .delegate('#hotelUpgrade .copy.button', 'click', function (e) {
                e.preventDefault();
                var $tr = $(e.target).parents('tr');
                $tr.after($tr.clone());
            })
        .delegate('#hotelUpgrade .delete.button,#selectableRoutes .delete.button', 'click', function (e) {
            e.preventDefault();
            var $tr = $(e.target).parents('tr');
            $tr.remove();
        })
        .delegate('#hotelUpgrade .add.button', 'click', function (e) {
            e.preventDefault();
            $('#hotelUpgrade tbody').loadTemplate('#hotelPriceTmpl',"", tmplOpt);
        })
        .delegate('#selectableRoutes .copy.button', 'click', function (e) {
            e.preventDefault();
            var $tr = $(e.target).parents('tr');
            $tr.after($tr.clone());
            $tr.next('tr').find('.dropdown').dropdown();
            
        })
        .delegate('#selectableRoutes .add.button', 'click', function (e) {
            e.preventDefault();
            var routeId = $.uuid(), $route;
            $('#selectableRoutes tbody').loadTemplate('#routeTmpl', { "routeId": routeId }, tmplOpt);
            $route = $('#' + routeId);
            $route.find('.menu').html(localStorage.sceneries);
            $route.find('.dropdown').dropdown();
        })
        .delegate('#selfChooseActivities .button.delete', 'click', function (e) {
            e.preventDefault();
            $(e.target).parents('tr').remove();
            return false;
        })
        .delegate('#selfChooseActivities .button.add', 'click', function (e) {
            e.preventDefault();
            var tmpl = '<tr><td><input type="text" name="name" /></td><td><input type="text" name="price" /></td><td><button class="ui red icon button delete"><i class="icon delete"></i></button></td></tr>';
            $('#selfChooseActivities tbody').append(tmpl);

            return false;
        })
    }
    function BindTripPlans() {
        $('#TripPlans')
        .delegate('.tabular.menu a', 'click', function (e) {
            var $target = $(e.target), $form = $('#dateGenerator .ui.form'), $tableBody = $('#dateGenerator tbody'), index = 0, $input;
            $target.parents('.menu').find('.item').removeClass('active')
            $target.addClass('active');
            index = $target.parent().data('index');
            var activePlan = TripPlans[index];
            $form.find('.trip-type').dropdown('set selected', activePlan.Type);
            $form.find('.OneDayOnly').dropdown('set selected', activePlan.IsOneDayOnly);
            $tableBody.find('input').each(function (index, input) {
                $input = $(input);
                $input.val(activePlan.TripPrices[0].BasePrice[$input.attr('name')]);
            })
            switch (activePlan.Type) {
                case '天天发团':
                case '定期发团':
                    var dateArray = activePlan.SelectedDates.split(',');
                    $form.find('input[name=startdate]').val(dateArray[0]);
                    $form.find('input[name=enddate]').val(dateArray[1]);
                    var weekInfo = activePlan.WeekInfo.split(',');
                    $form.find('.week.checkbox').each(function (index, element) {
                        $element = $(element);
                        if (weekInfo.indexOf($element.find('input[name=week]').val()) != -1) {
                            $element.checkbox('check');
                        }
                    })
                    break;
                case '指定日期发团':
                    $form.find('input[name=selecteddates]').val(activePlan.SelectedDates);
                    break;
                default:
                    $.tip('.message-container', "发团类型错误", "请检查您的发团类型！！", "negative", 4)
                    break;
            }
        })
        .delegate('.plan-option.selected-days input', 'click', function () {
            $('.modal.calendar-modal').modal('show');
        })
        .delegate('#generatorTable input[name=ShareRoomPrice]', 'focus', function (e) {
            var $tr = $(e.target).parents('tr'), diff = 0;
            diff = $tr.find('input[name=SinglePrice]').val() - $tr.find('input[name=DoublePrice]').val();
            $(e.target).val(diff);
        })
        .delegate('#generatorTable .button.add', 'click', function (e) {
            var $table = $('#generatorTable'), $form = $('#dateGenerator'), $container = $('#displayTable tbody');
            var type = $form.find('.trip-type input').val(), startDate = FormatDate($form.find('.depart-date input[name=startdate]').val(), '起始日期'),
                endDate = FormatDate($form.find('.depart-date input[name=enddate]').val(), '结束日期'), day = 24 * 60 * 60 * 1000, source = {}, $input;
            $table.find('input').each(function (index, input) {
                $input = $(input);
                source[$input.attr('name')] = $input.val();
            })
            var $activeItem = $('#TripPlans .top.menu .active.item').parent();
            var planId = $activeItem.attr('id'), activeIndex = $activeItem.data('index');
            var tripPlan = TripPlans.find(function (plan) { return plan.Id == planId });
            tripPlan.Type = type;
            tripPlan.IsOneDayOnly = $('input[name=IsOneDayOnly]').val();
            tripPlan.IsRoomDiffApplied = $('input[name=IsRoomDiffApplied]').val();
            switch (type) {
                case '天天发团':
                    try {
                        if (typeof startDate != 'object') {
                            return $.tip('.message-container', "起始时间选择错误", "起始时间不能为空哦！！", "negative", 4);
                        } else if (typeof endDate != 'object') {
                            return $.tip('.message-container', "截止时间选择错误", "截止时间不能为空哦！！", "negative", 4);
                        }
                        var dateRange = (endDate.getTime() - startDate.getTime()) / day;
                        if (dateRange > 0) {
                            tripPlan.SelectedDates = startDate.toSimpleDateString() + ',' + endDate.toSimpleDateString();
                            tripPlan.TripPrices.splice(0, tripPlan.TripPrices.length);
                            for (var i = 0; i <= dateRange; i++) {
                                source.TripDate = (new Date(startDate.getTime() + i * day)).toSimpleDateString();
                                source.Name = '计划' + (TripPlans.indexOf(tripPlan) + 1);
                                tripPlan.TripPrices.push(new TripPricesForSpecificDate(source.TripDate, new HotelPrice(source.Name, source.SinglePrice, source.DoublePrice, source.TriplePrice, source.QuadplexPrice, source.ChildPrice, source.ShareRoomPrice)));
                            }
                            Trip.SetTripPlansForPage(activeIndex);
                        } else {
                            $.tip('.message-container', "时间选择错误", "截止时间要比起始时间大哦！！", "negative", 4)
                        }
                    } catch (ex) {
                        console.log(ex);
                    }
                    break;
                case '定期发团':
                    var weekInfo = '', $element;
                    $('#dateGenerator .week.checkbox').each(function (index, element) {
                        $element = $(element);
                        if ($element.checkbox('is checked')) {
                            weekInfo += $element.find('input').val() + ',';
                        }
                    });
                    if (weekInfo == '') {
                        return $.tip('.message-container', "周信息没有选择", "总得选个出发的日子呀亲！！", "negative", 4)
                    }
                    tripPlan.WeekInfo = weekInfo;
                    weekInfo = weekInfo.replaceAll('7', '0').split(',');
                    try {
                        if (typeof startDate != 'object') {
                            return $.tip('.message-container', "起始时间选择错误", "起始时间不能为空哦！！", "negative", 4);
                        } else if (typeof endDate != 'object') {
                            return $.tip('.message-container', "截止时间选择错误", "截止时间不能为空哦！！", "negative", 4);
                        }
                        var dateRange = (endDate.getTime() - startDate.getTime()) / day;
                        if (dateRange > 0) {
                            tripPlan.SelectedDates = startDate.toSimpleDateString() + ',' + endDate.toSimpleDateString();
                            tripPlan.TripPrices.splice(0, tripPlan.TripPrices.length);
                            for (var i = 0; i <= dateRange; i++) {
                                source.TripDate = new Date(startDate.getTime() + i * day);
                                if (weekInfo.indexOf(source.TripDate.getDay().toString()) != -1) {
                                    source.Name = '计划' + (TripPlans.indexOf(tripPlan) + 1);
                                    source.TripDate = source.TripDate.toSimpleDateString();
                                    tripPlan.TripPrices.push(new TripPricesForSpecificDate(source.TripDate, new HotelPrice(source.Name, source.SinglePrice, source.DoublePrice, source.TriplePrice, source.QuadplexPrice, source.ChildPrice, source.ShareRoomPrice)));
                                }
                            }
                            Trip.SetTripPlansForPage(activeIndex);
                        }
                    } catch (ex) {
                        console.log(ex);
                    }
                    break;
                case '指定日期发团':
                    tripPlan.SelectedDates = $('.selected-days input[name=selecteddates]').val();
                    if (tripPlan.SelectedDates.length > 0) {
                        tripPlan.TripPrices.splice(0, tripPlan.TripPrices.length);
                        $.each(tripPlan.SelectedDates.split(','), function (index, element) {
                            if (element != '') {
                                source.Name = '计划' + (TripPlans.indexOf(tripPlan) + 1);
                                source.TripDate = element;
                                tripPlan.TripPrices.push(new TripPricesForSpecificDate(source.TripDate, new HotelPrice(source.Name, source.SinglePrice, source.DoublePrice, source.TriplePrice, source.QuadplexPrice, source.ChildPrice, source.ShareRoomPrice)));
                            }
                        })
                    }
                    Trip.SetTripPlansForPage(activeIndex);
                    break;
                default:
                    $.tip('.message-container', "发团类型错误", "请检查您的发团类型！！", "negative", 4)
                    break;
            }
        })
        .delegate('#generatorTable .button.save', 'click', function (e) {
            e.preventDefault();
            $('#saveTab').click();
        })
        .delegate('.item-container .plus.icon', 'click', function (e) {
            var _this = this, $menuContainer = $('#TripPlans .ui.top.menu'), $form = $('#dateGenerator .ui.form'), menuHtml = '';
            var tripPlan = new TripPlan($.uuid());
            menuHtml = '<div class="item-container" id="' + tripPlan.Id + '" data-index="' + TripPlans.length + '"><a class="item">发团计划<span>' + (TripPlans.length + 1) + '</span></a><div class="icon-container"><i class="plus icon"></i><i class="remove icon"></i></div></div>';
            TripPlans.push(tripPlan);
            $menuContainer.append(menuHtml);
            $form.find('.trip-type').dropdown('set selected', tripPlan.Type);
            $form.find('.OneDayOnly').dropdown('set selected', tripPlan.IsOneDayOnly);
            $form.find('input[name=selecteddates]').val('');
        })
        .delegate('.item-container .remove.icon', 'click', function (e) {
            var $item = $(e.target).parents('.item-container'),index = +$item.data('index');
            TripPlans.splice(index, 1);
            if (index <= TripPlans.length - 1 && TripPlans.length > 0) {
                $.each(TripPlans, function (iterator, element) {
                    if (iterator >= index) {
                        $.each(element.TripPrices, function (ind, ele) {
                            ele.BasePrice.Name = '计划' + (parseInt(ele.BasePrice.Name.replace('计划', '')) - 1);
                        })
                    }
                })
            }
            Trip.SetTripPlansForPage(index - 1);
        })
        .delegate('#displayTable .save.button', 'click', function (e) {
            var $tr = $(e.target).parents('tr'), $input;
            var plan = TripPlans.find(function (plan) { return plan.Id == $tr.data('parentid') });
            var priceIndex = $tr.data('index');
            $tr.find('input').each(function(index,input){
                $input = $(input);
                plan.TripPrices[priceIndex].BasePrice[$input.attr('name')] = $input.val();
            })
        })
        .delegate('#displayTable .delete.button', 'click', function (e) {
            var $tr = $(e.target).parents('tr');
            var plan = TripPlans.find(function (plan) { return plan.Id == $tr.data('parentid') });
            var priceIndex = -1;
            for (var i = 0; i < plan.TripPrices.length; i++) {
                if (plan.TripPrices[i].TripDate == $tr.find('td')[1].innerText) {
                    priceIndex = i; break;
                }
            }
            plan.TripPrices.splice(priceIndex, 1);
            $tr.remove();
        })
        function FormatDate(dateString, name) {
            if (dateString != '' && typeof dateString!='undefined') {
                dateString = dateString.replace('年', '-').replace('月', '-').replace('日', '');
                return new Date(dateString);
            } else {
                return;
            }
        }
        $('.modal.calendar-modal')
        .delegate('.positive.button', 'click', function (e) {
            var selectedDates = '';
            $.each(highlight, function (index, element) {
                selectedDates += element.toSimpleDateString() + ',';
            })
            $('.plan-option.selected-days input').val(selectedDates);
        })
        $('.tripplan-dropdown.trip-type').dropdown({
            onChange: function (value, text, $choice) {
                $('.plan-option').addClass('hidden');
                switch (value) {
                    case '天天发团':
                        $('.plan-option.everyday').removeClass('hidden');
                        break;
                    case '定期发团':
                        $('.plan-option.everyday,.plan-option.regular-days').removeClass('hidden');
                        break;
                    case '指定日期发团':
                        $('.plan-option.selected-days').removeClass('hidden');
                        break;
                    default:
                        break;
                }
            }
        }).dropdown("set selected", "天天发团");
        $('.tripplan-dropdown').not('.trip-type').dropdown();
        $('.depart-date').calendar({
            ampm: false,
            type: 'date',
            minDate : new Date(),
            formatter: {
                date: function (date, settings) {
                    if (!date) return '';
                    var day = date.getDate();
                    var month = settings.text.monthsShort[date.getMonth()];
                    var year = date.getFullYear();
                    return year + '年' + month + day + '日';
                },
            },
        })
        $('#calendar').calendar({
            ampm: false,
            type: 'date',
            closable: false,
            multiMonth: 12,
            inline: true,
            minDate: new Date(),
            formatter: {
                date: function (date, settings) {
                    if (!date) return '';
                    var day = date.getDate();
                    var month = settings.text.monthsShort[date.getMonth()];
                    var year = date.getFullYear();
                    return year + '年' + month + day + '日';
                },
                cell: function (cell, date, cellOptions) {
                    if (cellOptions.type === 'day'
                        && cellOptions.adjacent === false
                        && cellOptions.disabled === false) {
                        for (var i in highlight) {
                            if (highlight[i].toDateString() == date.toDateString()) {
                                $(cell).addClass('highlighted');
                            }
                        }
                    }
                }
            },
            onChange: function (date, text, mode) {
                var dateObj = new Date(date);
                if (highlight.length == 0) {
                    highlight.push(dateObj)
                } else {
                    for (var i in highlight) {
                        if (highlight[i].toDateString() == dateObj.toDateString()) {
                            highlight.splice(i, 1);
                            return;
                        } 
                    }
                    highlight.push(dateObj);
                }
            },
        });
    }
    function BindSchedules() {
        $('#scheduleList').delegate('.button.add-scenery', 'click', function (e) {
            e.preventDefault();
            var containerId = $(e.target).siblings('.schedule-item-container').attr('id');
            var scheduleItem = new ScheduleItem('s' + $.uuid(), '', '', '', '');
            var scheduleId = $('.tab.active.day').attr('id');
            var schedule = Schedules.find(function (schedule) { return schedule.Id == scheduleId });
            typeof schedule != 'undefined' ? schedule.Details.push(scheduleItem) : $.tip(".message-container", "数据读取错误", "找不到对应行程，建议刷新页面重试", "positive", 4);
            $('#' + containerId).loadTemplate('#scheduleItemTmpl', scheduleItem, tmplOpt);
            var $sceneryDropdown = $('#' + scheduleItem.Id).parents('.inline.fields').siblings('.inline.field').find('.scenery-selection');
            $sceneryDropdown.find('.menu').html(localStorage.sceneries);
            $sceneryDropdown.dropdown();
            var editor = new E('#' + scheduleItem.Id);
            editor.customConfig.zIndex = 1;
            editor.create();
            scheduleEditor[scheduleItem.Id] = editor;
            return false;
        }).delegate('.icon-container .plus.icon', 'click', function (e) {
            var tabPath = $(e.target).parent().siblings().data('tab');//获取当前的tabPath;
            var tabId = '';
            $('#scheduleContainer .day.tab').each(function (index, element) {
                if ($(element).data('tab') == tabPath) {
                    return tabId = $(element).attr('id');//获取被点击添加按钮的tabid
                }
            })
            var index = Schedules.findIndex(function (schedule) { return schedule.Id == tabId });//通过tabid获取对应日程
            if (index >= 0) {
                //添加新的日程
                var schedule = new Schedule($.uuid(), (+Schedules[index].Day) + 1);
                Schedules.splice(index + 1, 0, schedule);
                schedule.tabPath = '单日行程/' + Schedules.length;
                schedule.containerId = $.uuid();
                schedule.descId = $.uuid();
                schedule.introId = $.uuid();
                //loadTempate没找到after方法，先将生成的日程html导入到一个临时的container
                $('#tempHtml').loadTemplate('#scheduleTmpl', schedule, { 'append': false });

                var menuContainers = $('#scheduleList .item-container'), $span;
                //生成tab菜单，data-tab属性根据Schedules的最大长度生成，以保证唯一性。第几天则按照该日程在日程中实际的日期数显示，跟data-tab并不一致
                var menuHtml = '<div class="item-container"><a class="item" data-tab="单日行程/' + Schedules.length + '">第<span>' + (+Schedules[index].Day + 1) + '</span>天</a><div class="icon-container"><i class="plus icon"></i><i class="remove icon"></i></div></div>';
                //在对应菜单后面写入菜单的html
                $(menuContainers[index]).after(menuHtml);
                //修改该菜单后面的菜单日期跟tab对应
                $(menuContainers).each(function (i, e) {
                    if (i > index) {
                        $span = $(e).find('span');
                        $span.text(parseInt($span.text()) + 1);
                    }
                })
                //在当前tab的后面插入生成的tab html
                $('#' + tabId).after($('#tempHtml').html());
                //为插入的元素设置day
                $('#' + schedule.Id).find('input[name=Day]').val(schedule.Day)
                for (var i = index + 2; i <= Schedules.length - 1; i++) {
                    //为插入的元素后面的元素设置day
                    Schedules[i].Day++;
                    $('#' + Schedules[i].Id).find('input[name=Day]').val(Schedules[i].Day);
                }
                //UE.getEditor(schedule.descId);
                //UE.getEditor(schedule.introId);

                //重新绑定tab
                BindTabs();
                //Trip.SetSchedulesForPage();
            } else {
                alert('本地数据错误，请刷新页面重试');
            }
        })
        .delegate('.icon-container .remove.icon', 'click', function (e) {
            var tabPath = $(e.target).parent().siblings().data('tab');//获取当前的tabPath;
            var tabId = '';
            $('#scheduleContainer .day.tab').each(function (index, element) {
                if ($(element).data('tab') == tabPath) {
                    return tabId = $(element).attr('id');//获取被点击添加按钮的tabid
                }
            })
            var index = Schedules.findIndex(function (schedule) { return schedule.Id == tabId });//通过tabid获取对应日程
            if (index >= 0) {
                if (Schedules.length == 1) {
                    $.tip(".message-container", "这已经是最后一天啦！", "要是想清空信息，可以新添加一天再删除这一天", "positive", 4);
                } else {
                    Schedules.splice(index, 1);
                    //删除menu
                    $($('#scheduleList .menu .item-container')[index]).remove();
                    for (var i = index; i <= Schedules.length - 1; i++) {
                        //更新js数据
                        Schedules[i].Day -= 1;
                        //更新页面数据
                        $($('#scheduleList .menu .item-container')[i]).find('span').text(Schedules[i].Day);
                        $('#' + Schedules[i].Id).find('input[name=Day]').val(Schedules[i].Day);
                    }
                }
            } else {
                alert('本地数据错误，请刷新页面重试');
            }
        })
    }
    function BindEvents() {
        BindCommonInfo();
        BindSchedules();
        BindTripProperty();
        BindTripPlans();
        $('#regionSelection').dropdown({
            action: function (text, value, $choice) {
                if (text.indexOf('----') != -1) {
                    $(this).dropdown('set selected', value).dropdown('hide');
                    CommonInfo.RegionName = text.split('----')[1];
                } 
                
            }
        });
        $('.dropdown').not('.tripplan-dropdown').dropdown();
        $('#saveTab').on('click', function (e) {
            e.preventDefault();
            var tab = $(this).find('span').text().trim(), $tripContext = $('#tripContext');
            if (!$('#commonInfo').form('validate form')) {
                return $tripContext.tab("change tab", "通用信息");
            }
            if (!$('#productInfo').form('validate form')) {
                return $tripContext.tab("change tab", "产品概要");
            }
            if (tab.indexOf('单日行程') != -1)
                tab = '单日行程';
            Trip.SyncRemoteData(tab)
            
        })
        $('.button#saveAll').on('click', function (e) {
            var $target = $(e.target);
            if (!$target.hasClass('loading')) {
                $target.addClass('loading');
                $.post(api.toggleTrip, { tripId: Trip.Id,__RequestVerificationToken: token }).done(function (result) {
                    if (result.Success) {
                        $target.html('<i class="icon refresh"></i>' + result.Message);
                    }
                    $target.removeClass('loading');
                }).fail(function () {
                    $target.removeClass('loading');
                    $.tip(".message-container", "数据传送失败", "服务器超时，请稍后重试！", "negative", 4);
                })
            }
            
        });
       
        BindTabs();
    }
    function InitFileUpload() {
        $('#fileUpload').fileupload({
            url: api.uploadPhotoUrl,
            dataType: 'json',
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png|bmp)$/i,
            maxFileSize: 999000,
            disableImageResize: false,
            imageMaxWidth: 800,
            imageMaxHeight: 800,
            imageCrop: true,
            singleFileUploads: true,
            limitMultiFileUploads: 5,
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                var originFiles = data.originalFiles;
                var processingId = 0;
                if (originFiles.length > 1) {
                    var fileName = data.files[0].name;
                    $.each(originFiles, function (index, ele) {
                        if (ele.name == fileName)
                            processingId = ele.uniqueId;
                    })

                } else {
                    processingId = data.originalFiles[0].uniqueId;
                }
                $('#' + processingId + " .progress").progress({ percent: progress });
                if (progress == 100)
                    $('#' + processingId).addClass('upload-success');
            },
            progressall: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
            },

        }).bind('fileuploadadd', function (e, data) {
            var count = 0;
            $.each(data.files, function (index, file) {
                loadImage(file, function (img) {
                    if (img.type == 'error') {
                        alert(file.name + '不是可接受的文件类型');
                    } else {
                        var uniqueId = img.src.split('/')[3];
                        file.uniqueId = uniqueId;
                        data.uniqueId = uniqueId;
                        var photo = new PhotoInfo(uniqueId, file.name, '暂无描述', '');
                        CommonInfo.PendingPhotos.push(photo);
                        $('#pendingList').loadTemplate('#uploadTmpl', photo, tmplOpt)
                        $('#' + uniqueId).find('.image').append(img);
                    }

                }, {
                    maxWidth: 200
                });
            })
            uploadArr.push(data);
        }).bind('fileuploadsubmit', function (e, data) {
            data.formData = {
                tripId: Trip.Id, photoInfo: JSON.stringify(CommonInfo.PendingPhotos.find(function (element) {
                    return element.Id == data.uniqueId;
                })), __RequestVerificationToken: token
            };
        }).bind('fileuploaddone', function (e, data) {
            if (data.result.status == 200) {
                for (var i = 0; i <= CommonInfo.PendingPhotos.length - 1; i++) {
                    if (CommonInfo.PendingPhotos[i].Id == data.uniqueId) {
                        CommonInfo.PendingPhotos[i].FileLocation = data.result.data;
                        CommonInfo.Photos.push(CommonInfo.PendingPhotos[i]);
                        CommonInfo.PendingPhotos.splice(i, 1);
                        uploadArr.splice(i, 1);
                    }
                }
                 $('#' + data.uniqueId).remove();
                 CommonInfo.SetPhotosForPage();
            } else {
                $('#' + data.uniqueId).find('.save-single.button').removeClass('loading');
            }
        }).bind('fileuploadfail', function (e, data) {
            if (data.textStatus == "abort") {
                $.tip(".message-container", data.files[0].name + " 传送失败", "用户已取消！", "negative", 4);
            } else {
                $.tip(".message-container", "数据传送失败", "服务器超时，请稍后重试！", "negative", 4);
            }
            $('#' + data.uniqueId).find('.save-single.button').removeClass('loading');
        })
    }
    function InitFormValidators() {
        $('#commonInfo').form({
            on: 'blur',
            inline:true,
            fields: {
                Name: {
                    identifier: 'Name',
                    rules: [{ type: 'empty', prompt: '旅游名称不能为空' }]
                },
                ProviderName: {
                    identifier: 'ProviderName',
                    rules: [{ type: 'empty', prompt: '供应商名称不能为空' }]
                },
                RegionId: {
                    identifier: 'RegionId',
                    rules: [{ type: 'empty', prompt: '区域不能为空' }]
                },
                LowestPrice: {
                    identifier: 'LowestPrice',
                    rules: [{ type: 'regExp[^\\d+$]', prompt: '费用必须为非负整数' }]
                },
            },
            onSuccess: function (event, fields) {
                //return Trip.SyncRemoteData();
                //return SubmitForm($(this));
            },
            onFailure: function (formErrors, fields) {
                console.log(formErrors)
                return false;
            }
        })
        $('#productInfo').form({
            on: 'blur',
            inline: true,
            fields: {
                DepartingCity: {
                    identifier: 'DepartingCity',
                    rules: [{ type: 'empty', prompt: '出发城市不能为空' }]
                },
                ArrivingCity: {
                    identifier: 'ArrivingCity',
                    rules: [{ type: 'empty', prompt: '到达城市不能为空' }]
                },
                FinishingCity: {
                    identifier: 'FinishingCity',
                    rules: [{ type: 'empty', prompt: '终点城市不能为空' }]
                },
            },
            onSuccess: function (event, fields) {
                //return Trip.SyncRemoteData();
                //return SubmitForm($(this));
            },
            onFailure: function (formErrors, fields) {
                console.log(formErrors)
                return false;
            }
        })
    }
    function UpdateObj(target, source) {
        if (!(typeof source == 'undefined')) {
            for (var property in source) {
                if (typeof source[property] != 'function')
                    target[property] = source[property];
                    
            }
        }
    }
    function UpdateObjWithFunctions(target, source) {
        if (!(typeof source == 'undefined')) {
            for (var property in source) {
                target[property] = source[property];
            }
        }
        
    }
    function RemoveElementFromArray(array,element) {
        var index = array.indexOf(element);
        if (index > -1) {
            array.splice(index, 1);
        }
    }
    function Preload() {
        var dfd = $.Deferred();
        ProductInfo.LoadCityData(api.listCitiesUrl);
        ProductInfo.LoadSceneryData(api.listSceneriesUrl);
        CommonInfo.LoadRegionData(api.regionDataUrl);
        CommonInfo.LoadTypeData(api.listTypeUrl);
        TripProperty.LoadStartingPointsData(api.listStartPointsUrl);
        setTimeout(function () {
            dfd.resolve();
        }, 500);
        return dfd.promise();
    }
    function InitPage() {
        InitFileUpload();
        InitFormValidators();
        Preload().done(Trip.LoadServerData).done(BindEvents)
    }
    InitPage();
    
})