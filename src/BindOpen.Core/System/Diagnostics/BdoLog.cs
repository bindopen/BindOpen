﻿using BindOpen.Application.Scopes;
using BindOpen.Data.Elements;
using BindOpen.Data.Helpers.Objects;
using BindOpen.Data.Items;
using BindOpen.Extensions.Runtime;
using BindOpen.System.Diagnostics.Events;
using BindOpen.System.Processing;
using BindOpen.System.Scripting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace BindOpen.System.Diagnostics
{
    /// <summary>
    /// This class represents a logger of tasks.
    /// </summary>
    [XmlType("BdoLog", Namespace = "https://docs.bindopen.org/xsd")]
    [XmlRoot(ElementName = "log", Namespace = "https://docs.bindopen.org/xsd", IsNullable = false)]
    public class BdoLog : NamedDataItem, IBdoLog
    {
        // ------------------------------------------
        // VARIABLES
        // ------------------------------------------

        #region Variables

        private IBdoLogger _logger = null;

        private BdoTaskConfiguration _task = null;

        #endregion

        // ------------------------------------------
        // PROPERTIES
        // ------------------------------------------

        #region Properties

        // General ----------------------------------

        /// <summary>
        /// The display name of this instance.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The description of this instance.
        /// </summary>
        public string Description { get; set; }

        // Logger ----------------------------------

        /// <summary>
        /// The logger of this instance.
        /// </summary>
        public IBdoLogger Logger => _logger;

        // Execution ----------------------------------

        /// <summary>
        /// Execution of this instance.
        /// </summary>
        [XmlElement("execution")]
        public ProcessExecution Execution { get; set; } = null;

        // Task ----------------------------------

        /// <summary>
        /// Logged by this instance. By default, a new task is initialized when this instance is initialized.
        /// </summary>
        [XmlElement("task")]
        public BdoTaskConfiguration Task
        {
            get => _task;
            set => _task = value;
        }

        /// <summary>
        /// Function that filters event.
        /// </summary>
        [XmlIgnore()]
        public Predicate<IBdoLogEvent> SubLogEventPredicate
        {
            get;
            set;
        }

        // Detail ----------------------------------

        /// <summary>
        /// Detail of this instance.
        /// </summary>
        [XmlElement("detail")]
        public DataElementSet Detail { get; set; } = null;

        // Events ----------------------------------

        /// <summary>
        /// The event with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        [XmlIgnore()]
        public BdoLogEvent this[string id] => id == null ? null : Events?.Find(p => p.Id.KeyEquals(id));

        /// <summary>
        /// The event with the specified ID.
        /// </summary>
        /// <param name="index"></param>
        [XmlIgnore()]
        public BdoLogEvent this[int index] => Events?.Cast<object>().ToArray().GetObjectAtIndex(index) as BdoLogEvent;

        /// <summary>
        /// Events of this instance.
        /// </summary>
        /// <seealso cref="Errors"/>
        /// <seealso cref="Warnings"/>
        /// <seealso cref="Messages"/>
        /// <seealso cref="Exceptions"/>
        /// <seealso cref="Checkpoints"/>
        /// <seealso cref="SubLogs"/>
        [XmlArray("events")]
        [XmlArrayItem("event")]
        public List<BdoLogEvent> Events { get; set; } = null;

        /// <summary>
        /// Errors of this instance.
        /// </summary>
        /// <seealso cref="Events"/>
        /// <seealso cref="Warnings"/>
        /// <seealso cref="Messages"/>
        /// <seealso cref="Exceptions"/>
        /// <seealso cref="Checkpoints"/>
        /// <seealso cref="SubLogs"/>
        [XmlIgnore()]
        public List<IBdoLogEvent> Errors => Events == null ? new List<IBdoLogEvent>() : Events.Where(p => p.Kind == EventKinds.Error).ToList<IBdoLogEvent>();

        /// <summary>
        /// Warnings of this instance.
        /// </summary>
        /// <seealso cref="Errors"/>
        /// <seealso cref="Events"/>
        /// <seealso cref="Messages"/>
        /// <seealso cref="Exceptions"/>
        /// <seealso cref="Checkpoints"/>
        /// <seealso cref="SubLogs"/>
        [XmlIgnore()]
        public List<IBdoLogEvent> Warnings => Events == null ? new List<IBdoLogEvent>() : Events.Where(p => p.Kind == EventKinds.Warning).ToList<IBdoLogEvent>();

        /// <summary>
        /// Messages of this instance.
        /// </summary>
        /// <seealso cref="Errors"/>
        /// <seealso cref="Warnings"/>
        /// <seealso cref="Events"/>
        /// <seealso cref="Exceptions"/>
        /// <seealso cref="Checkpoints"/>
        /// <seealso cref="SubLogs"/>
        [XmlIgnore()]
        public List<IBdoLogEvent> Messages => Events == null ? new List<IBdoLogEvent>() : Events.Where(p => p.Kind == EventKinds.Message).ToList<IBdoLogEvent>();

        /// <summary>
        /// Exceptions of this instance.
        /// </summary>
        /// <seealso cref="Errors"/>
        /// <seealso cref="Warnings"/>
        /// <seealso cref="Messages"/>
        /// <seealso cref="Events"/>
        /// <seealso cref="Checkpoints"/>
        /// <seealso cref="SubLogs"/>
        [XmlIgnore()]
        public List<IBdoLogEvent> Exceptions => Events == null ? new List<IBdoLogEvent>() : Events.Where(p => p.Kind == EventKinds.Exception).ToList<IBdoLogEvent>();

        /// <summary>
        /// Checkpoints of this instance.
        /// </summary>
        /// <seealso cref="Errors"/>
        /// <seealso cref="Warnings"/>
        /// <seealso cref="Messages"/>
        /// <seealso cref="Exceptions"/>
        /// <seealso cref="Events"/>
        /// <seealso cref="SubLogs"/>
        [XmlIgnore()]
        public List<IBdoLogEvent> Checkpoints => Events == null ? new List<IBdoLogEvent>() : Events.Where(p => p.Kind == EventKinds.Checkpoint).ToList<IBdoLogEvent>();

        /// <summary>
        /// Logs of this instance.
        /// </summary>
        /// <seealso cref="Errors"/>
        /// <seealso cref="Warnings"/>
        /// <seealso cref="Messages"/>
        /// <seealso cref="Exceptions"/>
        /// <seealso cref="Events"/>
        /// <seealso cref="SubLogs"/>
        [XmlIgnore()]
        public List<IBdoLog> SubLogs => Events == null ? new List<IBdoLog>() : Events.Where(p => p.Log != null).Select(p => p.Log as BdoLog).ToList<IBdoLog>();

        // Tree ----------------------------------

        /// <summary>
        /// Parent of this instance.
        /// </summary>
        [XmlIgnore()]
        public IBdoLog Parent { get; set; } = null;

        /// <summary>
        /// Root of this instance.
        /// </summary>
        [XmlIgnore()]
        public IBdoLog Root
        {
            get { return GetRoot(); }
        }

        /// <summary>
        /// The level of this instance.
        /// </summary>
        [XmlIgnore()]
        public int Level => Parent == null ? 0 : Parent.Level + 1;

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        public BdoLog() : base(null, "log_")
        {
        }

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        /// <param name="logger">The logger to consider.</param>
        public BdoLog(IBdoLogger logger) : this(null, logger)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        /// <param name="eventFilter">The function that filters events.</param>
        /// <param name="logger">The logger to consider.</param>
        public BdoLog(
            Predicate<IBdoLogEvent> eventFilter,
            IBdoLogger logger) : this()
        {
            SubLogEventPredicate = eventFilter;
            _logger = logger;
        }

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        /// <param name="task">The task to consider.</param>
        /// <param name="eventFilter">The function that filters events.</param>
        /// <param name="logger">The logger to consider.</param>
        public BdoLog(
            IBdoTaskConfiguration task,
            Predicate<IBdoLogEvent> eventFilter = null,
            IBdoLogger logger = null)
            : this(eventFilter, logger)
        {
            _task = task as BdoTaskConfiguration;
        }

        /// <summary>
        /// Instantiates a new instance of the Log class specifying parent.
        /// </summary>
        /// <param name="parentLog">The parent logger to consider.</param>
        /// <param name="task">The task to consider.</param>
        /// <param name="eventFilter">The function that filters events.</param>
        public BdoLog(
            IBdoLog parentLog,
            IBdoTaskConfiguration task = null,
            Predicate<IBdoLogEvent> eventFilter = null)
            : this(eventFilter, parentLog?.Logger)
        {
            _task = task as BdoTaskConfiguration;
            if (parentLog != null)
            {
                Parent = parentLog as BdoLog;
            }
        }

        #endregion

        // ------------------------------------------
        // MUTATORS
        // ------------------------------------------

        #region Mutators

        // Logging ----------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public IBdoLog SetLogger(IBdoLogger logger)
        {
            _logger = logger;

            return this;
        }

        /// <summary>
        /// Sets the specified logger.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        public IBdoLog SetLogger(ILogger logger)
        {
            _logger = BdoLoggerFactory.Create<BdoSnapLoggerFormat>(logger);

            return this;
        }

        /// <summary>
        /// Sets the specified logger.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        public IBdoLog SetLogger<T>(ILogger logger)
            where T : IBdoLoggerFormat, new()
        {
            _logger = BdoLoggerFactory.Create<T>(logger);

            return this;
        }

        // Events ------------------------------------

        /// <summary>
        /// Adds a new log event.
        /// </summary>
        /// <param name="logEvent">The log event to add.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddEvent(
            IBdoLogEvent logEvent,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            IBdoLogEvent _event = null;
            if (logEvent != null)
            {
                if (logFinder == null || (childLog != null && logFinder.Invoke(childLog)))
                {
                    if (childLog != null)
                    {
                        if (logEvent.DisplayName == null && childLog.DisplayName != null) logEvent.DisplayName = childLog.DisplayName;
                        if (logEvent.Description == null && childLog.Description != null) logEvent.Description = childLog.Description;
                        if (logEvent.Kind == EventKinds.Any) logEvent.Kind = childLog.GetMaxEventKind();
                        childLog.Parent = this;
                        childLog.SetLogger(_logger);
                        logEvent.Log = childLog;
                    }

                    if (SubLogEventPredicate == null || SubLogEventPredicate.Invoke(logEvent))
                    {
                        (Events ??= new List<BdoLogEvent>()).Add(logEvent as BdoLogEvent);
                        logEvent.Parent = this;
                        Logger?.Log(logEvent);
                        _event = logEvent;
                    }
                }
            }

            return _event;
        }

        /// <summary>
        /// Adds the specified log event.
        /// </summary>
        /// <param name="kind">The kind of this instance.</param>
        /// <param name="title">The title of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="source">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddEvent(
            EventKinds kind,
            string title,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string source = null,
            DateTime? date = null,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            IBdoLogEvent logEvent;
            AddEvent(
                logEvent = new BdoLogEvent(
                    kind,
                    title ?? childLog?.DisplayName,
                    criticality,
                    description,
                    resultCode,
                    source,
                    date),
                childLog,
                logFinder);

            return logEvent;
        }

        /// <summary>
        /// Adds the specified warning.
        /// </summary>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="aSource">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddWarning(
            string title,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string aSource = null,
            DateTime? date = null,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            return AddEvent(
                EventKinds.Warning,
                title,
                criticality,
                description,
                resultCode,
                aSource,
                date);
        }

        /// <summary>
        /// Adds the specified error.
        /// </summary>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="aSource">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddError(
            string title,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string aSource = null,
            DateTime? date = null,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            return AddEvent(
                EventKinds.Error,
                title,
                criticality,
                description,
                resultCode,
                aSource,
                date);
        }

        /// <summary>
        /// Adds the specified checkpoint.
        /// </summary>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="source">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddCheckpoint(
            string title,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string source = null,
            DateTime? date = null,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            return AddEvent(
                EventKinds.Checkpoint,
                title,
                criticality,
                description,
                resultCode,
                source,
                date);
        }

        /// <summary>
        /// Adds the specified message.
        /// </summary>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="source">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddMessage(
            string title,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string source = null,
            DateTime? date = null,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            return AddEvent(
                EventKinds.Message,
                title,
                criticality,
                description,
                resultCode,
                source,
                date);
        }

        /// <summary>
        /// Adds the specified exception.
        /// </summary>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="source">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddException(
            string title,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string source = null,
            DateTime? date = null,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            return AddEvent(
                EventKinds.Exception,
                title,
                criticality,
                description,
                resultCode,
                source,
                date);
        }

        /// <summary>
        /// Adds the specified exception.
        /// </summary>
        /// <param name="exception">The exception to consider.</param>
        /// <param name="criticality">The criticality to consider.</param>
        /// <param name="resultCode">The result code to consider.</param>
        /// <param name="source">The ExtensionDataContext to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public IBdoLogEvent AddException(
            Exception exception,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string resultCode = null,
            string source = null,
            IBdoLog childLog = null,
            Predicate<IBdoLog> logFinder = null)
        {
            BdoLogEvent logEvent;

            AddEvent(logEvent = new BdoLogEvent(exception, criticality, resultCode, source));

            return logEvent;
        }

        /// <summary>
        /// Adds the specified events.
        /// </summary>
        /// <param name="eventFuncs">The functions that return events.</param>
        /// <returns>Returns the added events.</returns>
        public IBdoLog WithEvents(params Func<IBdoLog, IBdoLogEvent>[] eventFuncs)
        {
            var events = new List<IBdoLogEvent>();
            foreach (var fun in eventFuncs)
            {
                events.Add(AddEvent(fun?.Invoke(this)));
            }
            return this;
        }

        /// <summary>
        /// Adds the events of the specified log.
        /// </summary>
        /// <param name="log">The log whose task results must be added.</param>
        /// <param name="kinds">The event kinds to add.</param>
        /// <returns>Returns the added events.</returns>
        public List<IBdoLogEvent> AddEvents(
            IBdoLog log,
            params EventKinds[] kinds)
        {
            return AddEvents(log, null, kinds);
        }

        /// <summary>
        /// Adds the events of the specified log.
        /// </summary>
        /// <param name="log">The log to consider.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        /// <param name="kinds">The event kinds to add.</param>
        /// <returns>Returns the added events.</returns>
        public List<IBdoLogEvent> AddEvents(
            IBdoLog log,
            Predicate<IBdoLog> logFinder = null,
            params EventKinds[] kinds)
        {
            List<IBdoLogEvent> events = new List<IBdoLogEvent>();

            if ((log?.Events != null) && (logFinder?.Invoke(log) != false))
            {
                events = log.Events.Where(p => kinds.Length == 0 || kinds.Contains(p.Kind)).ToList<IBdoLogEvent>();
                if (events != null)
                {
                    foreach (IBdoLogEvent currentEvent in events)
                    {
                        var clonedEvent = currentEvent.Clone<BdoLogEvent>(this);
                        AddEvent(clonedEvent);
                    }
                }
            }

            return events;
        }

        /// <summary>
        /// Inserts the events of this instance into the specified log.
        /// </summary>
        /// <param name="log">The log whose task results must be added.</param>
        /// <param name="kinds">The event kinds to add.</param>
        /// <returns>Returns the added events.</returns>
        public List<IBdoLogEvent> AddEventsTo(
            IBdoLog log,
            params EventKinds[] kinds)
        {
            return AddEvents(log, null, kinds);
        }

        /// <summary>
        /// Adds the events of this instance to the specified log.
        /// </summary>
        /// <param name="log">The log to consider.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        /// <param name="kinds">The event kinds to add.</param>
        /// <returns>Returns the added events.</returns>
        public List<IBdoLogEvent> AddEventsTo(
            IBdoLog log,
            Predicate<IBdoLog> logFinder = null,
            params EventKinds[] kinds)
        {
            List<IBdoLogEvent> events = new List<IBdoLogEvent>();

            if ((log?.Events != null) && (logFinder?.Invoke(log) != false))
            {
                events = Events?.Where(p => kinds.Length == 0 || kinds.Contains(p.Kind)).ToList<IBdoLogEvent>();
                if (events != null)
                {
                    foreach (IBdoLogEvent currentEvent in events)
                    {
                        var clonedEvent = currentEvent.Clone<BdoLogEvent>(this);
                        log.AddEvent(clonedEvent);
                    }
                }
            }

            return events;
        }

        /// <summary>
        /// Clears the specified events.
        /// </summary>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <param name="kinds">The kinds to consider.</param>
        public void ClearEvents(
            bool isRecursive = true,
            params EventKinds[] kinds)
        {
            Events?.RemoveAll(p => kinds.Contains(p.Kind));

            if (isRecursive)
            {
                foreach (BdoLog childLog in SubLogs)
                {
                    childLog.ClearEvents(isRecursive, kinds);
                }
            }
        }

        /// <summary>
        /// Sanitize this instance.
        /// </summary>
        public void Sanitize()
        {
            // we clear the task check points if there is no special results in load task
            if (!HasErrorsOrExceptionsOrWarnings())
            {
                ClearEvents(true, EventKinds.Checkpoint);
            }
        }

        // Sub logs ------------------------------------

        /// <summary>
        /// Adds the specified warning.
        /// </summary>
        /// <param name="eventKind">The event kind of this instance.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="source">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        public IBdoLog AddSubLog(
            IBdoLog childLog,
            Predicate<IBdoLog> logFinder = null,
            EventKinds eventKind = EventKinds.Any,
            string title = null,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string source = null,
            DateTime? date = null)
        {
            AddEvent(
                eventKind,
                title,
                criticality,
                description,
                resultCode,
                source,
                date,
                childLog,
                logFinder);
            return childLog;
        }

        /// <summary>
        /// Adds the specified warning.
        /// </summary>
        /// <param name="eventKind">The event kind of this instance.</param>
        /// <param name="filterFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="source">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        public IBdoLog AddSubLog(
            Predicate<IBdoLog> filterFinder = null,
            EventKinds eventKind = EventKinds.Any,
            string title = null,
            BdoEventCriticality criticality = BdoEventCriticality.None,
            string description = null,
            string resultCode = null,
            string source = null,
            DateTime? date = null)
        {
            BdoLog childLog = new BdoLog();

            AddEvent(
                eventKind,
                title,
                criticality,
                description,
                resultCode,
                source,
                date,
                childLog,
                filterFinder);
            return childLog;
        }

        ///// <summary>
        ///// Adds a new child log.
        ///// </summary>
        ///// <returns>The added child log.</returns>
        //public ILog AddLog(Task task)
        //{
        //    Log childLog = null;
        //    AddSubLog(childLog = new Log(task)
        //    {
        //        Parent = this,
        //        Logger= new List<ILogger>(_Logger)
        //    });
        //    return childLog;
        //}

        /// <summary>
        /// Removes the specified child log.
        /// </summary>
        /// <param name="childLog">The child log to remove.</param>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        public bool RemoveSubLog(IBdoLog childLog, bool isRecursive = true)
        {
            return childLog == null ? false : RemoveSubLog(childLog.Id, isRecursive);
        }

        /// <summary>
        /// Removes the child log with the specified ID.
        /// </summary>
        /// <param name="id">The ID to consider.</param>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        public bool RemoveSubLog(string id, bool isRecursive = true)
        {
            bool isRemoved = false;
            if ((id != null) && (Events != null) && (Events.RemoveAll(p => p.Log != null && id.KeyEquals(id)) == 0))
                foreach (BdoLog subLog in SubLogs)
                {
                    if (subLog.RemoveSubLog(id, isRecursive))
                    {
                        isRemoved = true;
                        break;
                    }
                }

            return isRemoved;
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors       

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns a cloned instance.</returns>
        public override object Clone(params string[] areas)
        {
            return Clone(null, Array.Empty<string>());
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="parent">The log to consider.</param>
        /// <returns>Returns a cloned instance.</returns>
        public IBdoLog Clone(IBdoLog parent, params string[] areas)
        {
            var cloned = base.Clone(areas) as BdoLog;

            cloned.Parent = parent;
            cloned.Task = Task?.Clone<BdoTaskConfiguration>();
            cloned.Detail = Detail?.Clone<DataElementSet>();
            cloned.Events = Events?.Select(p => p.Clone<BdoLogEvent>(cloned)).ToList();
            cloned.Execution = Execution?.Clone<ProcessExecution>();

            return cloned;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="parent">The parent to consider.</param>
        /// <returns>Returns a cloned instance.</returns>
        public T Clone<T>(IBdoLog parent, params string[] areas) where T : class
        {
            return Clone(parent, areas) as T;
        }

        // Events --------------------------------

        /// <summary>
        /// Returns the event of this instance with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the event to return.</param>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <returns>The event of this instance with the specified ID.</returns>
        public IBdoLogEvent GetEventWithId(string id, bool isRecursive = false)
        {
            if (id == null || Events == null) return null;

            IBdoLogEvent logEvent = Events.Find(p => p.Id.KeyEquals(id));
            if (isRecursive)
            {
                foreach (BdoLog childLog in SubLogs)
                {
                    logEvent = childLog.GetEventWithId(id, true);
                    if (logEvent != null) return logEvent;
                }
            }

            return logEvent;
        }

        /// <summary>
        /// Gets the specified events of this instance.
        /// </summary>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <param name="kinds">The kinds to consider.</param>
        /// <returns>Returns the specified events of this instance.</returns>
        public List<IBdoLogEvent> GetEvents(
            bool isRecursive = false,
            params EventKinds[] kinds)
        {
            if (Events == null) return new List<IBdoLogEvent>();

            List<IBdoLogEvent> events = Events.ToList<IBdoLogEvent>().GetEvents(kinds);

            if (isRecursive)
            {
                foreach (BdoLog childLog in SubLogs)
                {
                    events.AddRange(childLog.GetEvents(isRecursive, kinds));
                }
            }

            return events;
        }

        /// <summary>
        /// Returns the number of the specified events of this instance.
        /// </summary>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <param name="kinds">The kinds to consider.</param>
        /// <returns>The number of the specified events of this instance.</returns>
        public int GetEventCount(
            bool isRecursive = false,
            params EventKinds[] kinds)
        {
            if (Events == null) return 0;

            int i = Events.Count(p => kinds.Contains(p.Kind));

            if (isRecursive)
                foreach (IBdoLog childLog in SubLogs)
                    i += GetEventCount(isRecursive, kinds);

            return i;
        }

        /// <summary>
        /// Gets the warnings, errors or exceptions of this instance.
        /// </summary>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <param name="kinds">The kinds to consider.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public EventKinds GetMaxEventKind(
            bool isRecursive = true,
            params EventKinds[] kinds)
        {
            return GetEvents(isRecursive, kinds).Select(p => p.Kind).ToList().Max();
        }

        // Has events -----------------------------------

        /// <summary>
        /// Indicates whether this instance has the specified events.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <param name="kinds">The event kinds to consider.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasEvent(
            bool isRecursive = true,
            params EventKinds[] kinds)
        {
            if (Events == null) return false;

            bool hasEvents = Events.ToList<IBdoLogEvent>().Has(kinds);

            if (!hasEvents && isRecursive && SubLogs != null)
            {
                foreach (BdoLog childLog in SubLogs)
                    if (hasEvents = childLog.HasEvent(isRecursive, kinds))
                        return true;
            }

            return hasEvents;
        }

        /// <summary>
        /// Indicates whether this instance has the specified events.
        /// </summary>
        /// <param name="kinds">The event kinds to consider.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasEvent(
            params EventKinds[] kinds)
        {
            return HasEvent(false, kinds);
        }

        /// <summary>
        /// Checks this instance has any warnings.
        /// </summary>
        /// <param name="kinds">The event kinds to consider.</param>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasWarnings(
            bool isRecursive = true,
            params EventKinds[] kinds)
        {
            return HasEvent(isRecursive, EventKinds.Warning);
        }

        /// <summary>
        /// Checks this instance has any errors.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasErrors(bool isRecursive = true)
        {
            return HasEvent(isRecursive, EventKinds.Error);
        }

        /// <summary>
        /// Checks this instance has any exceptions.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasExceptions(bool isRecursive = true)
        {
            return HasEvent(isRecursive, EventKinds.Exception);
        }

        /// <summary>
        /// Checks this instance has any messages.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasMessages(bool isRecursive = true)
        {
            return HasEvent(isRecursive, EventKinds.Message);
        }

        /// <summary>
        /// Checks this instance has any errors or exceptions.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasErrorsOrExceptions(bool isRecursive = true)
        {
            return HasEvent(isRecursive, EventKinds.Error, EventKinds.Exception);
        }

        /// <summary>
        /// Checks this instance has any warnings, errors or exceptions.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasErrorsOrExceptionsOrWarnings(bool isRecursive = true)
        {
            return HasEvent(isRecursive, EventKinds.Warning, EventKinds.Error, EventKinds.Exception);
        }

        // Tree --------------------------------

        /// <summary>
        /// Returns the log root.
        /// </summary>
        /// <returns>The log root.</returns>
        public IBdoLog GetRoot()
        {
            return Parent == null ? this : Parent.GetRoot();
        }

        /// <summary>
        /// Returns the sub log with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the log to return.</param>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>The child with the specified ID.</returns>
        public IBdoLog GetSubLogWithId(string id, bool isRecursive = false)
        {
            if (Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return this;
            if (isRecursive)
            {
                foreach (BdoLog currentChildLog in SubLogs)
                {
                    IBdoLog log = currentChildLog.GetSubLogWithId(id);
                    if (log != null) return log;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks this instance has child log.
        /// </summary>
        /// <returns>True if this instance has child log. False otherwise.</returns>
        public bool HasSubLog()
        {
            return SubLogs.Count > 0;
        }

        /// <summary>
        /// Builds the tree of this instance.
        /// </summary>
        public void BuildTree()
        {
            foreach (IBdoLogEvent aEvent in Events)
            {
                aEvent.Parent = this;
                if (aEvent.Log != null)
                {
                    aEvent.Log.Parent = this;
                    aEvent.Log.BuildTree();
                }
            }
        }

        #endregion

        // ------------------------------------------
        // MUTATORS
        // ------------------------------------------

        #region Mutators

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            Execution ??= new ProcessExecution();
            Execution.Start();
        }

        /// <summary>
        /// Ends this instance specifying the status.
        /// </summary>
        /// <param name="status">The new status to consider.</param>
        public void End(ProcessExecutionStatus status = ProcessExecutionStatus.Completed)
        {
            Execution ??= new ProcessExecution();
            Execution.End(status);
        }

        #endregion

        // ------------------------------------------
        // SERIALIZATION / UNSERIALIZATION
        // ------------------------------------------

        #region Serialization_Unserialization

        /// <summary>
        /// Updates information for storage.
        /// </summary>
        /// <param name="log">The log to update.</param>
        public override void UpdateStorageInfo(IBdoLog log = null)
        {
            base.UpdateStorageInfo(log);

            Detail?.UpdateStorageInfo(log);
            Execution?.UpdateStorageInfo(log);

            if (Events != null)
            {
                foreach (BdoEvent currentEvent in Events)
                {
                    currentEvent.UpdateStorageInfo(log);
                }
            }
        }

        /// <summary>
        /// Updates information for runtime.
        /// </summary>
        /// <param name="scope">The scope to consider.</param>
        /// <param name="scriptVariableSet">The set of script variables to consider.</param>
        /// <param name="log">The log to update.</param>
        public override void UpdateRuntimeInfo(IBdoScope scope = null, IScriptVariableSet scriptVariableSet = null, IBdoLog log = null)
        {
            Detail?.UpdateRuntimeInfo(scope, scriptVariableSet, log);

            Execution?.UpdateRuntimeInfo(scope, scriptVariableSet, log);

            if (Events != null)
            {
                foreach (BdoEvent currentEvent in Events)
                {
                    currentEvent?.UpdateRuntimeInfo(scope, scriptVariableSet, log);
                }
            }

            base.UpdateRuntimeInfo(scope, scriptVariableSet, log);
        }

        #endregion

        // ------------------------------------------
        // IDISPOSABLE METHODS
        // ------------------------------------------

        #region IDisposable_Methods

        private bool _isDisposed = false;

        /// <summary>
        /// Disposes this instance. 
        /// </summary>
        /// <param name="isDisposing">Indicates whether this instance is disposing</param>
        protected override void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _task?.Dispose();

            _isDisposed = true;

            base.Dispose(isDisposing);
        }

        #endregion
    }
}