using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument())
            {
                doc.LoadFromFile(args[0]);
                doc.SaveToFile("test.pcl", Spire.Pdf.FileFormat.PCL);
            }

        }
    }
}
