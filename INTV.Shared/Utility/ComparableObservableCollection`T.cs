﻿// <copyright file="ComparableObservableCollection`T.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Implements a comparable, observable collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection, which must be comparable.</typeparam>
    public class ComparableObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, System.IComparable<ComparableObservableCollection<T>>, System.IComparable where T : System.IComparable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ComparableObservableCollection class.
        /// </summary>
        public ComparableObservableCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComparableObservableCollection class that contains
        /// elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public ComparableObservableCollection(System.Collections.Generic.IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComparableObservableCollection class that contains
        /// elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        public ComparableObservableCollection(System.Collections.Generic.List<T> list)
            : base(list)
        {
        }

        #endregion // Constructors

        #region IComparable<ComparableObservableCollection<T>>

        /// <inheritdoc />
        public int CompareTo(ComparableObservableCollection<T> other)
        {
            var result = 1;
            if (other != null)
            {
                var numCommonElements = System.Math.Min(this.Count, other.Count);
                result = 0;
                for (int i = 0; (result == 0) && (i < numCommonElements); ++i)
                {
                    result = this[i].CompareTo(other[i]);
                }
                if (result == 0)
                {
                    result = this.Count - other.Count;
                }
            }
            return result;
        }

        #endregion // IComparable<ComparableObservableCollection<T>>

        #region IComparable

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            return CompareTo(obj as ComparableObservableCollection<T>);
        }

        #endregion // IComparable
    }
}
