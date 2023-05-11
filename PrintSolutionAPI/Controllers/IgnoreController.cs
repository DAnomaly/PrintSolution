using Microsoft.AspNetCore.Mvc;

namespace PrintSolutionAPI.Controllers
{
    [ApiController]
    [Route("Ignore")]
    public class IgnoreController : Controller
    {
        private readonly ILogger<PrintContoller> _logger;

        public IgnoreController(ILogger<PrintContoller> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 무시 목록 확인
        /// </summary>
        /// <returns>무시 프린터 목록</returns>
        [HttpGet]
        [Route("Show")]
        public IEnumerable<string> Show()
        {
            List<string> result = new List<string>();

            string content = System.IO.File.ReadAllText("ignore.inf");
            string[] lines = content.Split('\n');
            foreach (string line in lines)
            {
                string printerName = line;
                if (printerName.Contains('\r'))
                {
                    printerName = printerName.Replace("\r", string.Empty);
                }
                printerName = printerName.Trim();
                if (printerName != string.Empty) 
                {
                    result.Add(printerName);
                }
            }

            return result;
        }

        /// <summary>
        /// 무시 프린터 추가
        /// </summary>
        /// <param name="name">새로운 무시프린터명</param>
        /// <returns>성공여부</returns>
        [HttpGet]
        [Route("Add")]
        public IEnumerable<bool> Add(string name)
        {
            string content = System.IO.File.ReadAllText("ignore.inf");
            content += "\r\n" + name;
            System.IO.File.WriteAllText("ignore.inf", content);

            yield return true;
        }

        /// <summary>
        /// 무시 프린터 제거
        /// </summary>
        /// <param name="name">삭제할 무시프린터명</param>
        /// <returns>성공여부</returns>
        [HttpGet]
        [Route("Remove")]
        public IEnumerable<bool> Remove(string name)
        {
            string content = System.IO.File.ReadAllText("ignore.inf");
            string[] lines = content.Split('\n');
            List<string> lineList = lines.ToList();
            bool delete = false;
            for (int i = 0; i < lineList.Count; i++)
            {
                string printerName = lines[i];
                if (printerName.Contains('\r'))
                {
                    printerName = printerName.Replace("\r", string.Empty);
                }

                if (printerName == name)
                {
                    delete = true;
                    lineList.RemoveAt(i);
                    break;
                }
            }

            if (delete)
            {
                content = string.Empty;
                for (int i = 0; i < lineList.Count; i++)
                {
                    if (content != string.Empty)
                        content += "\r\n";
                    content += lineList[i];
                }

                System.IO.File.WriteAllText("ignore.inf", content);

                yield return true;
            }

            yield return false;
        }
    }
}
