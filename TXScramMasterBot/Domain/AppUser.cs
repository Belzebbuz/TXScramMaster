namespace TXScramMasterBot.Domain;

public class AppUser
{
    public long Id { get; private set; }
    public string Name { get; private set; } = default!;

    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    private List<Role> _roles = default!;
    private AppUser()
    {
    }
    public static AppUser Create(long id, string name)
    {
        return new AppUser { Id = id, Name = name };
    }
    public void ClearRoles()
    {
		if (_roles is null)
			throw new InvalidOperationException(nameof(_roles));
        _roles.Clear();
	}
	public void AddRole(Role role)
    {
        if (_roles is null && Id == default)
            throw new InvalidOperationException(nameof(_roles));
        else
            _roles = new();
        if (_roles.Any(r => r.Name == role.Name))
            return;
        _roles.Add(role);
    }
}
