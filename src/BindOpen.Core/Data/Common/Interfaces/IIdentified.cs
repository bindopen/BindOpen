﻿namespace BindOpen.Data.Common
{
    /// <summary>
    /// This interface represents an identified data.
    /// </summary>
    public interface IIdentified
    {
        /// <summary>
        /// ID of this instance.
        /// </summary>
        string Id
        {
            get;
            set;
        }
    }
}
