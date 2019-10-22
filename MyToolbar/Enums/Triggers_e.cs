﻿//**********************
//MyToolbar - Custom toolbar manager
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestack-net-dev/my-toolbar/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/my-toolbar/
//**********************

using CodeStack.SwEx.Common.Attributes;
using System;

namespace CodeStack.Sw.MyToolbar.Enums
{
    [Flags]
    public enum Triggers_e
    {
        [Summary("Disabled command")]
        None = 0,

        [Summary("Invoked by clicking button in the toolbar")]
        Button = 1 << 0,

        [Title("Application Start")]
        ApplicationStart = 1 << 1,
        //ApplicationClose = 1 << 2,

        [Title("New Document")]
        DocumentNew = 1 << 2,

        [Title("Open Document")]
        DocumentOpen = 1 << 3,

        [Title("Activate Document")]
        DocumentActivated = 1 << 4,

        [Title("Save Document")]
        DocumentSave = 1 << 5,

        [Title("Close Document")]
        DocumentClose = 1 << 6,

        [Title("New Selection")]
        NewSelection = 1 << 7,

        [Title("Change Configuration")]
        ConfigurationChange = 1 << 8,

        [Title("Rebuild")]
        Rebuild = 1 << 9
    }
}
