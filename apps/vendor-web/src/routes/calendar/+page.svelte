<script lang="ts">
    import { onMount } from 'svelte';
    import { Calendar } from '@fullcalendar/core';
    import dayGridPlugin from '@fullcalendar/daygrid';
    import interactionPlugin from '@fullcalendar/interaction';
    import timeGridPlugin from '@fullcalendar/timegrid';

    let calendarEl: HTMLElement;
    let calendar: Calendar;

    onMount(() => {
        calendar = new Calendar(calendarEl, {
            plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
            initialView: 'dayGridMonth',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            selectable: true,
            selectMirror: true,
            select: handleDateSelect,
            eventClick: handleEventClick,
            events: [
                { title: 'Wedding Booking', start: new Date().toISOString().split('T')[0], color: '#2563eb' },
                { title: 'Blocked', start: new Date(new Date().setDate(new Date().getDate() + 2)).toISOString().split('T')[0], display: 'background', color: '#ff0000' }
            ]
        });

        calendar.render();

        return () => {
            calendar.destroy();
        };
    });

    function handleDateSelect(selectInfo: any) {
        let title = prompt('Please enter a new title for your event');
        let calendarApi = selectInfo.view.calendar;

        calendarApi.unselect(); // clear date selection

        if (title) {
            calendarApi.addEvent({
                id: createEventId(),
                title,
                start: selectInfo.startStr,
                end: selectInfo.endStr,
                allDay: selectInfo.allDay
            });
        }
    }

    function handleEventClick(clickInfo: any) {
        if (confirm(`Are you sure you want to delete the event '${clickInfo.event.title}'`)) {
            clickInfo.event.remove();
        }
    }

    let eventGuid = 0;
    function createEventId() {
        return String(eventGuid++);
    }
</script>

<div class="p-8 space-y-6">
    <h1 class="text-3xl font-bold">Availability Calendar</h1>
    <div class="card p-4 bg-white rounded-lg shadow border">
        <div bind:this={calendarEl}></div>
    </div>
</div>

<style>
    /* Basic overrides for shadcn/tailwind compatibility if needed */
    :global(.fc) {
        font-family: inherit;
    }
    :global(.fc-button-primary) {
        background-color: hsl(var(--primary));
        border-color: hsl(var(--primary));
    }
    :global(.fc-button-primary:hover) {
        background-color: hsl(var(--primary) / 0.9);
        border-color: hsl(var(--primary) / 0.9);
    }
</style>
