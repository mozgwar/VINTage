﻿// <copyright file="DeleteFile.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Implements the command to instruct the file system on a Locutus device to delete a file.
    /// </summary>
    internal sealed class DeleteFile : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 5000;

        private DeleteFile(ushort globalFileNumber)
            : base(ProtocolCommandId.LfsDeleteFile, DefaultResponseTimeout, globalFileNumber)
        {
        }

        /// <summary>
        /// Creates an instance of the DeleteFile command.
        /// </summary>
        /// <param name="globalFileNumber">The global file number of the file to delete.</param>
        /// <returns>A new instance of the command.</returns>
        public static DeleteFile Create(ushort globalFileNumber)
        {
            return new DeleteFile(globalFileNumber);
        }
    }
}
