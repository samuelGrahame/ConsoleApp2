using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"C:\Users\Samuel\Desktop\id.jpg";

            //if (args == null || args.Length == 0 || !System.IO.File.Exists(args[0]))
            //    return;
            Project project = new Project();

            project.Data = System.IO.File.ReadAllBytes(fileName);
            project.Length = project.Data.Length;
            project.Cycle = 0;

            while (true)
            {
                var patternanl = new PatternAnalysis(project, 0, project.Cycle);
                patternanl.Run();
                if (patternanl.Incremental > project.Cycle)
                    project.Cycle = patternanl.Incremental;                
            }
        }
    }
}
