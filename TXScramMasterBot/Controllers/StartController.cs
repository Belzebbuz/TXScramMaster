using Deployf.Botf;
using SQLitePCL;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TXScramMasterBot.Constants;
using TXScramMasterBot.Contracts;
using TXScramMasterBot.Domain;

namespace TXScramMasterBot.Controllers;

public class StartController : BotController
{
	private readonly IUserService _userService;
	private readonly ILogger _logger;

	public StartController(IUserService userService, ILogger<StartController> logger)
    {
		_userService = userService;
		_logger = logger;
	}

	[Action("/start")]
	public async Task StartAsync()
	{
		var user = await _userService.GetAsync(ChatId);
		if(user == null)
		{
			await RegisterAsync();
			return;
		}
		var roles = await _userService.GetRolesAsync(ChatId);
		var commands = string.Empty;
		if(roles.Any(x => x.Name == TXRoles.Manager))
		{
			commands += "/start_grade - начинает процесс оценки задачи\n";
			commands += "/result - получить результат оценки\n";
			KButton("/start_grade");
			KButton("/result");
		}
		if (roles.Any(x => x.Name == TXRoles.Developer || x.Name == TXRoles.BusinessAnalyst))
		{
			commands += "/grade - оценить задачу\n";
			KButton("/grade");
		}
		await Send(commands);
	}
	private async Task RegisterAsync()
	{
		await Send("Введите ваше имя:");
		var name = await AwaitText();
		if (string.IsNullOrEmpty(name))
			await RegisterAsync();
		var roles = await _userService.GetRolesAsync();
		foreach (var role in roles)
		{
			RowButton(role.Name, Q(AssignRoleAsync, role.Id, name));
		}
		await Send("Ваша группа:");
	}

	[Action]
	public async Task AssignRoleAsync(Guid roleId, string userName)
	{
		var messageId = Context.GetSafeMessageId();
		if (messageId.HasValue)
			await Client.DeleteMessageAsync(ChatId, messageId.Value);
		var result = await _userService.CreateUserAsync(ChatId, userName, roleId);
		if (result.Succeeded)
		{
			await StartAsync();
			return;
		}
		await Send(result.Messages[0]);
	}

	[Action("/change_role")]
	[Authorize]
	public async Task ChangeRoleAsync()
	{
		var roles = await _userService.GetRolesAsync();
		foreach(var role in roles)
		{
			RowButton(role.Name, Q(SetRoleAsync, role.Id));
		};
		await Send("Выберете группу пользователей");
	}
	[Action]
	private async Task SetRoleAsync(Guid roleId)
	{
		var messageId = Context.GetSafeMessageId();
		if (messageId.HasValue)
			await Client.DeleteMessageAsync(ChatId, messageId.Value);
		var result = await _userService.SetRoleAsync(ChatId, roleId);
		if (!result.Succeeded)
		{
			await Send(result.Messages[0]);
			return;
		}
		await Send("Группа пользователей установлена");
	}




	[On(Handle.Exception)]
	public async Task HandleAsync(Exception ex)
	{
		_logger.LogError(ex.ToString());
		await Send("Что-то пошло не так, обратитесь к администратору канала.");
	}
}
