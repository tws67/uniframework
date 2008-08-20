using System.Configuration;

namespace Uniframework.Services.db4oProviders
{
    public interface IConnectionStringStore
    {
        string GetConnectionString(string connectionStringName);
    }

    public sealed class ConfigurationManagerConnectionStringStore : IConnectionStringStore
    {
        public string GetConnectionString(string connectionStringName)
        {
            ConnectionStringSettings ConnectionStringSettings =
                ConfigurationManager.ConnectionStrings[connectionStringName];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
                return null;

            return ConnectionStringSettings.ConnectionString;
        }
    }
}