﻿using BindOpen.Framework.Core.Data.Items;
using BindOpen.Framework.Core.Data.Items.Datasources;
using System.Collections.Generic;

namespace BindOpen.Framework.Core.Extensions.Runtime.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExtensionLoadOptions : IDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        string LibraryFolderPath { get; }

        /// <summary>
        /// 
        /// </summary>
        string RemoteServerUri { get; }

        /// <summary>
        /// 
        /// </summary>
        List<DatasourceKind> SourceKinds { get; }

        /// <summary>
        /// Sets the path of the library folder of this instance.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>Returns this instance.</returns>
        IExtensionLoadOptions WithLibraryFolderPath(string folderPath);

        /// <summary>
        /// Sets the URI of the remote server of this instance.
        /// </summary>
        /// <param name="uri">The URI of the remote server.</param>
        /// <returns>Returns this instance.</returns>
        IExtensionLoadOptions WithRemoteServerUri(string uri);

        /// <summary>
        /// Sets the source kinds of this instance.
        /// </summary>
        /// <param name="sourceKinds">The source kinds to consider.</param>
        /// <returns>Returns this instance.</returns>
        IExtensionLoadOptions WithSourceKinds(params DatasourceKind[] sourceKinds);
    }
}