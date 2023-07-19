using TXScramMasterBot.Domain;
using TXScramMasterBot.Wrapper;

namespace TXScramMasterBot.Contracts;

public interface IUserService
{
	Task<IResult> CreateUserAsync(long chatId, string name, Guid roleId);
	Task<AppUser?> GetAsync(long chatId);
	Task<List<AppUser>> GetByRoleAsync(Guid roleId);
	Task<List<Role>> GetRolesAsync(long chatId);
	Task<List<Role>> GetRolesAsync();
	Task<IResult> SetRoleAsync(long chatId, Guid roleId);
}
