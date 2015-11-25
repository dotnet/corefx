project(System.IO.Compression.Native)

if("${CMAKE_SOURCE_DIR}" STREQUAL "${CMAKE_BINARY_DIR}")
    message(FATAL_ERROR "Binary directory isn't being correctly set before calling Cmake. Tree must be built in separate directory from source.")
endif()

set(CMAKE_INCLUDE_CURRENT_DIR ON)
include (GenerateExportHeader)

set(NATIVECOMPRESSION_SOURCES
    System.IO.Compression.Native.def
    ZLib-1.2.8/adler32.c
    ZLib-1.2.8/compress.c
    ZLib-1.2.8/crc32.c
    ZLib-1.2.8/deflate.c
    ZLib-1.2.8/inffast.c
    ZLib-1.2.8/inflate.c
    ZLib-1.2.8/inftrees.c
    ZLib-1.2.8/trees.c
    ZLib-1.2.8/zutil.c
    #ZLib-1.2.8/gzio.c
    #ZLib-1.2.8/infback.c
    #ZLib-1.2.8/uncompr.c
)

if ($ENV{__VSVersion} STREQUAL vs2015)
    add_definitions(-DHAVE_VSNPRINTF)
endif ()

add_library(System.IO.Compression.Native
    SHARED
    ${NATIVECOMPRESSION_SOURCES}
)

GENERATE_EXPORT_HEADER( System.IO.Compression.Native
     BASE_NAME System.IO.Compression.Native
     EXPORT_MACRO_NAME System.IO.Compression.Native_EXPORT
     EXPORT_FILE_NAME System.IO.Compression.Native_Export.h
     STATIC_DEFINE System.IO.Compression.Native_BUILT_AS_STATIC
)

install (TARGETS System.IO.Compression.Native DESTINATION .)
