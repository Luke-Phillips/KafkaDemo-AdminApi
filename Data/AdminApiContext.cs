using AdminApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminApi.Data;

public class AdminApiContext : DbContext
{
    public DbSet<SavedNumber> SavedNumbers { get; set; }

    public string DbPath { get; }

    public AdminApiContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "adminApi.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}