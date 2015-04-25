using System;
using System.Collections.Generic;
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
            var project = get_project_by_name("deleteme", application);
            var itemPath = solution.GetProjectItemTemplate("CodeFile", "CSharp");
            var project_item = project.ProjectItems.AddFromTemplate(itemPath, "files\\MyNewClass.cs");
        }

        public static Project get_project_by_name(string name, DTE2 application)
        {
            var solution = (Solution2)application.Solution;
            foreach (Project project in solution.Projects)
            {
                if (project.Name == name)
                    return project;
            }

            return null;
        }
    }
}
