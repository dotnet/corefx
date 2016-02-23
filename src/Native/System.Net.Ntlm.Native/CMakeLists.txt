project(System.Net.Ntlm.Native)

set(CMAKE_INCLUDE_CURRENT_DIR ON)

add_definitions(-DPIC=1 -Wno-padded)

find_library(LIB_HEIMNTLM NAMES heimntlm PATHS /usr/heimdal/lib /usr/lib/x86_64-linux-gnu/heimdal/)
find_package(OpenSSL REQUIRED)

if(LIB_HEIMNTLM STREQUAL LIB_HEIMNTLM-NOTFOUND)
    message(WARNING "Cannot find libheimntlm.so. System.Net.Security.NegotiateStream will be built without NTLM support. Try installing libheimntlm (or the appropriate package for your platform) for NTLM")
else()
    include_directories(/usr/heimdal/include)
    include_directories(/usr/include/heimdal)
    
    set(NATIVENTLM_SOURCES
        pal_ntlmapi.cpp
    )

    add_library(System.Net.Ntlm.Native
        SHARED
        ${NATIVENTLM_SOURCES}
    )

    target_link_libraries(System.Net.Ntlm.Native
           ${LIB_HEIMNTLM}
           ${OPENSSL_CRYPTO_LIBRARY}
    )

   install (TARGETS System.Net.Ntlm.Native DESTINATION .)
endif()

