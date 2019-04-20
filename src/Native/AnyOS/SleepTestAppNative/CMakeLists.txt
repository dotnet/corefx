cmake_minimum_required(VERSION 2.8.12)
project(SleepTestAppNative C)

add_executable(${PROJECT_NAME} program.c)
install (TARGETS ${PROJECT_NAME} DESTINATION .)
