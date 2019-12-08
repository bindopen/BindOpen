﻿using BindOpen.Framework.Databases.Data.Queries.Builders;

namespace BindOpen.Framework.Databases.PostgreSql.Data.Queries.Builders
{
    /// <summary>
    /// This class represents a builder of database query.
    /// </summary>
    internal partial class DbQueryBuilder_PostgreSql : DbQueryBuilder
    {
        // Date and time

        /// <summary>
        /// Evaluates the script word $SQLGETCURRENTDATE.
        /// </summary>
        /// <param name="parameters">The parameters to consider.</param>
        /// <returns>The interpreted string value.</returns>
        public override string GetSqlText_CurrentDate(object[] parameters)
        {
            return "now()";
        }
    }
}