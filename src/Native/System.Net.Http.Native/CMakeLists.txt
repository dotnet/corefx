project(System.Net.Http.Native)

set(CMAKE_LIBRARY_PATH ${CMAKE_LIBRARY_PATH}
	/usr/lib/x86_64-linux-gnu
	/usr/lib
	)

find_package(CURL)
if(NOT CURL_FOUND)
    message(FATAL_ERROR "!!! Cannot find libcurl and System.Net.Http.Native cannot build without it. Try installing libcurl4-openssl-dev (or the appropriate package for your platform) !!!")
endif(NOT CURL_FOUND)

if(CMAKE_STATIC_CURL)
	find_library(CURL_STATIC_LIB NAMES libcurl.a)
	if(NOT CURL_STATIC_LIB)
    		message(FATAL_ERROR "!!! Cannot find libcurl static lib and System.Net.Http.Native cannot build without it. Try installing libcurl4-openssl-dev (or the appropriate package for your platform) !!!")
	endif(NOT CURL_STATIC_LIB)
	set(CURL_LIBRARIES ${CURL_STATIC_LIB})
	add_compile_options(-DCURL_STATICLIB)
endif(CMAKE_STATIC_CURL)


set(NATIVEHTTP_SOURCES
    pal_curlinit.cpp
    pal_easy.cpp
    pal_multi.cpp
    pal_slist.cpp
    pal_versioninfo.cpp
)

include_directories(SYSTEM ${CURL_INCLUDE_DIR})

add_library(System.Net.Http.Native
    SHARED
    ${NATIVEHTTP_SOURCES}
    ${VERSION_FILE_PATH}
)

MESSAGE("CURL_LIBRARIES :" ${CURL_LIBRARIES})
target_link_libraries(System.Net.Http.Native
  ${CURL_LIBRARIES}
)

install_library_and_symbols (System.Net.Http.Native)
