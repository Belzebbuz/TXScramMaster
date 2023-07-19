using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TXScramMasterBot.Context;
using TXScramMasterBot.Contracts;
using TXScramMasterBot.Domain;
using TXScramMasterBot.Wrapper;

namespace TXScramMasterBot.Services;

public class TaskService : ITaskService
{
	private readonly AppDbContext _context;

	public TaskService(AppDbContext context)
    {
		_context = context;
	}
    public async Task<IResult> CreateAsync(int id, Guid roleId)
	{
		var existed = await _context.SprintTasks.FindAsync(id);
		if (existed != null)
			return Result.Fail("Задание уже существует");
		var task = SprintTask.Create(id, roleId);
		await _context.SprintTasks.AddAsync(task);
		await _context.SaveChangesAsync();
		return Result.Success();
	}

	public async Task<SprintTask?> GetAsync(int id)
	{
		return await _context.SprintTasks.Include(x => x.Grades).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<IResult> GradeAsync(long chatId, int taskId, int grade)
	{
		var user = await _context.Users.FindAsync(chatId);
		if (user == null)
			return Result.Fail("Польователя не существует");
		var task = await _context.SprintTasks.Include(x => x.Grades).FirstOrDefaultAsync(x => x.Id == taskId);
		if (task == null)
			return Result.Fail("Задание не существует");
		task.AddGrade(SprintTaskGrade.Create(user.Id, grade));
		_context.SprintTasks.Update(task);
		await _context.SaveChangesAsync();
		return Result.Success();
	}
}
