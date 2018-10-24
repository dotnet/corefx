// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
    class XTypeDescriptionProvider<T> : TypeDescriptionProvider
    {
        public XTypeDescriptionProvider() : base(TypeDescriptor.GetProvider(typeof(T)))
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type type, object instance)
        {
            return new XTypeDescriptor<T>(base.GetTypeDescriptor(type, instance));
        }
    }

    class XTypeDescriptor<T> : CustomTypeDescriptor
    {
        public XTypeDescriptor(ICustomTypeDescriptor parent) : base(parent)
        {
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
            if (attributes == null)
            {
                if (typeof(T) == typeof(XElement))
                {
                    properties.Add(new XElementAttributePropertyDescriptor());
                    properties.Add(new XElementDescendantsPropertyDescriptor());
                    properties.Add(new XElementElementPropertyDescriptor());
                    properties.Add(new XElementElementsPropertyDescriptor());
                    properties.Add(new XElementValuePropertyDescriptor());
                    properties.Add(new XElementXmlPropertyDescriptor());
                }
                else if (typeof(T) == typeof(XAttribute))
                {
                    properties.Add(new XAttributeValuePropertyDescriptor());
                }
            }
            foreach (PropertyDescriptor property in base.GetProperties(attributes))
            {
                properties.Add(property);
            }
            return properties;
        }
    }

    abstract class XPropertyDescriptor<T, TProperty> : PropertyDescriptor where T : XObject
    {
        public XPropertyDescriptor(string name) : base(name, null)
        {
        }

        public override Type ComponentType
        {
            get { return typeof(T); }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return typeof(TProperty); }
        }

        public override bool SupportsChangeEvents
        {
            get { return true; }
        }

        public override void AddValueChanged(object component, EventHandler handler)
        {
            bool hasValueChangedHandler = GetValueChangedHandler(component) != null;
            base.AddValueChanged(component, handler);
            if (hasValueChangedHandler)
                return;
            T c = component as T;
            if (c != null && GetValueChangedHandler(component) != null)
            {
                c.Changing += new EventHandler<XObjectChangeEventArgs>(OnChanging);
                c.Changed += new EventHandler<XObjectChangeEventArgs>(OnChanged);
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            base.RemoveValueChanged(component, handler);
            T c = component as T;
            if (c != null && GetValueChangedHandler(component) == null)
            {
                c.Changing -= new EventHandler<XObjectChangeEventArgs>(OnChanging);
                c.Changed -= new EventHandler<XObjectChangeEventArgs>(OnChanged);
            }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        protected virtual void OnChanged(object sender, XObjectChangeEventArgs args)
        {
        }

        protected virtual void OnChanging(object sender, XObjectChangeEventArgs args)
        {
        }
    }

    class XElementAttributePropertyDescriptor : XPropertyDescriptor<XElement, object>
    {
        XDeferredSingleton<XAttribute> value;
        XAttribute changeState;

        public XElementAttributePropertyDescriptor() : base("Attribute")
        {
        }

        public override object GetValue(object component)
        {
            return value = new XDeferredSingleton<XAttribute>((e, n) => e.Attribute(n), component as XElement, null);
        }

        protected override void OnChanged(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Add:
                    XAttribute a = sender as XAttribute;
                    if (a != null && value.element == a.Parent && value.name == a.Name)
                    {
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
                case XObjectChange.Remove:
                    a = sender as XAttribute;
                    if (a != null && changeState == a)
                    {
                        changeState = null;
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
            }
        }

        protected override void OnChanging(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Remove:
                    XAttribute a = sender as XAttribute;
                    changeState = a != null && value.element == a.Parent && value.name == a.Name ? a : null;
                    break;
            }
        }
    }

    class XElementDescendantsPropertyDescriptor : XPropertyDescriptor<XElement, IEnumerable<XElement>>
    {
        XDeferredAxis<XElement> value;
        XName changeState;

        public XElementDescendantsPropertyDescriptor() : base("Descendants")
        {
        }

        public override object GetValue(object component)
        {
            return value = new XDeferredAxis<XElement>((e, n) => n != null ? e.Descendants(n) : e.Descendants(), component as XElement, null);
        }

        protected override void OnChanged(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Add:
                case XObjectChange.Remove:
                    XElement e = sender as XElement;
                    if (e != null && (value.name == e.Name || value.name == null))
                    {
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
                case XObjectChange.Name:
                    e = sender as XElement;
                    if (e != null && value.element != e && value.name != null && (value.name == e.Name || value.name == changeState))
                    {
                        changeState = null;
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
            }
        }

        protected override void OnChanging(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Name:
                    XElement e = sender as XElement;
                    changeState = e != null ? e.Name : null;
                    break;
            }
        }
    }

    class XElementElementPropertyDescriptor : XPropertyDescriptor<XElement, object>
    {
        XDeferredSingleton<XElement> value;
        XElement changeState;

        public XElementElementPropertyDescriptor() : base("Element")
        {
        }

        public override object GetValue(object component)
        {
            return value = new XDeferredSingleton<XElement>((e, n) => e.Element(n), component as XElement, null);
        }

        protected override void OnChanged(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Add:
                    XElement e = sender as XElement;
                    if (e != null && value.element == e.Parent && value.name == e.Name && value.element.Element(value.name) == e)
                    {
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
                case XObjectChange.Remove:
                    e = sender as XElement;
                    if (e != null && changeState == e)
                    {
                        changeState = null;
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
                case XObjectChange.Name:
                    e = sender as XElement;
                    if (e != null)
                    {
                        if (value.element == e.Parent && value.name == e.Name && value.element.Element(value.name) == e)
                        {
                            OnValueChanged(value.element, EventArgs.Empty);
                        }
                        else if (changeState == e)
                        {
                            changeState = null;
                            OnValueChanged(value.element, EventArgs.Empty);
                        }
                    }
                    break;
            }
        }

        protected override void OnChanging(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Remove:
                case XObjectChange.Name:
                    XElement e = sender as XElement;
                    changeState = e != null && value.element == e.Parent && value.name == e.Name && value.element.Element(value.name) == e ? e : null;
                    break;
            }
        }
    }

    class XElementElementsPropertyDescriptor : XPropertyDescriptor<XElement, IEnumerable<XElement>>
    {
        XDeferredAxis<XElement> value;
        object changeState;

        public XElementElementsPropertyDescriptor() : base("Elements")
        {
        }

        public override object GetValue(object component)
        {
            return value = new XDeferredAxis<XElement>((e, n) => n != null ? e.Elements(n) : e.Elements(), component as XElement, null);
        }

        protected override void OnChanged(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Add:
                    XElement e = sender as XElement;
                    if (e != null && value.element == e.Parent && (value.name == e.Name || value.name == null))
                    {
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
                case XObjectChange.Remove:
                    e = sender as XElement;
                    if (e != null && value.element == (changeState as XContainer) && (value.name == e.Name || value.name == null))
                    {
                        changeState = null;
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
                case XObjectChange.Name:
                    e = sender as XElement;
                    if (e != null && value.element == e.Parent && value.name != null && (value.name == e.Name || value.name == (changeState as XName)))
                    {
                        changeState = null;
                        OnValueChanged(value.element, EventArgs.Empty);
                    }
                    break;
            }
        }

        protected override void OnChanging(object sender, XObjectChangeEventArgs args)
        {
            if (value == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Remove:
                    XElement e = sender as XElement;
                    changeState = e != null ? e.Parent : null;
                    break;
                case XObjectChange.Name:
                    e = sender as XElement;
                    changeState = e != null ? e.Name : null;
                    break;
            }
        }
    }

    class XElementValuePropertyDescriptor : XPropertyDescriptor<XElement, string>
    {
        XElement element;

        public XElementValuePropertyDescriptor() : base("Value")
        {
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override object GetValue(object component)
        {
            element = component as XElement;
            if (element == null)
                return string.Empty;
            return element.Value;
        }

        public override void SetValue(object component, object value)
        {
            element = component as XElement;
            if (element == null)
                return;
            element.Value = value as string;
        }

        protected override void OnChanged(object sender, XObjectChangeEventArgs args)
        {
            if (element == null)
                return;
            switch (args.ObjectChange)
            {
                case XObjectChange.Add:
                case XObjectChange.Remove:
                    if (sender is XElement || sender is XText)
                    {
                        OnValueChanged(element, EventArgs.Empty);
                    }
                    break;
                case XObjectChange.Value:
                    if (sender is XText)
                    {
                        OnValueChanged(element, EventArgs.Empty);
                    }
                    break;
            }
        }
    }

    class XElementXmlPropertyDescriptor : XPropertyDescriptor<XElement, string>
    {
        XElement element;

        public XElementXmlPropertyDescriptor() : base("Xml")
        {
        }

        public override object GetValue(object component)
        {
            element = component as XElement;
            if (element == null)
                return string.Empty;
            return element.ToString(SaveOptions.DisableFormatting);
        }

        protected override void OnChanged(object sender, XObjectChangeEventArgs args)
        {
            if (element == null)
                return;
            OnValueChanged(element, EventArgs.Empty);
        }
    }

    class XAttributeValuePropertyDescriptor : XPropertyDescriptor<XAttribute, string>
    {
        XAttribute attribute;

        public XAttributeValuePropertyDescriptor() : base("Value")
        {
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override object GetValue(object component)
        {
            attribute = component as XAttribute;
            if (attribute == null)
                return string.Empty;
            return attribute.Value;
        }

        public override void SetValue(object component, object value)
        {
            attribute = component as XAttribute;
            if (attribute == null)
                return;
            attribute.Value = value as string;
        }

        protected override void OnChanged(object sender, XObjectChangeEventArgs args)
        {
            if (attribute == null)
                return;
            if (args.ObjectChange == XObjectChange.Value)
            {
                OnValueChanged(attribute, EventArgs.Empty);
            }
        }
    }

    class XDeferredAxis<T> : IEnumerable<T>, IEnumerable where T : XObject
    {
        Func<XElement, XName, IEnumerable<T>> func;
        internal XElement element;
        internal XName name;

        public XDeferredAxis(Func<XElement, XName, IEnumerable<T>> func, XElement element, XName name)
        {
            if (func == null)
                throw new ArgumentNullException("func");
            if (element == null)
                throw new ArgumentNullException("element");
            this.func = func;
            this.element = element;
            this.name = name;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return func(element, name).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> this[string expandedName]
        {
            get
            {
                if (expandedName == null)
                    throw new ArgumentNullException("expandedName");
                if (name == null)
                {
                    name = expandedName;
                }
                else if (name != expandedName)
                {
                    return Enumerable.Empty<T>();
                }
                return this;
            }
        }
    }

    class XDeferredSingleton<T> where T : XObject
    {
        Func<XElement, XName, T> func;
        internal XElement element;
        internal XName name;

        public XDeferredSingleton(Func<XElement, XName, T> func, XElement element, XName name)
        {
            if (func == null)
                throw new ArgumentNullException("func");
            if (element == null)
                throw new ArgumentNullException("element");
            this.func = func;
            this.element = element;
            this.name = name;
        }

        public T this[string expandedName]
        {
            get
            {
                if (expandedName == null)
                    throw new ArgumentNullException("expandedName");
                if (name == null)
                {
                    name = expandedName;
                }
                else if (name != expandedName)
                {
                    return null;
                }
                return func(element, name);
            }
        }
    }
}
