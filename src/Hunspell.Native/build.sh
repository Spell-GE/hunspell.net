#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ARCH="${1:-x64}"
BUILD_DIR="${SCRIPT_DIR}/build-${ARCH}"
OUTPUT_DIR="${SCRIPT_DIR}/out/${ARCH}"

CMAKE_EXTRA_ARGS=()

case "$(uname -s)" in
    Darwin)
        if [ "${ARCH}" = "x64" ]; then
            CMAKE_EXTRA_ARGS+=("-DCMAKE_OSX_ARCHITECTURES=x86_64")
        elif [ "${ARCH}" = "arm64" ]; then
            CMAKE_EXTRA_ARGS+=("-DCMAKE_OSX_ARCHITECTURES=arm64")
        fi
        ;;
    Linux)
        if [ "${ARCH}" = "x64" ]; then
            CMAKE_EXTRA_ARGS+=("-DCMAKE_C_FLAGS=-m64" "-DCMAKE_CXX_FLAGS=-m64")
        fi
        ;;
esac

mkdir -p "${BUILD_DIR}"
cmake -S "${SCRIPT_DIR}" -B "${BUILD_DIR}" \
    -DCMAKE_BUILD_TYPE=Release \
    "${CMAKE_EXTRA_ARGS[@]}"

cmake --build "${BUILD_DIR}" --config Release --parallel

mkdir -p "${OUTPUT_DIR}"

case "$(uname -s)" in
    Darwin)
        cp "${BUILD_DIR}"/libhunspell.dylib "${OUTPUT_DIR}/"
        ;;
    Linux)
        cp "${BUILD_DIR}"/libhunspell.so "${OUTPUT_DIR}/"
        ;;
esac

echo "Build complete (${ARCH}). Output in: ${OUTPUT_DIR}"
