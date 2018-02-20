# CopyLocalFixer

## Use Case

In large builds assemblies are sometimes excessively copied. This projects attempts to optimize builds on slow machines by tuning CopyLocal conservatively.

It takes reference and checks if it is located in the same directory as the project is built into and changes the reference CopyLocal to false..

```xml

    <Reference Include="MyAssembly">
      <HintPath>outputpath\MyAssembly.dll</HintPath>
      <Private>False</Private>
    </Reference>

```

## Usage

```bat
    CopyLocalFixer c:\Projects\LargeProject
```

Or just cloning the source and doing things directly.