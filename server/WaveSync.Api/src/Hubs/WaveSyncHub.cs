using Microsoft.AspNetCore.SignalR;

namespace WaveSync.Api.Hubs;

public class WaveSyncHub : Hub
{
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            roomId
        );

        await Clients.Group(roomId).SendAsync(
            "ParticipantJoined",
            Context.ConnectionId
        );
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            roomId
        );

        await Clients.Group(roomId).SendAsync(
            "ParticipantLeft",
            Context.ConnectionId
        );
    }
}