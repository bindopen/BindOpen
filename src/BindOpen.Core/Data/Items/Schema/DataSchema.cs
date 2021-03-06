﻿using BindOpen.Data.Elements.Schema;
using BindOpen.Data.References;
using System;
using System.Xml.Serialization;

namespace BindOpen.Data.Items
{
    /// <summary>
    /// This class represents a data schema.
    /// </summary>
    [XmlType("DataSchema", Namespace = "https://docs.bindopen.org/xsd")]
    [XmlRoot("schema", Namespace = "https://docs.bindopen.org/xsd", IsNullable = false)]
    public class DataSchema : DescribedDataItem, IDataSchema
    {
        //------------------------------------------
        // PROPERTIES
        //-----------------------------------------

        #region Properties

        /// <summary>
        /// Root zone of this instance. 
        /// </summary>
        [XmlElement("rootZone")]
        public SchemaZoneElement RootZone { get; set; } = new SchemaZoneElement();

        /// <summary>
        /// The meta schema reference of this instance. 
        /// </summary>
        [XmlElement("metaSchema.reference")]
        public DataReferenceDto MetaSchemreference { get; set; } = null;

        #endregion

        //------------------------------------------
        // CONSTRUCTORS
        //-----------------------------------------

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DataSchema class.
        /// </summary>
        public DataSchema() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataSchema class.
        /// </summary>
        /// <param name="name">The name of this instance.</param>
        public DataSchema(string name) : base(name, "schema_")
        {
            WithTitle("My schema");
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors

        /// <summary>
        /// Gets the schema element with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the meta object to consider.</param>
        /// <param name="parentMetobject1">The parent meta object to consider.</param>
        /// <returns>The bmeta object with the specified ID.</returns>
        public SchemaElement GetElementWithId(String id, SchemaElement parentMetobject1 = null)
        {
            return RootZone?.GetElementWithId(id);
        }

        #endregion
    }
}
