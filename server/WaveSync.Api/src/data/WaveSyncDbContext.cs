using Microsoft.EntityFrameworkCore;
using WaveSync.Api.Models;

namespace WaveSync.Api.Data;

public class WaveSyncDbContext : DbContext
{
    public WaveSyncDbContext(DbContextOptions<WaveSyncDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();
}