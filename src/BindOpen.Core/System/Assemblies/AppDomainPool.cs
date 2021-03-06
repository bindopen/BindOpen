﻿using BindOpen.Data.Helpers.Objects;
using BindOpen.System.Diagnostics;
using BindOpen.System.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BindOpen.System.Assemblies
{
    /// <summary>
    /// This class represents an assembly pool.
    /// </summary>
    public class AppDomainPool
    {
        // ------------------------------------------
        // VARIABLES
        // ------------------------------------------

        #region Variables

        private readonly List<AppDomain> _appDomains = new List<AppDomain>();
        private readonly List<ResourceAllocation> _resourceAllocations = new List<ResourceAllocation>();
        private readonly Hashtable _resolveEventHandlerHashTable = new Hashtable();

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of AppDomainPool class.
        /// </summary>
        public AppDomainPool()
        {
            _appDomains = new List<AppDomain>();
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors

        /// <summary>
        /// Gets the specified application domain.
        /// </summary>
        public AppDomain GetAppDomain(string appDomainId)
        {
            if (appDomainId == null)
                return null;

            return _appDomains.FirstOrDefault(p => p.FriendlyName.KeyEquals(appDomainId));
        }

        ///// <summary>
        ///// Gets the type of the specified proxy object.
        ///// </summary>
        ///// <param name="aProxyObject">The proxy object to consider.</param>
        //public static Object GetRealObject(object aProxyObject)
        //{
        //    if (RemotingServices.IsTransparentProxy(aProxyObject))
        //        return RemotingServices.GetRealProxy(aProxyObject);
        //    else
        //        return aProxyObject;
        //}

        #endregion

        // ------------------------------------------
        // MUTATORS
        // ------------------------------------------

        #region Mutators

        /// <summary>
        /// Allocates an application domain specifying its ID and its owner.
        /// </summary>
        /// <param name="appDomainId">The ID of the application domain to consider.</param>
        /// <param name="ownerId">The ID of the owner.</param>
        public AppDomain Allocate(
            string appDomainId = null,
            string ownerId = null)
        {
            if (appDomainId == null)
                return null;

            AppDomain appDomain = _appDomains.FirstOrDefault(p => p.FriendlyName.KeyEquals(appDomainId));
            if (appDomain == null)
            {
                _appDomains.Add(appDomain = AppDomain.CreateDomain(appDomainId));
            }

            _resourceAllocations.RemoveAll(p =>
                    ((p.AllocatedResourceId != null) && (p.AllocatedResourceId == appDomainId))
                    & ((ownerId == p.OwnerId) | ((ownerId != null) && (p.OwnerId != null) && (string.Equals(ownerId, p.OwnerId, StringComparison.OrdinalIgnoreCase)))));
            _resourceAllocations.Add(new ResourceAllocation(appDomainId, ownerId));

            return appDomain;
        }

        /// <summary>
        /// Deallocates the specified application domain.
        /// </summary>
        /// <param name="appDomainId">The ID of the application domain to consider.</param>
        /// <param name="ownerId">The ID of the owner.</param>
        public bool Deallocate(string appDomainId, string ownerId = null)
        {
            if (appDomainId == null)
                return false;

            _resourceAllocations.RemoveAll(p =>
                ((p.AllocatedResourceId != null) && (string.Equals(p.AllocatedResourceId, appDomainId, StringComparison.OrdinalIgnoreCase))) &
                (((p.OwnerId == ownerId) & (ownerId == null)) || ((p.OwnerId != null) && (string.Equals(p.OwnerId, ownerId, StringComparison.OrdinalIgnoreCase)))));

            if (!_resourceAllocations.Any(p => (p.AllocatedResourceId != null) && (string.Equals(p.AllocatedResourceId, appDomainId, StringComparison.OrdinalIgnoreCase))))
            {
                // we retrieve the application domain
                AppDomain appDomain = _appDomains.FirstOrDefault(p => p.FriendlyName.KeyEquals(appDomainId));
                if ((appDomain != null) && (appDomain != AppDomain.CurrentDomain))
                {
                    // we remove the resolve event handler from the main domain
                    ResolveEventHandler aResolveEventHandler = (ResolveEventHandler)_resolveEventHandlerHashTable[appDomainId];
                    if (aResolveEventHandler != null)
                    {
                        AppDomain.CurrentDomain.AssemblyResolve -= aResolveEventHandler;
                        _resolveEventHandlerHashTable.Remove(appDomainId);
                    }

                    // we remove the application domain from list
                    _appDomains.Remove(appDomain);
                    AppDomain.Unload(appDomain);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    //appDomain = null;
                }

                return true;
            }

            return false;
        }

        #endregion

        // ------------------------------------------
        // ASSEMBLIES
        // ------------------------------------------

        #region Assemblies

        /// <summary>
        /// Gets the assembly of this instance from file.
        /// </summary>
        /// <param name="appDomain">Application domain to consider.</param>
        /// <param name="filePath">Path of the file to use.</param>
        /// <param name="log">The loading log to consider.</param>
        /// <returns>The assembly of this instance.</returns>
        public static Assembly LoadAssemblyFromFile(AppDomain appDomain, string filePath, IBdoLog log = null)
        {
            Assembly assembly = null;

            if (((appDomain != null) & (!string.IsNullOrEmpty(filePath)))
                && (File.Exists(filePath)))
            {
                string assemblyName = Path.GetFileNameWithoutExtension(filePath);
                foreach (global::System.Reflection.Assembly currentAssembly in appDomain.GetAssemblies())
                {
                    if (!currentAssembly.IsDynamic)
                    {
                        string assemblyCodeBasePath = currentAssembly.CodeBase.ToLower();
                        string assemblyFilePath = filePath.ToLower().Replace(@"\", "/");

                        if (assemblyCodeBasePath.Contains(assemblyFilePath))
                        {
                            assembly = currentAssembly;
                            break;
                        }
                    }
                }

                if (assembly == null)
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(filePath);
                    }
                    catch (Exception ex)
                    {
                        log?.AddException(
                            title: "Error while attempting to load assembly from file '" + filePath + "'",
                            description: ex.ToString());
                    }
                }
            }

            return assembly;
        }

        /// <summary>
        /// Gets the assembly of this instance from embed resource.
        /// </summary>
        /// <param name="appDomain">Application domain to consider.</param>
        /// <param name="assemblyName">The assembly name to use.</param>
        /// <param name="log">The loading log to consider.</param>
        /// <returns>The assembly of this instance.</returns>
        public static Assembly LoadAssembly(AppDomain appDomain, string assemblyName, IBdoLog log = null)
        {
            Assembly assembly = null;

            if ((appDomain != null) && (!string.IsNullOrEmpty(assemblyName)))
            {
                assembly = Array.Find(appDomain.GetAssemblies(), p => p.GetName().Name.KeyEquals(assemblyName));
                if (assembly == null)
                {
                    try
                    {
                        assembly = Assembly.Load(assemblyName);
                    }
                    catch (FileNotFoundException)
                    {
                        log?.AddError("Could not find the assembly '" + assemblyName + "'");
                    }
                    catch (Exception ex)
                    {
                        log?.AddException("Error while attempting to load assembly '" + assemblyName + "'", description: ex.ToString());
                    }
                }
            }

            return assembly;
        }

        #endregion
    }
}
