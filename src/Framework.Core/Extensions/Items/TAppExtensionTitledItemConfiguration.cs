﻿using System.Xml.Serialization;
using BindOpen.Framework.Core.Data.Elements;
using BindOpen.Framework.Core.Data.Items.Dictionary;
using BindOpen.Framework.Core.Extensions.Common;
using BindOpen.Framework.Core.Extensions.Items;

namespace BindOpen.Framework.Core.Extensions.Items
{
    /// <summary>
    /// This class represents an application extension titled item configuration.
    /// </summary>
    public abstract class TAppExtensionTitledItemConfiguration<T>
        : TAppExtensionItemConfiguration<T>, ITAppExtensionTitledItemConfiguration<T>
        where T : IAppExtensionItemDefinition
    {
        // -----------------------------------------------
        // PROPERTIES
        // -----------------------------------------------

        #region Properties

        // General -------------------------------

        /// <summary>
        /// Title of this instance.
        /// </summary>
        [XmlElement("title")]
        public DictionaryDataItem Title { get; set; } = null;

        /// <summary>
        /// Specification of the Title property of this instance.
        /// </summary>
        [XmlIgnore()]
        public bool TitleSpecified => this.Title != null && (this.Title.AvailableKeysSpecified || this.Title.ValuesSpecified || this.Title.SingleValueSpecified);

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the AppExtensionTitledItemConfiguration class.
        /// </summary>
        protected TAppExtensionTitledItemConfiguration() : this(AppExtensionItemKind.Any, null)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the AppExtensionTitledItemConfiguration class.
        /// </summary>
        /// <param name="kind">The kind to consider.</param>
        /// <param name="definitionUniqueId">The definition unique ID to consider.</param>
        /// <param name="items">The items to consider.</param>
        protected TAppExtensionTitledItemConfiguration(
            AppExtensionItemKind kind,
            string definitionUniqueId,
            params IDataElement[] items)
            : base(AppExtensionItemKind.Any, definitionUniqueId, items)
        {
        }

        #endregion

        // --------------------------------------------------
        // MUTATORS
        // --------------------------------------------------

        #region Mutators

        // Title -------------------------------

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void AddTitleText(string text)
        {
            this.AddTitleText("*", text);
        }

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void AddTitleText(string key, string text)
        {
            (this.Title ?? (this.Title = new DictionaryDataItem())).AddValue(key, text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void SetTitleText(string text)
        {
            this.SetTitleText("*", text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void SetTitleText(string key = "*", string text = "*")
        {
            if (this.Title == null) this.Title = new DictionaryDataItem();
            this.Title.SetValue(key, text);
        }

        // Description -------------------------------

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void AddDescriptionText(string text)
        {
            this.AddDescriptionText("*", text);
        }

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void AddDescriptionText(string key, string text)
        {
            if (this.Description == null) this.Description = new DictionaryDataItem();
            this.Description.AddValue(key, text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void SetDescriptionText(string text)
        {
            this.SetDescriptionText("*", text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void SetDescriptionText(string key = "*", string text = "*")
        {
            (this.Description ?? (this.Description = new DictionaryDataItem())).SetValue(key, text);
        }

        #endregion

        // --------------------------------------------------
        // ACCESSORS
        // --------------------------------------------------

        #region Accessors

        /// <summary>
        /// Returns the title label.
        /// </summary>
        /// <param name="variantName">The variant variant name to consider.</param>
        /// <param name="defaultVariantName">The default variant name to consider.</param>
        public string GetTitleText(string variantName = "*", string defaultVariantName = "*")
        {
            if (this.Title == null) return "";
            string label = this.Title.GetContent(variantName);
            if (string.IsNullOrEmpty(label))
                label = this.Title.GetContent(defaultVariantName);
            if (string.IsNullOrEmpty(label))
                label = this.Name;
            return label ?? "";
        }

        /// <summary>
        /// Returns the description label.
        /// </summary>
        /// <param name="variantName">The variant variant name to consider.</param>
        /// <param name="defaultVariantName">The default variant name to consider.</param>
        public string GetDescriptionText(string variantName = "*", string defaultVariantName = "*")
        {
            if (this.Description == null) return "";
            string label = this.Description.GetContent(variantName);
            if (string.IsNullOrEmpty(label))
                label = this.Description.GetContent(defaultVariantName);
            return label ?? "";
        }

        #endregion

        // ------------------------------------------
        // CLONING
        // ------------------------------------------

        #region Cloning

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the cloned metrics definition.</returns>
        public override object Clone()
        {
            ITAppExtensionTitledItemConfiguration<T> dto = base.Clone() as TAppExtensionTitledItemConfiguration<T>;
            if (this.Title != null)
                dto.Title = this.Title.Clone() as DictionaryDataItem;

            return dto;
        }

        #endregion

        // ------------------------------------------
        // ITitledDataItem IMPLEMENTATION
        // ------------------------------------------

        #region ITitledDataItem Implementation

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void AddTitle(string text)
        {
            this.AddTitle("*", text);
        }

        /// <summary>
        /// Adds the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void AddTitle(string key, string text)
        {
            (this.Title ?? (this.Title = new DictionaryDataItem())).AddValue(key, text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="text">The text to consider.</param>
        public void SetTitle(string text)
        {
            this.SetTitle("*", text);
        }

        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="key">The key to consider.</param>
        /// <param name="text">The text to consider.</param>
        public void SetTitle(string key = "*", string text = "*")
        {
            (this.Title ?? (this.Title = new DictionaryDataItem())).SetValue(key, text);
        }

        /// <summary>
        /// Returns the title label.
        /// </summary>
        /// <param name="variantName">The variant variant name to consider.</param>
        /// <param name="defaultVariantName">The default variant name to consider.</param>
        public virtual string GetTitle(string variantName = "*", string defaultVariantName = "*")
        {
            if (this.Title == null) return "";

            string label = this.Title.GetContent(variantName);
            if (string.IsNullOrEmpty(label))
            {
                label = this.Title.GetContent(defaultVariantName);
            }
            if (string.IsNullOrEmpty(label))
            {
                label = this.Name;
            }

            return label ?? "";
        }

        #endregion
    }

}
