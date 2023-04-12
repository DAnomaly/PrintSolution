using Spire.Doc;
using Spire.Pdf.Print;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Printing;

namespace Danomaly.PrintSolution
{
    public class PrintController
    {
        /// <summary>
        /// 프린터 목록을 가져옵니다.
        /// </summary>
        /// <returns>프린터 목록</returns>
        public static string[] GetPrintQueues()
        {
            List<string> printers = new List<string>();
            // 로컬 프린트 서버를 생성합니다.
            using (PrintServer printServer = new PrintServer())
            {
                // 연결된 프린터 목록을 가져옵니다.
                PrintQueueCollection printQueues = printServer.GetPrintQueues();
                foreach (PrintQueue printer in printQueues)
                    if (!printer.Name.ToUpper().Equals("FAX") && !printer.Name.ToUpper().Contains("MICROSOFT") && !printer.Name.ToUpper().Contains("ONENOTE"))
                        printers.Add(printer.Name);
            }

            return printers.ToArray();
        }

        /// <summary>
        /// 프린터의 상태를 가져옵니다.
        /// 하지만 해당 프린터가 USB로 연결되어 있지 않으면 상태를 정확하게 알수 없습니다.
        /// </summary>
        /// <param name="printerName">프린터 이름</param>
        /// <returns>프린터 상태</returns>
        public static string GetPrinterStatus(string printerName)
        {
            string status = string.Empty;
            using (PrintServer printServer = new PrintServer())
            {
                PrintQueue printQueue = printServer.GetPrintQueue(printerName);
                status = printQueue.QueueStatus.ToString();
                string ip = printQueue.QueuePort.ToString();
            }
            return status;
        }

        /// <summary>
        /// 문서를 프린트 합니다.
        /// </summary>
        /// <param name="printer">프린터이름</param>
        /// <param name="filepath">파일경로</param>
        /// <param name="err">에러메시지</param>
        /// <returns>성공여부</returns>
        public static bool PrintDocument(string printer, string filepath, out string err)
        {
            string documentName = "PrintSolution";
            bool result = true;
            err = "";
            try
            {
                string ext = filepath.Substring(filepath.LastIndexOf('.')).ToLower();

                if (ext == "pdf")
                {
                    using (PdfDocument document = new PdfDocument())
                    {
                        document.LoadFromFile(filepath);
                        document.Print(new PdfPrintSettings()
                        {
                            PrinterName = printer,
                            DocumentName = documentName
                        });
                    }
                }
                else
                {
                    using (Document document = new Document())
                    {
                        document.LoadFromFile(filepath);
                        PrintDocument pDocument = document.PrintDocument;
                        pDocument.DocumentName = documentName;
                        pDocument.PrinterSettings = new PrinterSettings()
                        {
                            PrinterName = printer
                        };
                        pDocument.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                result = false;
            }
            return result;
        }


    }
}
