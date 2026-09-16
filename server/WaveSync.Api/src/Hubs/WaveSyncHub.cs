using Microsoft.AspNetCore.SignalR;
using WaveSync.Api.Services;

namespace WaveSync.Api.Hubs;

public class WaveSyncHub : Hub
{
    private readonly RoomStateService _roomState;

    public WaveSyncHub(RoomStateService roomState)
    {
        _roomState = roomState;
    }

    public async Task JoinRoom(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            throw new HubException("Room ID is required.");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            roomId
        );

        _roomState.AddParticipant(
            roomId,
            Context.ConnectionId
        );

        var participants = _roomState.GetParticipants(roomId);

        await Clients.Caller.SendAsync(
            "RoomState",
            new
            {
                roomId,
                participants
            });

        await Clients.OthersInGroup(roomId).SendAsync(
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

        _roomState.RemoveParticipant(
            roomId,
            Context.ConnectionId
        );

        await Clients.OthersInGroup(roomId).SendAsync(
            "ParticipantLeft",
            Context.ConnectionId
        );
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        var rooms = _roomState.RemoveParticipantFromAllRooms(
            Context.ConnectionId
        );

        foreach (var roomId in rooms)
        {
            await Clients.Group(roomId).SendAsync(
                "ParticipantLeft",
                Context.ConnectionId
            );
        }

        await base.OnDisconnectedAsync(exception);
    }
}