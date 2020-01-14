﻿using BindOpen.Framework.Extensions.Runtime;
using System.Data;

namespace BindOpen.Framework.Data.Connections
{
    /// <summary>
    /// This interfaces represents a connection.
    /// </summary>
    public interface IBdoDbConnection : IBdoConnection, IDbConnection
    {
        /// <summary>
        /// Gets the .NET database connection of this instance.
        /// </summary>
        /// <returns>Returns the connection of this instance.</returns>
        IDbConnection Native { get; }

        /// <summary>
        /// Connector of the connection.
        /// </summary>
        new BdoDbConnector Connector { get; }
    }
}
