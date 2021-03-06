﻿namespace BindOpen.Data.Elements
{
    /// <summary>
    /// This static class provides methods to create data elements.
    /// </summary>
    public static partial class ElementFactory
    {
        /// <summary>
        /// Initializes a new object element.
        /// </summary>
        /// <param name="name">The name to consider.</param>
        /// <param name="items">The items to consider.</param>
        public static ObjectElement CreateObject(
            string name,
            params object[] items)
        {
            return CreateObject(name, null, null, null, items);
        }

        /// <summary>
        /// Initializes a new object element.
        /// </summary>
        /// <param name="items">The items to consider.</param>
        public static ObjectElement CreateObject(
            params object[] items)
        {
            return CreateObject(null, null, null, null, items);
        }

        /// <summary>
        /// Initializes a new object element.
        /// </summary>
        /// <param name="name">The name to consider.</param>
        /// <param name="classFullName">The class full name to consider.</param>
        /// <param name="items">The items to consider.</param>
        public static ObjectElement CreateObject(
            string name,
            string classFullName,
            params object[] items)
        {
            return CreateObject(name, null, classFullName, null, items);
        }

        /// <summary>
        /// Initializes a new object element.
        /// </summary>
        /// <param name="name">The name to consider.</param>
        /// <param name="id">The ID to consider.</param>
        /// <param name="classFullName">The class full name to consider.</param>
        /// <param name="items">The items to consider.</param>
        public static ObjectElement CreateObject(
            string name,
            string id,
            string classFullName,
            params object[] items)
        {
            ObjectElement element = new ObjectElement(name, id)
            {
                ClassFullName = classFullName,
            };
            element.WithItems(items);

            return element;
        }
    }
}
