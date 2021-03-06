﻿using BindOpen.Data.Expression;
using BindOpen.Data.Helpers.Serialization;
using BindOpen.Extensions.Runtime;
using BindOpen.System.Diagnostics;
using BindOpen.System.Scripting;
using BindOpen.Tests.Core.Fakers;
using NUnit.Framework;
using System;

namespace BindOpen.Tests.Core.System.Diagnostics
{
    [TestFixture, Order(401)]
    public class ScriptInterpreterTests
    {
        private readonly string _script1 = "$isEqual('MYTABLE', $text('MYTABLE'))";
        private readonly string _interpretedScript1 = "true";
        private readonly string _script2 = "$func1('abc', 'efg').func2('ijk')";
        private readonly string _interpretedScript2 = "false:ijk";
        private readonly string _script3 = "$isEqual($(constant), 'const')";
        private readonly string _interpretedScript3 = "true";
        private readonly string _script4 = "abc{{$isEqual($(constant), 'const')}}defg";
        private readonly string _interpretedScript4 = "abctruedefg";
        private readonly string _interpretedScript5 = "true";
        private readonly string _script6 = "{{$func3($isEqual($(constant), 'const'), $isEqual($(constant), 'const'), 'toto', 'titi')}}";
        private readonly string _interpretedScript6 = "true-true-toto-titi";

        private readonly BdoScriptword _scriptword1 =
            BdoScript.Function("isEqual", "MYTABLE", BdoScript.Function("text", "mytable"));

        private readonly BdoScriptword _scriptword5 =
            BdoScript.Function("isEqual", "mypath", BdoScript.Function("text", new CarrierFake("mypath")));

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test, Order(1)]
        public void LoadInterpreterTest()
        {
            var interpreter = GlobalVariables.Scope.Interpreter;
            Assert.That(interpreter.GetDefinitions().Count > 0, "Bad interpreter loading");
        }

        [Test, Order(11)]
        public void InterpreteScriptNullTest()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<bool?>(null as DataExpression, scriptVariableSet, log)?.ToString();

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(resultScript == null, "Bad script interpretation" + xml);
        }

        [Test, Order(101)]
        public void InterpreteWord1Test()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<bool?>(_scriptword1, scriptVariableSet, log)?.ToString();

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_interpretedScript1.Equals(resultScript, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }

        [Test, Order(102)]
        public void WordToStringTest()
        {
            var log = new BdoLog();

            var script = _scriptword1?.ToString();

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_script1.Equals(script, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }

        [Test, Order(201)]
        public void InterpreteScript1Test()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<bool?>(_script1, DataExpressionKind.Script, scriptVariableSet, log)?.ToString();

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_interpretedScript1.Equals(resultScript, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }

        [Test, Order(202)]
        public void InterpreteScript2Test()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<string>(_script2, DataExpressionKind.Script, scriptVariableSet, log);

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_interpretedScript2.Equals(resultScript, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }

        [Test, Order(203)]
        public void InterpreteScript3Test()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<bool?>(_script3, DataExpressionKind.Script, scriptVariableSet, log)?.ToString();

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_interpretedScript3.Equals(resultScript, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }

        [Test, Order(204)]
        public void InterpreteScript4Test()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<string>(_script4, DataExpressionKind.Auto, scriptVariableSet, log);

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_interpretedScript4.Equals(resultScript, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }

        [Test, Order(205)]
        public void InterpreteScript5Test()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<bool?>(_scriptword5.CreateExp(), scriptVariableSet, log)?.ToString();

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_interpretedScript5.Equals(resultScript, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }

        [Test, Order(206)]
        public void InterpreteScript6Test()
        {
            var log = new BdoLog();

            var scriptVariableSet = BdoScript.CreateVariableSet();
            var resultScript = GlobalVariables.Scope.Interpreter.Evaluate<string>(
                _script6, default, scriptVariableSet, log)?.ToString();

            string xml = string.Empty;
            if (log.HasErrorsOrExceptions())
            {
                xml = ". Result was '" + log.ToXml() + "'";
            }
            Assert.That(_interpretedScript6.Equals(resultScript, StringComparison.OrdinalIgnoreCase), "Bad script interpretation" + xml);
        }
    }
}
