project(System.Net.Security.Native)

set(CMAKE_INCLUDE_CURRENT_DIR ON)

add_definitions(-DPIC=1)

if (APPLE)
   find_library(LIBGSS NAMES GSS)
else()
   find_library(LIBGSS NAMES gssapi_krb5)
endif()

if(LIBGSS STREQUAL LIBGSS-NOTFOUND)
    message(SEND_ERROR "!!! Cannot find libgssapi_krb5 and System.Net.Security.Native cannot build without it. Try installing libkrb5-dev (or the appropriate package for your platform) !!!")
    return()
endif()

set(NATIVEGSS_SOURCES
    pal_gssapi.cpp
)

add_library(System.Net.Security.Native
    SHARED
    ${NATIVEGSS_SOURCES}
)

target_link_libraries(System.Net.Security.Native
    ${LIBGSS}
)

install (TARGETS System.Net.Security.Native DESTINATION .)
