using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    const int Iterations = 50;

    static void Main(string[] args)
    {
        // If launched with --child argument, exit immediately as a trivial target
        if (args.Length > 0 && args[0] == "--child")
        {
            return;
        }

        string execPath = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule!.FileName;

        // Measure Process Creation Overhead
        var processStopwatch = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = execPath,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add("--child");

            using (Process? p = Process.Start(startInfo))
            {
                p?.WaitForExit();
            }
        }
        processStopwatch.Stop();

        // Measure Thread Creation Overhead
        var threadStopwatch = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            Thread t = new Thread(() => { /* Trivial work */ });
            t.Start();
            t.Join();
        }
        threadStopwatch.Stop();

        double avgProcessMs = processStopwatch.Elapsed.TotalMilliseconds / Iterations;
        double avgThreadMs = threadStopwatch.Elapsed.TotalMilliseconds / Iterations;

        Console.WriteLine($"Average process creation time: {avgProcessMs:F3} ms");
        Console.WriteLine($"Average thread creation time:  {avgThreadMs:F3} ms");
        Console.WriteLine($"Process creation was {(avgProcessMs / avgThreadMs):F1}x more expensive than thread creation.");
    }
}