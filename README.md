# Generator

A CLI tool to generate PKHeX files.

[!WARNING]
This documentation is still incomplete. Some argument behaviors or descriptions may be missing or inaccurate.

## Usage

View the usage guide [here](https://github.com/CalRL/Generator/blob/main/docs/Usage.md)

## 🛠 Building Yourself

### Prerequisites

-   .NET 9.0+

### Clone The Repo

```bash
git clone https://github.com/CalRL/Generator
cd Generator
```

### Add PKHeX Dependency

Clone the PKHeX repository into a subfolder named `PKHeX`

```bash
git clone --depth 1 https://github.com/kwsch/PKHeX.git PKHeX
```

The `.csproj` references will pick up `PKHeX.Core` from this local path.

### Build The Executable

Run the following to create a standalone executable:

```bash
dotnet publish Generator/Generator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

```
