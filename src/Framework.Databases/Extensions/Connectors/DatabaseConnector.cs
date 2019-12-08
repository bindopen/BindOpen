﻿using BindOpen.Framework.Core.Data.Elements.Sets;
using BindOpen.Framework.Core.Data.Items.Dictionary;
using BindOpen.Framework.Core.Extensions.Attributes;
using BindOpen.Framework.Core.Extensions.Runtime.Items;
using BindOpen.Framework.Core.System.Diagnostics;
using BindOpen.Framework.Core.System.Scripting;
using BindOpen.Framework.Databases.Data.Queries;
using BindOpen.Framework.Databases.Data.Queries.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace BindOpen.Framework.Databases.Extensions.Connectors
{
    /// <summary>
    /// This class defines a database connector.
    /// </summary>
    public abstract class DatabaseConnector : BdoConnector
    {
        // -----------------------------------------------
        // PROPERTIES
        // -----------------------------------------------

        #region Properties

        /// <summary>
        /// The provider of this instance.
        /// </summary>
        [XmlIgnore()]
        public DbQueryBuilder QueryBuilder { get; set; } = null;

        /// <summary>
        /// The provider of this instance.
        /// </summary>
        [DetailProperty(Name = "provider")]
        public string Provider
        {
            get;
            set;
        }

        /// <summary>
        /// The server address of this instance.
        /// </summary>
        [DetailProperty(Name = "serverAddress")]
        public string ServerAddress
        {
            get;
            set;
        }

        /// <summary>
        /// The initial catalog of this instance.
        /// </summary>
        [DetailProperty(Name = "initialCatalog")]
        public string InitialCatalog
        {
            get;
            set;
        }

        /// <summary>
        /// The integrated security of this instance.
        /// </summary>
        [DetailProperty(Name = "integratedSecurity")]
        public string IntegratedSecurity
        {
            get;
            set;
        }

        /// <summary>
        /// The user name of this instance.
        /// </summary>
        [DetailProperty(Name = "userName")]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// The password of this instance.
        /// </summary>
        [DetailProperty(Name = "password")]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// The database kind of this instance.
        /// </summary>
        [DetailProperty(Name = "databaseKind")]
        public DatabaseConnectorKind DatabaseConnectorKind
        {
            get;
            set;
        }

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the DatabaseConnector class.
        /// </summary>
        protected DatabaseConnector() : base()
        {
        }

        /// <summary>
        /// Instantiates a new instance of the DatabaseConnector class.
        /// </summary>
        /// <param name="name">The name of this instance.</param>
        /// <param name="connectionString">The connection string to consider.</param>
        protected DatabaseConnector(
            string name, string connectionString = null) : base(name, connectionString)
        {
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors

        /// <summary>
        /// Gets the SQL text of the specified query.
        /// </summary>
        /// <param name="query">The query to consider.</param>
        /// <param name="scriptVariableSet">The script variable set to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>Returns the SQL text of the specified query.</returns>
        public string GetSqlText(
            IDbQuery query,
            IBdoScriptVariableSet scriptVariableSet = null,
            IBdoLog log = null) => GetSqlText(query, null, scriptVariableSet, log);

        /// <summary>
        /// Gets the SQL text of the specified query.
        /// </summary>
        /// <param name="query">The query to consider.</param>
        /// <param name="parameterSet">The parameter set to consider.</param>
        /// <param name="scriptVariableSet">The script variable set to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>Returns the SQL text of the specified query.</returns>
        public string GetSqlText(
            IDbQuery query,
            DataElementSet parameterSet,
            IBdoScriptVariableSet scriptVariableSet = null,
            IBdoLog log = null)
        {
            string sqlText = "";

            if (QueryBuilder == null)
                log.AddError("Data builder missing");
            else
                log.Append(QueryBuilder.BuildQuery(query, parameterSet, scriptVariableSet, out sqlText));

            return sqlText;
        }

        /// <summary>
        /// Updates the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to consider.</param>
        /// <returns>Returns a clone of this instance.</returns>
        public override void UpdateConnectionString(string connectionString = null)
        {
            base.UpdateConnectionString(connectionString);
            DictionaryDataItem item = DictionaryDataItem.Create(connectionString);

            Provider = item.GetContent("Provider").Trim().ToLower();
            DatabaseConnectorKind = GuessDatabaseConnectorKind();
            ServerAddress = item.GetContent("Data Source");
            InitialCatalog = item.GetContent("Initial Catalog");
            UserName = item.GetContent("User Id");
            Password = item.GetContent("Password");
        }

        #endregion

        // ------------------------------------------
        // DATABASE MANAGEMENT
        // ------------------------------------------

        #region Database Management

        /// <summary>
        /// Gets the .NET database connection of this instance.
        /// </summary>
        /// <returns>Returns the connection of this instance.</returns>
        public virtual IDbConnection GetDotNetDbConnection()
        {
            return null;
        }

        // ------------------------------------------------

        /// <summary>
        /// Gets the database kind from the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The database provider  of the specified connection string.</returns>
        public static DatabaseConnectorKind GuessDatabaseConnectorKind(string connectionString)
        {
            connectionString = connectionString.Trim();

            if (connectionString.IndexOf("SQLOLEDB", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DatabaseConnectorKind.MSSqlServer;
            }
            else if (connectionString.IndexOf("MSDASQL", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DatabaseConnectorKind.MySQL;
            }
            else if (connectionString.IndexOf("MSDAORA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DatabaseConnectorKind.Oracle;
            }
            //else if ((connectionString.ToUpper().Contains("MICROSOFT.JET.OLEDB.4.0")) &
            //    (connectionString.Contains("EXTENDED PROPERTIES=\"EXCEL")))
            //    databaseKind = ConnectorKind_database.Excel;
            //else if ((connectionString.ToUpper().Contains("MICROSOFT.JET.OLEDB.4.0")) &
            //    (connectionString.Contains("EXTENDED PROPERTIES=\"TEXT")))
            //    databaseKind = ConnectorKind_database.TextFiles;
            else if (connectionString.IndexOf("POSTGRESQL", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DatabaseConnectorKind.PostgreSql;
            }

            return DatabaseConnectorKind.Any;
        }

        /// <summary>
        /// Guesses the database kind of this instance.
        /// </summary>
        /// <returns>The database kind of this instance.</returns>
        public DatabaseConnectorKind GuessDatabaseConnectorKind()
        {
            return DatabaseConnector.GuessDatabaseConnectorKind(ConnectionString);
        }

        // Execution non query  ---------------------------------------

        /// <summary>
        /// Executes a database data query that returns nothing.
        /// </summary>
        /// <param name="queryText">The text to execute.</param>
        /// <param name="scriptVariableSet">The interpretation variables to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>The log of the data query execution task.</returns>
        public virtual void ExecuteNonQuery(
            string queryText,
            IBdoScriptVariableSet scriptVariableSet = null,
            IBdoLog log = null)
        {
        }

        // Execution query data reader  ---------------------------------------

        /// <summary>
        /// Executes a database data query putting the result into a data reader.
        /// </summary>
        /// <param name="queryText">The text to execute.</param>
        /// <param name="dataReader">The output data reader.</param>
        /// <param name="scriptVariableSet">The interpretation variables to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>The log of the data query execution task.</returns>
        public virtual void ExecuteQuery(
            string queryText,
            ref IDataReader dataReader,
            IBdoScriptVariableSet scriptVariableSet = null,
            IBdoLog log = null)
        {
        }

        // Execution query dataset  ---------------------------------------

        /// <summary>
        /// Executes a database data query putting the result into a dataset.
        /// </summary>
        /// <param name="queryText">The text to execute.</param>
        /// <param name="dataSet">The output dataset.</param>
        /// <param name="scriptVariableSet">The interpretation variables to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>The log of the data query execution task.</returns>
        public virtual void ExecuteQuery(
            string queryText,
            ref DataSet dataSet,
            IBdoScriptVariableSet scriptVariableSet = null,
            IBdoLog log = null)
        {
        }

        // Table ---------------------------------------

        /// <summary>
        /// Gets the identity of the last inserted item
        /// </summary>
        /// <param name="id">The long identifier to populate.</param>
        /// <param name="log">The log to consider.</param>
        public virtual void GetIdentity(
            ref long id,
            IBdoLog log = null)
        {
        }

        /// <summary>
        /// Executes the specified data query and updates the specified data table.
        /// </summary>
        /// <param name="queryText">The text to execute.</param>
        /// <param name="dataTable">The data table to update.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>The log of the task.</returns>
        public virtual void UpdateDataTable(
            string queryText,
            DataTable dataTable,
            IBdoLog log = null)
        {
        }

        /// <summary>
        /// Executes the specified data query and updates the specified data table.
        /// </summary>
        /// <param name="queryText">The text to execute.</param>
        /// <param name="dataSet">The data set to update.</param>
        /// <param name="tableNames">The table names to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns>The log of the task.</returns>
        public virtual void UpdateDataSet(
            string queryText,
            DataSet dataSet,
            List<string> tableNames,
            IBdoLog log = null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connector"></param>
        public void SetConnector(IBdoConnector connector)
        {
        }

        #endregion
    }
}
