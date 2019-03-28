﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using BindOpen.Framework.Core.Data.Business.Entities;
using BindOpen.Framework.Core.Data.Common;

namespace BindOpen.Framework.Core.Data.Elements.Schema
{
    /// <summary>
    /// This class represents a schema element.
    /// </summary>
    [Serializable()]
    [XmlType("SchemaElement", Namespace = "http://meltingsoft.com/bindopen/xsd")]
    [XmlRoot("schema", Namespace = "http://meltingsoft.com/bindopen/xsd", IsNullable = false)]
    [XmlInclude(typeof(SchemaZoneElement))]
    public class SchemaElement : DataElement
    {
        //------------------------------------------
        // VARIABLES
        //-----------------------------------------

        #region Variables

        private SchemaZoneElement _parentZone = null;
        private String _imageFileName = null;

        private BusinessEntity _businessEntity = null;

        #endregion

        //------------------------------------------
        // PROPERTIES
        //-----------------------------------------

        #region Properties

        /// <summary>
        /// The parent zone of this instance.
        /// </summary>
        [XmlIgnore()]
        public SchemaZoneElement ParentZone
        {
            get { return this._parentZone; }
            set
            {
                this._parentZone = value;
                this.RaizePropertyChanged(nameof(ParentZone));
            }
        }

        /// <summary>
        /// The image file name of this instance.
        /// </summary>
        [XmlElement("imageFileName")]
        public String ImageFileName
        {
            get { return this._imageFileName; }
            set
            {
                this._imageFileName = value;
                this.RaizePropertyChanged(nameof(ImageFileName));
            }
        }

        /// <summary>
        /// The business entity unique name of this instance.
        /// </summary>
        [XmlElement("businessEntityUniqueName")]
        public String BusinessEntityUniqueName
        {
            get;
            set;
        }

        /// <summary>
        /// The business entity of this instance.
        /// </summary>
        [XmlIgnore()]
        public BusinessEntity BusinessEntity
        {
            get { return this._businessEntity; }
            set
            {
                this._businessEntity = value;
                this.BusinessEntityUniqueName = _businessEntity?.Id;
                this.RaizePropertyChanged(nameof(BusinessEntity));
            }
        }

        /// <summary>
        /// The specification of this instance.
        /// </summary>
        [XmlElement("specification")]
        public new SchemaElementSpec Specification
        {
            get { return base.Specification as SchemaElementSpec; }
            set { base.Specification = value; }
        }

        #endregion

        //------------------------------------------
        // CONSTRUCTORS
        //-----------------------------------------

        #region Constructors
        
        /// <summary>
        /// Initializes a new schema element.
        /// </summary>
        public SchemaElement()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SchemaElement class.
        /// </summary>
        /// <param name="name">The name to consider.</param>
        /// <param name="items">The items to consider.</param>
        public SchemaElement(
            String name = null,
            params Object[] items)
            : base(name, "schemaElement_")
        {
            this.ValueType = DataValueType.Schema;
            this.Specification = new SchemaElementSpec();

            foreach (Object aItem in items)
                this.AddItem(aItem);
        }

        #endregion

        // --------------------------------------------------
        // ACCESSORS
        // --------------------------------------------------

        #region Accessors

        /// <summary>
        /// Indicates whether this instance is a descendant of the specified parent schema element zone.
        /// </summary>
        /// <param name="parentZoneElement">The parent schema element zone to consider.</param>
        /// <returns>True if this instance is a descendant of the specified parent schema element.</returns>
        public Boolean IsDescendantOf(
            SchemaZoneElement parentZoneElement)
        {
            SchemaElement currentSchemaZoneElement = this._parentZone;
            while (currentSchemaZoneElement != null)
            {
                if (currentSchemaZoneElement == parentZoneElement)
                    return true;
                currentSchemaZoneElement = currentSchemaZoneElement.ParentZone;
            }
            return false;
        }

        // Specification ---------------------

        /// <summary>
        /// Gets a new specification.
        /// </summary>
        /// <returns>Returns the new specifcation.</returns>
        public override DataElementSpec CreateSpecification()
        {
            return new SchemaElementSpec();
        }

        // Schema elements

        // Deletion / Removing -------------------------------------------

        /// <summary>
        /// Deletes the specified schema element.
        /// </summary>
        /// <param name="aElement">The schema element to consider.</param>
        /// <returns>True if the operation has succeded ; false otherwise.</returns>
        public Boolean DeleteElement(SchemaElement aElement)
        {
            if (aElement == null) return true;

            // we delete the object
            if (aElement.ParentZone != null)
                return aElement.ParentZone.SubElements.Remove(aElement);
            else
                return false;
        }

        /// <summary>
        /// Deletes the specified schema elements.
        /// </summary>
        /// <param name="elements">The schema elements to consider.</param>
        public void DeleteElements(
            List<SchemaElement> elements)
        {
            if (elements == null)
                return;

            foreach (SchemaElement currentElement in elements)
                this.DeleteElement(currentElement);
        }

        // Duplication / Copy / Move -------------------------------------------

        /// <summary>
        /// Duplicates the specified schema element to the specified parent schema element.
        /// </summary>
        /// <param name="aElement">The schema element to consider.</param>
        /// <param name="aSchemaZoneElement">The parent schema element zone to consider.</param>
        /// <returns>The duplicated schema element.</returns>
        public SchemaElement DuplicateElement(
            SchemaElement aElement,
            SchemaZoneElement aSchemaZoneElement = null)
        {
            if (aElement == null) return null;

            return aElement.Clone(aSchemaZoneElement) as SchemaElement;
        }

        /// <summary>
        /// Duplicates the specified schema element to the specified parents schema element.
        /// </summary>
        /// <param name="aElement">The schema element to consider.</param>
        /// <param name="parentElements">The parents schema element to consider.</param>
        /// <returns>The duplicated schema element.</returns>
        public List<SchemaElement> DuplicateElement(
            SchemaElement aElement,
            List<SchemaZoneElement> parentElements)
        {
            List<SchemaElement> duplicatedElements = new List<SchemaElement>();

            if (aElement == null)
                return duplicatedElements;
            foreach (SchemaZoneElement parentElement in parentElements)
                duplicatedElements.Add(this.DuplicateElement(aElement, parentElement));

            return duplicatedElements;
        }

        /// <summary>
        /// Duplicates the specified schema elements to the specified parent schema element.
        /// </summary>
        /// <param name="elements">The schema elements to consider.</param>
        /// <param name="parentZoneElement">The parent schema element zone object to consider.</param>
        public List<SchemaElement> DuplicateElements(
            List<SchemaElement> elements,
            SchemaZoneElement parentZoneElement = null)
        {
            List<SchemaElement> duplicatedElements = new List<SchemaElement>();

            if (elements == null)
                return duplicatedElements;

            foreach (SchemaElement currentElement in elements)
                duplicatedElements.Add(this.DuplicateElement(currentElement, parentZoneElement));

            return duplicatedElements;
        }

        /// <summary>
        /// Moves the specified schema element to the specified parent schema element.
        /// </summary>
        /// <param name="aElement">The schema element to consider.</param>
        /// <param name="parentZoneElement">The parent schema element zone to consider.</param>
        public Boolean MoveElement(
            SchemaElement aElement,
            SchemaZoneElement parentZoneElement)
        {
            if ((aElement == null) || (aElement.ParentZone == null) | (parentZoneElement == null))
                return true;
            if ((parentZoneElement == aElement) |
                ((aElement is SchemaZoneElement) && (parentZoneElement.IsDescendantOf((SchemaZoneElement)aElement))))
                return false;

            aElement.ParentZone.SubElements.Remove(aElement);
            aElement.ParentZone = parentZoneElement;
            if (!parentZoneElement.SubElements.Contains(parentZoneElement))
                parentZoneElement.SubElements.Add(aElement);
            return true;
        }

        /// <summary>
        /// Moves the specified schema elements to the specified parent schema element.
        /// </summary>
        /// <param name="elements">The schema elements to consider.</param>
        /// <param name="parentZoneElement">The parent schema element zone to consider.</param>
        public void MoveElements(
            List<SchemaElement> elements,
            SchemaZoneElement parentZoneElement)
        {
            if (elements == null)
                return;

            foreach (SchemaElement currentElement in elements)
                this.MoveElement(currentElement, parentZoneElement);
        }

        // Adding -------------------------------------------

        /// <summary>
        /// Add a new schema element zone to the specified parent schema element zone.
        /// </summary>
        /// <param name="parentZoneElement">The parent schema element zone to consider.</param>
        /// <returns>The created schema element zone.</returns>
        public SchemaZoneElement CreateSchemaZoneElement(
            SchemaZoneElement parentZoneElement)
        {
            if (parentZoneElement == null)
                return null;

            SchemaZoneElement aSchemaZoneElement = new SchemaZoneElement();
            aSchemaZoneElement.ParentZone = parentZoneElement;
            parentZoneElement.AddSubElement(aSchemaZoneElement);
            return aSchemaZoneElement;
        }

        /// <summary>
        /// Add a new object to the specified parent schema element zone.
        /// </summary>
        /// <param name="parentZoneElement">The parent schema element zone to consider.</param>
        /// <returns>The created schema element.</returns>
        public SchemaElement CreateElement(
            SchemaZoneElement parentZoneElement)
        {
            if (parentZoneElement == null)
                return null;

            SchemaElement aElement = new SchemaElement();
            aElement.ParentZone = parentZoneElement;
            parentZoneElement.AddSubElement(aElement);
            return aElement;
        }

        /// <summary>
        /// Add a new schema element zone to the specified parent schema element zone.
        /// </summary>
        /// <param name="parentZoneElement">The parent schema element zone to consider.</param>
        /// <param name="aElement">The schema element to consider.</param>
        public void AddElement(
            SchemaZoneElement parentZoneElement,
            SchemaElement aElement)
        {
            if ((parentZoneElement == null) | (aElement == null))
                return;

            aElement.ParentZone = parentZoneElement;
            parentZoneElement.AddSubElement(aElement);
        }

        #endregion

        // -------------------------------------------------------------
        // PROTECTION
        // -------------------------------------------------------------

        #region Protection

        /// <summary>
        /// Apply the specified visibility to this instance.
        /// </summary>
        /// <param name="accessibilityLevel">The visibility to apply.</param>
        /// <param name="isRecursive">Indicates whether the protection is applied to sub schema elements.</param>
        public void ApplyVisibility(AccessibilityLevel accessibilityLevel, Boolean isRecursive = true)
        {
            if ((this is SchemaZoneElement)&&(isRecursive))
                foreach (SchemaElement aElement in ((SchemaZoneElement)this).SubElements)
                    aElement.ApplyVisibility(accessibilityLevel, isRecursive);
        }

        /// <summary>
        /// Locks this instance.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the protection is applied to sub objects.</param>
        public override void Lock(Boolean isRecursive = true)
        {
            base.Lock(isRecursive);

            if ((this is SchemaZoneElement) && (isRecursive))
                foreach (SchemaElement aElement in ((SchemaZoneElement)this).SubElements)
                    aElement.Lock( isRecursive);
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        /// <param name="isRecursive">Indicates whether the protection is applied to sub objects.</param>
        public override void Unlock(Boolean isRecursive = true)
        {
            base.Lock(isRecursive);

            if ((this is SchemaZoneElement) && (isRecursive))
                foreach (SchemaElement aElement in ((SchemaZoneElement)this).SubElements)
                    aElement.Unlock( isRecursive);
        }

        #endregion

        // --------------------------------------------------
        // CHECK, UPDATE, REPAIR
        // --------------------------------------------------

        #region Check_Update_Repair


        #endregion

        // --------------------------------------------------
        // CLONING
        // --------------------------------------------------

        #region Cloning

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns a cloned instance.</returns>
        public override Object Clone()
        {
            return this.Clone(null);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="parentZoneElement">The parent schema element zone to consider.</param>
        /// <returns>Returns a cloned instance.</returns>
        public virtual Object Clone(SchemaZoneElement parentZoneElement)
        {
            if (parentZoneElement == null)
                parentZoneElement = this.ParentZone;

            SchemaZoneElement aSchemaElement = this.MemberwiseClone() as SchemaZoneElement;
            aSchemaElement.BusinessEntity = this._businessEntity;
            aSchemaElement.ParentZone = parentZoneElement;
            if (parentZoneElement != null)
                parentZoneElement.SubElements.Add(aSchemaElement);

            return aSchemaElement;
        }

        #endregion

        // ------------------------------------------
        // INOTIFY IMPLEMENTATION
        // ------------------------------------------

        #region INotify Implementation

        /// <summary>
        /// The event corresponding to a property that has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occures when a property changes.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected void RaizePropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
