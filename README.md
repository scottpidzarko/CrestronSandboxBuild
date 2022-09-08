# CrestronSandboxBuildSample

Samples of building Crestron

This is a work-in-progress.

## TODO

1) Minimum viable product
2) Documentation and user guides
3) Confirming this works from a freshly setup windows 10 system
4) Confirming this works from a freshly setup windows 11 system
5) User testing with a variety of real life projects

Stopping points

1) Registry edits needed?
2) File pathing - for people without C:\ drive?

## Stretch goals include

1) Introducing a Nuget package that provides the props/targets files and invokes the signing functionality using the Crestron plugin. Crestron plugin installed by user unless Crestron agrees to authorize distribution of the dll.
2) Changing nuget package to play nice with Crestron's nuget packages and utilize the dlls distributed by the nuget package
3) Custom C# project template for anything that can't be made with the nuget package?
4) An example of succesfully Multitargeting a library/program with an SDK-style .csproj and having compiler directives to utilize sandbox/non-sandbox function
5) A sample of building on non-windows systems (user must provide own Crestron plugin dll for signing)

