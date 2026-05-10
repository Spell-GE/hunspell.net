#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_DIR="${SCRIPT_DIR}/build-x64"
OUTPUT_DIR="${SCRIPT_DIR}/out/x64"

CMAKE_EXTRA_ARGS=("-DCMAKE_C_FLAGS=-m64" "-DCMAKE_CXX_FLAGS=-m64")

mkdir -p "${BUILD_DIR}"
cmake -S "${SCRIPT_DIR}" -B "${BUILD_DIR}" \
    -DCMAKE_BUILD_TYPE=Release \
    "${CMAKE_EXTRA_ARGS[@]}"

cmake --build "${BUILD_DIR}" --config Release --parallel

mkdir -p "${OUTPUT_DIR}"
cp "${BUILD_DIR}"/libhunspell.so "${OUTPUT_DIR}/"

echo "Build complete (x64). Output in: ${OUTPUT_DIR}"
