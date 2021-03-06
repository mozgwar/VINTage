﻿// <copyright file="RootCommandGroup.Mac.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
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
    public partial class RootCommandGroup
    {
        /// <summary>
        /// Command for the application menu.
        /// </summary>
        public static readonly VisualRelayCommand ApplicationMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".AppMenu",
            Weight = 0,
            MenuParent = RootMenuCommand
        };

        /// <summary>
        /// General data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return null; }
        }

        /// <summary>
        /// Creates the visual for a command, if applicable.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <returns>The visual for the command.</returns>
        public override NSObject CreateVisualForCommand(ICommand command)
        {
            var window = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
            var visualCommand = (VisualRelayCommand)command;
            var rootMenu = (NSMenu)RootMenuCommand.Visual;
            NSObject visual = null;
            NSMenuItem menuItem = null;
            switch (visualCommand.UniqueId)
            {
                case UniqueNameBase + ".Root":
                    if (window != null)
                    {
                        visual = window.Toolbar;
                    }
                    break;
                case UniqueNameBase + ".RootMenu":
                    if (window != null)
                    {
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Current.Menu;
                    }
                    break;
                case UniqueNameBase + ".AppMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Application);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".FileMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.File);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".EditMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Edit);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".ViewMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.View);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".ToolsMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Tools);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".WindowMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Window);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".HelpMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Help);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".Separator":
                    visual = NSMenuItem.SeparatorItem;
                    break;
                default:
                    visual = rootMenu.ItemWithTag(command.GetHashCode());
                    if (visual == null)
                    {
                        visual = CreateMenuItemForCommand(command);
                    }
                    break;
            }
            if ((menuItem != null) && (menuItem.RepresentedObject == null))
            {
                menuItem.RepresentedObject = new NSObjectWrapper<ICommand>(command);
            }
            return visual;
        }

        #region ICommandGroup

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item must be created.</param>
        /// <returns>The menu item.</returns>
        public override OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            var visualCommand = (VisualRelayCommand)command;
            if (visualCommand.Visual == null)
            {
                visualCommand.Visual = base.CreateMenuItemForCommand(command);
            }
            return visualCommand.Visual as NSMenuItem;
        }

        #region CommandGroup

        /// <summary>
        /// Attaches an event handler for a visual's 'Activated' event.
        /// </summary>
        /// <param name="command">The command whose <see cref=">ICommand.Execute"/> method should be called from the visual's 'Activated' event handler.</param>
        /// <param name="visual">The visual that will execute the given <paramref name="command"/>.</param>
        /// <remarks>In most cases, the MonoMac bindings to controls and other visual entities have added convenience 'Activated' events. These are simpler
        /// to use in C# than the traditional Cocoa 'Action' callbacks. This stock implementation handles NSControl, NSMenuItem, and NSToolbarItem.</remarks>
        protected override void AttachActivateHandler(RelayCommand command, NSObject visual)
        {
            // we do not need to attach handlers to these commands.
        }

        /// <summary>
        /// Attaches an event handler for the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="command">The command for which a 'CanExecute' event handler should be attached.</param>
        internal override void AttachCanExecuteChangeHandler(RelayCommand command)
        {
            // we do not need to attach can execute handlers to these
        }

        #endregion // CommandGroup

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands()
        {
            CommandList.Add(FileMenuCommand);
            CommandList.Add(EditMenuCommand);
            CommandList.Add(ViewMenuCommand);
            CommandList.Add(ToolsMenuCommand);
            CommandList.Add(WindowMenuCommand);
            CommandList.Add(HelpMenuCommand);
        }

        #endregion // ICommandGroup
    }
}
