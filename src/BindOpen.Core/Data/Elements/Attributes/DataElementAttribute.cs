﻿using BindOpen.Data.Common;
using BindOpen.Data.Items;
using BindOpen.Data.Specification;
using System;

namespace BindOpen.Data.Elements
{
    /// <summary>
    /// This class represents a data element attribute.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class DataElementAttribute : DescribedDataItemAttribute
    {
        // --------------------------------------------------
        // VARIABLES
        // --------------------------------------------------

        #region Variables

        private string[] _aliases = null;
        private DataAreaSpecification[] _areaSpecifications = null;

        private DataItemizationMode[] _availableItemizationModes = null;

        private string[] _defaultStringItems = null;

        private SpecificationLevels[] _itemSpecificationLevels = null;

        #endregion

        // --------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------

        #region Properties

        /// <summary>
        /// ID of the group of this instance.
        /// </summary>
        public string GroupId { get; set; } = string.Empty;

        /// <summary>
        /// The value type of this instance.
        /// </summary>
        public DataValueTypes ValueType { get; set; } = DataValueTypes.Any;

        /// <summary>
        /// The aliases of the entry.
        /// </summary>
        public string[] Aliases
        {
            get
            {
                return _aliases;
            }
            set
            {
                _aliases = value ?? Array.Empty<string>();
            }
        }

        /// <summary>
        /// The area specifications of this instance.
        /// </summary>
        public DataAreaSpecification[] AreaSpecifications
        {
            get
            {
                return _areaSpecifications;
            }
            set
            {
                _areaSpecifications = value ?? Array.Empty<DataAreaSpecification>();
            }
        }

        /// <summary>
        /// Indicates whether the instance can be allocated.
        /// </summary>
        public bool IsAllocatable { get; set; } = false;

        // Items ---------------------------------

        /// <summary>
        /// The available itemization modes of this instance.
        /// </summary>
        public DataItemizationMode[] AvailableItemizationModes
        {
            get
            {
                return _availableItemizationModes;
            }
            set
            {
                _availableItemizationModes = value ?? Array.Empty<DataItemizationMode>();
            }
        }

        /// <summary>
        /// Default string items of this instance.
        /// </summary>
        public string[] DefaultStringItems
        {
            get
            {
                return _defaultStringItems;
            }
            set { _defaultStringItems = value ?? Array.Empty<string>(); }
        }

        /// <summary>
        /// Minimum item number of this instance.
        /// </summary>
        public int MinimumItemNumber { get; set; } = 1;

        /// <summary>
        /// Maximum item number of this instance.
        /// </summary>
        public int MaximumItemNumber { get; set; } = -1;

        /// <summary>
        /// Indicates whether the value of this instance is a list.
        /// </summary>
        public bool IsValueList
        {
            get
            {
                return (MaximumItemNumber == -1) | (MaximumItemNumber > 1);
            }
        }

        /// <summary>
        /// The item requirement level of this instance.
        /// </summary>
        public RequirementLevels ItemRequirementLevel
        {
            get
            {
                RequirementLevels itemRequirementLevel;
                if (MaximumItemNumber == 0)
                {
                    itemRequirementLevel = RequirementLevels.Forbidden;
                }
                else if (MinimumItemNumber > 0)
                    itemRequirementLevel = RequirementLevels.Required;
                else if (MinimumItemNumber <= 0)
                    itemRequirementLevel = RequirementLevels.Optional;
                else
                    itemRequirementLevel = RequirementLevels.None;

                return itemRequirementLevel;
            }
        }

        /// <summary>
        /// Levels of specification of this instance.
        /// </summary>
        public SpecificationLevels[] ItemSpecificationLevels
        {
            get
            {
                return _itemSpecificationLevels;
            }
            set { _itemSpecificationLevels = value ?? Array.Empty<SpecificationLevels>(); }
        }

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the DataElementAttribute class.
        /// </summary>
        protected DataElementAttribute() : base()
        {
        }

        #endregion
    }
}
