﻿// <copyright file="FirmwareCommandGroup.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using INTV.Shared.Commands;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class FirmwareCommandGroup
    {
        #region CommandGroup

        /// <inheritdoc/>
        public override object Context
        {
            get { return null; }
        }

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands()
        {
            FirmwareGroupCommand.MenuParent = LtoFlashCommandGroup.LtoFlashGroupCommand;
            RestoreFirmwareCommand.Weight = 0.1;

            CommandList.Add(FirmwareGroupCommand);
            CommandList.Add(FirmwareGroupCommand.CreateSeparator(CommandLocation.After));
        }

        #endregion // CommandGroup
    }
}
