﻿using BindOpen.Application.Modules;
using BindOpen.Application.Settings;
using BindOpen.Data.Elements;
using BindOpen.Data.Stores;
using BindOpen.Extensions.References;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BindOpen.Application.Scopes
{
    /// <summary>
    /// The interface defines the host options.
    /// </summary>
    public interface ITBdoHostOptions<S> : IBdoHostOptions
        where S : class, IBdoAppSettings, new()
    {
        /// <summary>
        /// The root folder path.
        /// </summary>
        List<(Predicate<ITBdoHostOptions<S>> Predicate, string RootFolderPath)> RootFolderPathDefinitions { get; }

        /// <summary>
        /// Sets the specified environment.
        /// </summary>
        /// <param name="environment">The environment to consider.</param>
        /// <returns>Returns this instance.</returns>
        ITBdoHostOptions<S> SetEnvironment(string environment);

        // Paths ----------------------

        /// <summary>
        /// Set the root folder.
        /// </summary>
        /// <param name="predicate">The condition that muse be satistfied.</param>
        /// <param name="rootFolderPath">The root folder path to consider.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetRootFolder(Predicate<ITBdoHostOptions<S>> predicate, string rootFolderPath);

        /// <summary>
        /// Set the root folder.
        /// </summary>
        /// <param name="rootFolderPath">The root folder path to consider.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetRootFolder(string rootFolderPath);

        /// <summary>
        /// Set the host settings file.
        /// </summary>
        /// <param name="filePath">The host settings file path.</param>
        /// <param name="isRequired">Indicates whether the host settings file is required.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetHostSettingsFile(string filePath, bool? isRequired = false);

        /// <summary>
        /// Set the host settings file.
        /// </summary>
        /// <param name="isRequired">Indicates whether the host settings file is required.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetHostSettingsFile(bool? isRequired);

        // Settings ----------------------

        /// <summary>
        /// The settings.
        /// </summary>
        S Settings { get; }

        /// <summary>
        /// Sets the host settings.
        /// </summary>
        /// <param name="action">The action to consider on the settings.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetHostSettings(Action<IBdoHostSettings> action);

        /// <summary>
        /// Sets the host settings applying action on it.
        /// </summary>
        /// <param name="hostSettings">The host settings to consider.</param>
        /// <param name="action">The action to apply on the settings.</param>
        ITBdoHostOptions<S> SetHostSettings(IBdoHostSettings hostSettings, Action<IBdoHostSettings> action = null);

        /// <summary>
        /// Sets the application settings.
        /// </summary>
        /// <param name="action">The action to apply on the settings.</param>
        ITBdoHostOptions<S> SetAppSettings(Action<S> action);

        /// <summary>
        /// Sets the application settings applying action on it.
        /// </summary>
        /// <param name="appSettings">The application settings to consider.</param>
        /// <param name="action">The action to apply on the settings.</param>
        ITBdoHostOptions<S> SetAppSettings(S appSettings, Action<S> action = null);

        /// <summary>
        /// Defines the specified application settings.
        /// </summary>
        /// <param name="specificationSet">The set of data element specifcations to consider.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> DefineAppSettings(IDataElementSpecSet specificationSet = null);

        // Extensions ----------------------

        /// <summary>
        /// Adds the extensions.
        /// </summary>
        /// <param name="action">The action for adding extensions.</param>
        /// <param name="loadOptionsAction">The action for loading options.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> AddExtensions(Action<IBdoExtensionReferenceCollection> action = null, Action<IExtensionLoadOptions> loadOptionsAction = null);

        // Loggers ----------------------

        /// <summary>
        /// Adds the specified logger.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to consider.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetLoggerAtStartup(ILoggerFactory loggerFactory);

        /// <summary>
        /// Adds the specified logger.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to consider.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetLogger(ILoggerFactory loggerFactory);

        /// <summary>
        /// Adds the specified logger.
        /// </summary>
        /// <param name="initLogger">The logger initialization to consider.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetLogger(Func<IBdoHost, ILogger> initLogger);

        // Modules -------------------------------------------

        /// <summary>
        /// Set the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetModule(IAppModule module);

        /// <summary>
        /// Set the module.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <returns>Returns the host option.</returns>
        ITBdoHostOptions<S> SetModule(string moduleName);

        // Trigger actions -------------------------------------------

        /// <summary>  
        /// The action that is executed when start succeeds.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        ITBdoHostOptions<S> ExecuteOnStartSuccess(Action<ITBdoHost<S>> action);

        /// <summary>
        /// The action that is executed when start fails.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        ITBdoHostOptions<S> ExecuteOnStartFailure(Action<ITBdoHost<S>> action);

        /// <summary>
        /// The action that is executed when execution succeeds.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        ITBdoHostOptions<S> ExecuteOnExecutionSuccess(Action<ITBdoHost<S>> action);

        /// <summary>
        /// The action that is executed when execution fails.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        ITBdoHostOptions<S> ExecuteOnExecutionFailure(Action<ITBdoHost<S>> action);

        /// <summary>
        /// Throws an exception when start fails.
        /// </summary>
        ITBdoHostOptions<S> ThrowExceptionOnStartFailure();

        /// <summary>
        /// Throws an exception when execution fails.
        /// </summary>
        ITBdoHostOptions<S> ThrowExceptionOnExecutionFailure();

        // Depots -------------------------------------------

        /// <summary>
        /// Adds the data store executing the specified action.
        /// </summary>
        ITBdoHostOptions<S> AddDataStore(Action<IBdoDataStore> action = null);
    }
}