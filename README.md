# GitVer

Minimal .NET 8 console app that prints the Git version baked in at build time.

Each build collects three bits of metadata via Git (tag, commit hash, UTC build time) and combines them into a version string such as `v1.2.0-96a0c33 @ 2025-10-08T07:07:20Z`. If Git is unavailable the version falls back to `dev`.

## Build

- PowerShell: `dotnet build -p:DefineConstants="VERSION=\"$(git describe --tags --always)\""`
- Bash: `dotnet build /p:DefineConstants="VERSION=\"$(git describe --tags --always)\""`

These commands feed the current `git describe` output into the generated source file. You can also supply a plain `Version` property, for example `dotnet build /p:Version=1.2.3`.

## Run

```
dotnet run
```

The executable prints `App version: v<value>`, where `<value>` is the Git-derived string (tag + commit + build time) or `dev` when no Git metadata is available.
