$(document).ready(function () {
    const max = 5; // Maximum number of days to select
    let myData = new Date(),
        array = [],
        daysCount = 90, // Change this number for change available search count days
        myDataSave = new Date(),
        data = {
            startDate: new Date(myData.setDate(myData.getDate() - daysCount)),
            startDateSave: new Date(myDataSave.setDate(myDataSave.getDate() - daysCount)),
            currentDate: {}
        }
    $('#days-count').html(daysCount);
    $('.step-description strong').html(max);

    function selectDate(array) {
        $('.vanilla-calendar-date').on("click", function() {
            $('#ex2 strong').html(max);
            if (array.length == max && array.indexOf($(this).attr("id")) == -1) {
                $('#ex2').modal('show'); 
                return false;
            }
    
            if (this.classList[3] === 'bg-color' || this.classList[4] === 'bg-color') {
                $(this).removeClass('bg-color')
            } else {
                $(this).addClass('bg-color');
            }
    
            for (let i = 0; i < array.length; i++) {
                if ($(this).attr("id") == array[i]) {
                    array.splice(i, 1);
                    console.log(array);
                    return false;
                }
            }
    
            array.push($(this).attr("id"));
            console.log(array);
        });
    }

    function setIndent(date, className) {
        switch(date) {
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
            date = data.startDate;
        }

        if (start === undefined && end === undefined) {
            end = 32 - new Date(date.getFullYear(), date.getMonth(), 32).getDate();
            start = date.getDate();
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
        selectDate(array);

        if (array) {
            for (let i = 0; i < array.length; i++) {
                $('#' + array[i]).addClass('bg-color');
            }
        }
    }
    changeMonth();
    selectDate();

    $('.vanilla-calendar-btn-right').on('click', function() {
        data.startDate.setMonth(data.startDate.getMonth() + 1);
        let date = new Date(data.startDate.setDate(1));
        data.currentDate = date;

        let current = new Date();
        if (data.startDate.getMonth() === current.getMonth()) {
            endDate = current.getDate();
            let getFirstDay = new Date(current.getFullYear(), current.getMonth(), 1);
            let totalDateCurrent = moment(new Date(getFirstDay));

            changeMonth(getFirstDay, totalDateCurrent.get('date'), endDate - 1);

            return false;
        }

        changeMonth(data.currentDate);
    });

    $('.vanilla-calendar-btn-left').on('click', function() {
        data.startDate.setMonth(data.startDate.getMonth() - 1);
        let date = new Date(data.startDate.setDate(1));
        data.currentDate = date;

        if (data.currentDate.getMonth() === data.startDateSave.getMonth()) {
            endDate = 32 - new Date(data.startDateSave.getFullYear(), data.startDateSave.getMonth(), 32).getDate();
            startDate = data.startDateSave.getDate();

            changeMonth(data.startDateSave, startDate, endDate);

            return false;
        }

        changeMonth(data.currentDate);
    });
});