﻿// <copyright file="RomListCommandGroup.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class RomListCommandGroup
    {
        private int RemoveFilesSegmentIndex { get; set; }
        private int RefreshFilesSegmentIndex { get; set; }

        private RomListView RomListView
        {
            get
            {
                if (_romListView == null)
                {
                    if ((NSApplication.SharedApplication != null) && (NSApplication.SharedApplication.MainWindow != null))
                    {
                        var contentView = NSApplication.SharedApplication.MainWindow.ContentView;
                        _romListView = contentView.FindChild<RomListView>();
                    }
                }
                return _romListView;
            }
        }
        private RomListView _romListView;

        private object ProgressIndicatorViewModel
        {
            get
            {
                if (_progressIndicatorViewModel == null)
                {
                    if ((NSApplication.SharedApplication != null) && (NSApplication.SharedApplication.MainWindow != null))
                    {
                        var contentView = NSApplication.SharedApplication.MainWindow.ContentView;
                        var progressIndicator = contentView.FindChild<ProgressIndicator>();
                        if (progressIndicator != null)
                        {
                            _progressIndicatorViewModel = progressIndicator.DataContext;
                        }
                    }
                }
                return _progressIndicatorViewModel;
            }
        }
        private object _progressIndicatorViewModel;

        #region CheckRomsCommand

        /// <summary>
        /// Mac-specific implementation.
        /// </summary>
        static partial void ValidateRomsComplete()
        {
            Group.RomListView.NeedsDisplay = true;
        }

        #endregion // CheckRomsCommand

        #region EditProgramNameCommand

        private static void OnEditProgramName(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            var itemToEdit = viewModel.CurrentSelection.FirstOrDefault();
            if (itemToEdit != null)
            {
                Group.RomListView.Controller.EditProgramDescription(itemToEdit);
            }
        }

        #endregion // EditProgramNameCommand

        #region RemoveAndRefreshCommandsSegmentCommand

        /// <summary>
        /// Pseudo-command to group the Remove ROM and Refresh ROM list commands into a segment control.
        /// </summary>
        public static readonly VisualRelayCommand RemoveAndRefreshCommandsSegmentCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".RemoveAndRefreshCommandsSegmentCommand",
            Name = string.Empty,
            Weight = 0.109
        };

        #endregion // RemoveAndRefreshCommandsSegmentCommand

        #region RestoreRomListCommand

        /// <summary>
        /// Sorts the ROM list.
        /// </summary>
        public static void SortRoms()
        {
            Group.RomListView.Controller.SortRoms();
        }

        /// <summary>
        /// Mac-specific implementation.
        /// </summary>
        /// <param name="viewModel">View model.</param>
        static partial void RestoreRomListComplete(RomListViewModel viewModel)
        {
            SortRoms();
        }

        #endregion // RestoreRomListCommand

        #region EditRomFeaturesCommand

        private static void OnEditRomFeatures(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            var itemToEdit = viewModel.CurrentSelection.FirstOrDefault();
            var dialog = INTV.Shared.View.RomFeaturesConfiguration.Create(itemToEdit);
            dialog.ShowDialog(true);
        }

        #endregion // EditRomFeaturesCommand

        #region CommandGroup

        /// <summary>
        /// General data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return RomListViewModel; }
        }

        /// <summary>
        /// Creates the visual for a command, if applicable.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <returns>The visual for the command.</returns>
        public override NSObject CreateVisualForCommand(ICommand command)
        {
            var visual = base.CreateVisualForCommand(command);
            return visual;
        }

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item must be created.</param>
        /// <returns>The menu item.</returns>
        public override OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            var menuItem = base.CreateMenuItemForCommand(command);
            var relayCommand = command as RelayCommand;
            if (relayCommand.UniqueId == RomListCommandGroup.ShowRomInfoCommand.UniqueId)
            {
                var showDetails = Properties.Settings.Default.ShowRomDetails;
                menuItem.NativeMenuItem.State = showDetails ? NSCellStateValue.On : NSCellStateValue.Off;
            }
            return menuItem;
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="sender">A visual object associated with an <c>VisualRelayCommand</c>.</param>
        /// <param name="e">Unused argument.</param>
        protected override void HandleCommandActivated(object sender, System.EventArgs e)
        {
            var command = GetCommand(sender);
            var relayCommand = command as VisualRelayCommand;
            if (relayCommand.UniqueId == RomListCommandGroup.ShowRomInfoCommand.UniqueId)
            {
                var newState = !Properties.Settings.Default.ShowRomDetails;
                NSUserDefaults.StandardUserDefaults[RomListSettingsPageViewModel.ShowRomDetailsPropertyName] = new NSNumber(newState);
                relayCommand.MenuItem.NativeMenuItem.State = newState ? NSCellStateValue.On : NSCellStateValue.Off;
            }
            else
            {
                command.Execute(Context);
            }
        }

        /// <summary>
        /// Gets a Boolean value indicating of the given command is allowed to execute.
        /// </summary>
        /// <param name="command">The command of interest.</param>
        /// <returns><c>true</c> if the command should be allowed to execute, <c>false</c> otherwise.</returns>
        protected override bool HandleCanExecuteChangedForCommand(VisualRelayCommand command)
        {
            var context = Context;
            if (command.UniqueId == CancelRomsImportCommand.UniqueId)
            {
                context = ProgressIndicatorViewModel;
            }
            var canExecute = command.CanExecute(context);
            if (command.UniqueId == RemoveRomsCommand.UniqueId)
            {
                if (canExecute)
                {
                    NSResponder firstResponder = null;
                    if ((NSApplication.SharedApplication != null) && (NSApplication.SharedApplication.MainWindow != null))
                    {
                        firstResponder = NSApplication.SharedApplication.MainWindow.FirstResponder;
                        canExecute = firstResponder is NSTableView;
                        if (canExecute)
                        {
                            canExecute = (RomListView != null) && (RomListView.FindChild<NSTableView>() == firstResponder);
                        }
                        command.MenuItem.NativeMenuItem.KeyEquivalent = canExecute ? RemoveRomsCommand.KeyboardShortcutKey : string.Empty;
                        command.MenuItem.NativeMenuItem.KeyEquivalentModifierMask = (NSEventModifierMask)(canExecute ? RemoveRomsCommand.KeyboardShortcutModifiers : OSModifierKeys.None);
                    }
                }
            }
            return canExecute;
        }

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands()
        {
            ////CommandList.Add(RemoveAndRefreshCommandsSegmentCommand);
            AddRomFilesCommand.VisualParent = RootCommandGroup.RootCommand; // add to toolbar
            AddRomFilesCommand.MenuParent = RootCommandGroup.FileMenuCommand;

            AddRomFoldersCommand.VisualParent = RootCommandGroup.RootCommand; // add to toolbar
            AddRomFoldersCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            ////RemoveRomsCommand.VisualParent = RemoveAndRefreshCommandsSegmentCommand;

            EditRomFeaturesCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            EditProgramNameCommand.MenuParent = RootCommandGroup.EditMenuCommand;

            RomListGroupCommand.MenuParent = RootCommandGroup.EditMenuCommand;

            RemoveRomsCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            RemoveRomsCommand.SmallIcon = NSImage.ImageNamed("NSRemoveTemplate");
            RemoveRomsCommand.KeyboardShortcutKey = CommandProviderHelpers.NSBackspaceCharacterString; // delete

            ValidateRomsCommand.KeyboardShortcutKey = "r";
            ValidateRomsCommand.KeyboardShortcutModifiers = OSModifierKeys.Menu;
            ValidateRomsCommand.MenuParent = RootCommandGroup.ViewMenuCommand;
            ////RefreshRomsCommand.VisualParent = RemoveAndRefreshCommandsSegmentCommand;
            RefreshRomsCommand.MenuParent = RootCommandGroup.ViewMenuCommand;
            RefreshRomsCommand.SmallIcon = NSImage.ImageNamed("NSRefreshTemplate");
            CommandList.Add(RefreshRomsCommand.CreateSeparator(CommandLocation.After));

            ShowRomInfoCommand.MenuParent = RootCommandGroup.ViewMenuCommand;

            CommandList.Add(EditRomFeaturesCommand);
        }

        #endregion // CommandGroup
    }
}
