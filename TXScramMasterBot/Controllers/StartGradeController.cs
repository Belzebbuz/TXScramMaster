using Deployf.Botf;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TXScramMasterBot.Constants;
using TXScramMasterBot.Contracts;

namespace TXScramMasterBot.Controllers;

public class StartGradeController : BotController
{
	private readonly ITaskService _taskService;
	private readonly IUserService _userService;

	public StartGradeController(ITaskService taskService, IUserService userService)
    {
		_taskService = taskService;
		_userService = userService;
	}

	[Action("/start_grade")]
	[Authorize(TXRoles.Manager)]
	public async Task StartGradeAsync()
	{
		await Send("Введите номер задачи:");
		var taskId = await AwaitText();
		if (int.TryParse(taskId, out var id))
		{
			var existed = await _taskService.GetAsync(id);
			if (existed != null)
			{
				await ShowTaskMenuAsync(id);
				return;
			}
			await FirstStageCreateTaskAsync(id);
		}
		else
			await Send("Ожидалось число");
	}
	[Action("/results")]
	[Authorize(TXRoles.Manager)]
	public async Task GetResultsAsync()
	{
		await Send("Введите номер задачи:");
		var taskId = await AwaitText();
		if (int.TryParse(taskId, out var id))
		{
			var existed = await _taskService.GetAsync(id);
			if (existed != null)
			{
				await ShowTaskMenuAsync(id);
				return;
			}
			await Send("Задача не найдена");
		}
		else
			await Send("Ожидалось число");
	}

	[Action("/grade")]
	[Authorize]
	public async Task UserGradeAsync()
	{
		await Send("Введите номер задачи:");
		var taskId = await AwaitText();
		if (int.TryParse(taskId, out var id))
		{
			var existed = await _taskService.GetAsync(id);
			if (existed != null)
			{
				var user = await _userService.GetAsync(ChatId);
                if (user!.Roles.Any(x => x.Id != existed.RoleId))
                {
					await Send("Неверная группа пользователей");
					return;
                }
                await CallBackGradeAsync(id);
				return;
			}
			await Send("Задача не найдена");
		}
		else
			await Send("Ожидалось число");
	}
	private async Task FirstStageCreateTaskAsync(int id)
	{
		var roles = await _userService.GetRolesAsync();
		foreach (var role in roles)
		{
			RowButton(role.Name, Q(LastStageCreateTaskAsync, id, role.Id));
		}
		await Send("Группа пользователей");
	}
	[Action]
	private async Task LastStageCreateTaskAsync(int id, Guid roleId)
	{
		var messageId = Context.GetSafeMessageId();
		if (messageId.HasValue)
			await Client.DeleteMessageAsync(ChatId, messageId.Value);
		var result = await _taskService.CreateAsync(id, roleId);
		if(!result.Succeeded)
		{
			await Send(result.Messages[0]);
			return;
		}
		var usersInRole = await _userService.GetByRoleAsync(roleId);
		var keyBoard = new InlineKeyboardMarkup(new List<InlineKeyboardButton>
		{
			InlineKeyboardButton.WithUrl("Задача Bitrix",$"https://bis-24.bitrix24.ru/workgroups/group/289/tasks/task/view/{id}/"),
			InlineKeyboardButton.WithCallbackData("Оценить", Q(CallBackGradeAsync, id))
		});

		foreach (var user in usersInRole)
        {
			await Client.SendTextMessageAsync(user.Id, $"Оценка задачи №{id}", replyMarkup: keyBoard);
        }
		await Send("Сообщения отправлены.\n/results - Результаты оценки");
    }
	[Action]
	public async Task CallBackGradeAsync(int id)
	{
		var messageId = Context.GetSafeMessageId();
		foreach (var item in Enumerable.Range(1, 10))
		{
			RowButton(item.ToString(), Q(GradeAsync, id, item));
		}
		await Send("Оценка от 1 до 10");
		if(messageId.HasValue)
			await Client.DeleteMessageAsync(ChatId, messageId.Value);
	}

	[Action]
	private async Task GradeAsync(int taskId, int grade)
	{
		var messageId = Context.GetSafeMessageId();
		var result = await _taskService.GradeAsync(ChatId, taskId, grade);
		if(!result.Succeeded)
		{
			await Send(result.Messages[0]);
		}
		await Send($"Задача №{taskId} успешно оценена!");
		if (messageId.HasValue)
			await Client.DeleteMessageAsync(ChatId, messageId.Value);
	}

	
	private async Task ShowTaskMenuAsync(int id)
	{
		var task = await _taskService.GetAsync(id);
		if(task == null)
		{
			await Send("Задача не найдена!");
			return;
		}
		var avg = task.Grades.Average(x => x.Grade);
		var count = task.Grades.Count();
		if(count != 0)
		{
			RowButton("Оценившие", Q(ShowGradedUserAsync, id));
		}
		await Send($"Задача <b>№{id}</b>\nСредняя - {avg.ToString("0.00")}\nОценили - {count} чел.");
	}
	[Action]
	private async Task ShowGradedUserAsync(int taskId)
	{
		var messageId = Context.GetSafeMessageId();
		var task = await _taskService.GetAsync(taskId);
		if (task == null)
		{
			await Send("Задача не найдена!");
			return;
		}
		var message = String.Empty;
		foreach (var item in task.Grades)
		{
			message += $"{item.User.Name} - <b>{item.Grade}</b>\n";
		}
		if (message == string.Empty)
			message = "Оценок нет";
		await Send(message);
		if (messageId.HasValue)
			await Client.DeleteMessageAsync(ChatId, messageId.Value);
	}
}
