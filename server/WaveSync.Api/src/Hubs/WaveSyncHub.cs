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
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            roomId
        );
    }
}