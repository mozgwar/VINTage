﻿// <copyright file="ProgramDescription.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Device;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Models the description of an Intellivision program.
    /// </summary>
    public class ProgramDescription : INTV.Core.ComponentModel.ModelBase, IProgramDescription
    {
        /// <summary>
        /// Maximum length for a program name.
        /// </summary>
        public static readonly int MaxProgramNameLength = INTV.Core.Restricted.Model.Program.IntvFunhouseXmlProgramInformation.MaxProgramNameLength;

        /// <summary>
        /// Maximum length for a vendor string.
        /// </summary>
        public static readonly int MaxVendorNameLength = INTV.Core.Restricted.Model.Program.IntvFunhouseXmlProgramInformation.MaxVendorNameLength;

        /// <summary>
        /// Maximum length for a game's year.
        /// </summary>
        public static readonly int MaxYearTextLength = 4;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ProgramDescription.
        /// </summary>
        /// <param name="crc">The CRC of the program.</param>
        /// <param name="rom">The ROM file or files.</param>
        /// <param name="programInfo">Additional information about the program.</param>
        public ProgramDescription(uint crc, IRom rom, IProgramInformation programInfo)
        {
            _crc = crc;
            _rom = rom;
            _programInfo = new UserSpecifiedProgramInformation(programInfo);
            _name = _programInfo.GetNameForCrc(crc);
            _shortName = programInfo.ShortName;
            _programFiles = new ProgramSupportFiles(rom);
            var allIncompatibilities = _programInfo.Crcs.Select(c => c.Incompatibilities);
            var crcData = _programInfo.Crcs.First(c => c.Crc == crc);
            var romSpecificIncompatibilities = crcData.Incompatibilities;
            if ((romSpecificIncompatibilities != IncompatibilityFlags.None) || (allIncompatibilities.Distinct().Count() > 1))
            {
                var incompatibilities = _programInfo.Features.ToIncompatibilityFlags();
                if (romSpecificIncompatibilities != incompatibilities)
                {
                    _features = romSpecificIncompatibilities.ApplyFlagsToProgramFeatures(_programInfo.Features.Clone());
                }
                else
                {
                    _features = programInfo.Features;
                }
            }
            else
            {
                _features = programInfo.Features;
            }
        }

        /// <summary>
        /// Initializes a new instance of ProgramDescription.
        /// </summary>
        /// <remarks>This constructor exists so XmlSerializer is easy to use with this class.</remarks>
        private ProgramDescription()
        {
        }

        #endregion // Constructors

        #region Properties

        #region IProgramDescription

        /// <inheritdoc />
        public uint Crc
        {
            get { return _crc; }
            set { _crc = UpdateFromXml(value); } // only here for serialization
        }
        private uint _crc;

        /// <inheritdoc />
        public IRom Rom
        {
            get { return _rom; }
        }
        private IRom _rom;

        /// <inheritdoc />
        public IProgramInformation ProgramInformation
        {
            get { return _programInfo; }
        }
        private IProgramInformation _programInfo;

        /// <inheritdoc />
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.AssignAndUpdateProperty("Name", value.EnforceNameLength(MaxProgramNameLength, false), ref _name, (p, v) => UpdateNameFromXml(v));
                }
            }
        }
        private string _name;

        /// <inheritdoc />
        public string ShortName
        {
            get
            {
                return _shortName;
            }

            set
            {
                this.AssignAndUpdateProperty("ShortName", value.EnforceNameLength(RomInfoIndexHelpers.MaxShortNameLength, false), ref _shortName, (p, v) => UpdateShortNameFromXml(v));
            }
        }
        private string _shortName;

        /// <inheritdoc />
        public string Vendor
        {
            get
            {
                return _programInfo.Vendor;
            }

            set
            {
                _programInfo.Vendor = value.EnforceNameLength(MaxVendorNameLength, false);
                RaisePropertyChanged("Vendor");
            }
        }

        /// <inheritdoc />
        public string Year
        {
            get
            {
                return _programInfo.Year;
            }

            set
            {
                _programInfo.Year = value.EnforceNameLength(MaxYearTextLength, false);
                RaisePropertyChanged("Year");
            }
        }

        /// <inheritdoc />
        public ProgramFeatures Features
        {
            get { return _features; }
            set { AssignAndUpdateProperty("Features", value, ref _features); }
        }
        private ProgramFeatures _features;

        #endregion // IProgramDescription

        /// <summary>
        /// Gets or sets the support files for the program.
        /// </summary>
        public ProgramSupportFiles Files
        {
            get { return _programFiles; }
            set { this.AssignAndUpdateProperty("Files", value, ref _programFiles, (s, p) => UpdateRomFromXml(p)); }
        }
        private ProgramSupportFiles _programFiles;

        // UNDONE Plan to use this to produce links to INTV Funhouse pages.
#if false
        /// <summary>
        /// Gets or sets the INTV Funhouse four-character code for the program.
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { this.AssignAndUpdateProperty("Code", value, ref _code); }
        }
#endif // false

        #endregion // Properties

        /// <summary>
        /// Validate the program configuration.
        /// </summary>
        /// <param name="program">The program for which validation is performed.</param>
        /// <param name="peripherals">The peripherals attached to the system, used for compatibility checks.</param>
        /// <param name="connectedPeripheralsHistory">The peripherals that have been attached to the system over time.</param>
        /// <param name="reportMessagesChanged">If <c>true</c>, report whether any new messages should be immediately reported.</param>
        /// <returns>If the program's configuration is valid, return <c>true</c>, otherwise <c>false</c>.</returns>
        public static bool Validate(ProgramDescription program, IEnumerable<IPeripheral> peripherals, IEnumerable<IPeripheral> connectedPeripheralsHistory, bool reportMessagesChanged)
        {
            bool validSettings = false;
            program.Files.ValidateSupportFile(ProgramFileKind.Rom, program.Crc, program, peripherals, connectedPeripheralsHistory, true);
            return validSettings;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="lhs">The value on the left hand side of the operator.</param>
        /// <param name="rhs">The value on the right hand side of the operator.</param>
        /// <returns><c>true</c> if <paramref name="lhs"/> is considered to be equal to <paramref name="rhs"/>.</returns>
        public static bool operator ==(ProgramDescription lhs, ProgramDescription rhs)
        {
            bool areEqual = object.ReferenceEquals(lhs, rhs);
            if (!areEqual && !object.ReferenceEquals(lhs, null) && !object.ReferenceEquals(rhs, null))
            {
                areEqual = lhs.CompareToIProgramDescription(rhs) == 0;
            }
            return areEqual;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="lhs">The value on the left hand side of the operator.</param>
        /// <param name="rhs">The value on the right hand side of the operator.</param>
        /// <returns><c>true</c> if <paramref name="lhs"/> is considered to be NOT equal to <paramref name="rhs"/>.</returns>
        public static bool operator !=(ProgramDescription lhs, ProgramDescription rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Create a copy of the object.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        /// <remarks>Note that the underlying ProgramInformation data and support files are not deep copied -- shallow copies are used.</remarks>
        public ProgramDescription Copy()
        {
            var description = new ProgramDescription(Crc, Rom, ProgramInformation);
            description._name = _name;
            description._shortName = _shortName;
            description._programFiles = _programFiles.Copy();
            return description;
        }

        #region object Overrides

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is IProgramDescription))
            {
                return false;
            }
            return CompareToIProgramDescription((IProgramDescription)obj) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _crc.GetHashCode();
        }

        #endregion // object Overrides

        /// <summary>
        /// Compares the instance to another <see cref="IProgramDescription"/>.
        /// </summary>
        /// <param name="other">The other <see cref="IProgramDescription"/> to compare against.</param>
        /// <returns>If this instance and <paramref name="other"/> are considered equivalent, returns zero. A non-zero value indicates inequality.</returns>
        /// <remarks>Consider implementing the IComparable / IComparer / IEquatable / IEqualityComparer interfaces on ProgramDescription instead?</remarks>
        public int CompareToIProgramDescription(IProgramDescription other)
        {
            var result = 1;
            if (other != null)
            {
                // Always use strict comparer.
                result = RomComparer.GetDefaultComparerForMode(RomComparison.Strict).Compare(Rom, ProgramInformation, other.Rom, other.ProgramInformation);
            }
            return result;
        }

        private uint UpdateFromXml(uint crc)
        {
            var programInfo = ProgramInformationTable.Default.FindProgram(crc);
            if (programInfo == null)
            {
                _programInfo = new UserSpecifiedProgramInformation(crc);
            }
            else
            {
                _programInfo = new UserSpecifiedProgramInformation(programInfo);
            }
            return crc;
        }

        private void UpdateRomFromXml(ProgramSupportFiles supportFiles)
        {
            if ((Rom == null) && (supportFiles.Rom != Rom))
            {
                _rom = supportFiles.Rom;
            }
        }

        private void UpdateNameFromXml(string name)
        {
            var info = _programInfo as UserSpecifiedProgramInformation;
            if (info != null)
            {
                info.Title = name;
            }
        }

        private void UpdateShortNameFromXml(string shortName)
        {
            var info = _programInfo as UserSpecifiedProgramInformation;
            if (info != null)
            {
                info.ShortName = shortName;
            }
        }
    }
}
