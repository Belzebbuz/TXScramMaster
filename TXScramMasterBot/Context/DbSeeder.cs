using Microsoft.EntityFrameworkCore;
using TXScramMasterBot.Constants;
using TXScramMasterBot.Domain;

namespace TXScramMasterBot.Context;

public class DbSeeder
{
	private readonly AppDbContext _context;

	public DbSeeder(AppDbContext context)
    {
		_context = context;
	}

	public async Task SeedAsync()
	{
		if (await _context.Roles.AnyAsync())
			return;

		var roleNames = TXRoles.GetRoleNames();
		var roles = new List<Role>();
		roleNames.ForEach(x => roles.Add(Role.Create(x)));
		await _context.AddRangeAsync(roles);
		await _context.SaveChangesAsync();
	}
}
