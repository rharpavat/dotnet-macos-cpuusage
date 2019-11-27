using System;
using System.Diagnostics;

namespace MacHighCpu
{
    /// <summary>
    /// Default implementation of <see cref="IBashShell"/>.
    /// </summary>
    public static class BashShell
    {
        /// <summary>
        /// Runs a command in the bash shell (macos/linux only).
        /// </summary>
        /// <param name="cmd">The command to run.</param>
        /// <param name="timeout">The time (in milliseconds) the system will wait for the command to complete before timing out.</param>
        /// <returns>The STDOUT output of the command.</returns>
        public static string RunCommand(string cmd, int timeout = 30000)
        {
            var escaped = cmd.Replace("\"", "\\\"");
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escaped}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }
            };
            process.Start();
            var toreturn = process.StandardOutput.ReadToEnd();
            if (!process.WaitForExit(timeout))
            {
                throw new TimeoutException($"Process \"{cmd}\" timed out.");
            }

            return toreturn;
        }
    }
}