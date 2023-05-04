using System.Diagnostics;

namespace PrintSolutionAPI.Util
{
    public class Command
    {
        /// <summary>
        /// CMD 명령어 실행
        /// </summary>
        /// <param name="executable">실행할 파일</param>
        /// <param name="arguments">파라미터</param>
        /// <returns></returns>
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
