﻿// <copyright file="Crc8.cs" company="INTV Funhouse">
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// Provides methods to compute an 8-bit DOW CRC value.
    /// </summary>
    /// <remarks>Based on the method by Koopman and Chakravarty. Generation of the data table
    /// adapted from the crc_calc.hpp header in the Locutus sources from jzIntv.</remarks>
    public static class Crc8
    {
        /// <summary>
        /// The initial value to use for a CRC8 (DOW).
        /// </summary>
        public const byte InitialValue = 0;

        /// <summary>
        /// Lookup table used for the CRC-24 code.
        /// </summary>
        private static readonly byte[] Crc8Table =
        {
            0x00, 0x34, 0x68, 0x5c, 0xd0, 0xe4, 0xb8, 0x8c, 0x91, 0xa5, 0xf9, 0xcd, 0x41, 0x75, 0x29, 0x1d,
            0x13, 0x27, 0x7b, 0x4f, 0xc3, 0xf7, 0xab, 0x9f, 0x82, 0xb6, 0xea, 0xde, 0x52, 0x66, 0x3a, 0x0e,
            0x26, 0x12, 0x4e, 0x7a, 0xf6, 0xc2, 0x9e, 0xaa, 0xb7, 0x83, 0xdf, 0xeb, 0x67, 0x53, 0x0f, 0x3b,
            0x35, 0x01, 0x5d, 0x69, 0xe5, 0xd1, 0x8d, 0xb9, 0xa4, 0x90, 0xcc, 0xf8, 0x74, 0x40, 0x1c, 0x28,
            0x4c, 0x78, 0x24, 0x10, 0x9c, 0xa8, 0xf4, 0xc0, 0xdd, 0xe9, 0xb5, 0x81, 0x0d, 0x39, 0x65, 0x51,
            0x5f, 0x6b, 0x37, 0x03, 0x8f, 0xbb, 0xe7, 0xd3, 0xce, 0xfa, 0xa6, 0x92, 0x1e, 0x2a, 0x76, 0x42,
            0x6a, 0x5e, 0x02, 0x36, 0xba, 0x8e, 0xd2, 0xe6, 0xfb, 0xcf, 0x93, 0xa7, 0x2b, 0x1f, 0x43, 0x77,
            0x79, 0x4d, 0x11, 0x25, 0xa9, 0x9d, 0xc1, 0xf5, 0xe8, 0xdc, 0x80, 0xb4, 0x38, 0x0c, 0x50, 0x64,
            0x98, 0xac, 0xf0, 0xc4, 0x48, 0x7c, 0x20, 0x14, 0x09, 0x3d, 0x61, 0x55, 0xd9, 0xed, 0xb1, 0x85,
            0x8b, 0xbf, 0xe3, 0xd7, 0x5b, 0x6f, 0x33, 0x07, 0x1a, 0x2e, 0x72, 0x46, 0xca, 0xfe, 0xa2, 0x96,
            0xbe, 0x8a, 0xd6, 0xe2, 0x6e, 0x5a, 0x06, 0x32, 0x2f, 0x1b, 0x47, 0x73, 0xff, 0xcb, 0x97, 0xa3,
            0xad, 0x99, 0xc5, 0xf1, 0x7d, 0x49, 0x15, 0x21, 0x3c, 0x08, 0x54, 0x60, 0xec, 0xd8, 0x84, 0xb0,
            0xd4, 0xe0, 0xbc, 0x88, 0x04, 0x30, 0x6c, 0x58, 0x45, 0x71, 0x2d, 0x19, 0x95, 0xa1, 0xfd, 0xc9,
            0xc7, 0xf3, 0xaf, 0x9b, 0x17, 0x23, 0x7f, 0x4b, 0x56, 0x62, 0x3e, 0x0a, 0x86, 0xb2, 0xee, 0xda,
            0xf2, 0xc6, 0x9a, 0xae, 0x22, 0x16, 0x4a, 0x7e, 0x63, 0x57, 0x0b, 0x3f, 0xb3, 0x87, 0xdb, 0xef,
            0xe1, 0xd5, 0x89, 0xbd, 0x31, 0x05, 0x59, 0x6d, 0x70, 0x44, 0x18, 0x2c, 0xa0, 0x94, 0xc8, 0xfc
        };

#if false
        private static string makeTable(byte poly /* = 0x98 */)
        {
            var sb = new StringBuilder();

            for (uint i = 0; i < 256; ++i)
            {
                var v = i;
                for (int j = 0; j < 8; ++j)
                {
                    if ((v & 1) == 0)
                    {
                        v = v >> 1;
                    }
                    else
                    {
                        v = (v >> 1) ^ poly;
                    }
                }
                sb.Append("0x" + v.ToString("x2") + ", ");
                if (((i + 1) % 16) == 0)
                {
                    sb.AppendLine();
                }
            }

            var s = sb.ToString();
            return s;
        }
#endif

        /// <summary>
        /// Accumulates a CRC value using the internally specified lookup table.
        /// </summary>
        /// <param name="crc">The current CRC value.</param>
        /// <param name="data">The new data to incorporate into the CRC.</param>
        /// <returns>The new CRC value.</returns>
        public static byte Update(byte crc, byte data)
        {
            var newCrc = crc ^ data;
            newCrc = (newCrc >> 8) ^ Crc8Table[newCrc & 0xFF];
            return (byte)(newCrc & 0xFF);
        }

        /// <summary>
        /// Compute a CRC value for a block of data.
        /// </summary>
        /// <param name="data">The block of data upon which to compute a CRC.</param>
        /// <returns>The 8-bit CRC of the data.</returns>
        public static byte OfBlock(byte[] data)
        {
            byte crc = InitialValue;

            foreach (var value in data)
            {
                crc = Update(crc, value);
            }

            return (byte)(crc & 0xff);
        }
    }
}