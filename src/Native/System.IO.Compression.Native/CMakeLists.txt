project(System.IO.Compression.Native)

set(NATIVECOMPRESSION_SOURCES
    pal_zlib.cpp
)

add_library(System.IO.Compression.Native
    SHARED
    ${NATIVECOMPRESSION_SOURCES}
)

if(CMAKE_SYSTEM_PROCESSOR STREQUAL x86_64 OR CMAKE_SYSTEM_PROCESSOR STREQUAL amd64 AND CMAKE_SYSTEM_NAME STREQUAL LINUX)

	add_dependencies(System.IO.Compression.Native System.IO.Compression.Native-Static)

	set(ZLIB_FLAGS 
	    -w
	    -mpclmul 
	    -msse4
	    -O3
	    -fPIC
	    -Ds_LARGEFILE64_SOURCE=1 
	    -DHAVE_HIDDEN -DX86_64 
	    -DUNALIGNED_OK 
	    -DADLER32_UNROLL_LESS 
	    -DCRC32_UNROLL_LESS 
	    -UCHECK_SSE2 
	    -DHAVE_SSE2 
	    -DUSE_SSE4_2_CRC_HASH 
	    -DHAVE_PCLMULQDQ 
	    -DUSE_MEDIUM
   	)

	set(ZLIB_SOURCES)
		../Windows/clrcompression/zlib-intel/adler32.c
		../Windows/clrcompression/zlib-intel/crc_folding.c
		../Windows/clrcompression/zlib-intel/crc32.c
		../Windows/clrcompression/zlib-intel/deflate_quick.c
		../Windows/clrcompression/zlib-intel/deflate.c
		../Windows/clrcompression/zlib-intel/fill_window_sse.c
		../Windows/clrcompression/zlib-intel/match.c
		../Windows/clrcompression/zlib-intel/trees.c
		../Windows/clrcompression/zlib-intel/x86.c
		../Windows/clrcompression/zlib-intel/zutil.c
		../Windows/clrcompression/zlib/compress.c 
		../Windows/clrcompression/zlib/inffast.c
		../Windows/clrcompression/zlib/inflate.c
		../Windows/clrcompression/zlib/inftrees.c
	)
		
	add_definitions(${ZLIB_FLAGS})
        add_library(System.IO.Compression.Native-Static
	    STATIC
	    ${ZLIB_SOURCES}
	)

	
	set(ZLIB_LIBRARIES 
	    System.IO.Compression.Native-Static
	)

else()
    find_package(ZLIB REQUIRED)
endif()

target_link_libraries(System.IO.Compression.Native
    ${ZLIB_LIBRARIES}
)

install (TARGETS System.IO.Compression.Native DESTINATION .)
