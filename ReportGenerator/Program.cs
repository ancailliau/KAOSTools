using System;
using RazorEngine;
using System.IO;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Reflection;
using KAOSTools.DotExporter;
using KAOSTools.Parsing;
using System.Collections.Generic;
using KAOSTools.MetaModel;

namespace ReportGenerator
{
    public class ViewModel {
        public KAOSTools.MetaModel.KAOSModel Model;
        public IDictionary<KAOSMetaModelElement, IList<Declaration>> Declarations;   
    }
    
    class MainClass : KAOSTools.Utils.KAOSToolCLI
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Initialization");
            Init (args);
            var viewModel = new ViewModel {
                Model = model,
                Declarations = declarations
            };

            Console.WriteLine ("Setuping template engine");
            string viewPathTemplate = "../../Views/{0}";
            var templateConfig = new TemplateServiceConfiguration();
            templateConfig.Resolver = new DelegateTemplateResolver(name =>
                {
                    string resourcePath = string.Format(viewPathTemplate, name);
                    return File.ReadAllText (resourcePath);
                    
                });
            Razor.SetTemplateService(new TemplateService(templateConfig));

            // string template = File.ReadAllText ("./Templates/GoalModel.cshtml");
            // string result = Razor.Parse(template, new { });

            Console.WriteLine ("Copying common files");
            if (Directory.Exists ("./Report")) {
                Directory.Delete ("./Report", true);
            }
            Directory.CreateDirectory ("Report");
            DirectoryCopy ("../../Content", "./Report/Content", true);

            Console.WriteLine ("Building goal page");
            string result = Razor.Resolve ("GoalModel.cshtml", viewModel).Run(new ExecuteContext());
            File.WriteAllText ("./Report/goals.html", result);
            
            Console.WriteLine ("Building agent page");
            result = Razor.Resolve("AgentModel.cshtml", viewModel).Run(new ExecuteContext());
            File.WriteAllText ("./Report/agents.html", result);

            Console.WriteLine ("Building obstacle page");
            result = Razor.Resolve("ObstacleModel.cshtml", viewModel).Run(new ExecuteContext());
            File.WriteAllText ("./Report/obstacles.html", result);
            
            Console.WriteLine ("Building domain property page");
            result = Razor.Resolve("DomPropModel.cshtml", viewModel).Run(new ExecuteContext());
            File.WriteAllText ("./Report/domprops.html", result);
            
            Console.WriteLine ("Building domain hypothesis page");
            result = Razor.Resolve("DomHypModel.cshtml", viewModel).Run(new ExecuteContext());
            File.WriteAllText ("./Report/domhyps.html", result);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
