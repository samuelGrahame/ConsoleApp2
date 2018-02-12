using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class PatternAnalysis
    {
        public long Incremental;
        public Random Rng;
        public Project Project;
        public long Cycle;
        public int Length;
        public bool HasSetupRan;
        public int Seed;

        public PatternAnalysis(Project project, int seed, long cycle)
        {
            Project = project;
            Seed = seed;
            Rng = new Random(seed);
            Cycle = cycle;
            Length = Project.Length;            
        }

        public void SetupRng()
        {
            if (HasSetupRan)
                return;

            for (Incremental = 0; Incremental < Cycle; Incremental++)
            {
                Rng.Next(0, Length);
            }
            HasSetupRan = true;
        }

        public void Run()
        {
            SetupRng();
            // create empty bytearray of size;

            byte[] data = new byte[Length];
            List<int> indexMatched = new List<int>();
            int i = 0;

            while(indexMatched.Count != Length)
            {
                var index = Rng.Next(0, Length);
                Incremental++;
                if (!indexMatched.Contains(index))
                {
                    data[index] = Project.Data[i];
                    indexMatched.Add(index);
                    i++;

                    if (i % 100 == 0)
                        Console.WriteLine(i.ToString());
                }
            }

            // match compression score...

            Compress(ref data, out data);

            if(data.Length < Length)
            {
                // passed in some degree
                var outScore = data.Length / Length;
                lock (Project)
                {
                    if (outScore < Project.BestRating)
                    {
                        Project.BestRating = outScore;
                        Project.BestRatingCycle = Cycle;
                        Project.BestRatingSeed = Seed;

                        Console.WriteLine("New Findings: Ratio: " + outScore);
                    }
                    else
                    {
                        // discard findings...
                        Console.WriteLine("Discarded Cycle " + Cycle);
                    }
                }
            }
            else
            {
                // equal or larger... - failed.
                // discard.
                Console.WriteLine("Discarded Cycle " + Cycle);
            }            
        }

        public static void Compress(ref byte[] inputData, out byte[] outData)
        {            
            using (var compressIntoMs = new MemoryStream())
            {
                using (var gzs = new BufferedStream(new GZipStream(compressIntoMs,
                 CompressionMode.Compress)))
                {
                    gzs.Write(inputData, 0, inputData.Length);
                }
                outData = compressIntoMs.ToArray();
            }
        }

    }
}
