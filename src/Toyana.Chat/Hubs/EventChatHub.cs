using Microsoft.AspNetCore.SignalR;

namespace Toyana.Chat.Hubs;

public class EventChatHub : Hub
{
    // Client joins a room for a specific Booking or Event
    public async Task JoinRoom(string eventId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, eventId);
        await Clients.Group(eventId).SendAsync("UserJoined", Context.ConnectionId, eventId);
    }

    public async Task SendMessage(string eventId, string user, string message)
    {
        await Clients.Group(eventId).SendAsync("ReceiveMessage", user, message);
    }
}
