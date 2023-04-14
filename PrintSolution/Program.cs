using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Danomaly.PrintSolution
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "list")
            {
                string[] printQueues = PrintUtil.GetPrintQueues();
                foreach (string printer in printQueues)
                    Console.WriteLine(printer);
            }
            else if (args[0] == "status")
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Need more parameters.");
                    Console.WriteLine("PrintCommand.exe status [printerName]");
                    return;
                }
                string printerName = args[1].Replace('+', ' ');
                bool useYN = PrintUtil.GetPrinterStatus(printerName, out string status);
                Console.WriteLine(useYN);
                Console.Write(status);
            }
            else if (args[0] == "print")
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Need more parameters.");
                    Console.WriteLine("PrintCommand.exe print [printerName] [filepath]");
                    return;
                }
                string printerName = args[1].Replace('+', ' ');
                bool result = PrintUtil.PrintDocument(printerName, args[2], out string err);
                if (result)
                {
                    Console.WriteLine("Success");
                }
                else
                {
                    Console.WriteLine("FAIL");
                    Console.WriteLine(err);
                }
            }
        }
    }
}