using Deployf.Botf;
using TXScramMasterBot.Contracts;

namespace TXScramMasterBot.Services;

public class AuthService : IBotUserService
{
	private readonly IUserService _userService;

	public AuthService(IUserService userService)
    {
		_userService = userService;
	}
    public async ValueTask<(string? id, string[]? roles)> GetUserIdWithRoles(long tgUserId)
	{
		var user = await _userService.GetAsync(tgUserId);
		var id = user?.Id.ToString();
		var roles = user?.Roles.Select(x => x.Name).ToArray();
		return new (id, roles);
	}
}
