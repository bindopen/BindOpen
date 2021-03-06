﻿using System;
using System.Xml.Serialization;
using BindOpen.Framework.Core.Application.Scopes.Interfaces;
using BindOpen.Framework.Core.Data.Common;
using BindOpen.Framework.Core.Data.Elements.Interfaces;
using BindOpen.Framework.Core.Extensions.Configuration.Tasks;
using BindOpen.Framework.Core.System.Diagnostics.Interfaces;
using BindOpen.Framework.Core.System.Scripting.Interfaces;

namespace BindOpen.Framework.Core.Application.Commands
{
    /// <summary>
    /// This class represents a command.
    /// </summary>
    [Serializable()]
    [XmlType("Command", Namespace = "http://meltingsoft.com/bindopen/xsd")]
    [XmlRoot(ElementName = "command", Namespace = "http://meltingsoft.com/bindopen/xsd", IsNullable = false)]
    [XmlInclude(typeof(ShellCommand))]
    [XmlInclude(typeof(ReferenceCommand))]
    [XmlInclude(typeof(ScriptCommand))]
    public abstract class Command : TaskConfiguration
    {
        // --------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------

        #region Properties

        /// <summary>
        /// The kind of this instance.
        /// </summary>
        [XmlElement("kind")]
        public CommandKind Kind { get; set; } = CommandKind.None;

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the Command class.
        /// </summary>
        protected Command()
            : this(CommandKind.None)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the Command class.
        /// </summary>
        /// <param name="kind">The kidn of command to consider.</param>
        /// <param name="name">The name of this instance.</param>
        /// <param name="items">The items to consider.</param>
        protected Command(
            CommandKind kind,
            string name = null,
            params IDataElement[] items)
            : base(name, items)
        {
            this.Kind = kind;
        }

        #endregion

        //------------------------------------------
        // EXECUTION
        //-----------------------------------------

        #region Execution

        /// <summary>
        /// Executes this instance with result.
        /// </summary>
        /// <param name="resultString">The result to get.</param>
        /// <param name="appScope">The application scope to consider.</param>
        /// <param name="scriptVariableSet">The script variable set to use.</param>
        /// <param name="runtimeMode">The runtime mode to consider.</param>
        /// <returns>The log of execution log.</returns>
        public abstract ILog ExecuteWithResult(
            out string resultString,
            IAppScope appScope = null,
            IScriptVariableSet scriptVariableSet = null,
            RuntimeMode runtimeMode = RuntimeMode.Normal);

        #endregion
    }
}
