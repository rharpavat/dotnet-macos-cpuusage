using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace MacHighCpu
{
    public static class ConsoleUtility
    {
        /// <summary>
        /// Block the calling thread until shutdown is triggered via Ctrl+C or SIGTERM.
        /// </summary>
        public static void WaitForShutdown()
        {
            WaitForShutdownAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns a Task that completes when shutdown is triggered via the given token, Ctrl+C or SIGTERM.
        /// </summary>
        /// <param name="token">The token to trigger shutdown.</param>
        public static async Task WaitForShutdownAsync(CancellationToken token = default(CancellationToken))
        {
            var done = new ManualResetEventSlim(false);
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                AttachCtrlcSigtermShutdown(cts, done, shutdownMessage: string.Empty);
                await WaitForTokenShutdownAsync(cts.Token);
                done.Set();
            }
        }

        private static async Task WaitAsync(CancellationToken token, string shutdownMessage)
        {
            if (!string.IsNullOrEmpty(shutdownMessage))
            {
                Console.WriteLine(shutdownMessage);
            }
            await WaitForTokenShutdownAsync(token);
        }

        private static void AttachCtrlcSigtermShutdown(CancellationTokenSource cts, ManualResetEventSlim resetEvent, string shutdownMessage)
        {
            void ShutDown()
            {
                if (!cts.IsCancellationRequested)
                {
                    if (!string.IsNullOrWhiteSpace(shutdownMessage))
                    {
                        Console.WriteLine(shutdownMessage);
                    }
                    try
                    {
                        cts.Cancel();
                    }
                    catch (ObjectDisposedException) { }
                }
                //Wait on the given reset event
                resetEvent.Wait();
            }

            var assemblyLoadContext = AssemblyLoadContext.GetLoadContext(typeof(ConsoleUtility).GetTypeInfo().Assembly);
            assemblyLoadContext.Unloading += context => ShutDown();
            //AppDomain.CurrentDomain.ProcessExit += delegate { ShutDown(); };
            Console.CancelKeyPress += (sender, eventArgs) => {
                ShutDown();
                //Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
        }

        private static async Task WaitForTokenShutdownAsync(CancellationToken token)
        {
            var waitForStop = new TaskCompletionSource<object>();
            token.Register(obj => {
                var tcs = (TaskCompletionSource<object>)obj;
                tcs.TrySetResult(null);
            }, waitForStop);
            await waitForStop.Task;
        }
    }
}
