using ChatApp.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Core.Domain.Extensions
{
    public static class ModelBuilderExtensionsChats
    {
        public static void SeedChats(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>().HasData(new Chat
            {
                Id = new Guid("427FCAA3-2655-401D-AD6B-9418B2410763"),
                Name = "Global",
                CreatedAt = new DateTime(2025, 05, 03, 20, 0, 0)
            });

        }
    }
}
