﻿using BindOpen.Application.Languages;
using BindOpen.Data.Common;
using BindOpen.Data.Helpers.Objects;
using BindOpen.Data.Items;
using BindOpen.System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace BindOpen.Application.Modules
{
    /// <summary>
    /// This class represents a Sphere application module.
    /// </summary>
    public class AppModule : DescribedDataItem, IAppModule
    {
        // ------------------------------------------
        // PROPERTIES
        // ------------------------------------------

        #region Properties

        // Visibility ------------------------------

        /// <summary>
        /// File name of the icon of this instance.
        /// </summary>
        [DefaultValue("")]
        [XmlElement("iconFileName")]
        public string IconFileName { get; set; }

        /// <summary>
        /// Rank of this instance.
        /// </summary>
        [XmlElement("rank")]
        public int Rank { get; set; }

        /// <summary>
        /// File name of the thumb icon of this instance.
        /// </summary>
        [DefaultValue("")]
        [XmlElement("thumbIconFileName")]
        public string ThumbIconFileName { get; set; }

        /// <summary>
        /// Indicates whether this instance is visible.
        /// </summary>
        [XmlElement("isVisible")]
        public bool IsVisible { get; set; }

        // Tree ------------------------------------

        /// <summary>
        /// The sub sections of this instance.
        /// </summary>
        [XmlElement("sections")]
        public ITDataItemSet<AppSection> Sections { get; set; } = null;

        // Languages ------------------------------------

        /// <summary>
        /// The languages of this instance.
        /// </summary>
        [XmlElement("languages")]
        public ITDataItemSet<ApplicationLanguage> Languages { get; set; } = null;

        /// <summary>
        /// The default UI culture name of this instance.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue("")]
        public string DefaultUICulture { get; set; }

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AppModule class.
        /// </summary>
        public AppModule() : base(null, "module_")
        {
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors

        /// <summary>
        /// Returns the sub section with the specified name.
        /// </summary>
        /// <param name="name">The name of the sub application sections to return.</param>
        /// <returns>The sub application section with the specified name.</returns>
        public IAppSection GetSubSectionWithName(string name)
        {
            if (Sections != null)
            {
                foreach (IAppSection moduleSection in Sections.Items)
                {
                    IAppSection section;
                    if ((section = moduleSection.GetSubSectionWithName(name)) != null)
                    {
                        return section;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the sub section with the specified unique name.
        /// </summary>
        /// <param name="completeName">The complete name of the sub application sections to return.</param>
        /// <returns>The sub application sections with the specified unique name.</returns>
        public IAppSection GetSubSectionWithUniqueName(string completeName)
        {
            if (Sections != null)
            {
                foreach (IAppSection moduleSection in Sections.Items)
                {
                    IAppSection section;
                    if ((section = moduleSection.GetSubSectionWithUniqueName(completeName)) != null)
                    {
                        return section;
                    }
                }
            }

            return null;
        }

        // Languages ---------------------------------

        /// <summary>
        /// Gets the specified language.
        /// </summary>
        /// <param name="uiCulureName">The UI culture name.</param>
        /// <returns>The language to return.</returns>
        public IApplicationLanguage GetLanguageWithUICulture(string uiCulureName)
        {
            return Languages?.Items?.FirstOrDefault(p => p.UICultureName.KeyEquals(uiCulureName));
        }

        #endregion


        // ----------------------------
        // MUTATORS
        // ----------------------------

        #region Mutators

        /// <summary>
        /// Adds the specified section.
        /// </summary>
        /// <param name="section">The section to add.</param>
        /// <returns>Returns the specified section.</returns>
        public IAppModule AddSection(IAppSection section)
        {
            if (section != null)
            {
                (Sections ?? (Sections = new TDataItemSet<AppSection>())).Add(section as AppSection);
                section.Module = this;
            }

            return this;
        }

        /// <summary>
        /// Adds the specified sections.
        /// </summary>
        /// <param name="sections">The sections to add.</param>
        /// <returns>Returns the specified sections.</returns>
        public IAppModule AddSections(params IAppSection[] sections)
        {
            foreach (IAppSection section in sections)
            {
                AddSection(section);
            }

            return this;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="item">The item to consider.</param>
        /// <param name="specificationAreas">The specification areas to consider.</param>
        /// <param name="updateModes">The update modes to consider.</param>
        /// <returns>Log of the operation.</returns>
        /// <remarks>Put reference collections as null if you do not want to repair this instance.</remarks>
        public override IBdoLog Update<T>(
            T item = default,
            string[] specificationAreas = null,
            UpdateModes[] updateModes = null)
        {
            var log = new BdoLog();

            if (item is IAppModule)
            {
                IAppModule module = item as AppModule;
                log.AddEvents(Sections.Update(
                    module.Sections,
                    null,
                    new[] { UpdateModes.Incremental_AddItemsMissingInTarget }));
            }

            return log;
        }

        #endregion
    }
}