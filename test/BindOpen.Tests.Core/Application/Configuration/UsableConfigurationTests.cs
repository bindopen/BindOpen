﻿using BindOpen.Application.Configuration;
using BindOpen.Data.Common;
using BindOpen.Data.Elements;
using BindOpen.Data.Helpers.Serialization;
using BindOpen.System.Diagnostics;
using Bogus;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace BindOpen.Tests.Core.Application.Configuration
{
    [TestFixture, Order(100)]
    public class UsableConfigurationTests
    {
        private readonly string _filePath1 = GlobalVariables.WorkingFolder + "UsableConfiguration_Main.xml";
        private readonly string _filePath20 = GlobalVariables.WorkingFolder + "UsableConfiguration_Child1.xml";
        private readonly string _filePath21 = GlobalVariables.WorkingFolder + "UsableConfiguration_Child2.xml";

        private BdoUsableConfiguration _usableConfiguration1 = null;
        private BdoUsableConfiguration _usableConfiguration20 = null;
        private BdoUsableConfiguration _usableConfiguration21 = null;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var f = new Faker();

            _usableConfiguration1 = ConfigurationFactory.CreateUsable(
                _filePath1,
                new[] { Path.GetFileName(_filePath20), Path.GetFileName(_filePath21) },
                new[]
                {
                    ElementFactory.CreateScalar("float1", DataValueType.Number, 10),
                }
            );

            _usableConfiguration20 = ConfigurationFactory.CreateUsable(
                _filePath20,
                new[]
                {
                    ElementFactory.CreateScalar("text1", DataValueType.Text, f.Lorem.Words(10)),
                    ElementFactory.CreateScalar("integer1", DataValueType.Integer, Enumerable.Range(0, 10).Select(p=>f.Random.Int(5000))),
                    ElementFactory.CreateScalar("byteArray1", DataValueType.ByteArray, Enumerable.Range(0, 100).Select(p=>f.PickRandom<byte>())),
                }
            );

            _usableConfiguration21 = ConfigurationFactory.CreateUsable(
                _filePath21,
                new[]
                {
                    ElementFactory.CreateScalar("float2", DataValueType.Number, 1.1, 1.2, 1.3)
                }
            );
        }

        [Test, Order(1)]
        public void SaveUsableConfigurationTest()
        {
            var log = new BdoLog();

            _usableConfiguration1.SaveXml(_filePath1, log);
            _usableConfiguration20.SaveXml(_filePath20, log);
            _usableConfiguration21.SaveXml(_filePath21, log);

            string xml = "";
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml();
            }
            Assert.That(!log.HasErrorsOrExceptions(), "Usable configuration saving failed. Result was '" + xml);
        }

        [Test, Order(2)]
        public void LoadUsableConfigurationTest()
        {
            var log = new BdoLog();

            if (_usableConfiguration1 == null || !File.Exists(_filePath1))
            {
                SaveUsableConfigurationTest();
            }

            _ = ConfigurationFactory.Load<BdoUsableConfiguration>(_filePath1, null, null, log);
            if (log.HasErrorsOrExceptions())
            {
                string xml = ". Result was '" + log.ToXml();
            }
            //Assert.That(!log.HasErrorsOrExceptions(), "Usable configuration loading failed. Result was '" + xml);

            //Assert.That(
            //    ((string)configuration["text1"]?[0] == "item1")
            //    && ((string)configuration["text1"]?[1] == "item2")
            //    && ((string)configuration["text1"]?[2] == "item3"), "Bad usable configuration loading");
            //Assert.That(
            //    ((int)configuration["integer1"]?[0] == 1)
            //    && ((int)configuration["integer1"]?[1] == 2)
            //    && ((int)configuration["integer1"]?[2] == 3), "Bad usable configuration loading");
            //Assert.That(
            //    ((double)configuration["float2"]?[0] == 1.1)
            //    && ((double)configuration["float2"]?[1] == 1.2)
            //    && ((double)configuration["float2"]?[2] == 1.3), "Bad usable configuration loading");

            //Assert.That(
            //    configuration.Count == 4, "Bad usable configuration loading");
        }
    }
}