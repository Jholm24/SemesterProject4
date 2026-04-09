namespace SkateboardAS.Services;

public class RoleState
{
    public string CurrentRole { get; private set; } = "Manager";
    public event Action? OnChange;

    public void SetRole(string role)
    {
        CurrentRole = role;
        OnChange?.Invoke();
    }
}
