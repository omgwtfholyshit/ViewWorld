$(function () {
    var api = {
        regionDataUrl: '/Trip/ListRegionsAPI',
        tripDataUrl: '/Trip/GetTripArrangementById',
        partialDataUrl: '/Trip/UpdateTripPartial',
        addTripUrl: '/Trip/AddTripArrangement',
        uploadPhotoUrl: '/Trip/UploadTripArrangementPhoto',
        deletePhotoUrl: '/Trip/DeletePhotoById',
        listCitiesUrl: '/Trip/SearchCityByKeyword',
        listSceneriesUrl: '/Trip/ListSceneriesAPI',
    }, tmplOpt = { 'append': true }, token = $('input[name=__RequestVerificationToken]').val(), uploadArr = new Array(),
    introEditor = UE.getEditor('introduction'), includeEditor = UE.getEditor('include'), excludeEditor = UE.getEditor('exclude'),
    pIntroEditor = UE.getEditor('Intro'), pFeatureEditor = UE.getEditor('Feature');
    var CommonInfo = {
        Name:'',
        ProviderName: '',
        GroupId: '',//团号
        RegionId: '',
        RegionName: '',
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
            }).fail(function (xhr) { $.tip(".message-container", "删除图片失败", "服务器超时，请稍后重试！", "negative", 4); });
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
                    }
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
            var $input,_this=this;
            $inputs.each(function (index, input) {
                $input = $(input);
                // console.log($input.data('db-key') + " :" + $input.val().trim());
                _this[$input.data('db-key')] = $input.val().trim();
            });
            _this.Introduction =$.htmlEncode(introEditor.getContent());
            _this.Include = $.htmlEncode(includeEditor.getContent());
            _this.Exclude = $.htmlEncode(excludeEditor.getContent());
        },
        SetSelfPayTableForJs: function () {
            var $trs = $('#selfpayActivities tbody tr'), $tr, kvpair, name, price;
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
                }
            })
        },
        SetFormDataForPage: function () {
            var $inputs = $('#commonInfo').find('input').not('.search,#fileUpload,input[type=checkbox],#selfpayActivities input,input[name=__RequestVerificationToken],.saved-photo input');
            var $input, outcome, _this = this;
            $inputs.each(function (index, input) {
                $input = $(input);
                //console.log($input.data('db-key') + " :" + $input.val().trim());
                $input.val(_this[$input.data('db-key')]);
            });
            $('#regionSelection').dropdown("set selected", CommonInfo.RegionId);
            $('#currency').dropdown("set selected", CommonInfo.CurrencyType);
            introEditor.ready(function () { introEditor.setContent($.htmlDecode(_this.Introduction));})
            includeEditor.ready(function () { includeEditor.setContent($.htmlDecode(_this.Include)); })
            excludeEditor.ready(function () { excludeEditor.setContent($.htmlDecode(_this.Exclude)); })
        },
        SetSelfPayTableForPage: function () {
            var _this = this, $table = $('#selfpayActivities tbody'), html = '', elementArray;
            $.each(_this.SelfPayActivities, function (index, ele) {
                elementArray = ele.split('|');
                html += '<tr><td><input type="text" class="selfpay-name" value=' + elementArray[0] + ' /></td><td><input type="text" class="selfpay-price" value=' + elementArray[1] + ' /></td><td><button class="ui red icon button delete"><i class="icon delete"></i></button></td></tr>'
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
                $('#' + ele.Id).find('.image').append(img);
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
        Sceneries: '',
        TotalDays: 1,
        Feature: '',
        Intro: '',
        LoadCityData: function (url) {
            $.get(url).done(function (data) {
                var html = '';
                if (data.status==200) {
                    $.each(data.data, function (index, element) {
                        html += '<div class="item" data-value="' + element.Id + '" data-initial="' + element.Initial + '">' + element.Initial + "----" + element.Name + '</div>'
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
                }
                $('.scenery-selection .menu').html(html);
            }).fail(function (xhr) { $.tip(".message-container", "载入城市数据失败", "服务器超时，请稍后重试！", "negative", 4); })
        },
        SetFormDataForJs: function () {
            var $inputs = $('#productInfo').find('input').not('.search');
            var $input, _this = this;
            $inputs.each(function (index, input) {
                $input = $(input);
                _this[$input.data('db-key')] = $input.val().trim();
            });
            _this.Feature = $.htmlEncode(pFeatureEditor.getContent());
            _this.Intro = $.htmlEncode(pIntroEditor.getContent());
        },
        SetFormDataForPage: function () {
            var _this = this;
            $('#productInfo input[name=TotalDays]').val(ProductInfo.TotalDays);
            $('#departingCity').dropdown("set selected", ProductInfo.DepartingCity);
            $('#arrivingCity').dropdown("set selected", ProductInfo.ArrivingCity);
            if (ProductInfo.Sceneries != null) {
                $('#sceneries').dropdown("set selected", ProductInfo.Sceneries.split(','));
            }
            
            pIntroEditor.ready(function () { pIntroEditor.setContent($.htmlDecode(_this.Intro)); })
            pFeatureEditor.ready(function () { pFeatureEditor.setContent($.htmlDecode(_this.Feature)); })
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
        PickUpInfos: new Array(),
        DepartingLocation: '',
        SelectableRoutes: new Array(),

    },
    TripPlan = {
        Id: '',
        Type: '',
        OneDayOnly: false,
        CurrencyType: '美元',
        TripId: '',
        TripPrice: {},
        GetTripPrice: function () {

        }
    },
    Trip = {
        Id: '',
        Name: '',
        IsVisible: false,
        IsDeleted: false,
        ProductId: '',
        LoadServerData: function () {
            Trip.Id = $.getQueryStringByName('tripId');
            if (Trip.Id.length == 24) {
                $.get(api.tripDataUrl, { tripId: Trip.Id }).done(function (data) {
                    if (data.Success) {
                        Trip.SyncLocalData(data, true);
                    } else {
                        Trip.Id = '';
                    }
                }).fail(function (xhr) { $.tip(".message-container", "载入数据失败", "服务器超时，请稍后重试！", "negative", 4); });
            } else {
                if (Trip.Id != '')
                    $.tip(".message-container", "载入数据失败", "TripId参数有误", "negative", 4);
            }
        },
        SetSceneriesForJs: function (remoteData) {
            var _this = this;
            if (remoteData.length > 0) {
                $.each(remoteData, function (index, element) {
                    var schedule = new Schedule(element.Id, element.Day, element.Name, element.Description, element.Details, element.Meal, element.Accommodation, element.GroupPickUp, element.PickUp, element.Introduction);
                    Schedules.push(schedule);
                })
            }
        },
        SyncLocalData: function (data, writePage) {
            if (data.Success) {
                var _this = this;
                UpdateObjWithFunctions(CommonInfo, data.Entity.CommonInfo);
                UpdateObjWithFunctions(ProductInfo, data.Entity.ProductInfo);
                UpdateObjWithFunctions(TripProperty, data.Entity.TripProperty);
                UpdateObjWithFunctions(TripPlan, data.Entity.TripPlan);
                _this.SetSceneriesForJs(data.Entity.Schedules);
                _this.Id = data.Entity.Id;
                _this.Name = data.Entity.Name;
                _this.ProductId = data.Entity.ProductId;
                _this.IsDeleted = data.Entity.IsDeleted;
                _this.IsVisible = data.Entity.IsVisible;
            }
            if (writePage) {
                CommonInfo.SyncPage();
                ProductInfo.SyncPage();
            } else {
                console.log(_this);
            }
        },
        SyncRemoteData: function (type) {
            var _this = this;
            //ProductInfo.CollectFormData();
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
                        break;
                    case '发团属性':
                        break;
                    case '发团计划':
                        break;
                }
                for (var p in model) {
                    if (typeof model[p] == 'array') {
                        model[p] = JSON.stringify(model[p]);
                    }
                }
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
                            $.tip(".message-container", "保存失败", result.Message, "negative", 4);
                        }
                        $buttons.removeClass('loading');
                    },
                    error: function (data) { $.tip(".message-container", "数据传输失败", "服务器超时，请稍后重试！", "negative", 4); $buttons.removeClass('loading'); }
                })
            }
            
        }

    };
    var Schedule = function (ParentId,Day,Name,Description,Details,Meal,Accommodation,GroupPickUp,PickUp,Introduction) {
        if (this instanceof Schedule) {
            this.ParentId = ParentId;
            this.Day = Day;
            this.Name = Name;
            this.Description = Description;
            this.Details = Details;
            this.Meal = Meal;
            this.Accommodation = Accommodation;
            this.GroupPickUp = GroupPickUp;
            this.PickUp = PickUp;
            this.Introduction = Introduction;
        } else {
            return new Schedule(ParentId, Day, Name, Description, Details, Meal, Accommodation, GroupPickUp, PickUp, Introduction);
        }
    },
    ScheduleItem = function (Sceneries, ActivityTime, Arrangement, Memo) {
        if (this instanceof ScheduleItem) {
            this.Sceneries = Sceneries;
            this.ActivityTime = ActivityTime;
            this.Arrangement = Arrangement;
            this.Memo = Memo;
        } else {
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
    HotelPrice = function (Name, SinglePrice, DoublePrice, TriplePrice, QuadplexPrice, ChildPrice, RoomDifference) {
        if (this instanceof HotelPrice) {
            this.Name = Name;
            this.SinglePrice = SinglePrice;
            this.DoublePrice = DoublePrice;
            this.TriplePrice = TriplePrice;
            this.QuadplexPrice = QuadplexPrice;
            this.ChildPrice = ChildPrice;
            this.RoomDifference = RoomDifference;
        } else {
            return new HotelPrice(Name, SinglePrice, DoublePrice, TriplePrice, QuadplexPrice, ChildPrice, RoomDifference);
        }
    },
    TripPriceForSpecificDate = function (TripDate, BasePrice, RaisePriceByPercentage, AdditionalPrice) {
        if (this instanceof TripPriceForSpecificDate) {
            this.TripDate = TripDate;
            this.BasePrice = BasePrice;
            this.RaisePriceByPercentage = RaisePriceByPercentage;
            this.AdditionalPrice = AdditionalPrice;
        } else {
            return new TripPriceForSpecificDate(TripDate, BasePrice, RaisePriceByPercentage, AdditionalPrice);
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
    };
    function BindEvents() {
        $('#tripContext .menu .item').tab({
            context: $('#tripContext'),
            onLoad: function (tabPath) {
                if (tabPath.indexOf('单日行程') != -1)
                    tabPath = '单日行程';
                $('#saveTab span').text(tabPath);
            }
        });
       // $('#photoListContainer .saved-photo li').popup({ on: 'click', inline: true });
        $('#photoListContainer').delegate('.content-editable.photo-desc', 'click', function (e) {
            $(e.target).attr('contenteditable', true);
        }).delegate('#pendingList .content-editable.photo-desc', 'blur', function (e) {
            var id = $(e.target).parents('.photo-item').attr('id');
            var photo = CommonInfo.PendingPhotos.find(function (element) {
                return element.Id == id;
            });
            photo.Description = $(e.target).text();
        }).delegate('.saved-photo .content-editable.photo-desc', 'blur', function (e) {
            var id = $(e.target).parents('.photo-item').attr('id');
            var photo = CommonInfo.Photos.find(function (element) {
                return element.Id == id;
            });
            photo.Description = $(e.target).text();
        }).delegate('.saved-photo .button.delete', 'click', function (e) {
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
                    photoId:id,
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
                    }
                    $.tip(".message-container", "删除成功", "图片已删除", "positive", 4);
                },
                error: function (data) { $.tip(".message-container", "图片删除失败", "服务器超时，请稍后重试！", "negative", 4); $(e.target).removeClass('loading'); }
            })
            return false;
        }).delegate('.save-single.button', 'click', function (e) {
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
                console.log(CommonInfo.PendingPhotos);
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
        $('#regionSelection').dropdown({
            action: function (text, value, $choice) {
                if (text.indexOf('----') != -1) {
                    $(this).dropdown('set selected', value).dropdown('hide');
                    //Update RegionName Here. 
                    CommonInfo.RegionName = text.split('----')[1];
                } 
                
            }
        });
        $('.city-selection,.scenery-selection').dropdown();
        $('#saveTab').on('click', function (e) {
            var tab = $(this).find('span').text().trim();
            switch (tab) {
                case '通用信息':
                    if ($('#commonInfo').form('validate form')) {
                        Trip.SyncRemoteData('通用信息');
                    }
                    break;
                default:
                    if (tab.indexOf('单日行程') != -1)
                        tab = '单日行程';
                    Trip.SyncRemoteData(tab)
                    break;
            }
        })
        $('#saveAll').on('click', function () { $('#commonInfo').form('validate form')});
        $('#currency').dropdown('set selected', TripPlan.CurrencyType);
        $('#selfpayActivities').delegate('.button.delete', 'click', function (e) {
            $(e.target).parents('tr').remove();
            return false;
        }).delegate('.button.add', 'click', function (e) {
            var tmpl = '<tr><td><input type="text" class="selfpay-name" /></td><td><input type="text" class="selfpay-price" /></td><td><button class="ui red icon button delete"><i class="icon delete"></i></button></td></tr>';
            $('#selfpayActivities tbody').append(tmpl);
            
            return false;
        })
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
    }
    function UpdateObj(target, source) {
        if (!(typeof source == 'undefined')) {
            for (var property in source) {
                if(typeof source[property]!='function')
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
    function PostPartialData(data,token) {
        if (Trip.Id == '') {
            Trip.SyncRemoteData()
        } else {
            $.post(api.partialDataUrl, { tripId: Trip.Id, data: data, __RequestVerificationToken: token }).done(function (result) {
                console.log(result);
            }).fail(function (xhr) { $.tip(".message-container", "数据传送失败", "服务器超时，请稍后重试！", "negative", 4); });
        }
    }
    function Preload() {
        var dfd = $.Deferred();
        ProductInfo.LoadCityData(api.listCitiesUrl);
        ProductInfo.LoadSceneryData(api.listSceneriesUrl);
        CommonInfo.LoadRegionData(api.regionDataUrl);
        setTimeout(function () {
            dfd.resolve();
        }, 500);
        return dfd.promise();
    }
    function InitPage() {
        BindEvents()
        InitFileUpload();
        InitFormValidators();
        Preload().done(Trip.LoadServerData)
        //ProductInfo.CollectFormData();
    }
    InitPage();
    
})