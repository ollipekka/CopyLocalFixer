# CopyLocalFixer

## Objective

In large builds assemblies are sometimes excessively copied. CopyLocalFixer attempts to optimize builds on slow machines by tuning CopyLocal conservatively.

## CopyLocal=false

CopyLocalFixer goes through all projects in specific folder. For each all references found in the outputpath of the assembly CopyLocalFixer adds  `<Private>False</Private>` under the reference element. CopyLocalFixer needs Reference elements to have HintPath elements in order to operate.

Resulting reference element after CopyLocalChecker has done its trick:

```xml
...
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    ...
    <OutputPath>outputpath\</OutputPath>
    ...
</PropertyGroup>
...
<Reference Include="MyAssembly">
    <HintPath>outputpath\MyAssembly.dll</HintPath>
    <Private>False</Private>
</Reference>
...

```

## Usage

```bat
    CopyLocalFixer c:\Projects\LargeProject
```

Or just cloning the source and runnig it from solution.

## Caveats

CopyLocal has to be true when [stackoverflow]:

* The assembly is expected to be found in GAC.
* The assembly is loaded via reflection at runtime.

Eagerly setting CopyLocal to false may cause runtime issues. [runtime-errors]

CopyLocalFixer won't work:

* References do not contain HintPaths.
* Debug / Release output paths differ.

## ToDo

* Nuget package for easy tooling.
* Unit tests.

[stackoverflow]: https://stackoverflow.com/questions/690033/best-practices-for-large-solutions-in-visual-studio-2008
[runtime-errors]: http://geekswithblogs.net/mnf/archive/2012/12/09/do-not-change-copy-local-project-references-to-false-unless.aspx