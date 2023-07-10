# CrestronSandboxBuildSample

Samples of building for a Crestron 3-series library using Visual Studio 2019 or Jetbrains Rider. This is a work-in-progress.

## Prerequisites

### Environment setup
Your machine should be setup to build Crestron C# libraries/programs for 3-series processors. If you haven't yet setup for 3-series development with VS2008 and the Crestron plugin, at minimum you'll need to do the following:

1) Install Visual Studio 2019 (Tested with Community edition but other versions should work OK). Make sure the following components are selected
* Workloads - select .NET development
* Individual Components - .Net Framework 3.5 development tools
* Individual Components - .Net Framework 4.7 SDK (if you also want to do 4-series development)
* Individual Components - .Net Framework 4.7 targeting pack (if you also want to do 4-series development)
2) Enable ".NET Framework 3.5 (includes .NET 2.0 and 3.0)" on the "turn windows features on and off" dialog
3) Install NETCFSetupv35.msi (get this from archive.org or from Crestron's support site if you are a Crestron partner)
4) Install Visual Studio 2008 - this is needed because the Crestron plugin checks for it
5) Install Visual Studio 2008 Service pack 1 - this is also needed because the Crestron plugin checks for it
6) Install Crestron Toolbox and Device Database using Master installer (again, because the Crestron plugin checks for it)
7) Install the Crestron S# plugin

Note that all that we really need from steps 4-8 are the Crestron plugin DLLs. If in the future Crestron can be convinced to provide the dlls along with a nuget package then that will dramatically lower the amount of effort needed.

### S# Library
1) In visual studio, create a new .NET framework project. Target framework version should be **3.5**.
2) Install the corresponding [Crestron.SimplSharp.SDK.SandboxLibrary](https://www.nuget.org/packages/Crestron.SimplSharp.SDK.SandboxLibrary/) nuget package. This provides everything needed.
3) Build the solution. The first time there will be an error, this is normal. This is because the package makes edits on the first build to your `.csproj` file to support targeting compact framework. All other builds will work as normal.
4) Proceed to develop as normal. If a warning about "The target "ResolveSDKReferences" does not exist in the project." shows up on the error list when using Visual Studio, it can be ignored. It will go away as soon as you build/rebuild the project.

### S# ProgramLibrary
1) In visual studio, create a new .NET framework project. Target framework version should be **3.5**.
2) Install the corresponding [Crestron.SimplSharp.SDK.SandboxProgramLibrary](https://www.nuget.org/packages/Crestron.SimplSharp.SDK.SandboxProgramLibrary/) nuget package. This provides everything needed.
3) Build the solution. The first time there will be an error, this is normal. This is because the package makes edits on the first build to your `.csproj` file to support targeting compact framework. All other builds will work as normal.
4) Proceed to develop as normal. If a warning about "The target "ResolveSDKReferences" does not exist in the project." shows up on the error list when using Visual Studio, it can be ignored. It will go away as soon as you build/rebuild the project.

### S# Program
1) In visual studio, create a new .NET framework project. Target framework version should be **3.5**.
2) Install the corresponding [Crestron.SimplSharp.SDK.SandboxProgram](https://www.nuget.org/packages/Crestron.SimplSharp.SDK.SandboxProgram/) nuget package. This provides everything needed.
3) Build the solution. The first time there will be an error, this is normal. This is because the package makes edits on the first build to your `.csproj` file to support targeting compact framework. All other builds will work as normal.
4) Proceed to develop as normal. If a warning about "The target "ResolveSDKReferences" does not exist in the project." shows up on the error list when using Visual Studio, it can be ignored. It will go away as soon as you build/rebuild the project.

## Known Issues

See the issues list on Github. A summary:

* There is no intellisense when using visual studio - this is related to the "ResolveSDKReferences" warning in Visual Studio
* Visual Studio 2022 has switched to using 64-bit MSBuild, so inline code task using COM Interop assemblies to sign the output for the sandbox does not build correctly. See the "Feature_VS2022_Support" Branch for this
* The SandboxLibrary nuget package fails on the first build - in Rider. Unknown if Visual studio works around this with it's design time builds

## TODO

1) ~~Registry edits needed? I had to do this in testing. Compact framework build targets look for registry keys to determine tool paths and file paths it needs and those were not set as part of the compact framework install path. I don't know if power toys would either. I don't know if they would get installed as part of net framework 2.0/3.5 SDK but have gotten cleaned due to windows updates. We can provide a .reg file if needed but this can be a manual step that makes adoption poorer.~~ DONE, we do not need these registry paths on a new system. They may still be required as part of the SDK-style project stretch goal but this can be revisited then
2) ~~File pathing - for people without C:\ drive? This shouldn't be a huge issue, MSBuild provides lots of properties we can utilize. A little bit of research~~ COMPLETE, needs testing
3) Make a .props file as well that the .csproj can reference so that the actual .csproj stays as vanilla as possible.
4) More Documentation and user guides
5) ~~Confirming this works from a freshly setup windows 10 system~~ DONE
6) Confirming this works from a freshly setup windows 11 system
7) User testing with a variety of real life projects
8) Figure out how to get rid of the warning about "ResolveSDKReferences" target not existing - In Progress. 
  
## Stretch Goals

1) ~~Introducing a Nuget package that provides the props/targets files and invokes the signing functionality using the Crestron plugin. Crestron plugin installed by user unless Crestron agrees to authorize distribution of the dll.~~ DONE
2) ~~Changing nuget package to play nice with Crestron's nuget packages and utilize the dlls distributed by the nuget package~~ Done
3) Custom C# project template for anything that can't be made with the nuget package?
4) Offer custom MSBuild properties in the .csproj to turn on S# plugin logging and to customize the S# plugin directory
4) ~~Crestron ProgramLibrary and Program samples~~ DONE
5) An example of succesfully Multitargeting a library/program with an SDK-style .csproj and having compiler directives to utilize sandbox/non-sandbox function. I have this mostly working but there are issues with a registry key for net 2.0 that isn't an issue for an older style `.csproj`
6) A sample of building on non-windows systems (user must provide own Crestron plugin dll for signing)
7) Test builds with visual studio 2022 - In Progress

## Resources and background

This came about because I was tired of opening VS2008 to do 3-series development. It offered less refactoring features, poorer Intellisense, no integration with git out of the box, but I think the thing that annoyed me the most was that it's "dark mode" wasn't quite dark mode. Then one day, I came accross [this](https://gist.github.com/skarllot/4953ddb6e23d8a6f0816029c4155997a) gist.

And so my dive into tinkering with the Crestron plugin, learning *way* more about MSBuild than I had originally intended, and many evenings of troubleshooting and research began. 

Here's a list of sites I kept in my bookmarks while working on this:

https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/bb397456(v=vs.100)?redirectedfrom=MSDN

https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-task-reference?view=vs-2022

https://learn.microsoft.com/en-us/visualstudio/msbuild/build-process-overview?source=recommendations&view=vs-2022

https://learn.microsoft.com/en-us/visualstudio/msbuild/standard-and-custom-toolset-configurations?view=vs-2022

https://learn.microsoft.com/en-us/visualstudio/msbuild/how-to-extend-the-visual-studio-build-process?view=vs-2022

https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-toolset-toolsversion?view=vs-2022

https://learn.microsoft.com/en-us/visualstudio/msbuild/overriding-toolsversion-settings?source=recommendations&view=vs-2022

https://askcodes.net/coding/is-there-a-simple-way-to-make-visual-studio-2015-use-a-specific-toolsversion-

https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022

https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-task-reference?view=vs-2022

https://kb.froglogic.com/coco/howto/visualstudio-verbosity/

https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-dot-targets-files?view=vs-2022

https://github.com/WindowsCE/NETStandard.WindowsCE/blob/master/NETStandard.WindowsCE.props

https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2008/bb383796(v=vs.90)?redirectedfrom=M

https://github.com/dotnet/roslyn/blob/main/docs/compilers/CSharp/Compiler%20Breaking%20Changes%20-%20VS2015.md

https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/breaking-changes?source=recommendations

https://natemcmaster.com/blog/2017/07/05/msbuild-task-in-nuget/

https://claires.site/2017/01/04/multi-targeting-the-world-a-single-project-to-rule-them-all/

https://learn.microsoft.com/en-us/visualstudio/ide/visual-studio-multi-targeting-overview?view=vs-2022

### Credits
 
A special thanks to [Troy](https://github.com/bitm0de) and [Andrew](https://github.com/andrew-welker) for their help and pointers they gave me along the way with this project.
