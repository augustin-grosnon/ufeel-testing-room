#!/bin/bash
set -e

mkdir -p build
cmake -B build -D CMAKE_BUILD_TYPE=Release
# cmake --build build -j$(nproc)
cmake --build build

cp build/lib/libemotion_cpp.so .
