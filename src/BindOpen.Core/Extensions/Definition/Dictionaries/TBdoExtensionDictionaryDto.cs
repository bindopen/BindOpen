﻿using BindOpen.Data.Items;
using BindOpen.Extensions.Runtime;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BindOpen.Extensions.Definition
{
    /// <summary>
    /// This class represents a BindOpen extension dictionary.
    /// </summary>
    /// <typeparam name="T">The class of extension item definition to consider.</typeparam>
    [XmlType("TBdoExtensionDictionaryDto", Namespace = "https://docs.bindopen.org/xsd")]
    [XmlRoot(ElementName = "dictionary", Namespace = "https://docs.bindopen.org/xsd", IsNullable = false)]
    public class TBdoExtensionDictionaryDto<T> : StoredDataItem, ITBdoExtensionDictionaryDto<T>
        where T : BdoExtensionItemDefinitionDto
    {
        // ------------------------------------------
        // VARIABLES
        // ------------------------------------------

        #region Variables

        private List<BdoExtensionItemGroup> _groups;

        #endregion

        // ------------------------------------------
        // PROPERTIES
        // ------------------------------------------

        #region Properties

        /// <summary>
        /// ID of the library of this instance.
        /// </summary>
        [XmlAttribute("libraryId")]
        public string LibraryId { get; set; } = string.Empty;

        /// <summary>
        /// Name of the library of this instance.
        /// </summary>
        [XmlAttribute("library")]
        public string LibraryName { get; set; } = string.Empty;

        /// <summary>
        /// Definitions of this instance.
        /// </summary>
        [XmlArray("definitions")]
        [XmlArrayItem("add.definition")]
        public List<T> Definitions { get; set; } = new List<T>();

        /// <summary>
        /// Groups of this instance.
        /// </summary>
        [XmlArray("groups")]
        [XmlArrayItem("group")]
        public List<BdoExtensionItemGroup> Groups => _groups ??= new List<BdoExtensionItemGroup>();

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the TBdoExtensionDictionaryDto class.
        /// </summary>
        public TBdoExtensionDictionaryDto()
        {
        }

        #endregion
    }
}
