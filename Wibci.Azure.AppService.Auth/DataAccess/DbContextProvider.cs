namespace Wibci.Azure.AppService.Auth
{
    public interface IDbContextProvider
    {
        string ContextConnectionString { get; }

        AuthIdentityDbContext Context();
    }

    public class DefaultDbContextProvider : IDbContextProvider
    {
        public string ContextConnectionString
        {
            get
            {
                return "";
            }
        }

        public AuthIdentityDbContext Context()
        {
            return new AuthIdentityDbContext(ContextConnectionString);
        }
    }
}