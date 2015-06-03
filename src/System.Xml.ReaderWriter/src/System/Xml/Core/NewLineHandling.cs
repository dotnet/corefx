// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Xml
{
    // NewLineHandling speficies what will XmlWriter do with new line characters. The options are:
    //  Replace  = Replaces all new line characters with XmlWriterSettings.NewLineChars so all new lines are the same; by default NewLineChars are "\r\n"
    //  Entitize = Replaces all new line characters that would be normalized away by a normalizing XmlReader with character entities
    //  None     = Does not change the new line characters in input
    //
    // Following table shows what will happen with new line characters in detail:
    //
    //										|      In text node value       |    In attribute value               |
    // input to XmlWriter.WriteString()		| \r\n		\n		\r		\t	|	\r\n		\n		\r		\t    |
    // ------------------------------------------------------------------------------------------------------------
    // NewLineHandling.Replace (default)	| \r\n		\r\n	\r\n	\t	|	&#D;&#A;	&#A;	&#D;	&#9;  |
    // NewLineHandling.Entitize			    | &#D;		\n		&#D;	\t	|	&#D;&#A;	&#A;	&#D;	&#9;  |
    // NewLineHandling.None				    | \r\n		\n		\r		\t	|	\r\n		\n		\r		\t    |
    // ------------------------------------------------------------------------------------------------------------

    // Specifies how end of line is handled in XmlWriter.
    public enum NewLineHandling
    {
        Replace = 0,
        Entitize = 1,
        None = 2
    }
}
