using TXScramMasterBot.Domain;

namespace TXScramMasterBot.Contracts;

public interface ITaskService
{
	Task<IResult> CreateAsync(int id, Guid roleId);
	Task<SprintTask?> GetAsync(int id);
	Task<IResult> GradeAsync(long chatId, int taskId, int grade);
}
