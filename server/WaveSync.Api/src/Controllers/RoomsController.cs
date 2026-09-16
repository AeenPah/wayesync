using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaveSync.Api.Data;
using WaveSync.Api.DTOs.Rooms;
using WaveSync.Api.Models;

namespace WaveSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly WaveSyncDbContext _db;

    public RoomsController(WaveSyncDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetRooms()
    {
        var rooms = await _db.Rooms
            .AsNoTracking()
            .Select(room => new RoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                CreatedAt = room.CreatedAt
            })
            .ToListAsync();

        return Ok(rooms);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> GetRoom(Guid id)
    {
        var room = await _db.Rooms
            .AsNoTracking()
            .Where(room => room.Id == id)
            .Select(room => new RoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                CreatedAt = room.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (room is null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> CreateRoom(
        CreateRoomRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Room name is required.");
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Rooms.Add(room);

        await _db.SaveChangesAsync();

        var response = new RoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            CreatedAt = room.CreatedAt
        };

        return CreatedAtAction(
            nameof(GetRoom),
            new { id = room.Id },
            response);
    }
}