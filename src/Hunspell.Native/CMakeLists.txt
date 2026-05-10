cmake_minimum_required(VERSION 3.20)
project(hunspell LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

set(HUNSPELL_SRC_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../../references/hunspell/src/hunspell")

set(HUNSPELL_SOURCES
    ${HUNSPELL_SRC_DIR}/affentry.cxx
    ${HUNSPELL_SRC_DIR}/affixmgr.cxx
    ${HUNSPELL_SRC_DIR}/csutil.cxx
    ${HUNSPELL_SRC_DIR}/filemgr.cxx
    ${HUNSPELL_SRC_DIR}/hashmgr.cxx
    ${HUNSPELL_SRC_DIR}/hunspell.cxx
    ${HUNSPELL_SRC_DIR}/hunzip.cxx
    ${HUNSPELL_SRC_DIR}/phonet.cxx
    ${HUNSPELL_SRC_DIR}/replist.cxx
    ${HUNSPELL_SRC_DIR}/suggestmgr.cxx
)

add_library(hunspell SHARED ${HUNSPELL_SOURCES})

target_include_directories(hunspell PRIVATE
    ${HUNSPELL_SRC_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}/../../references/hunspell/msvc
)

target_compile_definitions(hunspell PRIVATE
    BUILDING_LIBHUNSPELL
    _CRT_SECURE_NO_WARNINGS
)

if(WIN32)
    set_target_properties(hunspell PROPERTIES
        OUTPUT_NAME "hunspell"
        PREFIX ""
    )
else()
    set_target_properties(hunspell PROPERTIES
        OUTPUT_NAME "hunspell"
    )
endif()

install(TARGETS hunspell
    LIBRARY DESTINATION lib
    RUNTIME DESTINATION bin
)
