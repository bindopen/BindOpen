﻿using System;

namespace BindOpen.Extensions.Definition
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBdoMetricsDefinition : ITBdoExtensionItemDefinition<IBdoBdoMetricsDefinitionDto>
    {
        /// <summary>
        /// 
        /// </summary>
        Type RuntimeType { get; set; }
    }
}