﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using BindOpen.Framework.Core.Application.Scopes;
using BindOpen.Framework.Core.Data.Common;
using BindOpen.Framework.Core.Data.Elements.Sets;
using BindOpen.Framework.Core.Data.Helpers.Objects;
using BindOpen.Framework.Core.Data.Helpers.Strings;
using BindOpen.Framework.Core.Data.Items.Source;
using BindOpen.Framework.Core.Extensions;
using BindOpen.Framework.Core.System.Diagnostics;
using BindOpen.Framework.Core.System.Diagnostics.Loggers;
using BindOpen.Framework.Core.System.Processing;
using BindOpen.Framework.Runtime.Application.Hosts.Options;
using BindOpen.Framework.Runtime.Application.Security;
using BindOpen.Framework.Runtime.Application.Services;
using BindOpen.Framework.Runtime.Application.Settings;

namespace BindOpen.Framework.Runtime.Application.Hosts
{
    /// <summary>
    /// This class represents an application host.
    /// </summary>
    public class BdoAppHost : BdoAppService, IBdoAppHost
    {
        // ------------------------------------------
        // VARIABLES
        // ------------------------------------------

        #region Variables

        private AppHostOptions _options = new AppHostOptions();

        #endregion

        // ------------------------------------------
        // PROPERTIES
        // ------------------------------------------

        #region Properties

        /// <summary>
        /// The options of this instance.
        /// </summary>
        public AppHostOptions Options => _options;

        /// <summary>
        /// The settings of this instance.
        /// </summary>
        public AppSettings Settings => Options?.Settings;

        /// <summary>
        /// The set of user settings of this instance.
        /// </summary>
        public DataElementSet UserSettingsSet { get; set; } = new DataElementSet();

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the BdoAppHost class.
        /// </summary>
        public BdoAppHost() : base()
        {
            // we initiate the options
            this.Options.SetAppFolder(Directory.GetCurrentDirectory());

            this.Options.SetExtensions(
                new AppExtensionConfiguration(
                    new AppExtensionFilter("BindOpen.Framework.Runtime"),
                    new AppExtensionFilter("BindOpen.Framework.Standard")));
        }

        #endregion

        // ------------------------------------------
        // PROCESSING
        // ------------------------------------------

        #region Processing

        /// <summary>
        /// Configures the application host.
        /// </summary>
        /// <param name="setupOptions">The action to setup the application host.</param>
        /// <returns>Returns the application host.</returns>
        public IBdoAppHost Configure(Action<IAppHostOptions> setupOptions)
        {
            setupOptions?.Invoke(this.Options);

            return this;
        }

        /// <summary>
        /// Saves settings.
        /// </summary>
        public void SaveSettings()
        {
            String filePath = this.GetKnownPath(ApplicationPathKind.SettingsFolder) + "appsettings.xml";
            if ((this.UserSettingsSet != null) && (!String.IsNullOrEmpty(filePath)))
                this.UserSettingsSet.SaveXml(filePath);
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors

        // Credentials -----------------------------

        /// <summary>
        /// Get the credential with the specified name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public ApplicationCredential GetCredential(String name)
        {
            ApplicationCredential credential = new ApplicationCredential
            {
                Name = "[unkwnon]"
            };
            return this.Options?.Settings?.Credentials.Find(p => p.KeyEquals(name));
        }

        // Paths --------------------------------------

        /// <summary>
        /// Returns the path of the application temporary folder.
        /// </summary>
        /// <param name="pathKind">The kind of paths.</param>
        /// <returns>The path of the application temporary folder.</returns>
        public virtual String GetKnownPath(ApplicationPathKind pathKind)
        {
            String path = null;
            switch (pathKind)
            {
                case ApplicationPathKind.AppFolder:
                    path = this.Options?.AppFolderPath;
                    break;
                case ApplicationPathKind.DefaultLogFolder:
                    path = this.GetKnownPath(ApplicationPathKind.RuntimeFolder) + @"logs\";
                    break;
                case ApplicationPathKind.ExtensionsFolder:
                    path = this.Options?.Settings?.ExtensionsFolderPath;
                    if (String.IsNullOrEmpty(path))
                        path = this.GetKnownPath(ApplicationPathKind.RuntimeFolder) + @"extensions\";
                    break;
                case ApplicationPathKind.LibraryFolder:
                    path = this.Options?.LibraryFolderPath;
                    if (String.IsNullOrEmpty(path))
                        path = this.GetKnownPath(ApplicationPathKind.RuntimeFolder) + @"lib\";
                    break;
                case ApplicationPathKind.LogFolder:
                    path = this.Options?.Settings?.LogFolderPath;
                    if (String.IsNullOrEmpty(path))
                        path = this.GetKnownPath(ApplicationPathKind.DefaultLogFolder);
                    break;
                case ApplicationPathKind.RoamingFolder:
                    path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).GetEndedString(@"\");
                    break;
                case ApplicationPathKind.RuntimeFolder:
                    path = this.Options?.RuntimeFolderPath;
                    if (String.IsNullOrEmpty(path))
                        path = this.Options?.Settings?.RuntimeFolderPath;
                    break;
                case ApplicationPathKind.SettingsFile:
                    path = this.Options?.SettingsFilePath;
                    if (String.IsNullOrEmpty(path))
                        path = this.GetDefaultSettingsFilePath();
                    break;
                case ApplicationPathKind.SettingsFolder:
                    path = this.GetKnownPath(ApplicationPathKind.RuntimeFolder) + @"settings\";
                    break;
                case ApplicationPathKind.TemporaryFolder:
                    path = this.GetKnownPath(ApplicationPathKind.RuntimeFolder) + @"temp\";
                    break;
                case ApplicationPathKind.UsersFolder:
                    path = this.GetKnownPath(ApplicationPathKind.RuntimeFolder) + @"users\";
                    break;
            }

            return (String.IsNullOrEmpty(path) ? StringHelper.__NoneString : path).ToPath();
        }

        private string GetDefaultSettingsFilePath()
        {
            // by default, settings file is {{runtime folder}}\settings\appsettings.xml
            string defaultSettingsFilePath = string.IsNullOrEmpty(this.Options?.RuntimeFolderPath) ? null : this.Options?.RuntimeFolderPath + @"settings\".ToPath() + AppHostOptions._DEFAULTSETTINGSFILENAME;

            if (!File.Exists(defaultSettingsFilePath))
            {
                // by default, settings file is {{runtime folder}}\appsettings.xml
                defaultSettingsFilePath = string.IsNullOrEmpty(this.Options?.RuntimeFolderPath) ? null : this.Options?.RuntimeFolderPath + AppHostOptions._DEFAULTSETTINGSFILENAME;
                if (!File.Exists(defaultSettingsFilePath))
                {
                    // then {{application folder}}\app_data\appsettings.xml
                    defaultSettingsFilePath = string.IsNullOrEmpty(this.Options?.AppFolderPath) ? null : this.Options?.AppFolderPath + @"app_data\".ToPath() + AppHostOptions._DEFAULTSETTINGSFILENAME;
                    if (!File.Exists(defaultSettingsFilePath))
                    {
                        // then {{application folder}}\appsettings.xml
                        defaultSettingsFilePath = this.Options?.AppFolderPath + AppHostOptions._DEFAULTSETTINGSFILENAME;
                    }
                }
            }

            return defaultSettingsFilePath;
        }

        #endregion

        // ------------------------------------------
        // PROCESSING
        // ------------------------------------------

        #region Processing

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <returns>Returns true if this instance is started.</returns>
        public new virtual IBdoAppHost Start(Log log = null)
        {
            return base.Start(log) as BdoAppHost;
        }

        /// <summary>
        /// Indicates the application ends.
        /// </summary>
        /// <param name="executionStatus">The execution status to consider.</param>
        public new virtual IBdoAppHost End(ProcessExecutionStatus executionStatus = ProcessExecutionStatus.Stopped)
        {
            return base.End(executionStatus) as BdoAppHost;
        }

        #endregion

        // ------------------------------------------
        // LOAD MANAGEMENT
        // ------------------------------------------

        #region Load Management

        /// <summary>
        /// Initializes information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns the log of the task.</returns>
        protected override Log Initialize<T>()
        {
            // we initialize logging
            if (this.Options?.IsDefaultLoggerUsed == true)
            {
                this.Log.AddLoggers(LoggerFactory.Create<SnapLogger>(
                    Logger.__DefaultName, LoggerMode.Auto, DataSourceKind.Repository, false, null,
                    this.GetKnownPath(ApplicationPathKind.DefaultLogFolder), "log_" + this.Id + ".txt"));
            }
            this.Log.AddLoggers(this.Options?.Loggers);

            // we initialize as scoped service
            base.Initialize<T>();

            Log log = new Log();

            // we load extensions
            log.AddMessage("Loading extensions...");

            // we load the extensions
            this.AppScope.AppExtension.Clear();
            if (string.IsNullOrEmpty(this.Options?.ExtensionConfiguration.DefaultFolderPath))
            {
                this.Options.ExtensionConfiguration.DefaultFolderPath = this.GetKnownPath( ApplicationPathKind.LibraryFolder);
            }
            log.Append(
                this.AppScope.AppExtension.LoadLibrary(this.Options?.ExtensionConfiguration),
                p => p.HasErrorsOrExceptionsOrWarnings());
            this.AppScope.Update<RuntimeAppScope>();

            try
            {
                // if no errors was found
                if (!log.HasErrorsOrExceptions())
                {
                    log.AddMessage("Extensions loaded");

                    // we load the application settings
                    log.AddMessage("Loading application settings...");

                    String settingsFilePath = this.GetKnownPath(ApplicationPathKind.SettingsFile);

                    if (string.IsNullOrEmpty(settingsFilePath))
                        settingsFilePath = this.GetDefaultSettingsFilePath();

                    AppSettings appSettings = this.LoadSettings(settingsFilePath, log, this._appScope);

                    if (log.HasErrorsOrExceptions())
                    {
                        log.AddError("Could not load application settings");
                    }
                    else
                    {
                        log.AddMessage("Application settings loaded");

                        if (this.Options?.Settings!=null)
                        {
                            this.Options.Settings.AppScope = this._appScope;
                        }
                        this.Options?.Settings?.Update(appSettings);
                        this.Options?.Settings?.Update(new DataElementSpecificationSet(
                            this.Options?.SettingsSpecificationSet?.Items?
                                .Where(p =>
                                    p.SpecificationLevels.Has(SpecificationLevel.Definition)
                                    || p.SpecificationLevels.Has(SpecificationLevel.Configuration)).ToArray()),
                            null,
                            new List<UpdateMode> { UpdateMode.Incremental_UpdateCommonItems });

                        if (this.Options?.Settings!=null && String.IsNullOrEmpty(this.Options.Settings.ApplicationInstanceName))
                        {
                            this.Options.Settings.ApplicationInstanceName = _ApplicationInstanceName;
                        }

                        // we update the log folder path

                        this.Log.SetFilePath(this.GetKnownPath(ApplicationPathKind.LogFolder), true);

                        //// we load configuration

                        //log.AddMessage("Loading configuration...");

                        //DataElementSet settingsSet = DataItem.Load<DataElementSet>(
                        //    this.GetPath(ApplicationPathKind.SettingsFile), log, null, null, false);
                        //settingsSet?.Update(new DataElementSpecificationSet(
                        //        this.SettingsSpecificationSet.Items
                        //                .Where(p => p.SpecificationLevels.Has(SpecificationLevel.Runtime)).ToArray()),
                        //        null,
                        //        new List<UpdateMode> { UpdateMode.Incremental_UpdateCommonItems });

                        // we build the data module manager
                        this.DataSourceService.Clear();
                        if (this.AppExtension != null)
                        {
                            foreach (DataSource dataSource in this.Options?.Settings?.DataSources)
                            {
                                this.DataSourceService.AddSource(dataSource);
                            }
                        }

                        if (!log.HasErrorsOrExceptions())
                            log.AddMessage("Application settings loaded");
                    }
                }

                // if at this end, neither exceptions nor errors occured then
                if ((this.GetType() == typeof(BdoAppHost)) && (!log.HasErrorsOrExceptions()))
                {
                    // we indicate that the configuration has been well loaded
                    log.AddMessage("Application configuration loaded");
                }
            }
            catch (Exception ex)
            {
                log.AddException(ex);
            }
            finally
            {
            }

            this._isLoadCompleted = !log.HasErrorsOrExceptions();
            if (this.GetType() == typeof(BdoAppHost))
                this.LoadComplete();

            return log;
        }

        /// <summary>
        /// Loads the application configuration of this instance.
        /// </summary>
        /// <param name="filePath">The file path to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <param name="appScope">The application scope to consider.</param>
        /// <param name="xmlSchemaSet">The XML schema set to consider for checking.</param>
        /// <returns>Returns the loading log.</returns>
        public virtual AppSettings LoadSettings(
            String filePath, Log log, IAppScope appScope = null, XmlSchemaSet xmlSchemaSet = null)
        {
            return AppSettings.Load<AppSettings>(filePath, log, appScope, xmlSchemaSet);
        }

        #endregion

        // ------------------------------------------
        // MISCELLANEOUS
        // ------------------------------------------

        #region Miscellaneous

        /// <summary>
        /// Gets the name of the application instance.
        /// </summary>
        /// <returns>Returns the name of the application instance.</returns>
        public static String _ApplicationInstanceName => (Environment.MachineName ?? "").ToUpper();

        #endregion
    }
}