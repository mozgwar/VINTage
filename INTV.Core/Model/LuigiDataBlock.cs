﻿// <copyright file="LuigiDataBlock.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Base class for various LUIGI data blocks.
    /// </summary>
    public class LuigiDataBlock : INTV.Core.Utility.ByteSerializer
    {
        /// <summary>
        /// Block type size in bytes.
        /// </summary>
        private const int BlockTypeSize = sizeof(LuigiDataBlockType);

        /// <summary>
        /// Payload length size in bytes.
        /// </summary>
        private const int PayloadLengthSize = sizeof(ushort);

        /// <summary>
        /// Header checksum size in bytes.
        /// </summary>
        private const int HeaderChecksumSize = sizeof(byte);

        /// <summary>
        /// Payload checksum size in bytes.
        /// </summary>
        private const int PayloadChecksumSize = sizeof(uint);

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.LuigiDataBlock"/> class.
        /// </summary>
        /// <param name="type">The kind of block.</param>
        protected LuigiDataBlock(LuigiDataBlockType type)
        {
            Type = type;
            _deserializeByteCount = -1;
        }

        #region Properties

        #region Byteserializer

        /// <inheritdoc/>
        public override int SerializeByteCount
        {
            get { return -1; }
        }

        /// <inheritdoc/>
        public override int DeserializeByteCount
        {
            get { return _deserializeByteCount; }
        }

        private int _deserializeByteCount;

        #endregion // ByteSerializer

        /// <summary>
        /// Gets the LUIGI block type.
        /// </summary>
        public LuigiDataBlockType Type { get; private set; }

        /// <summary>
        /// Gets the length of the block's payload, in bytes.
        /// </summary>
        public ushort Length { get; private set; }

        /// <summary>
        /// Gets the CRC of the header.
        /// </summary>
        public byte HeaderCrc { get; private set; }

        /// <summary>
        /// Gets the CRC of the payload. This value will be zero if the block has no payload.
        /// </summary>
        public uint PayloadCrc { get; private set; }

        #endregion // Properties

        public static LuigiDataBlockType GetBlockType<T>()
        {
            var dataBlockType = LuigiDataBlockType.UnknownBlockType;
            if (typeof(T) == typeof(LuigiScrambleKeyBlock))
            {
                dataBlockType = LuigiDataBlockType.SetScrambleKey;
            }
            else if (typeof(T) == typeof(LuigiMemoryMapAndPermissionsTableBlock))
            {
                dataBlockType = LuigiDataBlockType.MemoryMapAndPermissionsTable;
            }
            else if (typeof(T) == typeof(LuigiDataHunkBlock))
            {
                dataBlockType = LuigiDataBlockType.DataHunk;
            }
            else if (typeof(T) == typeof(LuigiMetadataBlock))
            {
                dataBlockType = LuigiDataBlockType.Metadata;
            }
            else if (typeof(T) == typeof(LuigiEndOfFileBlock))
            {
                dataBlockType = LuigiDataBlockType.EndOfFile;
            }
            return dataBlockType;
        }

        /// <summary>
        /// Creates a new instance of a LuigiDataBlock by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a LuigiDataBlock.</returns>
        public static LuigiDataBlock Inflate(System.IO.Stream stream)
        {
            LuigiDataBlock dataBlock = null;
            using (var reader = new INTV.Core.Utility.BinaryReader(stream))
            {
                dataBlock = Inflate(reader);
            }
            return dataBlock;
        }

        /// <summary>
        /// Creates a new instance of a LuigiDataBlock by inflating it from a BinaryReader.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a LuigiDataBlock.</returns>
        /// <remarks>It is assumed that the reader is currently positioned at the beginning of a serialized LUIGI data block.</remarks>
        public static LuigiDataBlock Inflate(INTV.Core.Utility.BinaryReader reader)
        {
            LuigiDataBlock dataBlock = null;
            var blockType = (LuigiDataBlockType)reader.ReadByte();
            switch (blockType)
            {
                case LuigiDataBlockType.SetScrambleKey:
                    dataBlock = new LuigiScrambleKeyBlock();
                    break;
                case LuigiDataBlockType.MemoryMapAndPermissionsTable:
                    dataBlock = new LuigiMemoryMapAndPermissionsTableBlock();
                    break;
                case LuigiDataBlockType.DataHunk:
                    dataBlock = new LuigiDataHunkBlock();
                    break;
                case LuigiDataBlockType.Metadata:
                    dataBlock = new LuigiMetadataBlock();
                    break;
                case LuigiDataBlockType.EndOfFile:
                    dataBlock = new LuigiEndOfFileBlock();
                    break;
                default:
                    dataBlock = new LuigiDataBlock(LuigiDataBlockType.UnknownBlockType);
                    break;
            }
            dataBlock._deserializeByteCount = BlockTypeSize;
            dataBlock._deserializeByteCount += dataBlock.Deserialize(reader);
            return dataBlock;
        }

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(Core.Utility.BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <remarks>The precondition here is that the reader is positioned immediately after the LUIGI block type value.</remarks>
        protected override int Deserialize(Core.Utility.BinaryReader reader)
        {
            Length = reader.ReadUInt16();
            var bytesRead = PayloadLengthSize;
            HeaderCrc = reader.ReadByte();
            bytesRead += HeaderChecksumSize;
            if (Length > 0)
            {
                PayloadCrc = reader.ReadUInt32();
                bytesRead += PayloadChecksumSize;
                bytesRead += DeserializePayload(reader);
            }
            return bytesRead;
        }

        #endregion // ByteSerializer

        /// <summary>
        /// Deserializes the payload.
        /// </summary>
        /// <param name="reader">The binary reader containing the data to deserialize to create the object.</param>
        /// <returns>The number of bytes deserialized.</returns>
        /// <remarks>The default implementation merely advances the reader without parsing or even reading
        /// any of the payload into memory. Specific subclasses may override this implementation if
        /// any specific data from the payload is desired.</remarks>
        protected virtual int DeserializePayload(Core.Utility.BinaryReader reader)
        {
            reader.BaseStream.Seek(Length, System.IO.SeekOrigin.Current);
            return Length;
        }
    }
}
