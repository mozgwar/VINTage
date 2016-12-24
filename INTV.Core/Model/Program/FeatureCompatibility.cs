﻿// <copyright file="FeatureCompatibility.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// These values describe the level of compatibility a program has for a certain peripheral.
    /// In most cases, programs tolerate the presence (or absence) of a peripheral in the system.
    /// </summary>
    public enum FeatureCompatibility : uint
    {
        /// <summary>
        /// Program is incompatible with a peripheral.
        /// </summary>
        Incompatible = 0,

        /// <summary>
        /// Program tolerates the presence of a peripheral, but its presence
        /// neither enhances nor detracts from the experience.
        /// </summary>
        Tolerates,

        /// <summary>
        /// Program has enhanced features when the peripheral is present,
        /// but will function without it.
        /// </summary>
        Enhances,

        /// <summary>
        /// Program requires the peripheral to function correctly.
        /// </summary>
        Requires,

        /// <summary>
        /// Sentinel value for number of compatibility modes.
        /// </summary>
        NumCompatibilityModes,
    }

    /// <summary>
    /// Extension methods for FeatureCompatibility.
    /// </summary>
    public static class FeatureCompatibilityHelpers
    {
        /// <summary>
        /// Mask to use to retrieve basic compatibility mask from larger flag sets.
        /// </summary>
        public const uint CompatibilityMask = 0x03;

        /// <summary>
        /// Valid flags.
        /// </summary>
        public const FeatureCompatibility ValidFeaturesMask = (FeatureCompatibility)CompatibilityMask;

        /// <summary>
        /// Converts standard FeatureCompatibility to LuigiFeatureFlags.
        /// </summary>
        /// <param name="compatibility">The compatibility to convert.</param>
        /// <param name="category">The category of feature represented by the compatibility argument.</param>
        /// <returns>The compatibility represented as appropriate LuigiFeatureFlags.</returns>
        public static LuigiFeatureFlags ToLuigiFeatureFlags(this FeatureCompatibility compatibility, FeatureCategory category)
        {
            var luigiFeatureFlags = LuigiFeatureFlags.None;
            var offset = -1;
            switch (category)
            {
                case FeatureCategory.Intellivoice:
                    offset = LuigiFeatureFlagsHelpers.IntellivoiceOffset;
                    break;
                case FeatureCategory.Ecs:
                    offset = LuigiFeatureFlagsHelpers.EcsOffset;
                    break;
                case FeatureCategory.IntellivisionII:
                    // Intellivision II doesn't really 'enhance' anything. Ignore that value.
                    if (compatibility != FeatureCompatibility.Enhances)
                    {
                        offset = LuigiFeatureFlagsHelpers.IntellivisionIIOffset;
                    }
                    break;
                case FeatureCategory.Jlp:
                    offset = LuigiFeatureFlagsHelpers.JlpAccelerationOffset;
                    break;
                case FeatureCategory.LtoFlash:
                    throw new System.InvalidOperationException("No LTO Flash!-specific features enabled at this time.");
                default:
                    throw new System.InvalidOperationException("Attempted to create LUIGI feature flags for unsupported FeatureCategory: " + category);
            }
            if (offset >= 0)
            {
                luigiFeatureFlags = (LuigiFeatureFlags)((ulong)compatibility << offset);
            }

            return luigiFeatureFlags;
        }
    }
}
