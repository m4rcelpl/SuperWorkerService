using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperWorkerService
{
    public class DatabaseAccess
    {
        //requirement:
        //Dapper and some database connector like MySqlConnector

        /*
         * 
        
        
        public async Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters, string connectionString)
        {
            IEnumerable<T>? rows;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                rows = await connection.QueryAsync<T>(sql, parameters);
            }

            return rows.ToList();
        }

        public async Task<int> SaveDataAsync<T>(string sql, T parameters, string connectionString)
        {
            int result = 0;

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                result = await connection.ExecuteAsync(sql, parameters);
            }

            return result;
        }
        */
    }
}
