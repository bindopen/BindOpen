﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BindOpen.Framework.Tests.TestConsole
{
    /// <summary>
    /// This class represents an application client.
    /// </summary>
    public class XmlContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            this.ConfigureProperty(member, property);
            return property;
        }

        private void ConfigureProperty(MemberInfo member, JsonProperty property)
        {
            Attribute attribute = null;

            if ((Attribute.IsDefined(member, typeof(XmlIgnoreAttribute), true))
                || (member?.Name.EndsWith("Specified", StringComparison.OrdinalIgnoreCase) == true))
            {
                property.Ignored = true;
            }
            else
            {
                object defaultValue = null;
                if ((attribute = Attribute.GetCustomAttribute(member, typeof(DefaultValueAttribute))) != null)
                {
                    defaultValue = (attribute as DefaultValueAttribute)?.Value;
                }

                property.ShouldSerialize =
                    instance =>
                    {
                        bool? isSpecified = instance.GetType().GetProperty(member?.Name + "Specified")?.GetValue(instance) as bool?;
                        if (defaultValue != null && (isSpecified ?? true == true))
                        {
                            isSpecified = !String.Equals(instance.GetType().GetProperty(member?.Name)?.GetValue(instance)?.ToString(), defaultValue?.ToString(), StringComparison.InvariantCultureIgnoreCase);
                        }
                        return isSpecified ?? true;
                    };

                if ((attribute = Attribute.GetCustomAttribute(member, typeof(XmlAttributeAttribute))) != null)
                {
                    property.PropertyName = (attribute as XmlAttributeAttribute)?.AttributeName;
                }
                else if ((attribute = member.GetCustomAttributes(typeof(XmlArrayElementAttribute)).FirstOrDefault()) != null)
                {
                    property.PropertyName = (attribute as XmlArrayElementAttribute)?.ElementName;
                }
                else if ((attribute = member.GetCustomAttributes(typeof(XmlElementAttribute)).FirstOrDefault(p =>
                {
                    var typedP = p as XmlElementAttribute;
                    return typedP.Type == null || (typedP.Type == property.PropertyType);
                })) != null)
                {
                    property.PropertyName = (attribute as XmlElementAttribute)?.ElementName;
                }
                else if ((attribute = Attribute.GetCustomAttribute(member, typeof(XmlArrayAttribute))) != null)
                {
                    property.PropertyName = (attribute as XmlArrayAttribute)?.ElementName;
                }
                else if ((attribute = Attribute.GetCustomAttribute(member, typeof(XmlTextAttribute))) != null)
                {
                    property.PropertyName = "#text";
                }

                if (((property.PropertyType.IsArray)
                    || (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                    && (!property.PropertyName.EndsWith("s")))
                {
                    property.PropertyName += "s";
                }
                else if (property.PropertyType.IsEnum)
                {
                    property.Converter = new StringEnumConverter();
                }
                else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    property.Converter = new IsoDateTimeConverter();
                }
            }
        }
    }
}
