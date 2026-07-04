# Hunspell.Net

A .NET 10 wrapper library for [Hunspell](https://github.com/hunspell/hunspell), providing P/Invoke bindings to the native C API with instance-level thread safety.

[![NuGet SpellGE.Hunspell](https://img.shields.io/nuget/v/SpellGE.Hunspell.svg?label=SpellGE.Hunspell)](https://www.nuget.org/packages/SpellGE.Hunspell)
[![NuGet SpellGE.Hunspell.Windows](https://img.shields.io/nuget/v/SpellGE.Hunspell.Windows.svg?label=SpellGE.Hunspell.Windows)](https://www.nuget.org/packages/SpellGE.Hunspell.Windows)
[![NuGet SpellGE.Hunspell.Linux](https://img.shields.io/nuget/v/SpellGE.Hunspell.Linux.svg?label=SpellGE.Hunspell.Linux)](https://www.nuget.org/packages/SpellGE.Hunspell.Linux)

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

### Versioning

The wrapper follows upstream [Hunspell](https://github.com/hunspell/hunspell) releases. Given a Hunspell version `MAJOR.MINOR.PATCH`, the wrapper version is `MAJOR.MINOR.PATCHx100`:

- Hunspell `v1.7.3` -> wrapper `v1.7.300`

If the wrapper itself needs a fix without a corresponding new Hunspell release, increment the multiplied patch number:

- First wrapper-only fix for Hunspell `v1.7.3` -> `v1.7.301`
- Second wrapper-only fix -> `v1.7.302`

```bash
git tag v1.7.300 && git push origin v1.7.300        # stable release, matches Hunspell v1.7.3
git tag v1.7.301 && git push origin v1.7.301        # wrapper-only patch
git tag v1.7.300-beta.1 && git push origin v1.7.300-beta.1  # pre-release
```

The workflow builds the native library, packs `SpellGE.Hunspell.Linux` and `SpellGE.Hunspell`, and pushes both to nuget.org.

## License

This project wraps [Hunspell](https://github.com/hunspell/hunspell), an open-source spell checker by Németh László. The native library is distributed under its original triple license: **MPL 1.1 / GPL 2.0+ / LGPL 2.1+**. The .NET wrapper code is licensed under LGPL 2.1+. See [references/hunspell/license.hunspell](references/hunspell/license.hunspell) for full details.
