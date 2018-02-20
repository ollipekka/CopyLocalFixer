# CopyLocalFixer

## Use Case

In large builds assemblies are sometimes unneccesary copied. This projects attempts to optimize builds on slow machines by turning on CopyLocal=true conservatively.

## Usage

```bat
    CopyLocalFixer c:\Projects\LargeProject
```