using System.Configuration;

namespace HistoryLog
{
    public class Configuration : ConfigurationSection
    {
        private const string _connectionStringName = "connectionStringName";

        [ConfigurationProperty(_connectionStringName, IsRequired = true)]
        public string connectionStringName
        {
            get { return (string)base[_connectionStringName]; }
            set { base[_connectionStringName] = value; }
        }

        private const string _providerName = "providerName";

        [ConfigurationProperty(_providerName, IsRequired = true)]
        public string providerName
        {
            get { return (string)base[_providerName]; }
            set { base[_providerName] = value; }
        }
    
    }

}
