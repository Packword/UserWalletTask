namespace UserWallet.Data.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationState _authenticationState;
    private UserStore UserStore { get; }

    public CustomAuthenticationStateProvider(UserStore userStore)
    {
        _authenticationState = new AuthenticationState(userStore.CurrentUser);

        UserStore = userStore;
        UserStore.CurrentUserChanged += newUser =>
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(newUser)));
        };
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(_authenticationState);
}
