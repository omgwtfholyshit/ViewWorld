$(function () {
    var homepage = {
        mainSwiper : {
            context: $('.homepage'),
            obj:Object,
        },
        s2Swiper:{
            context: $('.slideTwo .swiper-wrapper'),
            obj: Object,
        },
        sliderOneContent: $('.homepage .content-container'),
        init: function () {
            this.s2LeftArrow = $('.leftwards.arrow-container');
            this.s2RightArrow = $('.rightwards.arrow-container');
            this.upArrow = $('.upwards.arrow-container');
            this.initSwiper();
            this.arrowEvents();
            this.bindEvents();
            this.initWorldMap();
        },
        initSwiper: function () {
            var _this = this, $nav = $('.nav'), tempArr = [], transform = '', s2SlideCount = _this.s2Swiper.context.find('.swiper-slide').length;
            _this.mainSwiper.obj = new Swiper('.main.swiper-container', {
                pagination: '.swiper-pagination',
                speed:1000,
                paginationClickable: true,
                threshold: 100,
                freeMode: false,
                direction: 'vertical',
                mousewheelControl: true,
                parallax: true,
                spaceBetween: 70,
                keyboardControl: true,
                onSlideChangeStart: function (swiper) {
                    swiper.activeIndex > 0 ? $nav.css('background-color', 'white') : $nav.css('background-color', 'transparent');
                },
                onAfterResize: function (swiper) {
                    if (!browser.versions().smallDevice) {
                        _this.s2LeftArrow.hide();
                        _this.s2RightArrow.hide();
                    }
                }
            });
            var slideNum = !browser.versions().smallDevice ? slideNum = 3 : slideNum = 1;
            _this.s2Swiper.obj = new Swiper('.slideTwo .swiper-container-h', {
                effect: 'coverflow',
                slidesPerView: slideNum,
                breakpoints: {
                    767: {
                       slidesPerView: 1
                    },
                    1000: {
                       slidesPerView: 2
                    }
                },
                centeredSlides: true,
                coverflow: {
                    rotate: 30,
                    stretch: 10,
                    depth: 50,
                    modifier: 2,
                    slideShadows: false
                },
                keyboardControl: true,
                slideToClickedSlide: true,
                onSlideChangeStart: function (swiper) {
                    swiper.realIndex == 0 || !browser.versions().smallDevice? _this.s2LeftArrow.hide() : _this.s2LeftArrow.show();
                    swiper.realIndex == s2SlideCount - 1 || !browser.versions().smallDevice ? _this.s2RightArrow.hide() : _this.s2RightArrow.show();
                },
            })
        },
        arrowEvents: function () {
            var _this = this;
            _this.upArrow.click(function () {
                _this.mainSwiper.obj.slideNext();
            })
            _this.s2LeftArrow.click(function () {
                _this.s2Swiper.obj.slidePrev();
            })
            _this.s2RightArrow.click(function () {
                _this.s2Swiper.obj.slideNext();
            })
            
        },
        bindEvents: function () {
            var _this = this;
            _this.s2Swiper.context
            .delegate('span.floated', 'click', function (e) {
                var $target = $(e.currentTarget), redirectUrl = '/Finder';
                $target.hasClass('favorite') ? redirectUrl += '/TripDetail/productid=' + $target.data('productid') : redirectUrl += '/FindTrips/region=' + $target.text().trim();
                window.open(redirectUrl);
            })
        },
        initWorldMap: function () {
            var regions = {
                "US": 1, "CA": 1,//北美
                "GB": 1, "IE": 1, "FR": 1, "NL": 1, "BE": 1, "LU": 1, //西欧
                "NO": 1, "SE": 1, "FI": 1, "DK": 1, "IS": 1, "FO": 1,//北欧
                "DE": 1, "PL": 1, "CZ": 1, "SK": 1, "HU": 1, "AT": 1, "LI": 1, "CH": 1,//中欧
                "RU": 1, "EE": 1, "LV": 1, "LT": 1, "BY": 1, "UA": 1, "MD": 1,//东欧
                "BG": 1, "HR": 1, "BA": 1, "JE": 1, "GR": 1, "PT": 1, "XK": 1, "AD": 1, "AL": 1, "IT": 1, "GG": 1, "IM": 1, "AX": 1, "ES": 1, "ME": 1, "RO": 1, "RS": 1, "MK": 1, "MT": 1, "SI": 1, "SM": 1,   //南欧
                "AU": 1, "NZ": 1, //澳新
            },url='';
            $('#worldMap').vectorMap({
                map: 'world_mill',
                zoomOnScroll: false,
                backgroundColor:'transparent',
                regionStyle: {
                    initial: {
                        fill: '#2595cf',
                        "fill-opacity": 1,
                        stroke: 'none',
                        "stroke-width": 0,
                        "stroke-opacity": 1,
                        "cursor":"default",
                    },
                    hover: {
                        "fill-opacity": 0.8,
                        cursor: 'pointer'
                    },
                    selected: {
                        fill: 'yellow'
                    },
                    selectedHover: {
                    }
                },
                series: {
                    regions: [{
                        scale: {
                            '1': '#00B5AD',
                            '2': '#FE9A76',
                            '3': '#FFD700',
                            '4': '#32CD32',
                            '5': '#B413EC',
                            '6': '#FF1493',
                        },
                        attribute: 'fill',
                        values: regions
                    }]
                },
                markers: [{ latLng: [49.04, -108.99], name: "北美旅行" },
                    { latLng: [52.21, 31.60], name: '东欧旅行' }, { latLng: [40.85, 20.99], name: '南欧旅行' },
                { latLng: [50.90, 2.05], name: '西欧旅行' }, { latLng: [68.50, 22.09], name: '北欧旅行' }, { latLng: [51.05, 14.35], name: '中欧旅行' },
                { latLng: [-32.37, 151.30], name: '澳新旅行' },
                ],
                markerStyle: {
                    initial: {
                        fill: '#F8E23B',
                        stroke: '#383f47',
                    }
                },
                onMarkerClick: function (e, code) {
                    //console.log($('.jvectormap-tip').text().trim());
                    //console.log(code);
                    var region = $('.jvectormap-tip').text().trim();
                    window.open("/Finder/FindTrips?region=" + region)
                }
            });
        }
    }
    homepage.init();
})