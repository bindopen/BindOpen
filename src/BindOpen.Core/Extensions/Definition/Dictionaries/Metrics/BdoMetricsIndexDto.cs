﻿using BindOpen.Extensions.Definition;
using System;
using System.Xml.Serialization;

namespace BindOpen.Extensions.Definition
{
    /// <summary>
    /// This class represents a DTO metrics dictionary.
    /// </summary>
    [Serializable()]
    [XmlType("BdoMetricsDictionary", Namespace = "https://bindopen.org/xsd")]
    [XmlRoot(ElementName = "metrics.dictionary", Namespace = "https://bindopen.org/xsd", IsNullable = false)]
    public class BdoMetricsDictionaryDto : TBdoExtensionDictionaryDto<BdoMetricsDefinitionDto>, IBdoMetricsDictionaryDto
    {
        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the MetricsDictionary class.
        /// </summary>
        public BdoMetricsDictionaryDto()
        {
        }

        #endregion
    }
}