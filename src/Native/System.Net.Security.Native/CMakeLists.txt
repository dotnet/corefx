project(System.Net.Security.Native)

set(CMAKE_INCLUDE_CURRENT_DIR ON)

add_definitions(-DPIC=1)

if (HAVE_HEIMNTLM_HEADERS)
   find_library(LIB_HEIMNTLM NAMES heimntlm PATHS /usr/heimdal/lib)
   include_directories(/usr/heimdal/include)

   if(LIB_HEIMNTLM STREQUAL LIB_HEIMNTLM-NOTFOUND)
       message(FATAL_ERROR "Cannot find libheimntlm.so and System.Net.Security.Native cannot build without it. Try installing libheimntlm (or the appropriate package for your platform)")
   endif()


   set(NATIVENTLM_SOURCES
     pal_ntlmapi.cpp
   )

   add_library(System.Net.Security.Native
    SHARED
    ${NATIVENTLM_SOURCES}
   )

   target_link_libraries(System.Net.Security.Native
           ${LIB_HEIMNTLM}
   )

   install (TARGETS System.Net.Security.Native DESTINATION .)

elseif (CMAKE_SYSTEM_NAME STREQUAL Linux)
   message (FATAL_ERROR "cannot find libheim ntlm headers and System.Net.Security.Native cannot build without it. Try installing libheimntlm (or the appropriate package for your platform)")
else ()
   message(STATUS "Heimdal shims not needed for this platform")
endif()
