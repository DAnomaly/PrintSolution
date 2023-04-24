using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PrintSolutionAPI.DTO;
using PrintSolutionAPI.Scheduler;
using PrintSolutionAPI.Util;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace PrintSolutionAPI.Controllers
{
    [ApiController]
    [Route("Print")]
    public class PrintContoller : Controller
    {
        private readonly ILogger<PrintContoller> _logger;
        private readonly string folderName = "upload";

        public PrintContoller(ILogger<PrintContoller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("Show")]
        public IEnumerable<PrinterDTO> Show()
        {
            List<PrinterDTO> printerList = new List<PrinterDTO>();

            DirectoryInfo di = new DirectoryInfo(LoadStatusSch.PrintFolderName);

            foreach (FileInfo file in di.GetFiles())
            {
                string readData = string.Empty;
                using (StreamReader fs = new StreamReader(file.OpenRead())) 
                {
                    string? line;
                    while((line = fs.ReadLine()) != null)
                    {
                        if (readData != string.Empty)
                            readData += "\r\n";
                        readData += line;
                    }
                }
                PrinterDTO? printerDTO = JsonSerializer.Deserialize<PrinterDTO>(readData);
                if (printerDTO != null)
                    printerList.Add(printerDTO);
            }

            return printerList.ToArray();
        }

        [HttpPost]
        [RequestSizeLimit(10_485_760)]
        [Route("Upload")]
        public async Task<string> Upload(IFormFile file)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            if (file.Length > 0)
            {
                string realFilename = file.FileName;
                string ext = realFilename.Substring(realFilename.LastIndexOf(".") + 1);
                string filename = "UF" + (Directory.GetFiles(folderName).Length + 1) + "." + ext;
                using (var fileStream = new FileStream(Path.Combine(folderName, filename), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return filename;
            }
            else
            {
                return "";
            }

        }

        [HttpGet]
        [Route("Call")]
        public IDictionary<string, Object> Call(string printer, string filename)
        {
            IDictionary<string, Object> response = new Dictionary<string, Object>();

            string output = Command.GetOutput("PrintSolution.exe", "print " + printer + " " + folderName + @"\" + filename);
            
            if (output.Contains("Success"))
            {
                response["result"] = true;
            }
            else
            {
                response["result"] = false;
                response["message"] = output;
            }

            return response;
        }

    }
}