﻿using BindOpen.Data.Queries;
using BindOpen.Extensions.Carriers;

namespace BindOpen.Data.Models
{
    public interface IBdoDbModelBuilder
    {
        IBdoDbModelBuilder AddJoinCondition(string name, DbQueryJoinCondition condition);

        IBdoDbModelBuilder AddJoinCondition(DbQueryJoinCondition condition);

        IBdoDbModelBuilder AddTable(string name, DbTable table);

        IBdoDbModelBuilder AddTable(DbTable table);

        IBdoDbModelBuilder AddTable<T>() where T : class;

        IBdoDbModelBuilder AddTuple(string name, DbField[] fields);

        IBdoDbModelBuilder AddQuery(string name, IDbQuery query);

        IBdoDbModelBuilder AddQuery(IDbQuery query);
    }
}