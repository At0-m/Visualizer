using System;
using System.Diagnostics;
using System.IO;

namespace Visualizer
{
    public static class DotVisualizer
    {
        private static string dotPath = "Graphviz-12.2.1-win64\\bin\\dot.exe";

        public static void RenderDotToPng(string inputDotFile, string outputPngFile)
        {
            if (!File.Exists(inputDotFile))
                throw new FileNotFoundException("The DOT file was not found: " + inputDotFile);

            if (File.Exists(outputPngFile))
            {
                 File.Delete(outputPngFile);
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = dotPath;
            psi.Arguments = $"-Tpng {inputDotFile} -o {outputPngFile}";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using (var proc = Process.Start(psi))
            {
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new Exception("Error when calling dot.exe . Make sure Graphviz is installed and dot is available");
                }
            }
        }
    }
}