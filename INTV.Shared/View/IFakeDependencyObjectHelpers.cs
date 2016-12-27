﻿// <copyright file="IFakeDependencyObjectHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using System.Collections.Generic;
using System.ComponentModel;
using INTV.Core.Utility;

#if MAC
#if __UNIFIED
using OSVisual = AppKit.NSView;
using OSVisualBase = AppKit.NSResponder;
using OSWindow = AppKit.NSWindow;
#else
using OSVisual = MonoMac.AppKit.NSView;
using OSVisualBase = MonoMac.AppKit.NSResponder;
using OSWindow = MonoMac.AppKit.NSWindow;
#endif
#endif

namespace INTV.Shared.View
{
    /// <summary>
    /// Extension methods for sort of kind of DependencyObject-like properties that can also assist
    /// with implementing IFakeDependencyObject.
    /// </summary>
    public static class IFakeDependencyObjectHelpers
    {
        /// <summary>
        /// Name used for a DataContext-like fake attached property.
        /// </summary>
        public const string DataContextPropertyName = "DataContext";

        private static WeakKeyDictionary<object, Dictionary<string, object>> AttachedProperties
        {
            get
            {
                if (_masterAttachedPropertiesList == null)
                {
                    _masterAttachedPropertiesList = new WeakKeyDictionary<object, Dictionary<string, object>>();
                }
                return _masterAttachedPropertiesList;
            }
        }
        private static WeakKeyDictionary<object, Dictionary<string, object>> _masterAttachedPropertiesList;

        /// <summary>
        /// Gets a named property's value.
        /// </summary>
        /// <typeparam name="T">The data type of the property.</typeparam>
        /// <param name="o">The object on which to read the property.</param>
        /// <param name="property">The name of the property to get a value for.</param>
        /// <returns>The value of the property, or its default if not found.</returns>
        public static T GetValue<T>(this object o, string property)
        {
            var value = default(T);
            object rawValue = null;
            if (o.GetValue(property, out rawValue))
            {
                value = (T)rawValue;
            }
            return value;
        }

        /// <summary>
        /// Gets a named property's value.
        /// </summary>
        /// <param name="o">The object on which to read the property.</param>
        /// <param name="property">The name of the property to get a value for.</param>
        /// <returns>The value of the property, or <c>null</c> if not found.</returns>
        public static object GetValue(this object o, string property)
        {
            object value = null;
            o.GetValue(property, out value);
            return value;
        }

        private static bool GetValue(this object o, string property, out object value)
        {
            bool gotValue = false;
            value = null;
            var properties = AttachedProperties.GetEntry(o);
            if (properties != null)
            {
                gotValue = properties.TryGetValue(property, out value);
            }
            return gotValue;
        }

        /// <summary>
        /// Gets a named property's value, moving up the visual's parent chain if not directly on the given visual.
        /// </summary>
        /// <typeparam name="T">The data type of the property.</typeparam>
        /// <param name="visual">The visual on which to read the property.</param>
        /// <param name="property">The name of the property to get a value for.</param>
        /// <returns>The value of the property, or its default if not found.</returns>
        public static T GetInheritedValue<T>(this OSVisualBase visual, string property)
        {
            var value = default(T);
            object rawValue = null;
            if (visual.GetInheritedValue(property, out rawValue))
            {
                value = (T)rawValue;
            }
            return value;
        }

        /// <summary>
        /// Gets a named property's value, moving up the visual's parent chain if not directly on the given visual.
        /// </summary>
        /// <param name="visual">The visual on which to read the property.</param>
        /// <param name="property">The name of the property to get a value for.</param>
        /// <returns>The value of the property, or <c>null</c> if not found.</returns>
        public static object GetInheritedValue(this OSVisualBase visual, string property)
        {
            object value = null;
            visual.GetInheritedValue(property, out value);
            return value;
        }

        private static bool GetInheritedValue(this OSVisualBase visual, string property, out object propertyValue)
        {
            bool gotValue = false;
            propertyValue = null;
            var window = visual as OSWindow;
            var view = visual as OSVisual;
            OSVisualBase root = window;
            if (root == null)
            {
                if (view != null)
                {
                    root = view.Window;
                }
                if (root == null)
                {
                    root = INTV.Shared.Utility.SingleInstanceApplication.Current;
                }
            }
            while ((visual != null) && !gotValue)
            {
                var properties = AttachedProperties.GetEntry(visual);
                if (properties != null)
                {
                    gotValue = properties.TryGetValue(property, out propertyValue);
                }
                if (!gotValue)
                {
                    if (view != null)
                    {
                        window = view.Window;
                        visual = view.Superview;
                        view = visual as OSVisual;
                        if (view == null)
                        {
                            visual = window;
                        }
                        if (visual == null)
                        {
                            visual = INTV.Shared.Utility.SingleInstanceApplication.Current;
                        }
                    }
                    else if (window != null)
                    {
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Current;
                        window = null;
                    }
                    else
                    {
                        visual = null;
                    }
                }
            }
            return gotValue;
        }

        /// <summary>
        /// Gets the property value as a fake attached property.
        /// </summary>
        /// <param name="d">The object upon which to set a value.</param>
        /// <param name="property">The name of the property to set.</param>
        /// <returns>The property value, or <c>null</c> if not found.</returns>
        public static object GetPropertyValue(this IFakeDependencyObject d, string property)
        {
            object value = null;
            var visual = d as OSVisualBase;
            if (visual != null)
            {
                visual.GetInheritedValue(property, out value);
            }
            else
            {
                d.GetValue(property, out value);
            }
            return value;
        }

        /// <summary>
        /// Sets a property's value on the given object.
        /// </summary>
        /// <param name="o">The object upon which a value is to be set.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public static void SetValue(this object o, string propertyName, object value)
        {
            o.SetPropertyValue(propertyName, value, null);
        }

        /// <summary>
        /// Sets a property's value on the given object, optionally calling a property changed handler if the value changes.
        /// </summary>
        /// <param name="o">The object upon which a value is to be set.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="propertyChanged">If not <c>null</c>, this method is invoked if the value of <paramref name="value"/> is changed.</param>
        public static void SetValue(this object o, string propertyName, object value, PropertyChangedEventHandler propertyChanged)
        {
            o.SetPropertyValue(propertyName, value, propertyChanged);
        }

        /// <summary>
        /// Sets a property's value on the given object.
        /// </summary>
        /// <param name="o">The object upon which to set the property.</param>
        /// <param name="property">The name of the property to set.</param>
        /// <param name="propertyValue">The value of the property.</param>
        public static void SetPropertyValue(this object o, string property, object propertyValue)
        {
            o.SetPropertyValue(property, propertyValue, null);
        }

        /// <summary>
        /// Sets a property's value on the given object, optionally calling a property changed handler if the value changes.
        /// </summary>
        /// <param name="o">The object upon which a value is to be set.</param>
        /// <param name="property">The name of the property to set.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <param name="propertyChanged">If not <c>null</c>, this method is invoked if the value of <paramref name="value"/> is changed.</param>
        public static void SetPropertyValue(this object o, string property, object propertyValue, PropertyChangedEventHandler propertyChanged)
        {
            object oldValue = null;
            if (propertyChanged != null)
            {
                oldValue = o.GetValue(property);
            }
            var properties = AttachedProperties.GetEntry(o);
            if (properties == null)
            {
                properties = new Dictionary<string, object>();
                AttachedProperties[o] = properties;
            }
            properties[property] = propertyValue;
            if ((propertyChanged != null) && INTV.Core.ComponentModel.INotifyPropertyChangedHelpers.SafeDidValueChangeCompare(oldValue, propertyValue))
            {
                propertyChanged(o, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Gets the special DataContext property.
        /// </summary>
        /// <param name="d">A fake 'DependencyObject'-like object from which to read the DataContext property.</param>
        /// <returns>The value of the DataContext property.</returns>
        public static object GetDataContext(this IFakeDependencyObject d)
        {
            return d.GetPropertyValue(DataContextPropertyName);
        }

        /// <summary>
        /// Sets the special DataContext property.
        /// </summary>
        /// <param name="d">A fake 'DependencyObject'-like object upon which to set the DataContext property.</param>
        /// <param name="value">The value of the DataContext.</param>
        public static void SetDataContext(this IFakeDependencyObject d, object value)
        {
            d.SetPropertyValue(DataContextPropertyName, value, null);
        }

        /// <summary>
        /// Sets the special DataContext property.
        /// </summary>
        /// <param name="d">A fake 'DependencyObject'-like object upon which to set the DataContext property.</param>
        /// <param name="value">The value of the DataContext.</param>
        /// <param name="propertyChanged">If not <c>null</c>, this method is invoked if the value of <paramref name="value"/> is changed.</param>
        public static void SetDataContext(this IFakeDependencyObject d, object value, PropertyChangedEventHandler propertyChanged)
        {
            d.SetPropertyValue(DataContextPropertyName, value, propertyChanged);
        }

        /// <summary>
        /// Sets the special DataContext property.
        /// </summary>
        /// <param name="d">A fake 'DependencyObject'-like object upon which to set the DataContext property.</param>
        /// <param name="value">The value of the DataContext.</param>
        /// <param name="propertyChanged">Adds this event handler to the <paramref name="value"/>'s PropertyChanged event. This requires that
        /// the type of <paramref name="value"/> implement the <see cref="INotifyPropertyChanged"/> interface.</param>
        public static void SetDataContextWithDataContextPropertyChangedHandler(this IFakeDependencyObject d, object value, PropertyChangedEventHandler propertyChanged)
        {
            d.SetDataContext(value);
            ((INotifyPropertyChanged)value).PropertyChanged += propertyChanged;
        }
    }
}
