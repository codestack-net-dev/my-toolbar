﻿//**********************
//MyToolbar - Custom toolbar manager
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestack-net-dev/my-toolbar/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/my-toolbar/
//**********************

using CodeStack.Sw.MyToolbar.Helpers;
using CodeStack.Sw.MyToolbar.Services;
using CodeStack.Sw.MyToolbar.Structs;
using CodeStack.Sw.MyToolbar.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CodeStack.Sw.MyToolbar.UI.ViewModels
{
    public class CommandManagerVM : NotifyPropertyChanged
    {
        private ICommandVM m_SelectedElement;
        private ICommand m_SelectCommandCommand;
        private ICommand m_BrowseToolbarSpecificationCommand;

        private ICommand m_MoveCommandUpCommand;
        private ICommand m_MoveCommandDownCommand;
        private ICommand m_InsertCommandAfterCommand;
        private ICommand m_InsertCommandBeforeCommand;
        private ICommand m_CommandRemoveCommand;

        private CommandsCollection<CommandGroupVM> m_Groups;

        private readonly IToolbarConfigurationProvider m_ConfsProvider;
        private readonly ISettingsProvider m_SettsProvider;
        private readonly IMessageService m_MsgService;

        private readonly ToolbarSettings m_Settings;

        private CustomToolbarInfo m_ToolbarInfo;
        private bool m_IsEditable;

        public CommandManagerVM(IToolbarConfigurationProvider confsProvider,
            ISettingsProvider settsProvider, IMessageService msgService)
        {
            m_ConfsProvider = confsProvider;
            m_SettsProvider = settsProvider;
            m_MsgService = msgService;

            m_Settings = m_SettsProvider.GetSettings();

            LoadCommands();
        }

        private void LoadCommands()
        {
            bool isReadOnly;

            try
            {
                m_ToolbarInfo = m_ConfsProvider.GetToolbar(out isReadOnly, ToolbarSpecificationPath);
            }
            catch
            {
                isReadOnly = true;
                m_MsgService.ShowMessage("Failed to load the toolbar from the specification file. Make sure that you have access to the specification file",
                    MessageType_e.Error);
            }

            IsEditable = !isReadOnly;

            if (Groups != null)
            {
                Groups.CommandsChanged -= OnGroupsCollectionChanged;
                Groups.NewCommandCreated -= OnNewCommandCreated;
            }

            Groups = new CommandsCollection<CommandGroupVM>(
                (m_ToolbarInfo.Groups ?? new CommandGroupInfo[0])
                .Select(g => new CommandGroupVM(g)));

            HandleCommandGroupCommandCreation(Groups.Commands);

            Groups.NewCommandCreated += OnNewCommandCreated;
            Groups.CommandsChanged += OnGroupsCollectionChanged;
        }

        private void OnNewCommandCreated(ICommandVM cmd)
        {
            SelectedElement = cmd;
        }

        public bool IsEditable
        {
            get
            {
                return m_IsEditable;
            }
            private set
            {
                m_IsEditable = value;
                NotifyChanged();
            }
        }

        public CustomToolbarInfo ToolbarInfo
        {
            get
            {
                return m_ToolbarInfo;
            }
        }

        public ToolbarSettings Settings
        {
            get
            {
                return m_Settings;
            }
        }

        public CommandsCollection<CommandGroupVM> Groups
        {
            get
            {
                return m_Groups;
            }
            private set
            {
                m_Groups = value;
                NotifyChanged();
            }
        }

        public ICommandVM SelectedElement
        {
            get
            {
                return m_SelectedElement;
            }
            set
            {
                m_SelectedElement = value;
                NotifyChanged();
            }
        }

        public string ToolbarSpecificationPath
        {
            get
            {
                return Settings.SpecificationFile;
            }
            set
            {
                if (!string.Equals(value, Settings.SpecificationFile, StringComparison.CurrentCultureIgnoreCase))
                {
                    Settings.SpecificationFile = value;
                    NotifyChanged();
                    LoadCommands();
                }
            }
        }

        public ICommand BrowseToolbarSpecificationCommand
        {
            get
            {
                if (m_BrowseToolbarSpecificationCommand == null)
                {
                    m_BrowseToolbarSpecificationCommand = new RelayCommand(() =>
                    {
                        var specFile = FileBrowseHelper.BrowseFile("Select toolbar specification file",
                            new FileFilter()
                            {
                                { "Toolbar Specification File", new FileFilterExtensions("setts") }
                            }, ToolbarSpecificationPath);

                        if (!string.IsNullOrEmpty(specFile))
                        {
                            ToolbarSpecificationPath = specFile;
                        }
                    });
                }

                return m_BrowseToolbarSpecificationCommand;
            }
        }

        public ICommand SelectCommandCommand
        {
            get
            {
                if (m_SelectCommandCommand == null)
                {
                    m_SelectCommandCommand = new RelayCommand<ICommandVM>(cmd =>
                    {
                        SelectedElement = cmd;
                    });
                }

                return m_SelectCommandCommand;
            }
        }

        public ICommand MoveCommandUpCommand
        {
            get
            {
                if (m_MoveCommandUpCommand == null)
                {
                    m_MoveCommandUpCommand = new RelayCommand<ICommandVM>(x =>
                    {
                        ExceptionHelper.ExecuteUserCommand(
                            () => MoveCommand(x, true),
                            e => "Failed to move command to this position");
                    });
                }

                return m_MoveCommandUpCommand;
            }
        }

        public ICommand MoveCommandDownCommand
        {
            get
            {
                if (m_MoveCommandDownCommand == null)
                {
                    m_MoveCommandDownCommand = new RelayCommand<ICommandVM>(x =>
                    {
                        ExceptionHelper.ExecuteUserCommand(
                            () => MoveCommand(x, false),
                            e => "Failed to move command to this position");
                    });
                }

                return m_MoveCommandDownCommand;
            }
        }

        public ICommand InsertCommandAfterCommand
        {
            get
            {
                if (m_InsertCommandAfterCommand == null)
                {
                    m_InsertCommandAfterCommand = new RelayCommand<ICommandVM>(x =>
                    {
                        ExceptionHelper.ExecuteUserCommand(
                            () => InsertNewCommand(x, true),
                            e => "Failed to move insert new command in this position");
                    });
                }

                return m_InsertCommandAfterCommand;
            }
        }

        public ICommand InsertCommandBeforeCommand
        {
            get
            {
                if (m_InsertCommandBeforeCommand == null)
                {
                    m_InsertCommandBeforeCommand = new RelayCommand<ICommandVM>(x =>
                    {
                        ExceptionHelper.ExecuteUserCommand(
                            () => InsertNewCommand(x, false),
                            e => "Failed to move insert new command in this position");
                    });
                }

                return m_InsertCommandBeforeCommand;
            }
        }

        public ICommand CommandRemoveCommand
        {
            get
            {
                if (m_CommandRemoveCommand == null)
                {
                    m_CommandRemoveCommand = new RelayCommand<ICommandVM>(x =>
                    {
                        ExceptionHelper.ExecuteUserCommand(
                            () => RemoveCommand(x),
                            e => "Failed to remove command");
                    });
                }

                return m_CommandRemoveCommand;
            }
        }

        private void MoveCommand(ICommandVM cmd, bool forward)
        {
            ICommandsCollection coll;

            var index = CalculateCommandIndex(cmd, forward, out coll);

            var cmds = coll.Commands;

            if (index < 0 || index >= cmds.Count)
            {
                throw new IndexOutOfRangeException("Index is outside the boundaries of the commands collection");
            }

            cmds.Remove(cmd);
            cmds.Insert(index, cmd);
            SelectedElement = cmd;
        }

        private void InsertNewCommand(ICommandVM cmd, bool after)
        {
            ICommandsCollection coll;

            var index = CalculateCommandIndex(cmd, !after, out coll);

            if (!after)
            {
                index++;//insert to current position
            }

            var newCmd = coll.AddNewCommand(index);
        }

        private void RemoveCommand(ICommandVM cmd)
        {
            var coll = FindCommandCollection(cmd);
            coll.Commands.Remove(cmd);
        }

        private int CalculateCommandIndex(ICommandVM cmd, bool forward, out ICommandsCollection coll)
        {
            var offset = forward ? -1 : 1;
            coll = FindCommandCollection(cmd);

            var index = coll.Commands.IndexOf(cmd);

            if (index == -1)
            {
                throw new IndexOutOfRangeException("Index of the command is not found");
            }

            return index + offset;
        }

        private ICommandsCollection FindCommandCollection(ICommandVM targetCmd)
        {
            foreach (var grp in Groups.Commands)
            {
                if (grp == targetCmd)
                {
                    return Groups;
                }
                else
                {
                    foreach (var cmd in grp.Commands.Commands)
                    {
                        if (cmd == targetCmd)
                        {
                            return grp.Commands;
                        }
                    }
                }
            }

            throw new NullReferenceException("Failed to find the command");
        }

        private void OnGroupsCollectionChanged(IEnumerable<CommandGroupVM> grps)
        {
            m_ToolbarInfo.Groups = grps
                .Select(g => g.Command).ToArray();

            HandleCommandGroupCommandCreation(grps);
        }

        private void HandleCommandGroupCommandCreation(IEnumerable<CommandGroupVM> grps)
        {
            foreach (var grp in grps)
            {
                grp.Commands.NewCommandCreated -= OnNewCommandCreated;
                grp.Commands.NewCommandCreated += OnNewCommandCreated;
            }
        }
    }
}