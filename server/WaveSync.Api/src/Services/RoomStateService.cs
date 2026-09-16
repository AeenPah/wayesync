using System.Collections.Concurrent;
using WaveSync.Api.Models;

namespace WaveSync.Api.Services;

public class RoomStateService
{
    private readonly ConcurrentDictionary<
        string,
        ConcurrentDictionary<string, Participant>
    > _rooms = new();

    public IReadOnlyCollection<Participant> GetParticipants(string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out var participants))
        {
            return [];
        }

        return participants.Values.ToArray();
    }

    public void AddParticipant(
        string roomId,
        Participant participant)
    {
        var participants = _rooms.GetOrAdd(
            roomId,
            _ => new ConcurrentDictionary<string, Participant>()
        );

        participants[participant.Id] = participant;
    }

    public void RemoveParticipant(string roomId, string participantId)
    {
        if (!_rooms.TryGetValue(roomId, out var participants))
        {
            return;
        }

        participants.TryRemove(
            participantId,
            out _
        );

        if (participants.IsEmpty)
        {
            _rooms.TryRemove(
                roomId,
                out _
            );
        }
    }

    public IReadOnlyCollection<string> RemoveParticipantFromAllRooms(
        string connectionId)
    {
        var removedFrom = new List<string>();

        foreach (var room in _rooms)
        {
            if (room.Value.TryRemove(connectionId, out _))
            {
                removedFrom.Add(room.Key);
            }

            if (room.Value.IsEmpty)
            {
                _rooms.TryRemove(room.Key, out _);
            }
        }

        return removedFrom;
    }

    public IReadOnlyCollection<string> RemoveConnection(string connectionId)
    {
        var removedRooms = new List<string>();

        foreach (var room in _rooms)
        {
            var participant = room.Value.Values
                .FirstOrDefault(p =>
                    p.ConnectionId == connectionId);

            if (participant is null)
            {
                continue;
            }

            room.Value.TryRemove(
                participant.Id,
                out _
            );

            removedRooms.Add(room.Key);

            if (room.Value.IsEmpty)
            {
                _rooms.TryRemove(
                    room.Key,
                    out _
                );
            }
        }

        return removedRooms;
    }
}