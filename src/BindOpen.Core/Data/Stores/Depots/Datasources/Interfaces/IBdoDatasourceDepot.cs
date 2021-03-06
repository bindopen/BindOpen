﻿using BindOpen.Data.Items;
using BindOpen.Extensions.Runtime;
using System.Collections.Generic;

namespace BindOpen.Data.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBdoDatasourceDepot : ITBdoDepot<IDatasource>
    {
        /// <summary>
        /// 
        /// </summary>
        List<Datasource> Sources { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="connectorDefinitionUniqueId"></param>
        /// <returns></returns>
        IBdoConnectorConfiguration GetConnectorConfiguration(string sourceName, string connectorDefinitionUniqueId = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        string GetInstanceName(string sourceName = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        string GetInstanceOtherwiseModuleName(string sourceName = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        string GetModuleName(string sourceName = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="connectorDefinitionUniqueId"></param>
        /// <returns></returns>
        string GetConnectionString(string sourceName = null, string connectorDefinitionUniqueId = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="connectorDefinitionUniqueId"></param>
        /// <returns></returns>
        bool HasConnectorConfiguration(string sourceName = null, string connectorDefinitionUniqueId = null);
    }
}