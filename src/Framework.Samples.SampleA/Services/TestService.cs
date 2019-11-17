﻿using BindOpen.Framework.Core.Data.Elements;
using BindOpen.Framework.Core.System.Diagnostics;
using BindOpen.Framework.Databases.MSSqlServer.Data.Queries.Builders;
using BindOpen.Framework.Runtime.Application.Services;
using BindOpen.Framework.Samples.SampleA.Settings;
using System;

namespace BindOpen.Framework.Samples.SampleA.Services
{
    public class TestService : THostedBotService<TestServiceSettings>
    {
        public override IBotService Start(ILog log = null)
        {
            base.Start(log);

            var element = ElementFactory.CreateScalar(Core.Data.Common.DataValueType.Boolean, false);

            Bot.Log.Append(
                new DbQueryBuilder_MSSqlServer(Bot.Scope).BuildQuery(
                    Queries.UpdateMyTable(new DbMyTable() { Name = "nameA" }, null), null, out string sql4));
            Console.WriteLine(sql4);

            Bot.Log.Append(
                new DbQueryBuilder_MSSqlServer(Bot.Scope).BuildQuery(
                    Queries.GetMyTables("", null), null, out string sql1));

            Console.WriteLine(sql1);

            Bot.Log.Append(
            new DbQueryBuilder_MSSqlServer(Bot.Scope).BuildQuery(
                Queries.GetMyTable("name", null), null, out string sql2));

            Console.WriteLine(sql2);

            Bot.Log.Append(
                new DbQueryBuilder_MSSqlServer(Bot.Scope).BuildQuery(
                    Queries.DeleteMyTable("", null), null, out string sql3));

            Console.WriteLine(sql3);

            return this;
        }
    }
}