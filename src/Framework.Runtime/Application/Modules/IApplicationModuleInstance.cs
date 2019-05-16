﻿using BindOpen.Framework.Core.Data.Common;
using BindOpen.Framework.Core.Data.Items;
using BindOpen.Framework.Core.Data.Items.Sets;
using BindOpen.Framework.Runtime.Application.Options;
using BindOpen.Framework.Runtime.System;

namespace BindOpen.Framework.Runtime.Application.Modules
{
    public interface IApplicationModuleInstance : IDescribedDataItem
    {
        string AbsoluteUri { get; }
        AccessibilityLevels AccessibilityLevel { get; set; }
        string ApplicationExecutionPath { get; }
        InstanceIndexations Indexation { get; set; }
        bool IsLocal { get; }
        ApplicationModuleKind Kind { get; set; }
        IAppModule Module { get; set; }
        string ModuleName { get; }
        IOptionSet OptionSet { get; }
        IDataItemSet<AppSection> Sections { get; set; }
        ApplicationModuleSubKind SubKind { get; set; }
        string Uri { get; }

        IAppSection GetSectionWithCompleteName(string completeName);
        IAppSection GetSectionWithName(string name);
    }
}