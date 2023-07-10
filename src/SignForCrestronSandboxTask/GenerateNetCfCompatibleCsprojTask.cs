using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text;
using Ionic.Zip;
using System.Xml.Linq;

namespace SignForCrestronSandboxTask
{
    public class GenerateNetCfCompatibleCsprojTask
        : Task
    {
        // https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items?view=vs-2022

        // takes the corecompile task and generates a csproj from it that is net 3.5 csc and compatible
        // and compatible with the version of mono.cecil used by the S# plugin
        // this allows development to happen with an SDK-compatible csproj and have intellisense,
        // and then temporarily build a csproj just for the sandbox build just like MSBuild would in VS2008

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateNetCfCompatibleCsprojTask"/> class.
        /// </summary>
        public GenerateNetCfCompatibleCsprojTask()
        {
        }

        /// <summary>
        /// Gets or sets the directory to the .csproj template .zip files used by Crestron
        /// These are installed by the S# build tools at
        /// C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\Crestron
        /// but may be at a difference source if the template is provided by a Nuget package.
        /// </summary>
        [Required]
        public string TemplateDirectory { get; set; }

        public string AssemblyName;

        public string Constants;

        public string OutputDirectory;

        public string OutputFileName;

        /// <summary>
        /// Referenced assemblies for the compiler. Taken from the $(ResourcePath) Msbuild prop.
        /// Project references are resolved by the ResolveAssemblyReferences MSBuild target that will run before core compile
        /// (which this task will run after anyway) so we us this and disregard the project reference MSBuild items.
        /// </summary>
        [Required]
        public IList<TaskItem> References { get; set; }

        /// <summary>
        /// List of source files for the compiler. Taken from the $(Compile) MSBuild prop.
        /// This is typically the .cs files in the project
        /// </summary>
        [Required]
        public IList<ITaskItem> Compile { get; set; }

        /// <summary>
        /// Files Represents files that are not compiled into the project, but may be embedded or published together with it.
        /// </summary>
        public IList<ITaskItem> Content;

        /// <summary>
        /// Represents files that have no role in the build process, but are (usually) copied to the output directory.
        /// </summary>
        public IList<ITaskItem> None;

        // Embedded resource from original is not supported in this task - TODO for later if needed

        // COM file references not supported because crestron sandbox prolly doesn't
        // Native referencess not supported for the same reason

        // TODO add assembly metadata and internalsvisibleto attributes

        /// <summary>
        /// Execute is part of the Microsoft.Build.Framework.ITask interface.
        /// When it's called, any input parameters have already been set on the task's properties.
        /// It returns true or false to indicate success or failure.
        /// </summary>
        public override bool Execute()
        {
            try
            {
                using (Stream fileStream = File.OpenRead(Path.Combine(TemplateDirectory, "SimplSharpLibrary.zip")))
                {
                    var files = Decompress(fileStream);
                    if (files.TryGetValue("SIMPLSharpLibrary.csproj", out var csProjTemplate))
                    {
                        var contents = Encoding.ASCII.GetString(csProjTemplate);
                        contents.Replace("$safeprojectname$", AssemblyName);
                        contents.Replace("$guid1$", new Guid().ToString());

                        XDocument xDocument = XDocument.Load(contents);
                        foreach (var itemGroup in xDocument.Root.Descendants("ItemGroup"))
                        {
                            foreach (var item in itemGroup.Descendants())
                            {
                                // remove all Compile, None, Reference, and Content nodes
                                if ((new List<string>() { "Compile", "None", "Reference", "Content" }).Where(name => name == itemGroup.Name).Any())
                                {
                                    item.Remove();
                                }
                            }
                        }
                        XElement compileItems = new XElement("ItemGroup");
                        XElement noneItems = new XElement("ItemGroup");
                        XElement contentItems = new XElement("ItemGroup");
                        XElement referenceItems = new XElement("ItemGroup");

                        foreach (var compileTarget in Compile)
                        {
                            XElement elem = new XElement("Compile");
                            elem.Add(new XAttribute("Include", compileTarget.ItemSpec);
                            foreach (var metadataName in compileTarget.MetadataNames)
                            {
                                elem.Add(new XElement((string)metadataName, compileTarget.GetMetadata((string)metadataName)));
                            }
                            compileItems.Add(elem);
                        }

                        
                        foreach (var referenceItem in References)
                        {
                            XElement elem = new XElement("Reference");
                            elem.Add(new XAttribute("Include", referenceItem.ItemSpec);
                            foreach (var metadataName in referenceItem.MetadataNames)
                            {
                                elem.Add(new XElement((string)metadataName, referenceItem.GetMetadata((string)metadataName)));
                            }
                            referenceItems.Add(elem);
                        }

                        foreach (var contentItem in Content)
                        {
                            XElement elem = new XElement("Content");
                            elem.Add(new XAttribute("Include", contentItem.ItemSpec);
                            foreach (var metadataName in contentItem.MetadataNames)
                            {
                                elem.Add(new XElement((string)metadataName, contentItem.GetMetadata((string)metadataName)));
                            }
                            contentItems.Add(elem);
                        }

                        foreach (var noneItem in None)
                        {
                            XElement elem = new XElement("Content");
                            elem.Add(new XAttribute("Include", noneItem.ItemSpec);
                            foreach (var metadataName in noneItem.MetadataNames)
                            {
                                elem.Add(new XElement((string)metadataName, noneItem.GetMetadata((string)metadataName)));
                            }
                            contentItems.Add(elem);
                        }

                        xDocument.Root.Add(compileItems));
                        xDocument.Root.Add(noneItems);
                        xDocument.Root.Add(contentItems); 
                        xDocument.Root.Add(referenceItems);

                        File.WriteAllText(Path.Combine(OutputDirectory, OutputFileName), xDocument.ToString());
                    }
                    else
                    {
                        throw new Exception("SimplSharpLibrary template not found in SimplSharpLibrary.zip at the templates directory");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                Log.LogError(ex.StackTrace);
                return false;
            }
        }

        public static Dictionary<string, byte[]> Decompress(Stream targetFileStream)
        {
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            using(ZipFile z = ZipFile.Read(targetFileStream))
            {
                foreach (ZipEntry zEntry in z)
                {
                    MemoryStream temp = new MemoryStream();
                    zEntry.Extract(temp);

                    files.Add(zEntry.FileName, temp.ToArray());
                }
            }

            return files;
        }
    }
}
