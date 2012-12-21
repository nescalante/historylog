using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace HistoryLog
{
    internal class DataAccess : IDisposable
    {
        private IDbConnection _conn;

        internal DataAccess(IDbConnection conn)
        {
            _conn = conn;
        }

        internal int GetApplicationId(string application)
        {
            // get application id if exists, if not, create and return id
            var ids = _conn.Query<int>("select id from applications where name = @application", new { application = application });

            if (ids.Count() > 0)
            {
                return ids.Single();
            }
            else
            {
                return _conn.Query<int>("insert into applications(name) values (@name); select cast(scope_identity() as int)", new { name = application }).Single();
            }
        }

        internal int GetEntityId(Type type, int applicationId)
        {
            var entity = type.Name;
            // get application id if exists, if not, create and return id
            var ids = _conn.Query<int>("select id from entities where name = @entity and applicationId = @applicationId", new { entity = entity, applicationid = applicationId });

            if (ids.Count() > 0)
            {
                return ids.Single();
            }
            else
            {
                var id = _conn.Query<int>("insert into entities(name, applicationId) values (@name, @applicationId); select cast(scope_identity() as int)", new { name = entity, applicationId = applicationId }).Single();

                // create columns for new entity
                foreach (var p in type.GetProperties())
                {
                    _conn.Execute("insert into entitycolumns(name, entityId) values(@name, @entityId)", new { name = p.Name, entityId = id });
                }

                return id;
            }
        }

        public void Dispose()
        {
            _conn.Close();
            _conn.Dispose();
        }

        internal IEnumerable<EntityColumn> GetColumns(Type type, int applicationId)
        {
            // get entityId
            var entityId = GetEntityId(type, applicationId);

            // get columns from database
            var columns = GetColumns(entityId);

            // compare columns
            if (type.GetProperties().All(p => columns.Select(c => c.Name).Contains(p.Name)))
            {
                return columns;
            }
            else
            {
                var newColumns = type.GetProperties().Where(p => !columns.Any(c => p.Name == c.Name));
                foreach (var c in newColumns)
                {
                    _conn.Execute("insert into entitycolumns(name, entityId) values(@name, @entityId)", new { name = c.Name, entityId = entityId });
                }

                return GetColumns(entityId);
            }
        }

        public IEnumerable<EntityColumn> GetColumns(int entityId)
        {
            return _conn.Query<EntityColumn>("select * from entityColumns where entityid = @entityId", new { entityId = entityId });
        }

        internal void Log(int logId, int columnId, object value)
        {
            _conn.Execute("insert into logvalues(logId, entityColumnId, value) values(@logId, @entityColumnId, @value)", new { logId = logId, entityColumnId = columnId, value = (value == null ? null : value.ToString()) });
        }

        internal int CreateLog(int entityId, string user)
        {
            return _conn.Query<int>("insert into logs(entityid, date, [user]) values (@entityId, @date, @user); select cast(scope_identity() as int)", new { entityId = entityId, date = DateTime.Now, user = user }).Single();
        }

        internal IEnumerable<LogInfo> GetLogs(int entityId, IEnumerable<KeyValuePair<int, string>> keyValues)
        {
            var sql = "select * from logs l where l.entityid = @entityId";
            var p = new DynamicParameters();
            p.Add("@entityId", entityId);

            foreach (var kv in keyValues)
            {
                sql += " and exists (select null from logvalues lv where l.id = lv.logid and lv.entitycolumnid = @key" + kv.Key + " and lv.value = @value" + kv.Key + ")";
                p.Add("@key" + kv.Key, kv.Key);
                p.Add("@value" + kv.Key, kv.Value);
            }

            return _conn.Query<LogInfo>(sql, p);
        }

        internal IEnumerable<Log<T>> GetHistory<T>(IEnumerable<LogInfo> logs) where T : new()
        {
            var info = _conn.Query<ColumnInfo>("select ec.name as [Column], lv.value, lv.logid from logvalues lv inner join entitycolumns ec on lv.entitycolumnId = ec.id where lv.logid in @ids", new { ids = logs.Select(l => l.Id) });

            foreach (var l in logs)
            {
                T entity = new T();
                foreach (var i in info.Where(x => x.LogId == l.Id))
                {
                    this.SetProperty<T>(entity, i.Column, i.Value);
                }

                yield return new Log<T> { Date = l.Date, User = l.User, Entity = entity, };
            }
        }

        private void SetProperty<T>(T entity, string property, string value)
        {
            var pi = typeof(T).GetProperties().Single(p => p.Name == property);
            var converter = TypeDescriptor.GetConverter(pi.PropertyType);

            pi.SetValue(entity, converter.ConvertFromString(value), null);
        }
    }
}
