﻿using BindOpen.Data.Items;
using System.Collections.Generic;

namespace BindOpen.Data.Specification
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataValueFilter : IDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        List<string> AddedValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        List<string> RemovedValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allValues"></param>
        /// <returns></returns>
        List<string> GetValues(List<string> allValues = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="allValues"></param>
        /// <returns></returns>
        bool IsValueAllowed(string value, List<string> allValues = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allValues"></param>
        void Repair(List<string> allValues = null);
    }
}