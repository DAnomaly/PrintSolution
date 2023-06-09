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

        /// <summary>
        /// 프린터 목록, 상태 확인
        /// </summary>
        /// <returns>프린터 목록, 상태 (JSON)</returns>
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

        /// <summary>
        /// 파일 수신
        /// </summary>
        /// <param name="file">업로드 파일</param>
        /// <returns>파일 수신 성공여부, 저장파일명</returns>
        [HttpPost]
        [RequestSizeLimit(10_485_760)]
        [Route("Upload")]
        public async Task<IDictionary<string, Object>> Upload(IFormFile file)
        {

            IDictionary<string, Object> response = new Dictionary<string, Object>();

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            if (file != null && file.Length > 0)
            {
                try
                {
                    string realFilename = file.FileName;
                    string ext = realFilename.Substring(realFilename.LastIndexOf(".") + 1);
                    string filename = "UF" + (Directory.GetFiles(folderName).Length + 1) + "." + ext;
                    using (var fileStream = new FileStream(Path.Combine(folderName, filename), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    response.Add("message", "Success");
                    response.Add("filename", filename);
                }
                catch (Exception e)
                {
                    response.Add("message", "Fail : " + e.Message);
                    response.Remove("filename");
                }
            }
            else
            {
                response.Add("message", "Fail : File is Empty!");
                response.Remove("filename");
            }

            return response;
        }

        /// <summary>
        /// 프린터 출력명령
        /// </summary>
        /// <param name="printer">프린터이름</param>
        /// <param name="filename">저장파일이름</param>
        /// <returns>프린터 명령 등록 여부</returns>
        [HttpGet]
        [Route("Call")]
        public IDictionary<string, Object> Call(string printer, string filename)
        {
            IDictionary<string, Object> response = new Dictionary<string, Object>();

            string output = Command.GetOutput("PrintSolution.exe", "print \"" + printer + "\" " + folderName + @"\" + filename);
            
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