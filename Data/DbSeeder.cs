using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GolfBookingApp.Models;

namespace GolfBookingApp.Data
{
    public class DbSeeder
    {
        private readonly GolfClubContext _context;

        public DbSeeder(GolfClubContext context)
        {
            _context = context;
        }

        public async Task SeedDatabase()
        {
            // Seed when the database is empty
            if (!_context.Members.Any())
            {
                // Path to the JSON file
                string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "members-seed.json");

                // Read the JSON file
                string jsonData = await File.ReadAllTextAsync(jsonFilePath);
                var members = JsonSerializer.Deserialize<List<Member>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Add the members to the database
                await _context.Members.AddRangeAsync(members);
                await _context.SaveChangesAsync();
            }
        }
    }
}
