using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    public class DataAccessConfiguration
    {
        public string ConnectionString { get; }

        public DataAccessConfiguration(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }

}
