# CopyLocalFixer

## Use Case

In large builds assemblies are sometimes unneccesary copied. This projects attempts to optimize builds on slow machines by turning on CopyLocal=true conservatively.

It takes reference and checks if it is located in the same directory as the project is built into and changes the reference to "Private".

```xml

    <Reference Include="MyAssembly">
      <HintPath>outputpath\MyAssembly.dll</HintPath>
      <Private>True</Private>
    </Reference>

```

## Usage

```bat
    CopyLocalFixer c:\Projects\LargeProject
```