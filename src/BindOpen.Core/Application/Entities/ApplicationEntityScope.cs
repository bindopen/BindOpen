﻿using System.Xml.Serialization;

namespace BindOpen.Application.Entities
{
    /// <summary>
    /// This enumeration represents the possible application entity scopes.
    /// </summary>
    [XmlType("ApplicationEntityScope", Namespace = "https://docs.bindopen.org/xsd")]
    public enum ApplicationEntityScope
    {
        /// <summary>
        /// Any.
        /// </summary>
        Any,

        /// <summary>
        /// System.
        /// </summary>
        System,

        /// <summary>
        /// Platform.
        /// </summary>
        Platform,

        /// <summary>
        /// Platform module.
        /// </summary>
        PlatformModule,

        /// <summary>
        /// Business.
        /// </summary>
        Business
    };

}
