$(document).ready(function () {
    if ($('#calendar').length) {
        var events = [];
        var selectedRange = null;

        $('#calendar-events .calendar-event').each(function () {
            var event = {
                title: $(this).data('title'),
                start: $(this).data('start'),
                end: $(this).data('end'),
                description: $(this).data('description'),
                groupId: $(this).data('group-id')
            };
            events.push(event);
        });

        $('#calendar').fullCalendar({
            locale: 'ru',
            events: events,
            height: 'auto',
            contentHeight: 'auto',
            aspectRatio: 1.5,
            buttonText: {
                today: 'Сегодня'
            },
            monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
            monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
            dayNames: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'],
            dayNamesShort: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
            eventRender: function (event, element) {
                element.qtip({
                    content: event.description
                });
            },
            viewRender: function (view, element) {
                resetHighlights();
                if (selectedRange) {
                    highlightRange(selectedRange.start, selectedRange.end);
                } else {
                    highlightToday();
                }
            }
        });

        function highlightRange(start, end) {
            var startDate = moment(start);
            var endDate = moment(end);

            while (startDate.isSameOrBefore(endDate, 'day')) {
                $("td[data-date='" + startDate.format('YYYY-MM-DD') + "']").addClass('fc-highlight').css({
                    'background-color': '#007bff',
                    'color': '#fff'
                });
                startDate.add(1, 'days');
            }
        }

        function highlightToday() {
            var today = moment().format('YYYY-MM-DD');
            $("td[data-date='" + today + "']").addClass('fc-highlight btn-dark').css({
                'background-color': '#007bff',
                'color': '#fff'
            });
        }

        function resetHighlights() {
            $("td.fc-highlight").removeClass('fc-highlight').css({
                'background-color': '',
                'color': ''
            });
        }

        $('.group-item').on('click', function () {
            var start = $(this).data('start');
            var end = $(this).data('end');
            selectedRange = {
                start: start,
                end: end
            };
            $('#calendar').fullCalendar('gotoDate', start);
            $('#calendar').fullCalendar('unselect');
            highlightRange(start, end);
        });

        $('#calendar').on('click', '.fc-today-button btn-dark', function () {
            selectedRange = null;
            highlightToday();
        });
    }
});
