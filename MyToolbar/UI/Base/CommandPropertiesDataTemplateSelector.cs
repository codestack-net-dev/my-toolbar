﻿using CodeStack.Sw.MyToolbar.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CodeStack.Sw.MyToolbar.UI.Base
{
    public class CommandPropertiesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommandMacroTemplate { get; set; }
        public DataTemplate CommandGroupTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item?.GetType() == typeof(CommandMacroVM))
            {
                return CommandMacroTemplate;
            }
            if (item?.GetType() == typeof(CommandGroupVM))
            {
                return CommandGroupTemplate;
            }
            else
            {
                return DefaultTemplate;
            }
        }
    }
}
