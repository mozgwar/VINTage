﻿// <copyright file="AppDelegate.cs" company="INTV Funhouse">
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

using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Locutus.View
{
    /// <summary>
    /// The application delegate implementation.
    /// </summary>
    public partial class AppDelegate : NSApplicationDelegate
    {
        private MainWindowController mainWindowController;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.SingleInstanceApplication"/> class.
        /// </summary>
        /// <param name="handle">Handle.</param>
        /// <remarks>Called when created from unmanaged code.
        /// NOTE: Added this in an attempt to address apparently random crashes that are
        /// happening in extremely rare circumstances on application launch.</remarks>
        public AppDelegate(System.IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Locutus.View.AppDelegate"/> class.
        /// </summary>
        public AppDelegate()
        {
        }

        /// <inheritdoc />
        public override void FinishedLaunching(NSObject notification)
        {
            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(this);
        }

        /// <inheritdoc />
        /// <remarks>There is a rare, but not rare enough, case in which the Mono code
        /// crashes when calling this method. Adding this custom, no-op implementation
        /// just in case it happens to see if anything can be debugged.</remarks>
        public override void WillUpdate(NSNotification notification)
        {
        }
    }
}