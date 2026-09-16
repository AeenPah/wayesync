namespace WaveSync.Api.DTOs.Rooms;

public class RoomResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}