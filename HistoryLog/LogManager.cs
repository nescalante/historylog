using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HistoryLog
{
    public class LogManager : IDisposable
    {
        private int _applicationId;
        private DataAccess _da;

        public LogManager(string application, IDbConnection conn)
        {
            Initialize(application, conn);
        }

        public LogManager(string application, string connectionString)
        {
            var conn = new SqlConnection(connectionString);
            Initialize(application, conn);
        }

        private void Initialize(string application, IDbConnection conn)
        {
            // open if closed
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            _da = new DataAccess(conn);
            _applicationId = _da.GetApplicationId(application);
        }

        private string[] GetKeyNames(Type type)
        {
            // find key
            var properties = type.GetProperties();
            var keys = properties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute));

            return keys.Select(k => k.Name).ToArray();
        }

        private string GetProperty<T>(T entity, string property)
        {
            var pi = typeof(T).GetProperties().Single(p => p.Name == property);
            var obj = pi.GetValue(entity, null);

            return obj == null ? null : obj.ToString();
        }

        /// <summary>
        /// saves the entity log into database
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="entity">object</param>
        /// <param name="user">user who perform action</param>
        public void Log<T>(T entity, string user = null) where T : new()
        {
            // get entity
            var entityId = _da.GetEntityId(typeof(T), _applicationId);

            // get columns
            var columns = _da.GetColumns(typeof(T), _applicationId);

            // create log
            var logId = _da.CreateLog(entityId, user);

            // log values
            foreach (var p in typeof(T).GetProperties())
            {
                var column = columns.Single(c => c.Name == p.Name);
                var value = p.GetValue(entity, null);
                _da.Log(logId, column.Id, value);
            }
        }

        /// <summary>
        /// gets the history of some entity
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="entity">object</param>
        /// <returns>history</returns>
        public IEnumerable<Log<T>> GetHistory<T>(T entity) where T : new()
        {
            var keys = this.GetKeyNames(typeof(T));
            var entityId = _da.GetEntityId(typeof(T), _applicationId);
            var columns = _da.GetColumns(entityId);
            var keyColumns = keys.Select(k => columns.Single(c => c.Name == k));
            var keyValues = keyColumns.Select(kc => new KeyValuePair<int, string>(kc.Id, this.GetProperty(entity, kc.Name)));
            var logs = _da.GetLogs(entityId, keyValues);

            return _da.GetHistory<T>(logs).OrderByDescending(l => l.Date).ToList();
        }

        public void Dispose()
        {
            _da.Dispose();
        }
    }
}
