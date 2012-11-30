using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace HistoryLog
{
    internal abstract class DataAccess
    {
        private static Configuration historyLogConfiguration = ((Configuration)ConfigurationManager.GetSection("historyLogConfiguration"));
        private static DbProviderFactory factory = DbProviderFactories.GetFactory(historyLogConfiguration.providerName);
        private static DbConnection dbConnection;

        public static DbConnection DbConnection
        {
            get { return DataAccess.dbConnection; }
            set { DataAccess.dbConnection = value; }
        }

        private static DbConnection CreateConnection()
        {
            if (dbConnection == null)
            {
                var conn = factory.CreateConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings[historyLogConfiguration.connectionStringName].ConnectionString;
                conn.Open();
                return conn;
            }
            else
                return dbConnection;
                
        }

        private static DbCommand CreateCommand(string commandText, DbConnection conn)
        {
            var comm = factory.CreateCommand();
            comm.CommandText = commandText;
            comm.CommandType = CommandType.Text;
            comm.Connection = conn;
            return comm;
        }

        private static DbDataAdapter CreateDataAdapter(DbCommand comm)
        {
            var adap = factory.CreateDataAdapter();
            adap.SelectCommand = comm;
            return adap;
        }

        internal static Model.EntityDataTable GetEntity(string application, string entity)
        {
            Model ds = new Model();
            using (var conn = CreateConnection())
            {
                var comm = CreateCommand("select HIST_ENTITY.* from HIST_APPLICATION inner join HIST_ENTITY on HIST_APPLICATION.APPLICATIONID = HIST_ENTITY.APPLICATIONID where HIST_APPLICATION.NAME= :parameter1 and HIST_ENTITY.NAME = :parameter2", conn);
                CreateParameter(comm, DbType.String, ":parameter1", application);
                CreateParameter(comm, DbType.String, ":parameter2", entity);
                CreateDataAdapter(comm).Fill(ds, "Entity");
                conn.Close();
            }
            return ds.Entity;
        }


        internal static Model.EntityPropertyDataTable GetProperties(int entityID)
        {
            Model ds = new Model();
            using (var conn = CreateConnection())
            {
                var comm = CreateCommand("select * from HIST_PROPERTY where ENTITYID= :parameter1", conn);
                CreateParameter(comm, DbType.Int32, ":parameter1", entityID);
                CreateDataAdapter(comm).Fill(ds, "EntityProperty");
                conn.Close();
            }
            return ds.EntityProperty;
        }

        internal static Model.EntityInstanceDataTable GetInstance(string tableName, int entityID, string keyInfo)
        {
            Model ds = new Model();
            using (var conn = CreateConnection())
            {
                var comm = CreateCommand(String.Format("select * from {0} where ENTITYID= :parameter1 and KEYINFO = :parameter2", tableName.ToUpper()), conn);
                CreateParameter(comm, DbType.Int32, ":parameter1", entityID);
                CreateParameter(comm, DbType.String, ":parameter2", keyInfo);
                CreateDataAdapter(comm).Fill(ds, "EntityInstance");
                conn.Close();
            }
            return ds.EntityInstance;
        }

        internal static Model.EntityInstanceDataTable AddInstance(string tableName, int entityID, string keyInfo)
        {
            using (var conn = CreateConnection())
            {
                var comm = conn.CreateCommand();
                comm.CommandText = String.Format("insert into {0} (ENTITYINSTANCEID,ENTITYID,KEYINFO) values ({0}_ID_SEQ.NEXTVAL,:parameter1,:parameter2)", tableName.ToUpper());
                CreateParameter(comm, DbType.Int32, ":parameter1", entityID);
                CreateParameter(comm, DbType.String, ":parameter2", keyInfo);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            return GetInstance(tableName, entityID, keyInfo);
        }

        
        internal static void AddHistory(string historyTableName, int entityInstanceID, LogAction action, string userName, string dataInfo)
        {
            using (var conn = CreateConnection())
            {
                var comm = conn.CreateCommand();
                comm.CommandText = String.Format("insert into {0} (ENTITYINSTANCEID,DATEMODIFIED,ACTION,USERNAME, DATAINFO) values (:parameter1,SYSDATE,:parameter2,:parameter3,:parameter4)", historyTableName.ToUpper());
                CreateParameter(comm, DbType.Int32, ":parameter1", entityInstanceID);
                CreateParameter(comm, DbType.Int16, ":parameter2", (byte)action);
                CreateParameter(comm, DbType.String, ":parameter3", userName);
                CreateParameter(comm, DbType.String, ":parameter4", dataInfo);

                comm.ExecuteNonQuery();
                conn.Close();
            }
        }


        internal static Model.HistoryDataTable GetHistory(string historyTableName, int entityInstanceID)
        {
            Model ds = new Model();
            using (var conn = CreateConnection())
            {
                var comm = CreateCommand(String.Format("select * from {0} where ENTITYINSTANCEID= :parameter1 order by DATEMODIFIED", historyTableName.ToUpper()), conn);
                CreateParameter(comm, DbType.Int32, ":parameter1", entityInstanceID);
                CreateDataAdapter(comm).Fill(ds, "History");
                conn.Close();
            }
            return ds.History;
        }

        private static void CreateParameter(DbCommand comm, DbType dbType, string parameterName, object value)
        {
            var parameter = comm.CreateParameter();
            parameter.DbType = dbType;
            parameter.ParameterName = parameterName;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = value;
            comm.Parameters.Add(parameter);
        }
        
    }
}
