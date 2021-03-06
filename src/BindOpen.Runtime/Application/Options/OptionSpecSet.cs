﻿using BindOpen.Data.Common;
using BindOpen.Data.Conditions;
using BindOpen.Data.Elements;
using BindOpen.Data.Helpers.Objects;
using BindOpen.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace BindOpen.Application.Options
{
    /// <summary>
    /// This class represents a option specification set.
    /// </summary>
    [XmlType("OptionSpecSet", Namespace = "https://docs.bindopen.org/xsd")]
    [XmlRoot("optionSpecSet", Namespace = "https://docs.bindopen.org/xsd", IsNullable = false)]
    public class OptionSpecSet : TDataItemSet<IOptionSpec>, IOptionSpecSet
    {
        // -------------------------------------------------------------
        // PROPERTIES
        // -------------------------------------------------------------

        #region Properties
        /// <summary>
        /// Description of this instance.
        /// </summary>
        [XmlElement("description")]
        public DictionaryDataItem Description { get; set; } = null;


        /// <summary>
        /// The name of this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The index of this instance.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The condition of this instance.
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// The sub sets of this instance.
        /// </summary>
        public List<IOptionSpecSet> SubSets { get; set; } = new List<IOptionSpecSet>();

        #endregion

        // -------------------------------------------------------------
        // CONSTRUCTORS
        // -------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the OptionSpecSet class.
        /// </summary>
        public OptionSpecSet() : base()
        {
        }

        /// <summary>
        /// Instantiates a new instance of the OptionSpecSet class.
        /// </summary>
        /// <param name="optionSpecifications">The option specifications to consider.</param>
        public OptionSpecSet(params IOptionSpec[] optionSpecifications)
        {
            Items = optionSpecifications?.ToList();
        }

        /// <summary>
        /// Instantiates a new instance of the OptionSpecSet class.
        /// </summary>
        /// <param name="condition">The condition to consider.</param>
        /// <param name="optionSpecifications">The option specifications to consider.</param>
        public OptionSpecSet(ICondition condition, params IOptionSpec[] optionSpecifications) : this(optionSpecifications)
        {
            Condition = condition as Condition;
        }

        #endregion

        // -------------------------------------------------------------
        // ACCESSORS
        // -------------------------------------------------------------

        #region Accessors

        /// <summary>
        /// Gets the help text.
        /// </summary>
        /// <param name="uiCulture">The UI culture to consider.</param>
        /// <returns>Returns the help text.</returns>
        public string GetHelpText(String uiCulture = "*")
        {
            String helpText = Description.GetContent(uiCulture);

            foreach (DataElementSpec aElementSpec in Items)
            {
                foreach (String aAlias in aElementSpec.Aliases)
                    helpText += (helpText?.Length == 0 ? string.Empty : ", ") + aAlias;
                helpText += ": " + aElementSpec.Description.GetContent(uiCulture) + "\r\n";
            }

            return helpText;
        }

        #endregion

        // -------------------------------------------------------------
        // MUTATORS
        // -------------------------------------------------------------

        #region Mutators

        // Clear ------------------------------------

        /// <summary>
        /// Clears the options.
        /// </summary>
        public void ClearOptions()
        {
            ClearItems();
        }

        // Add subset -------------------------------

        /// <summary>
        /// Adds the specified set of option specifications.
        /// </summary>
        /// <param name="subSet">The sub set to add.</param>
        public IOptionSpecSet AddSubSet(IOptionSpecSet subSet)
        {
            SubSets?.Add(subSet as OptionSpecSet);

            return this;
        }

        /// <summary>
        /// Adds a new set of option specifications.
        /// </summary>
        /// <param name="optionSpecifications">The option specifications to consider.</param>
        public IOptionSpecSet AddSubSet(params IOptionSpec[] optionSpecifications)
        {
            SubSets?.Add(new OptionSpecSet(optionSpecifications));

            return this;
        }

        /// <summary>
        /// Instantiates a new instance of the OptionSpecSet class.
        /// </summary>
        /// <param name="condition">The condition to consider.</param>
        /// <param name="optionSpecifications">The option specifications to consider.</param>
        public IOptionSpecSet AddSubSet(ICondition condition, params IOptionSpec[] optionSpecifications)
        {
            SubSets?.Add(new OptionSpecSet(condition, optionSpecifications));

            return this;
        }

        // Add option -------------------------------

        /// <summary>
        /// Adds a new option specification.
        /// </summary>
        /// <param name="aliases">Aliases of the option to add.</param>
        public IOptionSpecSet AddOption(params string[] aliases)
        {
            return AddOption(OptionNameKind.OnlyValue, aliases);
        }

        /// <summary>
        /// Adds a new option specification.
        /// </summary>
        /// <param name="nameKind">The name kind to consider.</param>
        /// <param name="aliases">Aliases of the option to add.</param>
        public IOptionSpecSet AddOption(
            OptionNameKind nameKind,
            params string[] aliases)
        {
            return AddOption(DataValueTypes.Text, RequirementLevels.Optional, OptionNameKind.OnlyValue, aliases);
        }

        /// <summary>
        /// Adds a new option specification.
        /// </summary>
        /// <param name="requirementLevel">The requirement level of the entry to add.</param>
        /// <param name="aliases">Aliases of the option to add.</param>
        public IOptionSpecSet AddOption(
            RequirementLevels requirementLevel,
            params string[] aliases)
        {
            Add(new OptionSpec(requirementLevel, aliases));
            return this;
        }

        /// <summary>
        /// Adds a new option specification.
        /// </summary>
        /// <param name="dataValueType">The value type to consider.</param>
        /// <param name="nameKind">The name kind to consider.</param>
        /// <param name="aliases">Aliases of the option to add.</param>
        public IOptionSpecSet AddOption(
            DataValueTypes dataValueType,
            OptionNameKind nameKind,
            params string[] aliases)
        {
            return AddOption(dataValueType, RequirementLevels.Required, OptionNameKind.OnlyValue, aliases);
        }

        /// <summary>
        /// Adds a new option specification.
        /// </summary>
        /// <param name="requirementLevel">The requirement level of the entry to add.</param>
        /// <param name="nameKind">The name kind to consider.</param>
        /// <param name="aliases">Aliases of the option to add.</param>
        public IOptionSpecSet AddOption(
            RequirementLevels requirementLevel,
            OptionNameKind nameKind,
            params string[] aliases)
        {
            return AddOption(DataValueTypes.Text, requirementLevel, nameKind, aliases);
        }

        /// <summary>
        /// Adds a new option specification.
        /// </summary>
        /// <param name="dataValueType">The value type to consider.</param>
        /// <param name="requirementLevel">The requirement level of the entry to consider.</param>
        /// <param name="nameKind">The name kind to consider.</param>
        /// <param name="aliases">Aliases of the option to add.</param>
        public IOptionSpecSet AddOption(
            DataValueTypes dataValueType,
            RequirementLevels requirementLevel,
            OptionNameKind nameKind,
            params string[] aliases)
        {
            Add(new OptionSpec(dataValueType, requirementLevel, nameKind, aliases));
            return this;
        }

        /// <summary>
        /// Adds a new option specification.
        /// </summary>
        /// <param name="type">The type to consider.</param>
        /// <param name="requirementLevel">The requirement level of the option to consider.</param>
        /// <param name="nameKind">The name kind to consider.</param>
        /// <param name="aliases">Aliases of the option to add.</param>
        public IOptionSpecSet AddOption(
            Type type,
            RequirementLevels requirementLevel,
            OptionNameKind nameKind,
            params string[] aliases)
        {
            return AddOption(type.GetValueType(), requirementLevel, nameKind, aliases);
        }

        // Remove -----------------------------------

        /// <summary>
        /// Deletes the specified option.
        /// </summary>
        /// <param name="name">Name of the statement entry to remove.</param>
        public IOptionSpecSet RemoveOption(String name)
        {
            Remove(name);

            return this;
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors

        /// <summary>
        /// Indicates whether this instance has the specified option.
        /// </summary>
        /// <param name="name">Name of the option to consider.</param>
        public bool HasOption(String name)
        {
            return HasItem(name);
        }

        /// <summary>
        /// Returns the item with the specified name.
        /// </summary>
        /// <param name="key">The name of the alias of the item to return.</param>
        /// <returns>Returns the item with the specified name.</returns>
        public new IOptionSpec Get(string key = null)
        {
            if (key == null) return this[0];

            return Items.Find(p =>
                p.KeyEquals(key) || (p?.Aliases?.Any(q => q.KeyEquals(key)) == true));
        }

        #endregion

        // ------------------------------------------
        // IDescribedDataItem IMPLEMENTATION
        // ------------------------------------------

        #region IDescribedDataItem Implementation

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void AddDescription(string text)
        {
            AddDescription("*", text);
        }

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void AddDescription(string key, string text)
        {
            (Description ?? (Description = new DictionaryDataItem())).Add(key, text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void SetDescription(string text)
        {
            SetDescription("*", text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void SetDescription(string key = "*", string text = "*")
        {
            (Description ?? (Description = new DictionaryDataItem())).Set(key, text);
        }

        /// <summary>
        /// Returns the description label.
        /// </summary>
        /// <param name="variantName">The variant variant name to consider.</param>
        /// <param name="defaultVariantName">The default variant name to consider.</param>
        public virtual string GetDescription(string variantName = "*", string defaultVariantName = "*")
        {
            if (Description == null) return string.Empty;
            string label = Description.GetContent(variantName);
            if (string.IsNullOrEmpty(label))
                label = Description.GetContent(defaultVariantName);
            return label ?? string.Empty;
        }

        #endregion
    }
}
