Build Time Code Generation
--------------------------

This demo shows the kind of code that a build-time tool could
generate in order to eliminate discovery/emit/JIT overheads
in the lightweight composition feature.

The scenario includes a non-shared RequestListener that
depends on a shared, disposable Log part.

Elements:

 * Generated/BuildTimeCodeGeneration_ExportDescriptorProvider.cs -
   the build-time generated code supplying the two
   parts.
 * Parts/ - the parts as they would be deployed at runtime,
   showing the 'stripped off' attributes in comments.

Points:

 * Only the core composition engine and runtime part support
   are required; discovery logic/emit code is completely absent.
 * The RequestListener-Log dependency is lazy, this shows how
   the Lazy functionality is provided by the container; this
   requires runtime reflection in this design.
 * Multiple pre-compiled EDPs can be loaded into a single
   container; graph construction and cycle checking are deferred
   until runtime (initialization only).