# Hunspell.Net

A .NET 10 wrapper library for [Hunspell](https://github.com/hunspell/hunspell), providing P/Invoke bindings to the native C API with instance-level thread safety.

## Prerequisites

- **CMake** 3.20+
- **C++ compiler** with C++17 support (Clang, GCC, or MSVC)
- **.NET 10 SDK**

## Project Structure

```
src/
  Hunspell.Native/        Native library build (CMake)
    CMakeLists.txt        Build configuration
    build.sh              macOS/Linux build script
    build.ps1             Windows build script
  Hunspell.Net/           .NET wrapper library
    NativeMethods.cs      P/Invoke declarations
    HunspellDictionary.cs Thread-safe high-level API
  Hunspell.Net.Tests/     Unit and integration tests
```

## Building

### 1. Build the native library

**macOS / Linux:**

```bash
cd src/Hunspell.Native
./build.sh x64      # x86_64 build
./build.sh arm64    # Apple Silicon build
```

**Windows (PowerShell):**

```powershell
cd src\Hunspell.Native
.\build.ps1
```

The compiled shared library is placed in `src/Hunspell.Native/out/<arch>/`.

### 2. Build the .NET wrapper

```bash
dotnet build
```

### 3. Run tests

```bash
dotnet test
```

The test project automatically copies the native library from `src/Hunspell.Native/out/` for the current platform and architecture.

## Usage

```csharp
using Hunspell.Net;

using var dict = new HunspellDictionary("en_US.aff", "en_US.dic");

bool correct = dict.Spell("hello");       // true
string[] suggestions = dict.Suggest("helo"); // ["hello", ...]
string[] stems = dict.Stem("created");       // ["create"]

dict.Add("myword");    // add to runtime dictionary
dict.Remove("hello");  // remove from runtime dictionary
```

## Thread Safety

Each `HunspellDictionary` instance is thread-safe. All public methods are serialized via a per-instance lock. Different instances can be used concurrently without contention.

## Publishing

Packages are published to nuget.org automatically via GitHub Actions when a version tag is pushed.

```bash
git tag v1.0.0 && git push origin v1.0.0        # stable release
git tag v1.0.0-beta.1 && git push origin v1.0.0-beta.1  # pre-release
```

The workflow builds the native library, packs `SpellGE.Hunspell.Linux` and `SpellGE.Hunspell`, and pushes both to nuget.org. The repository must have a `NUGET_ORG_API_KEY` secret configured.
