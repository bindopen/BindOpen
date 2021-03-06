﻿using BindOpen.Application.Modules;
using BindOpen.Application.Settings;
using BindOpen.Data.Elements;
using BindOpen.Data.Items;
using BindOpen.Data.Stores;
using BindOpen.Extensions.References;
using Microsoft.Extensions.Logging;
using System;

namespace BindOpen.Application.Scopes
{
    /// <summary>
    /// The interface defines the base BDO host options.
    /// </summary>
    public interface IBdoHostOptions : IDataItem
    {
        // Modules ----------------------

        /// <summary>
        /// The application module.
        /// </summary>
        IAppModule AppModule { get; }

        // Paths ----------------------

        /// <summary>
        /// The root folder path.
        /// </summary>
        string RootFolderPath { get; }

        /// <summary>
        /// The settings file path.
        /// </summary>
        string HostConfigFilePath { get; }

        /// <summary>
        /// Indicates whether the host settings file must exist.
        /// </summary>
        /// <remarks>If it does not exist then an exception is thrown.</remarks>
        bool? IsHostConfigFileRequired { get; }

        // Settings ----------------------

        /// <summary>
        /// The host settings.
        /// </summary>
        IBdoHostSettings HostSettings { get; }

        /// <summary>
        /// The application settings.
        /// </summary>
        IBdoAppSettings AppSettings { get; }

        /// <summary>
        /// The set of settings specifications of this instance.
        /// </summary>
        IDataElementSpecSet AppSettingsSpecificationSet { get; }

        // Extensions ----------------------

        /// <summary>
        /// The extension to load.
        /// </summary>
        IBdoExtensionReferenceCollection ExtensionReferences { get; }

        /// <summary>
        /// The extension load options.
        /// </summary>
        IExtensionLoadOptions ExtensionLoadOptions { get; }

        // Loggers ----------------------

        /// <summary>
        /// The statup logger.
        /// </summary>
        ILogger StartUpLogger { get; }

        /// <summary>
        /// The logger.
        /// </summary>
        Func<IBdoHost, ILogger> LoggerInit { get; }

        // Settings ----------------------

        /// <summary>
        /// Get the settings as the specified host settings class.
        /// </summary>
        /// <typeparam name="T">The host settings class to consider.</typeparam>
        T GetSettings<T>() where T : class, IBdoAppSettings;

        // Depots ----------------------

        /// <summary>
        /// The depot sets of this instance.
        /// </summary>
        IBdoDataStore DataStore { get; }
    }
}