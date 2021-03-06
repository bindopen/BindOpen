﻿using BindOpen.Data.Items;

namespace BindOpen.Application.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAppSection : IDescribedDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        string IconFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IAppModule Module { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IAppSection Parent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Rank { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TDataItemSet<AppSection> SubSections { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string ThumbIconFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        void AddSection(IAppSection section);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        IAppSection AddSections(params AppSection[] sections);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAppSection GetSubSectionWithName(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IAppSection GetSubSectionWithUniqueName(string key);
    }
}