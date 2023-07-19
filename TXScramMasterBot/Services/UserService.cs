using Microsoft.EntityFrameworkCore;
using TXScramMasterBot.Context;
using TXScramMasterBot.Contracts;
using TXScramMasterBot.Domain;
using TXScramMasterBot.Wrapper;

namespace TXScramMasterBot.Services;

public class UserService : IUserService
{
	private readonly AppDbContext _context;

	public UserService(AppDbContext context)
    {
		_context = context;
	}
    public async Task<IResult> CreateUserAsync(long chatId, string name, Guid roleId)
	{
		var existed = await _context.Users.AnyAsync(x => x.Name == name || x.Id == chatId);
		if (existed)
			return Result.Fail("Пользователь с таким именем уже существует!");

		var role = await _context.Roles.FindAsync(roleId);
		if (role is null)
			return Result.Fail("Роль не найдена");
		var user = AppUser.Create(chatId, name);
		user.AddRole(role);
		await _context.Users.AddAsync(user);
		await _context.SaveChangesAsync();
		return Result.Success();
	}

	public async Task<AppUser?> GetAsync(long chatId)
	{
		return await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == chatId);
	}

	public async Task<List<AppUser>> GetByRoleAsync(Guid roleId)
	{
		return await _context.Users.Where(x => x.Roles.Any(x => x.Id == roleId)).ToListAsync();
	}

	public async Task<List<Role>> GetRolesAsync(long chatId)
	{
		return await _context.Roles.Where(x => x.Users.Select(u => u.Id).Contains(chatId)).ToListAsync();
	}

	public async Task<List<Role>> GetRolesAsync()
	{
		return await _context.Roles.ToListAsync();
	}

	public async Task<IResult> SetRoleAsync(long chatId, Guid roleId)
	{
		var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == chatId);
		if (user == null)
			return Result.Fail("Пользователь не найден");
		user.ClearRoles();
		await _context.SaveChangesAsync();
		var role = await _context.Roles.FindAsync(roleId);
		if (role == null)
			return Result.Fail("Группа не найдена");
		user.AddRole(role);
		await _context.SaveChangesAsync();
		return Result.Success();
	}
}
