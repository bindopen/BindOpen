﻿namespace BindOpen.Framework.Core.Data.Items.Dto
{
    /// <summary>
    /// This interface represents a described DTO.
    /// </summary>
    public interface IDescribed
    {
        // ------------------------------------------
        // PROPERTIES
        // ------------------------------------------

        #region Properties

        /// <summary>
        /// The description of this instance.
        /// </summary>
        string Description
        {
            get;
            set;
        }

        #endregion
    }
}