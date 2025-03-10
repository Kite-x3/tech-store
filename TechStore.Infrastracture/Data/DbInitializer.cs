using Microsoft.EntityFrameworkCore;
using TechStore.Infrastracture.Data;

namespace TechStore.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
