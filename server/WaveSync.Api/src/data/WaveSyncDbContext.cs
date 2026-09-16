using Microsoft.EntityFrameworkCore;

namespace WaveSync.Api.Data;

public class WaveSyncDbContext : DbContext
{
    public WaveSyncDbContext(DbContextOptions<WaveSyncDbContext> options)
        : base(options)
    {
    }
}