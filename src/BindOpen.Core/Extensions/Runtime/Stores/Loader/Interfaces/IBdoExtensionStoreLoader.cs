﻿using BindOpen.Data.Items;
using BindOpen.System.Assemblies.References;
using BindOpen.System.Diagnostics;

namespace BindOpen.Extensions.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBdoExtensionStoreLoader : IDataItem
    {
        /// <summary>
        /// Loads the specified extensions into the specified scope.
        /// </summary>
        /// <param name="references">The library references to consider.</param>
        IBdoLog LoadExtensionsInStore(params IBdoAssemblyReference[] references);
    }
}