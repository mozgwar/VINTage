﻿// <copyright file="ProgramViewModel.Mac.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.ViewModel
{
    public partial class ProgramViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.ViewModel.ProgramViewModel"/> class.
        /// </summary>
        public ProgramViewModel()
        {
        }

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public ProgramViewModel(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Overload meaning of the Status Interface Builder binding to also mean other info... like program manual.
        /// </summary>
        [INTV.Shared.Utility.OSExport("Status")]
        public string Status
        {
            get { return Manual; }
        }
    }
}