﻿// <copyright file="ISettingsPageMetadata.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// This interface defines descriptive metadata for ISettingsPage implementations.
    /// </summary>
    public interface ISettingsPageMetadata
    {
        /// <summary>
        /// Gets the name of the group of settings.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the weight (in the range [0.0 - 1.0]) used to organize the pages. The higher the number,
        /// the closer to the end of the list.
        /// </summary>
        /// <remarks>Unknown if additional work would be needed for R-L languages.</remarks>
        double Weight { get; }

        /// <summary>
        /// Gets the icon to display for the settings page, if any.
        /// </summary>
        string Icon { get; }
    }
}
