namespace SupervisorMobility.Client.Services.LoginService
{
    public interface ILoginService
    {
        Task<AD_User> LoginAD(string username, string password);
    }
}
