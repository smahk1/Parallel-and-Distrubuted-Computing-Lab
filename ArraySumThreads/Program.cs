using System;
using System.Threading;

class Program
{
    static long[] data = new long[10_000_000];
    static long[] partialSums = Array.Empty<long>();
    static int numWorkers;

    static void SumSlice(int idx)
    {
        int sliceSize = data.Length / numWorkers;
        int start = idx * sliceSize;
        int end = (idx == numWorkers - 1) ? data.Length : start + sliceSize;

        long sum = 0;
        for (int i = start; i < end; i++)
        {
            sum += data[i];
        }
        partialSums[idx] = sum;
    }

    static void Main()
    {
        // Populate array with sequential numbers 1 to 10,000,000
        for (int i = 0; i < data.Length; i++) 
        {
            data[i] = i + 1;
        }

        numWorkers = Environment.ProcessorCount;
        partialSums = new long[numWorkers];
        Thread[] threads = new Thread[numWorkers];

        // Launch worker threads
        for (int i = 0; i < numWorkers; i++)
        {
            int idx = i; // Capture loop variable locally
            threads[i] = new Thread(() => SumSlice(idx));
            threads[i].Start();
        }

        // Wait for all worker threads to complete
        for (int i = 0; i < numWorkers; i++)
        {
            threads[i].Join();
        }

        // Sum the partial results
        long threadedTotal = 0;
        foreach (long partial in partialSums) 
        {
            threadedTotal += partial;
        }

        // Compute sequential total for validation
        long sequentialTotal = 0;
        foreach (long value in data) 
        {
            sequentialTotal += value;
        }

        Console.WriteLine($"Worker Threads:   {numWorkers}");
        Console.WriteLine($"Threaded total:   {threadedTotal}");
        Console.WriteLine($"Sequential total: {sequentialTotal}");
        Console.WriteLine($"Match:            {threadedTotal == sequentialTotal}");
    }
}