# CrestronSandboxBuildSample

Samples of building for a Crestron 3-series library using Visual Studio 2019

This is a work-in-progress.

## Prerequisites
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
4) Crestron ProgramLibrary and Program samples
5) An example of succesfully Multitargeting a library/program with an SDK-style .csproj and having compiler directives to utilize sandbox/non-sandbox function
6) A sample of building on non-windows systems (user must provide own Crestron plugin dll for signing)
7) Test builds with visual studio 2022

