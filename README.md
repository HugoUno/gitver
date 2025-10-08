# GitVer

Minimal .NET 8 console app that prints the Git version baked in at build time.

## Build

- PowerShell: `dotnet build -p:DefineConstants="VERSION=\"$(git describe --tags --always)\""`
- Bash: `dotnet build /p:DefineConstants="VERSION=\"$(git describe --tags --always)\""`

These commands feed the current `git describe` output into the generated source file. You can also supply a plain `Version` property, for example `dotnet build /p:Version=1.2.3`.

> Passing `VERSION=…` via `DefineConstants` triggers a compiler warning about the symbol being ignored. The generated source still captures the value, so the warning is safe to ignore.

## Run

```
dotnet run
```

If no override is supplied the application prints the default version `dev`.
