using Crestron.SIMPLSharp.VsPlugin;
using Crestron.Tools.SIMPLSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using System.Text;
using static System.Environment.SpecialFolder;

namespace Microsoft.Build.Tasks
{
    public class SignForSandboxTask : Utilities.Task
    {
        private string programInfoFileName;
        private string targetPath;
        private string targetName;
        private string packagePath;
        private string outputPath;
        private string archiveFilename;
        private string excludeTargets;
        private string references;
        private string assemblyVersion;
        private string projectDir;
        private string targetDir;

        // The Required attribute indicates the following to MSBuild:
        //	     - if the parameter is a scalar type, and it is not supplied, fail the build immediately
        //	     - if the parameter is an array type, and it is not supplied, pass in an empty array
        // In this case the parameter is an array type, so if a project fails to pass in a value for the
        // Directories parameter, the task will get invoked, but this implementation will do nothing,
        // because the array will be empty.
        [Required]
        public string ProgramInfoFileName
        {
            get => programInfoFileName;
            set => programInfoFileName = value;
        }

        [Required]
        public string TargetPath
        {
            get => targetPath;
            set => targetPath = value;
        }

        [Required]
        public string TargetName
        {
            get => targetName;
            set => targetName = value;
        }

        [Required]
        public string PackagePath
        {
            get => packagePath;
            set => packagePath = value;
        }

        [Required]
        public string OutputPath
        {
            get => outputPath;
            set => outputPath = value;
        }

        [Required]
        public string ArchiveFilename
        {
            get => archiveFilename;
            set => archiveFilename = value;
        }

        [Required]
        public string ExcludeTargets
        {
            get => excludeTargets;
            set => excludeTargets = value;
        }
        [Required]
        public string References
        {
            get => references;
            set => references = value;
        }
        [Required]
        public string AssemblyVersion
        {
            get => assemblyVersion;
            set => assemblyVersion = value;
        }
        [Required]
        public string ProjectDir
        {
            get => projectDir;
            set => projectDir = value;
        }
        [Required]
        public string TargetDir
        {
            get => targetDir;
            set => targetDir = value;
        }

        /// <summary>
        /// Execute is part of the Microsoft.Build.Framework.ITask interface.
        /// When it's called, any input parameters have already been set on the task's properties.
        /// It returns true or false to indicate success or failure.
        /// </summary>
        public override bool Execute()
        {
            try
            {
                // Constants setup
                // TODO Expose this to MSBuild 
                string logfolder = @"SIMPLSharpLogs";
                string datapath = Path.Combine(Environment.GetFolderPath(ProgramFilesX86), @"Crestron\Cresdb\Programming");
                bool refreshSdk = true;

                string path;
                // MSBuild docs say this has no trailing slash, but it appears it really does.
                // We don't want the trailing slash for the plugin
                if (projectDir[projectDir.Length - 1].Equals(Path.DirectorySeparatorChar))
                {
                    path = projectDir.Remove(projectDir.Length - 1, 1);
                }
                else
                {
                    path = projectDir;
                }


                string logpath = Path.Combine(path, logfolder);

                string absoluteOutputPath = targetDir;
                string fullAssemblyName = targetPath;
                string compiledFileName = archiveFilename;
                string archiveFullName = Path.Combine(absoluteOutputPath, compiledFileName);

                // setup the SIMPLSharp service
                const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
                var p = new SIMPLSharpPackage();
                Type t = p.GetType();
                var method = t.GetMethod("d", bf);
                if (method != null)
                {
                    method.Invoke(p, null);
                }
                else
                {
                    throw new Exception("Could not initialize the SimplSharp package. Initialization method was null.");
                }

                var data = t.GetProperty("ProviderConfig", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(p, null);

                SIMPLSharpService s;
                if (data != null)
                {
                    s = new SIMPLSharpService(data, targetName, path, logpath, refreshSdk, datapath);
                }
                else
                {
                    throw new Exception("Could not initialize SmplSharpService from plugin. Data was null.");
                }

                // these three fields are set up outside of making the simpl sharp service for some reason
                if (s.ProjectInfo.CompiledFilename == null)
                {
                    s.ProjectInfo.CompiledFilename = archiveFullName;
                }
                if (s.ProjectInfo.ProgramIdTag == null)
                {
                    s.ProjectInfo.ProgramIdTag = targetName;
                }
                if (s.ProjectInfo.SystemName == null)
                {
                    s.ProjectInfo.SystemName = targetName;
                }
                if (s.ProjectInfo.MinimumFirmwareVersion == null)
                {
                    s.SetMinimumFirmwareVersion("1.007.0017"); // TODO figure out how to systematically generate this?
                }

                // SDK resources not set by the SIMPLSharpProject class or any decendants
                // set by SIMPLSharpPackage			
                // the vsplugin builds this by comparing the project references with the list of sdkassemblies from SimplSharp service 
                // and the system assemblies from simplsharp service, and the deprecated assemblies from simplsharp service
                IList<ReferenceInfo> projectReferences = new List<ReferenceInfo>(); // TODO add code to generate this. Can we get from MSBuild?

                // I can't remember what the purpose of the result of this was in the actual plugin.
                // TODO determine what to do with the result here - tests didn't use the result
                s.ReconcileProjectReferences(projectReferences);
                // I can't remember what the purpose of the result of this was in the actual plugin.
                // TODO determine what to do with the result here - tests didn't use the result
                s.IsAssemblyVerified(fullAssemblyName);

                IList<string> resolutionPaths = new List<string>
                {
                    absoluteOutputPath,
                    SIMPLSharpService.SDKPath
                };
                // string passed is the full assembly name, which we get from SimplSharp project FullAssemblyName
                // for generateDebugSysmbols, we *may* have to set this to false in order to get Mono.Cecil to like the pdb format
                // TODO determine if we need to true/false and if that can depend on the project debug/release configuration passed in from MSBuild
                var result1 = s.ValidateAssembly(fullAssemblyName, resolutionPaths, false);
                if (!result1.IsValid)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var error in result1.Errors)
                    {
                        sb.Append($"Sandbox validation error: {error}\r\n");
                    }
                    throw new Exception(sb.ToString());
                }

                //string passed is the full assembly name, which we get from Simplsharp project FullAssemblyName
                var result2 = s.VerifyAssembly(fullAssemblyName);
                if (!result2)
                {
                    throw new Exception("Assembly could not be verified by S# plugin.");
                }

                //Create a list of dependency info
                var dependencies = new List<ReferenceInfo>(); // list of all files in the output path that have a .dll or .exe extension that isn't FullAssemblyName
                List<string> source = new List<string>
                {
                    ".dll",
                    ".exe"
                };
                string[] filesInDirectory = Directory.GetFiles(absoluteOutputPath, "*.*", SearchOption.AllDirectories);
                foreach (var text in filesInDirectory)
                {
                    if (source.Contains(Path.GetExtension(text), StringComparer.OrdinalIgnoreCase) && text != fullAssemblyName)
                    {
                        ReferenceInfo item = new ReferenceInfo
                        {
                            CopyLocal = true,
                            Filename = text, // full assembly name for file - dll or exe, ie. @"C:\Users\Username\OtherDirs\ProjectDir\bin\Debug\Newtonsoft.Json.Compact.dll"
                            ReferenceType = ReferenceType.Assembly,
                            IsVerified = true
                        };
                        dependencies.Add(item);
                    }
                }

                // list of everything in the output path that doesn't have .info, .pdb, .config, .clz, .cplz, .cpz, .dll, and .exe? Unsure if it actually does that, it builds a list of that and never returns it.
                // list of everything in the SimplSharpProject's SDK resources enumerator then gets copied to the output path and also added to this list
                var simplSharpReferences = new List<string>();
                foreach (ResourceFileInfo resourceFileInfo in SIMPLSharpService.SDKResources)
                {
                    if (!simplSharpReferences.Contains(resourceFileInfo.Filename, StringComparer.OrdinalIgnoreCase))
                    {
                        string text = Path.Combine(absoluteOutputPath, resourceFileInfo.Filename);
                        File.Copy(resourceFileInfo.Fullname, text, true);
                        simplSharpReferences.Add(text);
                    }
                }

                Log.LogMessage("Creating project manifest");
                s.CreateManifest(fullAssemblyName, compiledFileName, dependencies, simplSharpReferences);

                // If the project depends on the Crestron SDK nuget package to do the archiving we don't need to use the
                // below. Included for posterity
                /*
                IList<string> ListOfDllsInProjectInfo;
                // I think the below is how the plugin does it.
                ListOfDllsInProjectInfo = SIMPLSharpService.SystemAssemblies
                    .Select(x => x.Filename)
                    .Distinct()
                    .ToList<string>();

                 s.CreateArchive(
					absoluteOutputPath,
					ListOfDllsInProjectInfo,
					archiveFullName); 
				*/                

                return true;

            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                Log.LogError(ex.StackTrace);
                return false;
            }
        }
    }
}
