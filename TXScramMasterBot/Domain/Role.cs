namespace TXScramMasterBot.Domain;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public IReadOnlyList<AppUser> Users => _users;
    private List<AppUser> _users = default!;
	private Role() { }

    public static Role Create(string name) 
    {
        return new Role { Name = name };
    }
    public void AddUserToRole(AppUser user)
    {
        if (_users is null)
            throw new InvalidOperationException("Users was null");
        if (_users.Any(x => x.Id == user.Id))
            return;
        _users.Add(user);
    }
	public void RemoveUserFromRole(long id)
	{
		if (_users is null)
			throw new InvalidOperationException("Users was null");
        var existed = _users.First(x => x.Id == id);
		_users.Remove(existed);
	}
}