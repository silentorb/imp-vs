using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SilentOrb.Imp_VS
{
    static class Loader
    {
        public static void initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += resolve_assembly;
        }

        private static Assembly resolve_assembly(object sender, ResolveEventArgs args)
        {
            var assembly_name = args.Name.Substring(0, args.Name.IndexOf(','));

            var assembly = try_path(@"E:\Dev\imp\Imp\bin\Debug\", assembly_name);
            if (assembly != null)
                return assembly;

            return try_path(@"E:\Dev\metahub\MetaHub\bin\Debug\", assembly_name);
        }

        private static Assembly try_path(string folder, string assembly_name)
        {
            var path = folder + assembly_name + ".dll";

            if (!File.Exists(path))
                return null;

            return Assembly.LoadFile(path);
        }
    }
}
