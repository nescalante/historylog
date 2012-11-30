using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Xml;

namespace HistoryLog
{
    public enum LogAction
    {
        Add = 1,
        Update = 2,
        Delete = 3,
        Activate = 4,
        Deactivate = 5
    }

    public class LogManager
    {
        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="dbconnection">The dbconnection.</param>
        public static void SetConnection(IDbConnection dbconnection)
        {
            DataAccess.DbConnection = (DbConnection)dbconnection;
        }

        /// <summary>
        /// Log of add an object 
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="userName">Name of the user.</param>
        public static void LogAdd(string application, object instance, string userName)
        {
            Log(application, instance, LogAction.Add, userName);
        }

        /// <summary>
        /// Log of update an object 
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="userName">Name of the user.</param>
        public static void LogUpdate(string application, object instance, string userName)
        {
            Log(application, instance, LogAction.Update, userName);
        }

        /// <summary>
        /// Logs the activate.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="userName">Name of the user.</param>
        public static void LogActivate(string application, object instance, string userName)
        {
            LogActivate(application, instance, userName, false);
        }

        /// <summary>
        /// Logs the deactivate.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="userName">Name of the user.</param>
        public static void LogDeactivate(string application, object instance, string userName)
        {
            LogDeactivate(application, instance, userName, false);
        }

        /// <summary>
        /// Log of activate an object
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="logData">if set to <c>true</c> [log data].</param>
        public static void LogActivate(string application, object instance, string userName, bool logData)
        {
            Log(application, instance, LogAction.Activate, userName, logData);
        }

        /// <summary>
        /// Log of deactivate an object
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="logData">if set to <c>true</c> [log data].</param>
        public static void LogDeactivate(string application, object instance, string userName, bool logData)
        {
            Log(application, instance, LogAction.Deactivate, userName, logData);
        }



        /// <summary>
        /// Log of delete an object
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="userName">Name of the user.</param>
        public static void LogDelete(string application, object instance, string userName)
        {
            Log(application, instance, LogAction.Delete, userName);
        }

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="tableNamePrefix">The table name prefix.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <param name="keyInfo">The key info.</param>
        /// <param name="dtProperties">The dt properties.</param>
        /// <returns></returns>
        private static Model.HistoryDataTable GetHistory(string application,
            string tableNamePrefix, int entityID, string keyInfo,
            Model.EntityPropertyDataTable dtProperties)
        {
            var dtHistory = new Model.HistoryDataTable();

            var dtInstance = DataAccess.GetInstance(
                GetInstanceTableName(application, tableNamePrefix),
                entityID, keyInfo);

            if (dtInstance.Count > 0)
            {
                dtHistory = DataAccess.GetHistory(
                    GetHistoryTableName(application, tableNamePrefix),
                    dtInstance[0].EntityInstanceID);

                var columns = dtProperties.Where(p => !p.IsKey).ToList();
                columns.ForEach(p => dtHistory.Columns.Add(p.Name));
                var xml = new XmlDocument();
                foreach (var dr in dtHistory)
                {
                    if (!String.IsNullOrEmpty(dr.DataInfo))
                    {
                        xml.LoadXml(dr.DataInfo);
                        foreach (var column in columns)
                        {
                            var item = xml.GetElementsByTagName("data")[0].Attributes.GetNamedItem(column.Name);
                            if (item != null)
                                dr[column.Name] = item.Value.ToString();
                        }
                    }
                }
            }
            return dtHistory;
        }


        /// <summary>
        /// Gets the history for an object 
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="typeName">Name of the object type .</param>
        /// <param name="keys">The object keys in sequential order.</param>
        /// <returns></returns>
        public static Model.HistoryDataTable GetHistory(string application, string typeName, params object[] keys)
        {
            var dtHistory = new Model.HistoryDataTable();
            var dtEntities = DataAccess.GetEntity(application, typeName);
            if (dtEntities.Count > 0)
            {
                var dtProperties = DataAccess.GetProperties(dtEntities[0].EntityID);

                dtHistory = GetHistory(application, 
                    dtEntities[0].TableNamePrefix, 
                    dtEntities[0].EntityID,
                    GetDataXml(keys, true, dtProperties),
                    dtProperties );
            }
            return dtHistory;
        }

        /// <summary>
        /// Get history for an instance 
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Model.HistoryDataTable GetHistory(string application, object instance)
        {
            var dtHistory = new Model.HistoryDataTable();
            var dtEntities = DataAccess.GetEntity(application, instance.GetType().Name);
            if (dtEntities.Count > 0)
            {
                var dtProperties = DataAccess.GetProperties(dtEntities[0].EntityID);

                dtHistory = GetHistory(application,
                    dtEntities[0].TableNamePrefix,
                    dtEntities[0].EntityID,
                    GetDataXml(instance, true, dtProperties),
                    dtProperties);
            }
            return dtHistory;
        }

        private static void Log(string application, object instance, LogAction action, string userName)
        {
            Log(application, instance, action, userName, false);
        }

        /// <summary>
        /// Logs the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="action">The action.</param>
        /// <param name="userName">Name of the user.</param>
        private static void Log(string application, object instance, LogAction action, string userName, bool logData)
        {
            var dtEntities = DataAccess.GetEntity(application, instance.GetType().Name);
            if (dtEntities.Count > 0)
            {
                var dtProperties = DataAccess.GetProperties(dtEntities[0].EntityID);

                string keyInfo = GetDataXml(instance, true, dtProperties);
                var dtInstance = DataAccess.GetInstance(
                    GetInstanceTableName(application, dtEntities[0].TableNamePrefix),
                    dtEntities[0].EntityID, keyInfo);
                if (dtInstance.Count == 0)
                {
                    dtInstance = DataAccess.AddInstance(
                        GetInstanceTableName(application, dtEntities[0].TableNamePrefix),
                        dtEntities[0].EntityID, keyInfo);
                }
                string dataInfo = "";
                if (logData || action == LogAction.Add || action == LogAction.Update)
                    dataInfo = GetDataXml(instance, false, dtProperties);

                DataAccess.AddHistory(GetHistoryTableName(application, dtEntities[0].TableNamePrefix),
                    dtInstance[0].EntityInstanceID, action, userName, dataInfo);
            }
        }


        private static string GetInstanceTableName(string application, string tableNamePrefix)
        {
            return String.Format("HIST_{0}_{1}INSTANCE", application, tableNamePrefix.Trim());
        }

        private static string GetHistoryTableName(string application, string tableNamePrefix)
        {
            return String.Format("HIST_{0}_{1}HISTORY", application, tableNamePrefix.Trim());
        }

        private static string GetDataXml(object instance, bool isKey, Model.EntityPropertyDataTable dtProperties)
        {
            var xmlString = new StringBuilder("<" + (isKey ? "key" : "data"));

            foreach (var row in dtProperties.AsEnumerable().Where(dr => dr.IsKey == isKey).OrderBy(dr => dr.Sequence))
            {
                xmlString.Append(String.Format(@" {0}=""{1}""", row.Name, GetPropertyValue(instance, row.Name)));
            }

            xmlString.Append(" />");
            return xmlString.ToString();
        }

        private static string GetDataXml(object[] id, bool isKey, Model.EntityPropertyDataTable dtProperties)
        {
            var xmlString = new StringBuilder("<" + (isKey ? "key" : "data"));
            var index = 0;

            foreach (var row in dtProperties.AsEnumerable().Where(dr => dr.IsKey == isKey).OrderBy(dr => dr.Sequence))
            {
                xmlString.Append(String.Format(@" {0}=""{1}""", row.Name, id[index++]));
            }

            xmlString.Append(" />");
            return xmlString.ToString();
        }

        private static string GetPropertyValue(object instance, string name)
        {
            var value = "";
            var property = instance.GetType().GetProperty(name);

            if (property != null)
            {
                var propertyValue = property.GetValue(instance, null);
                if (propertyValue != null)
                    value = propertyValue.ToString();
            }

            return value;
        }
    }
}
