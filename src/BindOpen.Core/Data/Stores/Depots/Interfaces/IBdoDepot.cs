﻿using BindOpen.Application.Scopes;
using BindOpen.Data.Items;
using BindOpen.System.Diagnostics;
using System;

namespace BindOpen.Data.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBdoDepot : IIdentifiedDataItem
    {
        /// <summary>
        /// The scope of this instance.
        /// </summary>
        IBdoScope Scope { get; }

        /// <summary>
        /// Add the items from all the assemblies.
        /// </summary>
        /// <param name="log">The log to append.</param>
        IBdoDepot AddFromAllAssemblies(IBdoLog log = null);

        /// <summary>
        /// Add the items from the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="log">The log to append.</param>
        IBdoDepot AddFromAssembly(string assemblyName, IBdoLog log = null);

        /// <summary>
        /// Add the items from the assembly of the specified type.
        /// </summary>
        /// <param name="log">The log to append.</param>
        IBdoDepot AddFromAssembly<T>(IBdoLog log = null) where T : class;

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <param name="log">The log to append.</param>
        void LoadLazy(IBdoLog log);

        /// <summary>
        /// The initialization function.
        /// </summary>
        Func<IBdoDepot, IBdoLog, int> LazyLoadFunction { get; set; }

        /// <summary>
        /// Sets the scope of this instance.
        /// </summary>
        /// <param name="scope">The scope to append.</param>
        IBdoDepot WithScope(IBdoScope scope);
    }
}