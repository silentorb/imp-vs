using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace Imp_VS
{
    internal static class Actions
    {
        public static void create_project_item(DTE2 application)
        {
            var solution = (Solution2)application.Solution;
            var gen_folders = new List<ProjectItem>();
            
            foreach (Project project in solution.Projects)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    if (item.Name == "gen")
                    {
                        gen_folders.Add(item);
                        break;
                    }
                }
            }

            foreach (var folder in gen_folders)
            {
               synchronize(folder);
            }

        }

        static void synchronize(ProjectItem root)
        {
            var path = root.FileNames[0];
            var files = Directory.GetFiles(path);

            var missing_items = root.ProjectItems.Cast<ProjectItem>()
                .Where(item => !File.Exists(item.FileNames[0])).ToArray();

            foreach (ProjectItem item in missing_items)
            {
                item.Remove();
            }

            foreach (var file in files)
            {
                root.ProjectItems.AddFromFile(file);
            }

            var folders = Directory.GetDirectories(path);
            
            foreach (var folder in folders)
            {
                var folder_name = Path.GetFileName(folder);
                var child_folder = get_child_by_name(root, folder_name) 
                    ?? root.ProjectItems.AddFolder(folder_name);

                synchronize(child_folder);
            }
        }

        static ProjectItem get_child_by_name(ProjectItem root, string name)
        {
            return root.ProjectItems.Cast<ProjectItem>()
                .FirstOrDefault(item => item.Name == name);
        }
    }
}
