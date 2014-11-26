Web-Style Lifetime Demo
-----------------------

Shows how the lifetime model can be applied to
an ASP.NET MVC-style service-locator-based
application.

Elements:

 * Extension/Boundaries.cs - names some tags that
   the web server will used to describe the scope
   of a web request.
 * Parts/DatabaseConnection.cs - a "shared per web
   request" part that is constrained by the
   DataConsistency boundary.
 * Parts/WebServer.cs - emulates an ASP.NET MVC
   web request handler; note the imported
   ExportFactory<IExportProvider>.

Points:

 * Shows how non-declarative, non part-specific
   lifetimes can be created and used in the part
   lifetime model.
