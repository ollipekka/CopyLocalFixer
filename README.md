# CopyLocalFixer

## Objective

In large builds assemblies are sometimes excessively copied. CopyLocalFixer attempts to optimize builds on slow machines by tuning CopyLocal conservatively.

## CopyLocal=false

CopyLocalFixer goes through all projects through specific folder. For each project it checks whether the references are found in same folder. If reference is built in the same folder CopyLocalFixer adds xml element <Private>False</Private> to the reference element. CopyLocalFixer needs Reference elements to have HintPath elements in order to operate.

Resulting reference element after CopyLocalChecker has done its trick:

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

Or just cloning the source and runnig it from solution.

## Caveats

CopyLocal has to be true when [1]:

* The assembly is expected to be found in GAC.
* The assembly is loaded via reflection at runtime.

Eagerly setting CopyLocal to false may cause runtime issues. [2]

## ToDo

* Nuget package for easy tooling.
* Unit tests.

## References

[1]: https://stackoverflow.com/questions/690033/best-practices-for-large-solutions-in-visual-studio-2008
[2]: http://geekswithblogs.net/mnf/archive/2012/12/09/do-not-change-copy-local-project-references-to-false-unless.aspx