// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace System.ComponentModel.Design
{
    /// <summary>
    /// Represents a verb that can be executed by a component's designer.
    /// </summary>
    public class DesignerVerb : MenuCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.Design.DesignerVerb'/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DesignerVerb(string text, EventHandler handler) : base(handler, StandardCommands.VerbFirst)
        {
            Properties["Text"] = text == null ? null : Regex.Replace(text, @"\(\&.\)", "");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.Design.DesignerVerb'/>
        /// class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DesignerVerb(string text, EventHandler handler, CommandID startCommandID) : base(handler, startCommandID)
        {
            Properties["Text"] = text == null ? null : Regex.Replace(text, @"\(\&.\)", "");
        }

        /// <summary>
        /// Gets or sets the description of the menu item for the verb.
        /// </summary>
        public string Description
        {
            get
            {
                object result = Properties["Description"];
                if (result == null)
                {
                    return string.Empty;
                }

                return (string)result;
            }
            set => Properties["Description"] = value;
        }

        /// <summary>
        /// Gets or sets the text to show on the menu item for the verb.
        /// </summary>
        public string Text
        {
            get
            {
                object result = Properties["Text"];
                if (result == null)
                {
                    return string.Empty;
                }

                return (string)result;
            }
        }

        /// <summary>
        /// Overrides object's ToString().
        /// </summary>
        public override string ToString() => Text + " : " + base.ToString();
    }
}
