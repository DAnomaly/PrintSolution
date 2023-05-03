using System;
using System.Threading;

namespace Danomaly.GetPrintStatus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] printers = PrintUtil.GetPrintQueues();
            Console.WriteLine("Printer Count: " + printers.Length);

            Console.WriteLine("Printer Status ==");

            for (int i = 0; i < printers.Length; i++)
            {
                if (printers[i] != null && printers[i] != string.Empty)
                {
                    Console.Write(printers[i] + " : ");
                    bool useYn = PrintUtil.GetPrinterStatus(printers[i], out string status);
                    Console.WriteLine(useYn + " (" + status + ")");
                }
            }

            Console.WriteLine("\r\nIf you want to close this program, press 'Ctrl + C'.");
            while (true)
                Thread.Sleep(3000);
        }
    }
}
