﻿using BindOpen.Framework.Core.Application.Configuration;
using BindOpen.Framework.Core.Application.Scopes;
using BindOpen.Framework.Core.Data.Common;
using BindOpen.Framework.Core.Data.Elements;
using BindOpen.Framework.Core.Data.Elements.Sets;
using BindOpen.Framework.Core.Data.Helpers.Objects;
using BindOpen.Framework.Core.Data.Helpers.Strings;
using BindOpen.Framework.Core.Data.Items;
using BindOpen.Framework.Core.Extensions.Attributes;
using BindOpen.Framework.Core.System.Diagnostics;
using BindOpen.Framework.Core.System.Scripting;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace BindOpen.Framework.Runtime.Application.Settings
{
    /// <summary>
    /// This class represents a base settings.
    /// </summary>
    public abstract class BaseSettings : DataItem, IBaseSettings
    {
        /// <summary>
        /// The scope of this instance.
        /// </summary>
        protected IAppScope _scope = null;

        /// <summary>
        /// The configuration of this instance.
        /// </summary>
        protected IBaseConfiguration _configuration = null;

        // -------------------------------------------------------
        // PROPERTIES
        // -------------------------------------------------------

        #region Properties

        /// <summary>
        /// The application scope of this instance.
        /// </summary>
        public IAppScope AppScope => _scope;

        /// <summary>
        /// The item of this instance.
        /// </summary>
        public IBaseConfiguration Configuration => _configuration;

        #endregion

        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the BaseSettings class.
        /// </summary>
        public BaseSettings()
        {
        }

        /// <summary>
        /// Instantiates a new instance of the BaseSettings class.
        /// </summary>
        /// <param name="scope">The application scope to consider.</param>
        /// <param name="configuration">The configuration to consider.</param>
        public BaseSettings(IAppScope scope, IBaseConfiguration configuration)
        {
            _scope = scope;
            _configuration = configuration;
        }

        #endregion

        // ------------------------------------------
        // ACCESSORS
        // ------------------------------------------

        #region Accessors

        /// <summary>
        /// Returns the key of this instance.
        /// </summary>
        /// <returns></returns>
        public string Key()
        {
            return Configuration?.Name;
        }

        /// <summary>
        /// Gets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name to consider.</param>
        public T Get<T>(string name)
        {
            return Configuration.GetElementObject<T>(name, _scope);
        }

        /// <summary>
        /// Gets the specified value.
        /// </summary>
        /// <param name="name">The name to consider.</param>
        public object Get(string name)
        {
            return Configuration.GetElementObject(name, _scope);
        }

        /// <summary>
        /// Gets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">The calling property name to consider.</param>
        public T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (Configuration == null) return default;

            if (propertyName != null)
            {
                IDataElement element = Configuration.GetItem(propertyName);
                if (element != null)
                {
                    return (T)Configuration.GetElementObject(propertyName, _scope);
                }
                else
                {
                    PropertyInfo propertyInfo = GetType().GetPropertyInfo(
                        propertyName,
                        new Type[] { typeof(DetailPropertyAttribute) },
                        out DataElementAttribute attribute);

                    if (attribute is DetailPropertyAttribute)
                    {
                        object value = Configuration.GetElementObject(attribute.Name, _scope);
                        if (value is T t)
                            return t;
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Gets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">The default value to consider.</param>
        /// <param name="propertyName">The calling property name to consider.</param>
        public T GetProperty<T>(T defaultValue, [CallerMemberName] string propertyName = null) where T : struct, IConvertible
        {
            if (Configuration == null) return default;

            if (propertyName != null)
            {
                IDataElement element = Configuration.GetItem(propertyName);
                if (element != null)
                {
                    return (T)Configuration.GetElementObject(propertyName, _scope);
                }
                else
                {
                    PropertyInfo propertyInfo = GetType().GetPropertyInfo(
                        propertyName,
                        new Type[] { typeof(DetailPropertyAttribute) },
                        out DataElementAttribute attribute);

                    if (attribute is DetailPropertyAttribute)
                        return (Configuration.GetElementObject(attribute.Name, _scope) as string)?.ToEnum<T>(defaultValue) ?? default;
                }
            }

            return default;
        }

        #endregion

        // ------------------------------------------
        // MUTATORS
        // ------------------------------------------

        #region Mutators

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="name">The name to consider.</param>
        /// <param name="value">The value to set.</param>
        public void Set(string name, object value)
        {
            Configuration?.AddElementItem(name, value);
        }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="propertyName">The calling property name to consider.</param>
        public void SetProperty(object value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName != null)
            {
                PropertyInfo propertyInfo = GetType().GetPropertyInfo(
                    propertyName,
                    new Type[] { typeof(DetailPropertyAttribute) },
                    out DataElementAttribute attribute);

                if (attribute is DetailPropertyAttribute)
                {
                    Configuration.AddElement(ElementFactory.CreateScalar(attribute.Name, propertyInfo.PropertyType.GetValueType(), value));
                }
            }
        }

        #endregion

        /// <summary>
        /// Loads the application settings of this instance.
        /// </summary>
        /// <param name="filePath">The file path to consider.</param>
        /// <param name="specificationLevels">The specification levels to consider.</param>
        /// <param name="specificationSet">The specification set to consider.</param>
        /// <param name="scope">The application scope to consider.</param>
        /// <param name="scriptVariableSet">The script variable set to consider.</param>
        /// <param name="xmlSchemaSet">The XML schema set to consider for checking.</param>
        /// <returns>Returns the loading log.</returns>
        public virtual ILog UpdateFromFile(
            string filePath,
            SpecificationLevels[] specificationLevels = null,
            IDataElementSpecSet specificationSet = null,
            IAppScope scope = null,
            IScriptVariableSet scriptVariableSet = null,
            XmlSchemaSet xmlSchemaSet = null)
        => new Log();
    }
}
