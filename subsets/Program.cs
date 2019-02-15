using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace subsets
{
    class Program
    {

        static void Main(string[] args)
        {
            var input = Enumerable.Range(1, 4);
            
            var cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Press ESC to cancel");

            var keyTask = StartKeyboardTask(cancellationTokenSource);

            var processTask = StartProcessTask(input, cancellationTokenSource);

            try
            {
                Task.WaitAll(processTask, keyTask);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }

        private static Task StartKeyboardTask(CancellationTokenSource cancellationTokenSource)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) cancellationTokenSource.Cancel();
                    }
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Keyboard task ended");
                }
            });
        }

        private static Task StartProcessTask<T>(IEnumerable<T> input, CancellationTokenSource cancellationTokenSource)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    OutputSubsets(input, cancellationTokenSource.Token);

                    cancellationTokenSource.Cancel();
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        private static IEnumerable<IEnumerable<T>> OutputSubsets<T>(IEnumerable<T> input, CancellationToken cancellationToken)
        {
            var subsets = new List<IEnumerable<T>>();

            foreach (var t in input)
            {
                var subsetCount = subsets.Count;

                subsets.Add(new List<T> { t });

                OutputSubset(subsets.Last());

                for (var j = 0; j < subsetCount; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    subsets.Add(subsets[j].Append(t));

                    OutputSubset(subsets.Last());
                }
            }

            return subsets;
        }

        private static void OutputSubset<T>(IEnumerable<T> subset)
        {
            Console.WriteLine($"[{string.Join(",", subset)}]");
        }
    }
}

//  1   2   3   4

//  1
//  2 
//  1   2
//  3
//  1   3
//  2   3
//  1   2   3
//  4
//  1   4
//  2   4
//  1   2   4
//  3   4
//  1   3   4
//  2   3   4
//  1   2   3   4
