project(System.Net.Security.Native)

set(CMAKE_INCLUDE_CURRENT_DIR ON)

add_definitions(-DPIC=1)

if (HAVE_GSSFW_HEADERS)
   find_library(LIBGSS NAMES GSS)
   if(LIBGSS STREQUAL LIBGSS-NOTFOUND)
      message(FATAL_ERROR "Cannot find GSS.Framework and System.Net.Security.Native cannot build without it. Try installing GSS.Framework (or the appropriate package for your platform)")
   endif()
   find_path(LIBGSS_INCLUDE_DIR GSS/GSS.h)
else()
   find_library(LIBGSS NAMES gssapi_krb5)
   if(LIBGSS STREQUAL LIBGSS-NOTFOUND)
      message(FATAL_ERROR "Cannot find libgssapi_krb5 and System.Net.Security.Native cannot build without it. Try installing libkrb5-dev (or the appropriate package for your platform)")
   endif()
   # Look up for MIT-KRB5 specific header, not available in Heimdal
   find_path(LIBGSS_INCLUDE_DIR gssapi/gssapi_ext.h)
endif()

set(NATIVEGSS_SOURCES
    pal_gssapi.cpp
)

add_library(System.Net.Security.Native
    SHARED
    ${NATIVEGSS_SOURCES}
    ${VERSION_FILE_PATH}
)

target_link_libraries(System.Net.Security.Native
    ${LIBGSS}
)

include_directories(SYSTEM
    ${LIBGSS_INCLUDE_DIR}
)

install (TARGETS System.Net.Security.Native DESTINATION .)
