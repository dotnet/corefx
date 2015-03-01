// This is auto generated file. Please don’t modify manually.
// The file is generated as part of the build through the ResourceGenerator tool 
// which takes the project resx resource file and generated this source code file.
// By default the tool will use Resources\Strings.resx but projects can customize
// that by overriding the StringResourcesPath property group.
namespace System
{
    internal static partial class SR
    {
#pragma warning disable 0414
        private const string s_resourcesName = "System.Xml.XPath.XDocument.resources"; // assembly Name + .resources
#pragma warning restore 0414

#if !DEBUGRESOURCES
        internal static string Argument_CreateNavigator {
              get { return SR.GetResourceString("Argument_CreateNavigator", null); }
        }
        internal static string InvalidOperation_BadNodeType {
              get { return SR.GetResourceString("InvalidOperation_BadNodeType", null); }
        }
        internal static string InvalidOperation_UnexpectedEvaluation {
              get { return SR.GetResourceString("InvalidOperation_UnexpectedEvaluation", null); }
        }
        internal static string NotSupported_MoveToId {
              get { return SR.GetResourceString("NotSupported_MoveToId", null); }
        }
#else
        internal static string Argument_CreateNavigator {
              get { return SR.GetResourceString("Argument_CreateNavigator", @"This XPathNavigator cannot be created on a node of type {0}."); }
        }
        internal static string InvalidOperation_BadNodeType {
              get { return SR.GetResourceString("InvalidOperation_BadNodeType", @"This operation is not valid on a node of type {0}."); }
        }
        internal static string InvalidOperation_UnexpectedEvaluation {
              get { return SR.GetResourceString("InvalidOperation_UnexpectedEvaluation", @"The XPath expression evaluated to unexpected type {0}."); }
        }
        internal static string NotSupported_MoveToId {
              get { return SR.GetResourceString("NotSupported_MoveToId", @"This XPathNavigator does not support IDs."); }
        }

#endif
    }
}
