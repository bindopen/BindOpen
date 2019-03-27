﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using BindOpen.Framework.Core.Application.Scopes;
using BindOpen.Framework.Core.Data.Elements.Sets;
using BindOpen.Framework.Core.Data.Helpers.Objects;
using BindOpen.Framework.Core.Data.Items;
using BindOpen.Framework.Core.Data.Items.Dictionary;
using BindOpen.Framework.Core.Extensions.Configuration.Tasks;
using BindOpen.Framework.Core.System.Diagnostics.Events;
using BindOpen.Framework.Core.System.Diagnostics.Loggers;
using BindOpen.Framework.Core.System.Processing;

namespace BindOpen.Framework.Core.System.Diagnostics
{
    /// <summary>
    /// This class represents a logger of tasks.
    /// </summary>
    [Serializable()]
    [XmlType("Log", Namespace = "http://meltingsoft.com/bindopen/xsd")]
    [XmlRoot(ElementName = "log", Namespace = "http://meltingsoft.com/bindopen/xsd", IsNullable = false)]
    public class Log : DescribedDataItem
    {
        // ------------------------------------------
        // VARIABLES
        // ------------------------------------------

        #region Variables

        // Execution ----------------------------------

        private TaskConfiguration _task = null;

        #endregion

        // ------------------------------------------
        // PROPERTIES
        // ------------------------------------------

        #region Properties

        // Execution ----------------------------------

        /// <summary>
        /// Execution of this instance.
        /// </summary>
        [XmlElement("execution")]
        public ProcessExecution Execution { get; set; } = null;

        /// <summary>
        /// Specification of the Execution property of this instance.
        /// </summary>
        [XmlIgnore()]
        public bool ExecutionSpecified
        {
            get
            {
                return this.Execution != null;
            }
        }

        // Task ----------------------------------

        /// <summary>
        /// Logged by this instance. By default, a new task is initialized when this instance is initialized.
        /// </summary>
        [XmlElement("task")]
        public TaskConfiguration Task
        {
            get
            {
                //if (this._Task == null) this._Task = new Task();
                return this._task;
            }
            set
            {
                this.WriteLog(this._task = value, LoggerMode.Auto);
            }
        }

        /// <summary>
        /// Specification of the Task property of this instance.
        /// </summary>
        [XmlIgnore()]
        public bool TaskSpecified
        {
            get
            {
                return this._task != null;
            }
        }

        /// <summary>
        /// Function that filters event.
        /// </summary>
        [XmlIgnore()]
        public Predicate<LogEvent> SubLogEventPredicate
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

        /// <summary>
        /// Specification of the Detail property of this instance.
        /// </summary>
        [XmlIgnore()]
        public bool DetailSpecified
        {
            get
            {
                return Detail != null && this.Detail.Elements.Count > 0;
            }
        }

        // Events ----------------------------------

        /// <summary>
        /// The event with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        [XmlIgnore()]
        public Event this[String id] => id == null ? null : Events?.Find(p => p.Id.KeyEquals(id));

        /// <summary>
        /// The event with the specified ID.
        /// </summary>
        /// <param name="index"></param>
        [XmlIgnore()]
        public Event this[int index] => Events?.Cast<object>().ToArray().GetObjectAtIndex(index) as Event;

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
        public List<LogEvent> Events { get; set; } = null;

        /// <summary>
        /// Specification of the Events property of this instance.
        /// </summary>
        [XmlIgnore()]
        public bool EventsSpecified
        {
            get
            {
                return Events != null && this.Events.Count > 0;
            }
        }

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
        public List<LogEvent> Errors
        {
            get
            {
                return this.Events == null ? new List<LogEvent>() : this.Events.Where(p => p.Kind == EventKind.Error).ToList();
            }
        }

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
        public List<LogEvent> Warnings
        {
            get
            {
                return this.Events == null ? new List<LogEvent>() : this.Events.Where(p => p.Kind == EventKind.Warning).ToList();
            }
        }

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
        public List<LogEvent> Messages
        {
            get
            {
                return this.Events == null ? new List<LogEvent>() : this.Events.Where(p => p.Kind == EventKind.Message).ToList();
            }
        }

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
        public List<LogEvent> Exceptions
        {
            get
            {
                return this.Events == null ? new List<LogEvent>() : this.Events.Where(p => p.Kind == EventKind.Exception).ToList();
            }
        }

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
        public List<LogEvent> Checkpoints
        {
            get
            {
                return this.Events == null ? new List<LogEvent>() : this.Events.Where(p => p.Kind == EventKind.Checkpoint).ToList();
            }
        }

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
        public List<Log> SubLogs
        {
            get
            {
                return this.Events == null ? new List<Log>() : this.Events.Where(p => p.Log != null).Select(p => p.Log).ToList();
            }
        }

        // Tree ----------------------------------

        /// <summary>
        /// Parent of this instance.
        /// </summary>
        [XmlIgnore()]
        public Log Parent { get; set; } = null;

        /// <summary>
        /// Parent of this instance.
        /// </summary>
        [XmlIgnore()]
        public LogEvent Event { get; set; } = null;

        /// <summary>
        /// Root of this instance.
        /// </summary>
        [XmlIgnore()]
        public Log Root
        {
            get { return this.GetRoot(); }
        }

        /// <summary>
        /// Specification of the Task property of this instance.
        /// </summary>
        [XmlIgnore()]
        public int Level
        {
            get
            {
                return this.Parent == null ? 0 : this.Parent.Level + 1;
            }
        }

        // Logger ----------------------------------

        /// <summary>
        /// Loggers of this instance.
        /// </summary>
        [XmlIgnore()]
        public List<Logger> Loggers { get; set; } = null;

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        public Log() : base(null, "log_")
        {
        }

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        /// <param name="loggers">The loggers to consider.</param>
        public Log(
            params Logger[] loggers) : this(null, loggers)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        /// <param name="eventFinder">The function that filters event.</param>
        /// <param name="loggers">The loggers to consider.</param>
        public Log(
            Predicate<LogEvent> eventFinder = null,
            params Logger[] loggers) : this()
        {
            this.SubLogEventPredicate = eventFinder;
            this.Loggers = loggers.Where(p => p != null).ToList();
            foreach (Logger logger in this.Loggers)
                logger.SetLog(this);
        }

        /// <summary>
        /// Instantiates a new instance of the Log class.
        /// </summary>
        /// <param name="task">The task to consider.</param>
        /// <param name="eventFinder">The function that filters event.</param>
        /// <param name="loggers">The loggers to consider.</param>
        public Log(
            TaskConfiguration task,
            Predicate<LogEvent> eventFinder = null,
            params Logger[] loggers)
            : this(eventFinder, loggers)
        {
            this._task = task;
        }

        /// <summary>
        /// Instantiates a new instance of the Log class specifying parent.
        /// </summary>
        /// <param name="parentLog">The parent logger to consider.</param>
        /// <param name="task">The task to consider.</param>
        /// <param name="eventFinder">The function that filters event.</param>
        public Log(
            Log parentLog,
            TaskConfiguration task = null,
            Predicate<LogEvent> eventFinder = null)
            : this(eventFinder, (parentLog != null ? parentLog.Loggers.ToArray() : new Logger[0]))
        {
            this._task = task;
            if (parentLog != null)
                this.Parent = parentLog;
        }

        #endregion

        // ------------------------------------------
        // MUTATORS
        // ------------------------------------------

        #region Mutators

        // Logging ----------------------------------------

        /// <summary>
        /// Adds the specified loggers.
        /// </summary>
        /// <param name="loggers">The loggers to add.</param>
        public void AddLoggers(params Logger[] loggers)
        {
            if (loggers != null)
            {
                if (this.Loggers == null)
                    this.Loggers = new List<Logger>();

                foreach (Logger logger in loggers)
                {
                    if (logger != null)
                    {
                        this.Loggers.Add(logger);
                    }
                }
            }
        }

        /// <summary>
        /// Logs the specified task.
        /// </summary>
        /// <param name="task">The task to log.</param>
        /// <param name="mode">The mode to log.</param>
        public void WriteLog(TaskConfiguration task, LoggerMode mode = LoggerMode.Auto)
        {
            foreach (Logger logger in this.Loggers)
            {
                if ((logger.Mode != LoggerMode.Off && mode == LoggerMode.Any) || (logger.Mode == mode))
                {
                    logger.WriteTask(this, task);
                }
            }
        }

        /// <summary>
        /// Logs the specified event.
        /// </summary>
        /// <param name="logEvent">The event to log.</param>
        /// <param name="mode">The mode to log.</param>
        public void WriteLog(LogEvent logEvent, LoggerMode mode = LoggerMode.Auto)
        {
            if (this.Loggers != null)
            {
                foreach (Logger logger in this.Loggers)
                {
                    if ((logger.Mode != LoggerMode.Off && mode == LoggerMode.Any) || (logger.Mode == mode))
                    {
                        logger.WriteEvent(logEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Logs the specified element.
        /// </summary>
        /// <param name="elementName">The element name to log.</param>
        /// <param name="elementValue">The element value to log.</param>
        /// <param name="mode">The mode to log.</param>
        public void WriteLog(String elementName, Object elementValue, LoggerMode mode = LoggerMode.Auto)
        {
            foreach (Logger logger in this.Loggers)
            {
                if ((logger.Mode != LoggerMode.Off && mode == LoggerMode.Any) || (logger.Mode == mode))
                {
                    logger.WriteDetailElement(this, elementName, elementValue);
                }
            }
        }

        /// <summary>
        /// Logs the specified child log.
        /// </summary>
        /// <param name="childLog">The child log to consider.</param>
        /// <param name="mode">The mode to log.</param>
        public void WriteLog(Log childLog, LoggerMode mode = LoggerMode.Auto)
        {
            foreach (Logger logger in this.Loggers)
            {
                if ((logger.Mode != LoggerMode.Off && mode == LoggerMode.Any) || (logger.Mode == mode))
                {
                    logger.WriteChildLog(this, childLog);
                }
            }
        }

        // Events ------------------------------------

        //     /// <summary>
        //     /// Adds a new log event.
        //     /// </summary>
        //     /// <param name="logEvent">The log event to add.</param>
        //     public void AddEvent(LogEvent logEvent)
        //     {
        //if (this._Events != null)
        //    this._Events.Add(logEvent);
        //this.WriteLog(logEvent, LoggerMode.Auto);
        //     }

        /// <summary>
        /// Adds a new log event.
        /// </summary>
        /// <param name="logEvent">The log event to add.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public bool AddEvent(
            LogEvent logEvent,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            bool isAdded = false;
            if (logEvent != null)
            {
                if (logFinder == null || (childLog != null && logFinder.Invoke(childLog)))
                {
                    if (Loggers?.Any(q => q.IsHistoryRequired()) != false
                        || (SubLogEventPredicate == null || this.SubLogEventPredicate.Invoke(logEvent)))
                    {
                        if (childLog != null)
                        {
                            if (logEvent.Title == null && childLog.Title != null) logEvent.Title = childLog.Title.Clone() as DictionaryDataItem;
                            if (logEvent.Description == null && childLog.Description != null) logEvent.Description = childLog.Description.Clone() as DictionaryDataItem;
                            if (logEvent.Kind == EventKind.Any) logEvent.Kind = childLog.GetMaxEventKind();
                            childLog.Parent = this;
                            childLog.Loggers = this.Loggers;
                            logEvent.Log = childLog;
                            childLog.Event = logEvent;
                        }

                        if (this.Event != null && (this.Event.Kind == EventKind.None || this.Event.Kind == EventKind.Any))
                            this.Event.Kind = this.Event.Kind.Max(logEvent.Kind);

                        if (this.Events == null) this.Events = new List<LogEvent>();
                        this.Events.Add(logEvent);

                        isAdded = true;
                    }

                    logEvent.Parent = this;
                    this.WriteLog(logEvent, LoggerMode.Auto);
                }
            }

            return isAdded;
        }

        /// <summary>
        /// Adds the specified log event.
        /// </summary>
        /// <param name="kind">The kind of this instance.</param>
        /// <param name="title">The title of this instance.</param>
        /// <param name="description">The description of this instance.</param>
        /// <param name="criticality">The criticality of this instance.</param>
        /// <param name="resultCode">The result code of this instance.</param>
        /// <param name="source">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        /// <param name="childLog">The child log of this instance.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        public LogEvent AddEvent(
            EventKind kind,
            String title,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String source = null,
            DateTime? date = null,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            LogEvent logEvent;
            this.AddEvent(
                logEvent = new LogEvent(
                    kind,
                    title,
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
        public LogEvent AddWarning(
            String title,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String aSource = null,
            DateTime? date = null,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            return this.AddEvent(
                EventKind.Warning,
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
        public LogEvent AddError(
            String title,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String aSource = null,
            DateTime? date = null,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            return this.AddEvent(
                EventKind.Error,
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
        public LogEvent AddCheckpoint(
            String title,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String source = null,
            DateTime? date = null,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            return this.AddEvent(
                EventKind.Checkpoint,
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
        public LogEvent AddMessage(
            String title,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String source = null,
            DateTime? date = null,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            return this.AddEvent(
                EventKind.Message,
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
        public LogEvent AddException(
            String title,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String source = null,
            DateTime? date = null,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            return this.AddEvent(
                EventKind.Exception,
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
        public LogEvent AddException(
            Exception exception,
            EventCriticality criticality = EventCriticality.None,
            String resultCode = null,
            String source = null,
            Log childLog = null,
            Predicate<Log> logFinder = null)
        {
            LogEvent logEvent = null;
            this.AddEvent(
                logEvent = new LogEvent(
                    exception,
                    criticality,
                    resultCode,
                    source));
            return logEvent;
        }

        /// <summary>
        /// Adds the events of the specified log.
        /// </summary>
        /// <param name="log">The log whose task results must be added.</param>
        /// <param name="kinds">The event kinds to add.</param>
        /// <returns>Returns the added events.</returns>
        public List<LogEvent> AddEvents(
            Log log,
            params EventKind[] kinds)
        {
            return this.AddEvents(log, null, kinds);
        }

        /// <summary>
        /// Adds the events of the specified log.
        /// </summary>
        /// <param name="log">The log to consider.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        /// <param name="kinds">The event kinds to add.</param>
        /// <returns>Returns the added events.</returns>
        public List<LogEvent> AddEvents(
            Log log,
            Predicate<Log> logFinder = null,
            params EventKind[] kinds)
        {
            List<LogEvent> logEvents = new List<LogEvent>();

            if ((log != null) && (log.Events != null) && (logFinder == null || logFinder.Invoke(log)))
            {
                logEvents = log.Events.Where(p => kinds.Count() == 0 || kinds.Contains(p.Kind)).ToList();
                if (logEvents != null)
                    foreach (LogEvent currentEvent in logEvents)
                        this.AddEvent(currentEvent);
            }

            return logEvents;
        }

        /// <summary>
        /// Adds the events of the specified log.
        /// </summary>
        /// <param name="log">The log whose task results must be added.</param>
        /// <returns>Returns the added events.</returns>
        /// <remarks>This function equals to AddEventsexcept except that it does not allow to filter with log event kinds.</remarks>
        public List<LogEvent> Append(
            Log log)
        {
            return this.AddEvents(log);
        }

        /// <summary>
        /// Adds the events of the specified log.
        /// </summary>
        /// <param name="log">The log to consider.</param>
        /// <param name="logFinder">The filter function to consider. If true then the child log is added otherwise it is not.</param>
        /// <returns>Returns the added events.</returns>
        /// <remarks>This function equals to AddEventsexcept except that it does not allow to filter with log event kinds.</remarks>
        public List<LogEvent> Append(
            Log log,
            Predicate<Log> logFinder = null)
        {
            return this.AddEvents(log);
        }

        /// <summary>
        /// Clears the specified events.
        /// </summary>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <param name="kinds">The kinds to consider.</param>
        public void ClearEvents(
            bool isRecursive = true,
            params EventKind[] kinds)
        {
            if (this.Events != null)
                this.Events.RemoveAll(p => kinds.Contains(p.Kind));

            if (isRecursive)
                foreach (Log childLog in this.SubLogs)
                    this.ClearEvents(isRecursive, kinds);
        }

        /// <summary>
        /// Sanitize this instance.
        /// </summary>
        public void Sanitize()
        {
            // we clear the task check points if there is no special results in load task
            if (!this.HasErrorsOrExceptionsOrWarnings())
                this.ClearEvents(true, EventKind.Checkpoint);
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
        /// <param name="aSource">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        public LogEvent AddSubLog(
            Log childLog,
            Predicate<Log> logFinder = null,
            EventKind eventKind = EventKind.Any,
            String title = null,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String aSource = null,
            DateTime? date = null)
        {
            return this.AddEvent(
                eventKind,
                title,
                criticality,
                description,
                resultCode,
                aSource,
                date,
                childLog,
                logFinder);
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
        /// <param name="aSource">The ExtensionDataContext of this instance.</param>
        /// <param name="date">The date to consider.</param>
        public Log AddSubLog(
            Predicate<Log> filterFinder = null,
            EventKind eventKind = EventKind.Any,
            String title = null,
            EventCriticality criticality = EventCriticality.None,
            String description = null,
            String resultCode = null,
            String aSource = null,
            DateTime? date = null)
        {
            Log childLog = new Log();
            this.AddEvent(
                eventKind,
                title,
                criticality,
                description,
                resultCode,
                aSource,
                date,
                childLog,
                filterFinder);
            return childLog;
        }

        ///// <summary>
        ///// Adds a new child log.
        ///// </summary>
        ///// <returns>The added child log.</returns>
        //public Log AddLog(Task task)
        //{
        //    Log childLog = null;
        //    this.AddSubLog(childLog = new Log(task)
        //    {
        //        Parent = this,
        //        Loggers= new List<Logger>(this._Loggers)
        //    });
        //    return childLog;
        //}

        /// <summary>
        /// Removes the specified child log.
        /// </summary>
        /// <param name="childLog">The child log to remove.</param>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        public bool RemoveSubLog(Log childLog, bool isRecursive = true)
        {
            return (childLog == null ? false : this.RemoveSubLog(childLog.Id, isRecursive));
        }

        /// <summary>
        /// Removes the child log with the specified ID.
        /// </summary>
        /// <param name="id">The ID to consider.</param>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        public bool RemoveSubLog(String id, bool isRecursive = true)
        {
            bool isRemoved = false;
            if ((id != null) && (this.Events != null) && (this.Events.RemoveAll(p => p.Log != null && id.KeyEquals(id)) == 0))
                foreach (Log subLog in this.SubLogs)
                {
                    if (subLog.RemoveSubLog(id, isRecursive))
                    {
                        isRemoved = true;
                        break;
                    }
                }

            return isRemoved;
        }

        // Loggers ---------------------------------------------

        /// <summary>
        /// Gets the logger with the specified name.
        /// </summary>
        /// <param name="name">The name of the logger to consider.</param>
        /// <returns>Returns the logger with the specified name.</returns>
        public Logger GetLogger(String name = null)
        {
            if (name == null)
                return (this.Loggers.Count > 0 ? this.Loggers[0] : null);
            else
                return this.Loggers.Find(p => p.KeyEquals(name));
        }

        /// <summary>
        /// Gets the logger with the specified format.
        /// </summary>
        /// <param name="format">The name of the format to consider.</param>
        /// <returns>Returns the logger with the specified format.</returns>
        public Logger GetLogger(LogFormat format)
        {
            return this.Loggers.Find(p => p.Format == format);
        }

        /// <summary>
        /// Gets the loggers with the specified formats.
        /// </summary>
        /// <param name="formats">The log formats to consider.</param>
        /// <returns>The loggers with the specified formats.</returns>
        public List<Logger> GetLoggers(params LogFormat[] formats)
        {
            return this.Loggers.Where(p => formats.Contains(p.Format)).ToList();
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors       

        /// <summary>
        /// Returns the title label.
        /// </summary>
        /// <param name="variantName">The variant variant name to consider.</param>
        /// <param name="defaultVariantName">The default variant name to consider.</param>
        public override String GetTitleText(String variantName = "*", String defaultVariantName = "*")
        {
            if (this.Title == null && this.Task != null)
                return this.Task.GetTitleText(variantName, defaultVariantName);
            else
                return base.GetTitleText(variantName, defaultVariantName);
        }

        /// <summary>
        /// Returns the description label.
        /// </summary>
        /// <param name="variantName">The variant variant name to consider.</param>
        /// <param name="defaultVariantName">The default variant name to consider.</param>
        public override String GetDescriptionText(String variantName = "*", String defaultVariantName = "*")
        {
            if (this.Description == null && this.Task != null)
                return this.Task.GetDescriptionText(variantName, defaultVariantName);
            else
                return base.GetDescriptionText(variantName, defaultVariantName);
        }

        // Events --------------------------------

        /// <summary>
        /// Returns the event of this instance with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the event to return.</param>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <returns>The event of this instance with the specified ID.</returns>
        public Event GetEventWithId(String id, bool isRecursive = false)
        {
            if (id == null || this.Events == null) return null;

            Event logEvent = this.Events.First(p => p.Id.KeyEquals(id));
            if (isRecursive)
                foreach (Log childLog in this.SubLogs)
                {
                    logEvent = childLog.GetEventWithId(id, true);
                    if (logEvent != null) return logEvent;
                }

            return logEvent;
        }

        /// <summary>
        /// Gets the specified events of this instance.
        /// </summary>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <param name="kinds">The kinds to consider.</param>
        /// <returns>Returns the specified events of this instance.</returns>
        public List<LogEvent> GetEvents(
            bool isRecursive = false,
            params EventKind[] kinds)
        {
            if (this.Events == null) return new List<LogEvent>();

            List<LogEvent> events = this.Events.GetEvents(kinds);

            if (isRecursive)
                foreach (Log childLog in this.SubLogs)
                    events.AddRange(childLog.GetEvents(isRecursive, kinds));

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
            params EventKind[] kinds)
        {
            if (this.Events == null) return 0;

            int i = this.Events.Count(p => kinds.Contains(p.Kind));

            if (isRecursive)
                foreach (Log childLog in this.SubLogs)
                    i += this.GetEventCount(isRecursive, kinds);

            return i;
        }

        /// <summary>
        /// Gets the warnings, errors or exceptions of this instance.
        /// </summary>
        /// <param name="isRecursive">Indicate whether the search is recursive.</param>
        /// <param name="kinds">The kinds to consider.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public EventKind GetMaxEventKind(
            bool isRecursive = true,
            params EventKind[] kinds)
        {
            return this.GetEvents(isRecursive, kinds).Select(p => p.Kind).ToList().Max();
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
            params EventKind[] kinds)
        {
            if (this.Events == null) return false;

            bool aHasEvents = this.Events.Has(kinds);

            if (!aHasEvents && isRecursive)
                foreach (Log childLog in this.SubLogs)
                    if (aHasEvents = childLog.HasEvent(isRecursive, kinds))
                        return true;

            return aHasEvents;
        }

        /// <summary>
        /// Indicates whether this instance has the specified events.
        /// </summary>
        /// <param name="kinds">The event kinds to consider.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasEvent(
            params EventKind[] kinds)
        {
            return this.HasEvent(false, kinds);
        }

        /// <summary>
        /// Checks this instance has any warnings.
        /// </summary>
        /// <param name="kinds">The event kinds to consider.</param>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasWarnings(
            bool isRecursive = true,
            params EventKind[] kinds)
        {
            return this.HasEvent(isRecursive, EventKind.Warning);
        }

        /// <summary>
        /// Checks this instance has any errors.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasErrors(bool isRecursive = true)
        {
            return this.HasEvent(isRecursive, EventKind.Error);
        }

        /// <summary>
        /// Checks this instance has any exceptions.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasExceptions(bool isRecursive = true)
        {
            return this.HasEvent(isRecursive, EventKind.Exception);
        }

        /// <summary>
        /// Checks this instance has any messages.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasMessages(bool isRecursive = true)
        {
            return this.HasEvent(isRecursive, EventKind.Message);
        }

        /// <summary>
        /// Checks this instance has any errors or exceptions.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasErrorsOrExceptions(bool isRecursive = true)
        {
            return this.HasEvent(isRecursive, EventKind.Error, EventKind.Exception);
        }

        /// <summary>
        /// Checks this instance has any warnings, errors or exceptions.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>True if this instance has the specified events. False otherwise.</returns>
        public bool HasErrorsOrExceptionsOrWarnings(bool isRecursive = true)
        {
            return this.HasEvent(isRecursive, EventKind.Warning, EventKind.Error, EventKind.Exception);
        }

        // Tree --------------------------------

        /// <summary>
        /// Returns the log root.
        /// </summary>
        /// <returns>The log root.</returns>
        private Log GetRoot()
        {
            return (this.Parent == null ? this : this.Parent.GetRoot());
        }

        /// <summary>
        /// Returns the sub log with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the log to return.</param>
        /// <param name="isRecursive">Indicates whether the search must be recursive.</param>
        /// <returns>The child with the specified ID.</returns>
        public Log GetSubLogWithId(String id, bool isRecursive = false)
        {
            if (this.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return this;
            Log log = null;
            if (isRecursive)
            {
                foreach (Log currentChildLog in this.SubLogs)
                {
                    log = currentChildLog.GetSubLogWithId(id);
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
            return this.SubLogs.Count > 0;
        }

        /// <summary>
        /// Builds the tree of the specified log.
        /// </summary>
        /// <param name="log"></param>
        private static void BuildTree(Log log)
        {
            if (log != null)
                foreach (LogEvent aEvent in log.Events)
                {
                    aEvent.Parent = log;
                    if (aEvent.Log != null)
                    {
                        aEvent.Log.Parent = log;
                        Log.BuildTree(aEvent.Log);
                    }
                }
        }

        #endregion

        // ------------------------------------------
        // MUTATORS
        // ------------------------------------------

        #region Mutators

        /// <summary>
        /// Sets the log file location.
        /// </summary>
        /// <param name="newFolderPath">The new folder path to consider.</param>
        /// <param name="isFileToBeMoved">Indicates whether the file must be moved.</param>
        /// <param name="newFileName">The new file name to consider.</param>
        public void SetFilePath(String newFolderPath, bool isFileToBeMoved, String newFileName = null)
        {
            foreach (Logger logger in this.Loggers)
                logger.SetFilePath(newFolderPath, isFileToBeMoved, newFileName);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.Execution = this.Execution ?? new ProcessExecution();
            this.Execution.Start();
        }

        /// <summary>
        /// Ends this instance specifying the status.
        /// </summary>
        /// <param name="status">The new status to consider.</param>
        public void End(ProcessExecutionStatus status = ProcessExecutionStatus.Completed)
        {
            this.Execution = this.Execution ?? new ProcessExecution();
            this.Execution.End(status);
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
        public override void UpdateStorageInfo(Log log = null)
        {
            base.UpdateStorageInfo(log);

            if (this.Detail != null)
                this.Detail.UpdateStorageInfo(log);
            if (this.Execution != null)
                this.Execution.UpdateStorageInfo(log);
            if (this.Events != null)
                foreach (Event currentEvent in this.Events)
                    currentEvent.UpdateStorageInfo(log);
        }

        /// <summary>
        /// Updates information for runtime.
        /// </summary>
        /// <param name="appScope">The application scope to consider.</param>
        /// <param name="log">The log to update.</param>
        public override void UpdateRuntimeInfo(IAppScope appScope = null, Log log = null)
        {
            base.UpdateRuntimeInfo(appScope, log);

            this.Detail?.UpdateRuntimeInfo(appScope, log);

            this.Execution?.UpdateRuntimeInfo(appScope, log);
            if (this.Events != null)
            {
                foreach (Event currentEvent in this.Events)
                {
                    currentEvent.UpdateRuntimeInfo(appScope, log);
                }
            }
        }

        // Unserialization ---------------------------------

        /// <summary>
        /// Instantiates a new instance of Log class from a xml file.
        /// </summary>
        /// <param name="filePath">The path of the Xml file to load.</param>
        /// <param name="appScope">The application scope to consider.</param>
        /// <param name="isCheckXml">Indicates whether the file must be checked.</param>
        /// <param name="loadLog">The output log of the load task.</param>
        /// <param name="mustFileExist">Indicates whether the file must exist.</param>
        /// <returns>The load log.</returns>
        public static Log Load<T>(
            String filePath,
            AppScope appScope,
            bool isCheckXml,
            Log loadLog,
            bool mustFileExist = true) where T : Logger, new()
        {
            Log log = (new T()).LoadLog(filePath, loadLog, appScope, mustFileExist);
            if (log != null)
                Log.BuildTree(log);
            return log;
        }

        /// <summary>
        /// Instantiates a new instance of Log class from a xml string.
        /// </summary>
        /// <param name="xmlString">The Xml string to load.</param>
        /// <param name="appScope">The application scope to consider.</param>
        /// <param name="isCheckXml">Indicates whether the file must be checked.</param>
        /// <param name="loadLog">The output log of the load task.</param>
        /// <returns>The log defined in the Xml file.</returns>
        public static Log LoadFromString<T>(
            String xmlString,
            AppScope appScope,
            bool isCheckXml,
            Log loadLog = null) where T : Logger, new()
        {
            Log log = (new T()).LoadLogFromString(xmlString, loadLog, appScope);
            if (log != null)
                Log.BuildTree(log);
            return log;
        }

        // Serialization ---------------------------------

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns>Returns the saving log.</returns>
        public bool Save()
        {
            bool abool = true;

            if (this.Loggers.Count == 0)
                return false;
            else
                foreach (Logger logger in this.Loggers)
                    abool &= logger.Save(this, logger.Filepath);

            return abool;
        }

        /// <summary>
        /// Saves this instance in the specified log file.
        /// </summary>
        /// <param name="logFilePath">The path of the log file to save.</param>
        /// <returns>Returns the saving log.</returns>
        public bool Save<T>(String logFilePath)
            where T : Logger, new()
        {
            return (new T()).Save(this, logFilePath);
        }

        /// <summary>
        /// Gets the xml string of this instance.
        /// </summary>
        /// <returns>The Xml string of this instance.</returns>
        public String ToString<T>()
             where T : Logger, new()
        {
            return (new T()).ToString(this);
        }

        #endregion
    }
}