﻿using System;
using System.Linq;
using System.Reflection;
using BindOpen.Framework.Core.Data.Elements.Factories;
using BindOpen.Framework.Core.Extensions.Attributes;
using BindOpen.Framework.Core.Extensions.Items.Connectors;
using BindOpen.Framework.Core.Extensions.Indexes.Connectors;
using BindOpen.Framework.Core.Extensions.Items.Connectors;
using BindOpen.Framework.Core.System.Diagnostics;
using BindOpen.Framework.Core.Extensions.Items.Connectors.Definition;

namespace BindOpen.Framework.Core.Extensions.Libraries
{
    /// <summary>
    /// This static class provices methods to index library items.
    /// </summary>
    public static partial class LibraryIndexer
    {
        /// <summary>
        /// References the specified connectors into the specified library.
        /// </summary>
        /// <param name="library">The library to consider.</param>
        /// <param name="assembly">The assembly to consider.</param>
        /// <param name="indexDto">The script word index to consider.</param>
        /// <param name="log">The log to consider.</param>
        /// <returns></returns>
        public static int IndexConnectors(
            this ILibrary library,
            Assembly assembly,
            ConnectorIndexDto indexDto = null,
            ILog log = null)
        {
            if ((library == null) || (assembly == null))
            {
                return -1;
            }

            log = log ?? new Log();

            // we feach connector classes

            var types = assembly.GetTypes().Where(p => typeof(IConnector).IsAssignableFrom(p) && !p.IsAbstract);
            int count = 0;
            foreach (Type type in types)
            {
                IConnectorDefinitionDto definitionDto = new ConnectorDefinitionDto();

                // we update definition from connector attribute

                if (type.GetCustomAttribute(typeof(ConnectorAttribute)) is ConnectorAttribute connectorAttribute)
                {
                    (definitionDto).Update(connectorAttribute);
                }
                definitionDto.ItemClass = type.FullName;
                definitionDto.LibraryName = library?.Name;

                // we create the detail specification from detail property attributes

                foreach (PropertyInfo property in type.GetProperties().Where(p => p.GetCustomAttribute(typeof(DetailPropertyAttribute)) != null))
                {
                    definitionDto.DatasourceDetailSpec.Add(ElementSpecFactory.Create(property.Name, property.PropertyType));
                }

                // we build the runtime definition

                IConnectorDefinition definition = new ConnectorDefinition(library, definitionDto)
                {
                    RuntimeType = type
                };

                if (indexDto != null)
                {
                    // retrieve the definition index

                    // update definition with index
                }

                library.Add<IConnectorDefinition>(definition);

                count++;
            }

            return count;
        }
    }
}