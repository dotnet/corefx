// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    /// Provides the Integrated Development Environment (IDE) help
    /// system with contextual information for the current task.
    /// </summary>
    public interface IHelpService
    {
        /// <summary>
        /// Adds a context attribute to the document.
        /// </summary>
        void AddContextAttribute(string name, string value, HelpKeywordType keywordType);

        /// <summary>
        /// Clears all existing context attributes from the document.
        /// </summary>
        void ClearContextAttributes();

        /// <summary>
        /// Creates a Local IHelpService to manage subcontexts.
        /// </summary>
        IHelpService CreateLocalContext(HelpContextType contextType);

        /// <summary>
        /// Removes a previously added context attribute.
        /// </summary>
        void RemoveContextAttribute(string name, string value);

        /// <summary>
        /// Removes a context that was created with CreateLocalContext
        /// </summary>
        void RemoveLocalContext(IHelpService localContext);

        /// <summary>
        /// Shows the help topic that corresponds to the specified keyword.
        /// </summary>
        void ShowHelpFromKeyword(string helpKeyword);

        /// <summary>
        /// Shows the help topic that corresponds with the specified Url and topic navigation ID.
        /// </summary>
        void ShowHelpFromUrl(string helpUrl);
    }
}
