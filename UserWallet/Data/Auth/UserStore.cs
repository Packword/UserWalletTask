namespace UserWallet.Data.Auth;

/// <summary>
/// Used to persist the user during the application work.
/// </summary>
public class UserStore
{
    public event Action<ClaimsPrincipal>? CurrentUserChanged;
    private ClaimsPrincipal? _currentUser;

    public ClaimsPrincipal CurrentUser
    {
        get => _currentUser ?? new ClaimsPrincipal();
        set
        {
            _currentUser = value;
            CurrentUserChanged?.Invoke(value);
        }
    }
}
