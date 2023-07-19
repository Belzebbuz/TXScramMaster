namespace TXScramMasterBot.Domain;

public class SprintTask
{
    public int Id { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = default!;
    public IReadOnlyList<SprintTaskGrade> Grades => _grades.AsReadOnly();
    private List<SprintTaskGrade> _grades = default!;

    private SprintTask() { }
    public static SprintTask Create(int id, Guid roleId)
    {
        return new SprintTask { Id = id, RoleId = roleId };
    }

    public void AddGrade(SprintTaskGrade grade)
    {
        if (_grades is null)
            throw new InvalidOperationException("Grades not loaded");

        var existed = _grades.FirstOrDefault(x => x.UserId == grade.UserId);
        if(existed is null)
        {
			_grades.Add(grade);
            return;
		}
		existed.UpdateGrade(grade.Grade);
	}
}
public class SprintTaskGrade
{
	public Guid Id { get; set; }
	public long UserId { get; private set; }
	public AppUser User { get; private set; } = default!;
	public int Grade { get; private set; }
	private SprintTaskGrade() { }
	public static SprintTaskGrade Create(long userId, int grade)
	{
		if (grade < 1)
			throw new ArgumentOutOfRangeException(nameof(grade));
		return new SprintTaskGrade { UserId = userId, Grade = grade };
	}

	public void UpdateGrade(int grade)
	{
		if (grade < 1)
			throw new ArgumentOutOfRangeException(nameof(grade));
		Grade = grade;
	}
}