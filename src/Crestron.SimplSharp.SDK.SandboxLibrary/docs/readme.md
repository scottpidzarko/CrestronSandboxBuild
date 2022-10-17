This package modifies your .csproj and AssemblyInfo.cs files from the base .net framework 3.5 template to with the following lines getting commented out. 
These will need to be manually reverted if you ever need to uninstall this package and wish to continue developing applications that don't utilize Compact Framework 3.5!

These two lines will get commented out in your .csproj
```xml 
<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />";
```
and
```xml 
"<Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />";
```

In AssemblyInfo.cs, the `[assembly: AssemblyFileVersion("1.0.0.0")]` line will get commented out.

