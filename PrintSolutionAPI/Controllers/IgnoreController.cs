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
    }
}
