﻿// <copyright file="RomFeaturesConfigurationViewModel.WPF.cs" company="INTV Funhouse">
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

using System;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class RomFeaturesConfigurationViewModel
    {
        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        /// <param name="page">The features page for which a visual is being created.</param>
        partial void PreparePage(IRomFeaturesConfigurationPage page)
        {
            var pageVisual = page.CreateVisual();
            pageVisual.UseLayoutRounding = true;
            pageVisual.UpdateLayout();
            pageVisual.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            pageVisual.Arrange(new System.Windows.Rect(new System.Windows.Size(2000, 2000)));
            DesiredWidth = Math.Max(DesiredWidth, pageVisual.DesiredSize.Width);
            DesiredHeight = Math.Max(DesiredHeight, pageVisual.DesiredSize.Height);
        }
    }
}
