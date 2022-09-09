# CrestronSandboxBuildSample

Samples of building for a Crestron 3-series library using Visual Studio 2019. This is a work-in-progress.

## Prerequisites

### Environment setup
Your machine should be setup to build Crestron C# libraries/programs for 3-series processors. If you haven't yet setup for 3-series development with VS2008 and the Crestron plugin, at minimum you'll need to do the following:

1) Enable dotnet framework 3.5 on the turn windows features on and off
2) Install NETCFSetupv35.msi
3) Install NETCFv35PowerToys.msi
4) Install the Crestron plugin

There may need to be registry keys added too - stay tuned about that.

### S# Library
1) In visual studio, create a new .NET framework project. Target framework version should be 3.5.
2) Unload the project in solution view (right click the project -> unload project)
3) Double click the project to edit the `.csproj` file manually
4) Replace the contents of your `.csproj` file the `.csproj` file from this repository
5) Copy the `Test.Targets` file from your repository to the same directory as your `.csproj` file
6) Proceed to develop as normal. If a warning about "The target "ResolveSDKReferences" does not exist in the project." shows up on the error list, it can be ignored. It will go away as soon as you build/rebuild the project.

## TODO

1) Registry edits needed? I had to do this in testing. Compact framework build targets look for registry keys to determine tool paths and file paths it needs and those were not set as part of the compact framework install path. I don't know if power toys would either. I don't know if they would get installed as part of net framework 2.0/3.5 SDK but have gotten cleaned due to windows updates. We can provide a .reg file if needed but this can be a manual step that makes adoption poorer.
2) File pathing - for people without C:\ drive? This shouldn't be a huge issue, MSBuild provides lots of properties we can utilize. A little bit of research
3) Make a .props file as well that the .csproj can reference so that the actual .csproj stays as vanilla as possible.
4) More Documentation and user guides
5) Confirming this works from a freshly setup windows 10 system
6) Confirming this works from a freshly setup windows 11 system
7) User testing with a variety of real life projects
8) Figure out how to get rid of the warning about "ResolveSDKReferences" target not existing
  
## Stretch Goals

1) Introducing a Nuget package that provides the props/targets files and invokes the signing functionality using the Crestron plugin. Crestron plugin installed by user unless Crestron agrees to authorize distribution of the dll.
2) Changing nuget package to play nice with Crestron's nuget packages and utilize the dlls distributed by the nuget package
3) Custom C# project template for anything that can't be made with the nuget package?
4) Offer custom MSBuild properties in the .csproj to turn on S# plugin logging and to customize the S# plugin directory
4) Crestron ProgramLibrary and Program samples
5) An example of succesfully Multitargeting a library/program with an SDK-style .csproj and having compiler directives to utilize sandbox/non-sandbox function. I have this mostly working but there are issues with a registry key for net 2.0 that isn't an issue for an older style `.csproj`
6) A sample of building on non-windows systems (user must provide own Crestron plugin dll for signing)
7) Test builds with visual studio 2022

## Resources and background

This came about because I was tired of opening VS2008 to do 3-series development. It offered less refactoring features, poorer Intellisense, no integration with git out of the box, but I think the thing that annoyed me the most was that it's "dark mode" wasn't quite dark mode. Then one day, I came accross [this](https://gist.github.com/skarllot/4953ddb6e23d8a6f0816029c4155997a) gist.

And so my dive into tinkering with the Crestron plugin, learning *way* more about MSBuild than I had originally intended, and many evenings of troubleshooting and research began. 

Here's a list of sites I kept in my bookmarks while working on this:

https://docs.microsoft.com/en-us/visualstudio/msbuild/standard-and-custom-toolset-configurations?view=vs-2022
https://kb.froglogic.com/coco/howto/visualstudio-verbosity/
https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-dot-targets-files?view=vs-2022
https://github.com/WindowsCE/NETStandard.WindowsCE/blob/master/NETStandard.WindowsCE.props
https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2008/bb383796(v=vs.90)?redirectedfrom=M
https://github.com/dotnet/roslyn/blob/main/docs/compilers/CSharp/Compiler%20Breaking%20Changes%20-%20VS2015.md
https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/breaking-changes?source=recommendations
https://natemcmaster.com/blog/2017/07/05/msbuild-task-in-nuget/
 
A special thanks to [Troy](https://github.com/bitm0de) and [Andrew](https://github.com/andrew-welker) for their help and pointers they gave me along the way with this project.
