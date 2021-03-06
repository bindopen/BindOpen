﻿using BindOpen.Data.Elements;
using System;

namespace BindOpen.Extensions.Runtime
{
    /// <summary>
    /// This class represents a output property attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskOutputAttribute : DataElementAttribute
    {
        // ------------------------------------------
        // CONSTRUCTORS
        // ------------------------------------------

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of the TaskOutputAttribute class.
        /// </summary>
        public TaskOutputAttribute() : base()
        {
        }

        #endregion

    }
}
