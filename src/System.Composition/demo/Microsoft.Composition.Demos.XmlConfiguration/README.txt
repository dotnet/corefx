XML Configuration Demo
----------------------

Shows how POCO parts can be identified and configured
using XML in an App.config file.

Elements:

 * App.config - exports the MainWindow class under the
   Window contract.
 * Program.cs - requests the Window contract to show
   a window.
 * Extension/ContainerConfigurationExtensions.cs - reads
   XML and uses it to manipulate a ConventionBuilder.

Points:

 * The reader uses ConventionBuilder, which indicates
   that the approach is portable to both MEF composition
   engines.

