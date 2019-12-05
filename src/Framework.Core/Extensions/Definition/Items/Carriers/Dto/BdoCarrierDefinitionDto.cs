﻿using BindOpen.Framework.Core.Data.Elements.Sets;
using BindOpen.Framework.Core.Data.Items.Datasources;
using System;
using System.Xml.Serialization;

namespace BindOpen.Framework.Core.Extensions.Definition.Items
{
    /// <summary>
    /// This class represents a DTO carrier definition.
    /// </summary>
    [Serializable()]
    [XmlType("BdoCarrierDefinition", Namespace = "https://bindopen.org/xsd")]
    [XmlRoot(ElementName = "carrier.definition", Namespace = "https://bindopen.org/xsd", IsNullable = false)]
    public class BdoCarrierDefinitionDto : BdoExtensionItemDefinitionDto, IBdoCarrierDefinitionDto
    {
        // ------------------------------------------
        // PROPERTIES
        // ------------------------------------------

        #region Properties

        /// <summary>
        /// The item class of this instance.
        /// </summary>
        [XmlElement("itemClass")]
        public string ItemClass
        {
            get;
            set;
        }

        /// <summary>
        /// The data source kind of this instance.
        /// </summary>
        [XmlElement("dataSourceKind")]
        public DatasourceKind DatasourceKind { get; set; } = DatasourceKind.None;

        /// <summary>
        /// The set of element specifications of this instance.
        /// </summary>
        [XmlElement("detail.specification")]
        public DataElementSpecSet DetailSpec { get; set; } = new DataElementSpecSet();

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the CarrierDefinitionDto class.
        /// </summary>
        public BdoCarrierDefinitionDto()
        {
        }

        #endregion
    }
}
