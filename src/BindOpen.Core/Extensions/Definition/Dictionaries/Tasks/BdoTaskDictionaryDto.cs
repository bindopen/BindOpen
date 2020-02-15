﻿using BindOpen.Extensions.Definition;
using System;
using System.Xml.Serialization;

namespace BindOpen.Extensions.Definition
{
    /// <summary>
    /// This class represents a DTO task dictionary.
    /// </summary>
    [Serializable()]
    [XmlType("BdoTaskDictionary", Namespace = "https://bindopen.org/xsd")]
    [XmlRoot(ElementName = "tasks.dictionary", Namespace = "https://bindopen.org/xsd", IsNullable = false)]
    public class BdoTaskDictionaryDto : TBdoExtensionDictionaryDto<BdoTaskDefinitionDto>, IBdoTaskDictionaryDto
    {
        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the BdoTaskDictionaryDto class.
        /// </summary>
        public BdoTaskDictionaryDto()
        {
        }

        #endregion
    }
}