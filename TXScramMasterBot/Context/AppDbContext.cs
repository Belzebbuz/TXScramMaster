using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TXScramMasterBot.Domain;

namespace TXScramMasterBot.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<SprintTask> SprintTasks => Set<SprintTask>();
    public DbSet<SprintTaskGrade> SprintTaskGrades => Set<SprintTaskGrade>();
}
