﻿using BindOpen.Application.Scopes;
using BindOpen.Data.Elements;
using BindOpen.Data.Helpers.Serialization;
using BindOpen.Extensions.Runtime;
using BindOpen.System.Diagnostics;
using BindOpen.Tests.Core.Fakers;
using NUnit.Framework;
using System.IO;

namespace BindOpen.Tests.Core.Data.Elements
{
    [TestFixture]
    public class CarrierElementSetTest
    {
        private readonly string _filePath = GlobalVariables.WorkingFolder + "CarrierElementSet.xml";

        private ICarrierElement _carrierElement1 = null;
        private ICarrierElement _carrierElement2 = null;
        private ICarrierElement _carrierElement3 = null;
        private ICarrierElement _carrierElement4 = null;

        private IDataElementSet _carrierElementSetA = null;

        [SetUp]
        public void OneTimeSetUp()
        {
            var log = new BdoLog();

            _carrierElement1 = ElementFactory.CreateCarrier(
                "carrier1", "tests.core$dbField",
                new BdoCarrierConfiguration(
                    ElementFactory.CreateScalar("path", "file1.txt")));

            _carrierElement2 = ElementFactory.CreateCarrier(
                "carrier2", "tests.core$dbField",
                ElementFactory.CreateSetFromObject<BdoCarrierConfiguration>(new { path = "file2.txt" }));

            _carrierElement3 = new CarrierFake("file3.txt", "myfolder")?.AsElement();

            _carrierElement4 = GlobalVariables.Scope.CreateCarrier(
                new BdoCarrierConfiguration(
                    "tests.core$dbField",
                    ElementFactory.CreateSetFromObject(new { path = "file4.txt" })?.ToArray()),
                "carrier4", log)?.AsElement();

            _carrierElementSetA = ElementFactory.CreateSet(_carrierElement1, _carrierElement2, _carrierElement3, _carrierElement4);
        }

        [Test]
        public void CreateCarrierElementSetTest()
        {
            Assert.That(
                (_carrierElement1?.Item()?.GetValue<string>("path") == "file1.txt")
                && (_carrierElement2?.Item()?.GetValue<string>("path") == "file2.txt")
                && (_carrierElement3?.Item()?.GetValue<string>("path") == "file3.txt")
                && (_carrierElement4?.Item()?.GetValue<string>("path") == "file4.txt")
                , "Bad carrier element creation");

            Assert.That(
                _carrierElementSetA.Get<CarrierElement>("carrier1")?.Item().GetValue<string>("path") == "file1.txt"
                , "Bad carrier element set indexation");

            Assert.That(
                _carrierElementSetA?.Count == 4, "Bad carrier element set creation");
        }

        [Test]
        public void UpdateCheckRepairTest()
        {
            var log = new BdoLog();

            //test update
            //log = _scalarElementSetB.Update(_scalarElementSetA);

            //Assert.That(log.Errors.Count == itemsNumber, "Bad insertion of errors ({0} expected; {1} found)", itemsNumber, log.Errors.Count);
            //Assert.That(log.Exceptions.Count == itemsNumber, "Bad insertion of exceptions ({0} expected; {1} found)", itemsNumber, log.Exceptions.Count);
            //Assert.That(log.Messages.Count == itemsNumber, "Bad insertion of messages ({0} expected; {1} found)", itemsNumber, log.Messages.Count);
            //Assert.That(log.Warnings.Count == itemsNumber, "Bad insertion of warnings ({0} expected; {1} found)", itemsNumber, log.Warnings.Count);
            //Assert.That(log.SubLogs.Count == itemsNumber, "Bad insertion of sub logs ({0} expected; {1} found)", itemsNumber, log.SubLogs.Count);
        }

        [Test]
        public void SaveDataElementSetTest()
        {
            var log = new BdoLog();

            _carrierElementSetA.SaveXml(_filePath, log);

            string xml = "";
            if (log.HasErrorsOrExceptions())
            {
                xml = log.ToXml();
            }
            Assert.That(!log.HasErrorsOrExceptions(), "Element set saving failed. Result was '" + xml);
        }

        [Test]
        public void LoadDataElementSetTest()
        {
            var log = new BdoLog();

            if (_carrierElementSetA == null || !File.Exists(_filePath))
                SaveDataElementSetTest();

            var elementSet = XmlHelper.Load<DataElementSet>(_filePath, log: log);

            string xml = "";
            if (log.HasErrorsOrExceptions())
            {
                xml = log.ToXml();
            }
            Assert.That(!log.HasErrorsOrExceptions(), "Element set loading failed. Result was '" + xml);
        }
    }
}
