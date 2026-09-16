using System.Collections.Concurrent;

namespace WaveSync.Api.Services;

public class RoomStateService
{
    private readonly ConcurrentDictionary<
        string,
        ConcurrentDictionary<string, byte>
    > _rooms = new();

    public IReadOnlyCollection<string> GetParticipants(string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out var participants))
        {
            return [];
        }

        return participants.Keys.ToArray();
    }

    public void AddParticipant(string roomId, string connectionId)
    {
        var participants = _rooms.GetOrAdd(
            roomId,
            _ => new ConcurrentDictionary<string, byte>()
        );

        participants.TryAdd(connectionId, 0);
    }

    public void RemoveParticipant(string roomId, string connectionId)
    {
        if (!_rooms.TryGetValue(roomId, out var participants))
        {
            return;
        }

        participants.TryRemove(connectionId, out _);

        if (participants.IsEmpty)
        {
            _rooms.TryRemove(roomId, out _);
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
}