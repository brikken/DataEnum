using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataUpdate.Schema;

namespace DataUpdate.Schema.Analysis
{
    public class Analyzer
    {
        public static Description GetDescription(Configuration configuration, string server)
        {
            var connectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                Authentication = System.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryIntegrated,
                DataSource = server
            };
            var connection = new System.Data.SqlClient.SqlConnection(connectionStringBuilder.ConnectionString);


        }
    }
}
