using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            // Console.WriteLine("Waiting for you to attach the debugger if needed, else just enter");
            // Console.ReadLine();
            if (await context.Users.AnyAsync())
            {
                return;
            }
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{user.UserName}"));
                user.PasswordSalt = hmac.Key;
                await context.Users.AddAsync(user);
            }

            await context.SaveChangesAsync();
        }
    }
}