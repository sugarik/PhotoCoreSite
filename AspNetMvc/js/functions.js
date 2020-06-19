$(document).ready(function () {
    // $('#imageUpload').change(function() {
    //     let file = this.files[0];
    //     read(file, (result) => {
    //         console.log(result);
    //     });
    // });

    // function read(file, callback) {
    //     let reader = new FileReader();
        
    //     reader.readAsDataURL(file);
    //     reader.onload = () => {
    //         callback(reader.result)
    //     };
    // }

    let requestId = localStorage.getItem('requestId');
    let arrayFaces = [];

    function stopStreamedVideo(videoElem) {
        let stream = videoElem[0].srcObject;
        let tracks = stream.getTracks();
        
        tracks.forEach(function(track) {
            track.stop();
        });
        
        videoElem.srcObject = null;
    }

    canvas = window.canvas = document.createElement('canvas');
    const video = document.querySelector('video');
    canvas.width = '100%';
    canvas.height = 'auto';

    setTimeout(function () {
        $('#preloader').css('display', 'none');
    }, 2000);

    const button = document.getElementById('take_snapshot');
    //button.onclick = function () {

    //    $('#preloader').css('display', 'block');

    //    let countPhotos = $(".step1 .selfie-container").length;
    //    const photoLimit = 5;
    //    if (countPhotos >= photoLimit) {
    //        $('#preloader').css('display', 'none');
    //        $('#ex6 span').html('Можно сделать максимум ' + photoLimit + ' фото');
    //        $('#ex6').modal('show');
    //        return false;
    //    }

    //    canvas.width = video.videoWidth;
    //    canvas.height = video.videoHeight;
    //    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);

    //    let data = canvas.toDataURL('image/jpeg', 0.95);

    //    let requestId = localStorage.getItem('requestId');

    //    let image = data;

    //    if (image === undefined || requestId === undefined) {
    //        alert('Произошла ошибка');
    //        return false;
    //    }

    //    let request = {
    //        requestId: requestId,
    //        selfieImage: image
    //    };

    //    $.ajax({
    //        type: "POST",
    //        cache: false,
    //        url: "http://localhost:50018/Service/SearchFaces",
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        data: JSON.stringify(request),
    //        success: function (msg) {
    //            let arr = [];
    //            let faces = msg.faces;

    //            if (faces === null) {
    //                $('#preloader').css('display', 'none');
    //                $('#ex6 span').html('Не удалось определить лицо. Попробуйте сделать фото в другом ракурсе');
    //                $('#ex6').modal('show');
    //                return false;
    //            }

    //            for (let i = 0; i < faces.length; i++) {
    //                let content = "<div class=\"selfie-container\" id=\"" + faces[i].faceId + "\">\
    //                                  <div class=\"checkbox\">\
    //                                     <div class=\"check\"></div>\
    //                                  </div >\
    //                                  <img src=\"" + faces[i].faceImage + "\" alt=\"selfie\">\
    //                               </div>";
    //                arr.push(content);
    //            }
    //            $('#selfie-container').append(arr);
    //            selectSelfie();
    //            $('body,html').animate({ scrollTop: document.body.offsetHeight - 100 }, $(".step1").height() + 190);
    //        }
    //    });

    //    setTimeout(function () {
    //        $('#preloader').css('display', 'none');
    //    }, 1000);
    //};

    button.onclick = function ()
    {
        $('#preloader').css('display', 'block');

        let countPhotos = $(".step1 .selfie-container").length;

        const photoLimit = 5;
        if (countPhotos >= photoLimit) {
            $('#preloader').css('display', 'none');
            $('#ex6 span').html('Можно сделать максимум ' + photoLimit + ' фото');
            $('#ex6').modal('show');
            return false;
        }

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);

        let image = canvas.toDataURL('image/jpeg', 0.95);
        let requestId = localStorage.getItem('requestId');

        if (image === undefined || requestId === undefined) {
            alert('Произошла ошибка');
            return false;
        }

        let request = {
            requestId: requestId,
            selfieImage: image
        };

        $.ajax({
            type: "POST",
            cache: false,
            url: "http://localhost:50018/Service/SearchFaces",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (msg)
            {
                let arr = [];
                let faces = msg.faces;

                if (faces === null)
                {
                    $('#preloader').css('display', 'none');
                    $('#ex6 span').html('Не удалось определить лицо. Попробуйте сделать фото в другом ракурсе');
                    $('#ex6').modal('show');
                    return false;
                }

                for (let i = 0; i < faces.length; i++)
                {
                    let content = "<div class=\"selfie-container\" id=\"" + faces[i].faceId + "\">\
                                    <div class=\"checkbox\">\
                                        <div class=\"check checked\"></div>\
                                    </div >\
                                    <img src=\"" + faces[i].faceImage + "\" alt=\"selfie\">\
                                </div>";
                    arr.push(content);
                    arrayFaces.push(faces[i].faceId);
                }
                $('#selfie-container').append(arr);

                document.getElementById("approve_selfie").style.display = "block";
                $('#approve_selfie').prop('disabled', false);
                $('#approve_selfie').addClass('active');

                $('.selfie-container').off('click');
                $('.selfie-container').on('click', function () {
                    //let currentClass = $(this).children('.checkbox').children();
                    //let list = $(currentClass).attr('class').split(/\s+/);
                    //if (list[1] === 'checked')
                    //{
                        //$(currentClass).removeClass('checked');

                    if (confirm("Удалить селфи из списка поиска?"))
                    {

                        let removedId = $(this).attr("id");
                        for (let i = 0; i < arrayFaces.length; i++) {
                            if (removedId == arrayFaces[i]) {
                                for (let j = $('.selfie-container').length - 1; j > -1; j--) {
                                    if (removedId == $('.selfie-container')[j].id) {
                                        $('.selfie-container')[j].remove();
                                        break;
                                    }
                                }
                                arrayFaces.splice(i, 1);
                                break;
                            }
                        }
                        //}

                        if (arrayFaces.length > 0) {
                            document.getElementById("approve_selfie").style.display = "block";
                            $('#approve_selfie').prop('disabled', false);
                            $('#approve_selfie').addClass('active')
                        }
                        else {
                            document.getElementById("approve_selfie").style.display = "none";
                            $('#approve_selfie').prop('disabled', true);
                            $('#approve_selfie').removeClass('active');
                        }
                    }
                    $('body,html').animate({ scrollTop: document.body.offsetHeight - 100 }, $(".step1").height() + 190);
                })
            }
        });

        setTimeout(function () {
            $('#preloader').css('display', 'none');
        }, 1000);
    };



    //function selectSelfie() {
    //    $('.selfie-container').off('click');
    //    $('.selfie-container').on('click', function () {
    //        let currentClass = $(this).children('.checkbox').children();
    //        let list = $(currentClass).attr('class').split(/\s+/);
    //        if (list[1] === 'checked') {
    //            $(currentClass).removeClass('checked')
    //        } else {
    //            $(currentClass).addClass('checked');
    //        }

    //        for (let i = 0; i < arrayFaces.length; i++) {
    //            if ($(this).attr("id") == arrayFaces[i]) {
    //                arrayFaces.splice(i, 1);
    //                if (arrayFaces.length > 0) {
    //                    document.getElementById("approve_selfie").style.display = "block";
    //                    $('#approve_selfie').prop('disabled', false);
    //                    $('#approve_selfie').addClass('active')
    //                } else {
    //                    document.getElementById("approve_selfie").style.display = "none";
    //                    $('#approve_selfie').prop('disabled', true);
    //                    $('#approve_selfie').removeClass('active');
    //                }

    //                return false;
    //            }
    //        }

    //        arrayFaces.push($(this).attr("id"));
    //        if (arrayFaces.length > 0) {
    //            document.getElementById("approve_selfie").style.display = "block";
    //            $('#approve_selfie').prop('disabled', false);
    //            $('#approve_selfie').addClass('active')
    //        } else {
    //            document.getElementById("approve_selfie").style.display = "none";
    //            $('#approve_selfie').prop('disabled', true);
    //            $('#approve_selfie').removeClass('active');
    //        }
    //    });
    //}

    //function selectSelfie() {
    //    $('.selfie-container').off('click');
    //    $('.selfie-container').on('click', function () {
    //        let currentClass = $(this).children('.checkbox').children();
    //        let list = $(currentClass).attr('class').split(/\s+/);
    //        if (list[1] === 'checked') {
    //            $(currentClass).removeClass('checked')
    //        } else {
    //            $(currentClass).addClass('checked');
    //        }

    //        for (let i = 0; i < arrayFaces.length; i++) {
    //            if ($(this).attr("id") == arrayFaces[i]) {
    //                arrayFaces.splice(i, 1);
    //                if (arrayFaces.length > 0) {
    //                    document.getElementById("approve_selfie").style.display = "block";
    //                    $('#approve_selfie').prop('disabled', false);
    //                    $('#approve_selfie').addClass('active')
    //                } else {
    //                    document.getElementById("approve_selfie").style.display = "none";
    //                    $('#approve_selfie').prop('disabled', true);
    //                    $('#approve_selfie').removeClass('active');
    //                }

    //                return false;
    //            }
    //        }

    //        arrayFaces.push($(this).attr("id"));
    //        if (arrayFaces.length > 0) {
    //            document.getElementById("approve_selfie").style.display = "block";
    //            $('#approve_selfie').prop('disabled', false);
    //            $('#approve_selfie').addClass('active')
    //        } else {
    //            document.getElementById("approve_selfie").style.display = "none";
    //            $('#approve_selfie').prop('disabled', true);
    //            $('#approve_selfie').removeClass('active');
    //        }
    //    });
    //}

    let parkId;
    $('.park-container').on('click', function () {
        $('.check').removeClass('checked');
        let currentClass = $(this).children('.checkbox').children();
        let list = $(currentClass).attr('class').split(/\s+/);
        if (list[1] === 'checked') {
            parkId = 0;
            console.log(parkId);
            $(currentClass).removeClass('checked');
            $('.park-btn').prop('disabled', true);
            $('.park-btn').removeClass('active');
        } else {
            parkId = $(this).attr("id");
            console.log(parkId);
            $(currentClass).addClass('checked');
            $('.park-btn').prop('disabled', false);
            $('.park-btn').addClass('active');
        }
    });

    const constraints = {
        audio: false,
        video: { optional: [{ minWidth: 320 }, { minWidth: 640 }, { minWidth: 800 }, { minWidth: 1024 }, { minWidth: 1280 }, { minWidth: 1920 }, { minWidth: 2560 }] }
    };

    function handleSuccess(stream) {
        window.stream = stream;
        video.srcObject = stream;
    }

    function handleError(error) {
        $('#ex4').modal('show');
        console.log('navigator.MediaDevices.getUserMedia error: ', error.message, error.name);
        let isOpera = (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0,
            isFirefox = typeof InstallTrigger !== 'undefined',
            isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification)),
            isIE = /*@cc_on!@*/false || !!document.documentMode,
            isEdge = !isIE && !!window.StyleMedia,
            isChrome = !!window.chrome && (!!window.chrome.webstore || !!window.chrome.runtime),
            isEdgeChromium = isChrome && (navigator.userAgent.indexOf("Edg") != -1);

        if (isOpera) {
            document.getElementById('browser-name').innerHTML = '<b>Opera</b>?';
            document.getElementById('resolve-link').innerHTML = '<a href="#">возможные проблемы в браузере Opera</a>.';
            return false;
        }

        if (isFirefox) {
            document.getElementById('browser-name').innerHTML = '<b>Firefox</b>?';
            document.getElementById('resolve-link').innerHTML = '<a href="#">возможные проблемы в браузере Firefox</a>.';
            return false;
        }

        if (isSafari) {
            document.getElementById('browser-name').innerHTML = '<b>Safari</b>?';
            document.getElementById('resolve-link').innerHTML = '<a href="#">возможные проблемы в браузере Safari</a>.';
            return false;
        }

        if (isIE) {
            document.getElementById('browser-name').innerHTML = '<b>Internet Explorer</b>?';
            document.getElementById('resolve-link').innerHTML = '<a href="#">возможные проблемы в браузере Internet Explorer</a>.';
            return false;
        }

        if (isEdge) {
            document.getElementById('browser-name').innerHTML = '<b>Microsoft Edge</b>?';
            document.getElementById('resolve-link').innerHTML = '<a href="#">возможные проблемы в браузере Microsoft Edge</a>.';
            return false;
        }

        if (isChrome) {
            document.getElementById('browser-name').innerHTML = '<b>Google Chrome</b>?';
            document.getElementById('resolve-link').innerHTML = '<a href="#">возможные проблемы в браузере Google Chrome</a>.';
            return false;
        }

        if (isEdgeChromium) {
            document.getElementById('browser-name').innerHTML = '<b>Chromium</b>?';
            document.getElementById('resolve-link').innerHTML = '<a href="#">возможные проблемы в браузере Chromium</a>.';
            return false;
        }

        document.getElementById('camera-error-desc').innerHTML = 'Ваш браузер не определился. Ссылка на решение проблемы - <a href="#">перечень возможных проблем с камерой</a>.';
    }

    navigator.mediaDevices.getUserMedia(constraints).then(handleSuccess).catch(handleError);

    $('#approve_selfie').on('click', function () {
        let videoElem = $('video');
        stopStreamedVideo(videoElem);
        $('.step1').css('display', 'none');
        $('.step2').css('display', 'block');
    });

    // calendar
    const max = 4; // Maximum number of days to select
    arrayDates = [],
        daysCount = 30, // Change this number for change available search count days
        myDataSave = new Date(),
        data = {
            startDate: new Date(),
            startDateSave: new Date(myDataSave.setDate(myDataSave.getDate() - daysCount)),
            currentDate: {}
        }
    $('#days-count').html(daysCount);
    $('.step-description strong').html(max);

    function selectDate(arrayDates) {
        $('.vanilla-calendar-date').on("click", function () {
            $('#ex2 strong').html(max);
            if (arrayDates.length == max && arrayDates.indexOf($(this).attr("id")) == -1) {
                $('#ex2').modal('show');
                return false;
            }

            if (this.classList[3] === 'bg-color' || this.classList[4] === 'bg-color') {
                $(this).removeClass('bg-color')
            } else {
                $(this).addClass('bg-color');
            }

            for (let i = 0; i < arrayDates.length; i++) {
                if ($(this).attr("id") == arrayDates[i]) {
                    arrayDates.splice(i, 1);
                    if (arrayDates.length > 0) {
                        $('#search').prop('disabled', false);
                        $('#search').addClass('active')
                    } else {
                        $('#search').prop('disabled', true);
                        $('#search').removeClass('active');
                    }
                    return false;
                }
            }

            arrayDates.push($(this).attr("id"));
            if (arrayDates.length > 0) {
                $('#search').prop('disabled', false);
                $('#search').addClass('active')
            } else {
                $('#search').prop('disabled', true);
                $('#search').removeClass('active');
            }
        });
    }

    function setIndent(date, className) {
        switch (date) {
            case "ВТ":
                $(className).addClass('tuesday');
                break;

            case "СР":
                $(className).addClass('wednesday');
                break;

            case "ЧТ":
                $(className).addClass('thursday');
                break;

            case "ПТ":
                $(className).addClass('friday');
                break;

            case "СБ":
                $(className).addClass('saturday');
                break;

            case "ВС":
                $(className).addClass('sunday');
                break;
        }
    }

    function getWeekDay(date) {
        let days = ['ВС', 'ПН', 'ВТ', 'СР', 'ЧТ', 'ПТ', 'СБ'];

        return days[date.getDay()];
    }

    function changeMonth(date, start, end) {
        let current = new Date();

        if (data.startDate.getMonth() === current.getMonth()) {
            $('.vanilla-calendar-btn-right').css('display', 'none');
            $('.vanilla-calendar-btn-left').css('display', 'block');
        } else if (data.startDate.getMonth() != data.startDateSave.getMonth()) {
            $('.vanilla-calendar-btn-right').css('display', 'block');
            $('.vanilla-calendar-btn-left').css('display', 'block');
        } else {
            $('.vanilla-calendar-btn-right').css('display', 'block');
            $('.vanilla-calendar-btn-left').css('display', 'none');
        }

        if (date === undefined) {
            date = new Date(current.getFullYear(), current.getMonth(), 1);
        }

        if (start === undefined && end === undefined) {
            let firstDay = new Date(current.getFullYear(), current.getMonth(), 1);
            end = data.startDate.getDate();
            start = firstDay.getDate();
        }

        let totalDate = moment(new Date(date));

        $('.month')
            .html(
                "<div class=\"vanilla-calendar-header__label vanilla-calendar-header__label\" data-calendar-label=\"month\">" + date.toLocaleString('ru', { month: 'long' }) + "</div>"
            );

        let arr = [];
        for (let i = start; i <= end; i++) {
            let a = "<div class=\"vanilla-calendar-date vanilla-calendar-date--active\" id=" + totalDate.format('YYYY.MM.DD').replace(/[^\d]/g, '') + "><span>" + i + "</span></div>";
            totalDate.add(1, 'days');
            arr.push(a);
        }
        $('.vanilla-calendar-body').html(arr);

        let currentDay = getWeekDay(date);
        setIndent(currentDay, '.vanilla-calendar-date');
        selectDate(arrayDates);

        if (arrayDates) {
            for (let i = 0; i < arrayDates.length; i++) {
                $('#' + arrayDates[i]).addClass('bg-color');
            }
        }
    }
    changeMonth();
    selectDate();

    $('.vanilla-calendar-btn-right').on('click', function () {
        let current = new Date();
        data.startDate.setMonth(data.startDate.getMonth() + 1);
        let date = new Date(data.startDate.setDate(1));
        data.currentDate = date;
        endDate = 32 - new Date(data.startDateSave.getFullYear(), data.startDateSave.getMonth(), 32).getDate();
        startDate = new Date(current.getFullYear(), current.getMonth(), 1);

        if (data.startDate.getMonth() === current.getMonth()) {
            endDate = current.getDate();
            let getFirstDay = new Date(current.getFullYear(), current.getMonth(), 1);
            let totalDateCurrent = moment(new Date(getFirstDay));

            changeMonth(getFirstDay, totalDateCurrent.get('date'), endDate);

            return false;
        }

        changeMonth(data.currentDate, startDate.getDate(), endDate);
    });

    $('.vanilla-calendar-btn-left').on('click', function () {
        let current = new Date();
        data.startDate.setMonth(data.startDate.getMonth() - 1);
        let date = new Date(data.startDate.setDate(1));
        data.currentDate = date;
        endDate = 32 - new Date(data.startDateSave.getFullYear(), data.startDateSave.getMonth(), 32).getDate();
        startDate = new Date(current.getFullYear(), current.getMonth(), 1);

        if (data.currentDate.getMonth() === data.startDateSave.getMonth()) {
            endDate = 32 - new Date(data.startDateSave.getFullYear(), data.startDateSave.getMonth(), 32).getDate();
            startDate = data.startDateSave.getDate();

            changeMonth(data.startDateSave, startDate, endDate);

            return false;
        }

        changeMonth(data.currentDate, startDate.getDate(), endDate);
    });

    // end calendar

    $('#search').on('click', function() {
        $('#preloader').css('display', 'block');
        let request = {
            faces: arrayFaces,
            requestId: requestId,
            searchDates: arrayDates,
            locationId: parkId
        };

        $.ajax({
            type: "POST",
            cache: false,
            url: "http://localhost:50018/Service/SearchPhotos",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (msg) {
                let arr = [];
                let photos = msg.photos;
                $('.count').html(photos.length);
                for (let i = 0; i < photos.length; i++) {
                    let content = "<div class=\"image-container\" id=\"" + photos[i].photoProcessId + "\">\
                                      <div class=\"checkbox\">\
                                        <div class=\"check\"></div>\
                                      </div>\
                                      <img src =\"" + photos[i].thumbnailLink + "\" alt=\"photo\" >\
                                      <p>2020-06-18</p>\
                                   </div>";
                    arr.push(content);
                }
                $('#result-container').html(arr);
                selectPhoto();
            }
        });

        $('.search-main').css('display', 'none');
        $('.search-result').css('display', 'block');
        $('.order-block').css('display', 'block');
        setTimeout(function () {
            $('#preloader').css('display', 'none');
        }, 1000);
    });

    let arrayPhotos = [];
    function selectPhoto() {
        $('.image-container').on('click', function() {
            let currentClass = $(this).children('.checkbox').children();
            let list = $(currentClass).attr('class').split(/\s+/);
            if (list[1] === 'checked') {
                $(currentClass).removeClass('checked')
                $('#choose-count').html(arrayPhotos.length - 1);
            } else {
                $('#choose-count').html(arrayPhotos.length + 1);
                $(currentClass).addClass('checked');
            }
    
            for (let i = 0; i < arrayPhotos.length; i++) {
                if ($(this).attr("id") == arrayPhotos[i]) {
                    arrayPhotos.splice(i, 1);
                    $('#sum').html(total(arrayPhotos) + " BYN");
                    if (arrayPhotos.length > 0) {
                        $('.order-btn').prop('disabled', false);
                        $('.order-btn').addClass('active')
                    } else {
                        $('.order-btn').prop('disabled', true);
                        $('.order-btn').removeClass('active');
                    }
    
                    return false;
                }
            }
    
            arrayPhotos.push($(this).attr("id"));
            $('#sum').html(total(arrayPhotos) + " BYN");
            if (arrayPhotos.length > 0) {
                $('.order-btn').prop('disabled', false);
                $('.order-btn').addClass('active')
            } else {
                $('.order-btn').prop('disabled', true);
                $('.order-btn').removeClass('active');
            }
        });
    }

    function total(arrayTotal) {
        switch (arrayTotal.length) {
            case 0: 
                return 0;

            case 1: 
                return 6;
                
            case 2:
                return 12;

            case 3:
                return 18;

            default:
                return 20;
        }
    }

    $('#order').on('click', function() {
        let request = {
            requestId: requestId,
            photoProcessId: arrayPhotos,
            sessionId: 'iuh'
        };

        $.ajax({
            type: "POST",
            cache: false,
            url: "http://localhost:50018/Service/OrderReservation",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (msg) {
                $('#order-id').html(msg.orderId);
                $('#order-amount').html(msg.amount);
                $('#order-description').html(msg.description);
                $('#ex5').modal('show');
            }
        });
    });

    $('#order-email').on('input', function() {
        let email = $('#order-email').val();
        if (email !== '') {
            $('#pay-button').prop('disabled', false);
            $('#pay-button').addClass('active')
        } else {
            $('#pay-button').prop('disabled', true);
            $('#pay-button').removeClass('active');
        }
    });

    $('#pay-button').on('click', function() {
        $('#preloader').css('display', 'block');
        let email = $('#order-email').val();
        let emailRegExp = /[a-z0-9][a-z0-9\._-]*[a-z0-9]*@([a-z0-9]+([a-z0-9-]*[a-z0-9]+)*\.)+[a-z]+/i;
        let result = emailRegExp.test(email);
        let orderId = $('#order-id').text();

        if (email === '' || !result) {
            $('.email-error').css('display', 'block');
            return false;
        } else {
            $('.email-error').css('display', 'none');
        }

        let request = {
            requestId: requestId,
            email: email,
            orderId: orderId
        };

        $.ajax({
            type: "POST",
            cache: false,
            url: "http://localhost:50018/Service/OrderPayment",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(request),
            success: function (msg) {
                $(location).attr('href', msg.formUrl);
            }
        });
    });

    $('#park').on('click', function() {
        $('.step2').css('display', 'none');
        $('.step3').css('display', 'block');
    });
});