using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using imperative;
using metahub.render;
using Newtonsoft.Json;

namespace SilentOrb.Imp_VS
{
    internal class Target
    {
        public string path;
        public string type;
    }

    internal class Project_Configuration
    {
        public string[] sources;
        public Target[] targets;
    }

    class Actions
    {

        public static void create_project_item(DTE application)
        {
            var solution = (Solution2)application.Solution;
            var imp_configs = new List<ProjectItem>();

            foreach (Project project in solution.Projects)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    if (item.Name == "project.json")
                    {
                        imp_configs.Add(item);
                        break;
                    }
                }
            }

            foreach (var folder in imp_configs)
            {
                process_project_config(folder);
            }
        }

        static void process_project_config(ProjectItem config_file)
        {
            // Load configuration
            var project = config_file.ContainingProject;
            var config_contents = File.ReadAllText(config_file.FileNames[0]);
            var config = JsonConvert.DeserializeObject<Project_Configuration>(config_contents);
            var target = config.targets[0];
            var gen_name = target.path ?? "gen";
            var gen = get_child_by_name(project.ProjectItems, gen_name);
            /*
            // Run overlord
            var overlord = new Overlord(target.type);
            var project_path = Path.GetDirectoryName(project.FullName);
            var input = Path.Combine(project_path, config.sources[0]);
            var files = Directory.Exists(input)
                ? Overlord.aggregate_files(input)
                : new List<string> { input };

            var codes = files.Select(File.ReadAllText);
            try
            {
                overlord.summon_many(codes);
            }
            catch (Exception ex)
            {
                Imp_VSPackage.instance.add_error(ex);
            }
            overlord.flatten();
            overlord.post_analyze();
            var output_path = gen.FileNames[0];
            Generator.clear_folder(output_path);
            overlord.target.run(output_path);
            */
            
            // Synchronize generated files
            var folders = Directory.GetDirectories(gen.FileNames[0]);
            foreach (var folder in folders)
            {
                gen.ProjectItems.AddFromDirectory(folder);
                
            }
            synchronize(gen);
        }

        static void synchronize(ProjectItem root)
        {
            var path = root.FileNames[0];
            var files = Directory.GetFiles(path);

            var missing_items = root.ProjectItems.Cast<ProjectItem>()
                .Where(item => !File.Exists(item.FileNames[0]) && !Directory.Exists(item.FileNames[0])).ToArray();

            foreach (ProjectItem item in missing_items)
            {
                item.Remove();
            }

//            foreach (var file in files)
//            {
//                root.ProjectItems.AddFromFile(file);
//            }

            var folders = Directory.GetDirectories(path);

            foreach (var folder in folders)
            {
                var folder_name = Path.GetFileName(folder);
                var child_folder = get_child_by_name(root.ProjectItems, folder_name);
//                    ?? root.ProjectItems.AddFolder(folder_name);

                if (child_folder != null)
                    synchronize(child_folder);
            }
        }

        static ProjectItem get_child_by_name(ProjectItems items, string name)
        {
            return items.Cast<ProjectItem>().FirstOrDefault(item => item.Name == name);
        }
    }
}
