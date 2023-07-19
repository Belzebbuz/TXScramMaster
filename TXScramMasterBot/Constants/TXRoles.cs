namespace TXScramMasterBot.Constants;

public static class TXRoles
{
	public const string Manager = "Управляющие проектами";
	public const string Developer = ".NET разработчики";
	public const string BusinessAnalyst = "Бизнес аналитики";

	public static List<string> GetRoleNames() => new List<string>()
	{
		Manager,
		Developer,
		BusinessAnalyst
	};
}
