Desktop-Style Lifetime Demo
---------------------------

This demo shows how the boundary model of lifeime can be
used to implement correct sharing and disposal in a typical
desktop application.

Elements:

 * Parts/ - implements a typical Application-Project-Document
   dependency structure.

Points:

 * When the application runs, two Projects each with a number
   of Documents are created.
 * The dependency structure is dumped, showing how each 
   Document imports only the project it is included int
 * When the structure is torn down, documents and projects
   must be disposed in the correct order.
