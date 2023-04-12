using System.Diagnostics;

namespace PrintSolutionAPI.Util
{
    public class Command
    {
        public static string GetOutput(string executable, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = executable,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process? proc = Process.Start(psi);
            if (proc != null)
                return proc.StandardOutput.ReadToEnd();
            else
                return "error";
        }
    }
}
