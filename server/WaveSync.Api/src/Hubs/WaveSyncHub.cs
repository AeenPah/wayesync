using Microsoft.AspNetCore.SignalR;
using WaveSync.Api.Models;
using WaveSync.Api.Services;

namespace WaveSync.Api.Hubs;

public class WaveSyncHub : Hub
{
    private readonly RoomStateService _roomState;

    public WaveSyncHub(RoomStateService roomState)
    {
        _roomState = roomState;
    }

    public async Task JoinRoom(
        string roomId,
        string participantId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            throw new HubException(
                "Room ID is required."
            );
        }

        if (string.IsNullOrWhiteSpace(participantId))
        {
            throw new HubException(
                "Participant ID is required."
            );
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            roomId
        );

        var participant = new Participant
        {
            Id = participantId,
            ConnectionId = Context.ConnectionId
        };

        _roomState.AddParticipant(
            roomId,
            participant
        );

        var participants =
            _roomState.GetParticipants(roomId);

        await Clients.Caller.SendAsync(
            "RoomState",
            new
            {
                roomId,
                participants
            }
        );

        await Clients.OthersInGroup(roomId)
            .SendAsync(
                "ParticipantJoined",
                participant
            );
    }

    public async Task LeaveRoom(
        string roomId,
        string participantId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            roomId
        );

        _roomState.RemoveParticipant(
            roomId,
            participantId
        );

        await Clients.OthersInGroup(roomId)
            .SendAsync(
                "ParticipantLeft",
                participantId
            );
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        var rooms =
            _roomState.RemoveConnection(
                Context.ConnectionId
            );

        foreach (var roomId in rooms)
        {
            await Clients.Group(roomId)
                .SendAsync(
                    "ParticipantLeft",
                    Context.ConnectionId
                );
        }

        await base.OnDisconnectedAsync(
            exception
        );
    }
}