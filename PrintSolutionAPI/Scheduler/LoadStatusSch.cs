using PrintSolutionAPI.DTO;
using PrintSolutionAPI.Util;
using System.Text;
using System.Text.Json;

namespace PrintSolutionAPI.Scheduler
{
    public static class LoadStatusSch
    {
        public static string PrintFolderName { get { return "PRINTER"; } }

        private static void Task()
        {
            while (Program.Running)
            {
                // 작업 코드
                #region 프린터 리스트, 정보, 상태 확인
                string output = Command.GetOutput("PrintSolution.exe", "list");
                string[] printerNames = output.Split("\r\n");

                List<PrinterDTO> printerList = new List<PrinterDTO>();
                foreach (string printerName in printerNames)
                {
                    if (printerName == "") continue;

                    string status = Command.GetOutput("PrintSolution.exe", "status " + printerName.Replace(" ", "+"));
                    status = status.Replace("\r\n", "");
                    bool useYn = true;
                    if (status.Contains("||"))
                    {
                        if (status.Substring(0, status.IndexOf("||")).ToLower() == "true")
                        {
                            useYn = true;
                        }
                        else
                        {
                            useYn = false;
                        }
                        status = status.Substring(status.IndexOf("||") + 2);
                    }
                    PrinterDTO printerDTO = new PrinterDTO()
                    {
                        Name = printerName,
                        Status = status,
                        UseYN = useYn
                    };
                    printerList.Add(printerDTO);
                }
                #endregion

                #region 프린터 폴더 목록에 프린터 정보파일 생성
                DirectoryInfo printFolder = new DirectoryInfo(PrintFolderName);
                if (!printFolder.Exists)
                    printFolder.Create();
                FileInfo[] printInfoFiles = printFolder.GetFiles();
                // 불필요한 파일 삭제
                if (printInfoFiles.Length > 0)
                    foreach (FileInfo file in printInfoFiles)
                    {
                        bool isExist = false;
                        foreach (PrinterDTO dto in printerList)
                            if (dto.Name == file.Name)
                            {
                                isExist = true;
                                break;
                            }
                        if (!isExist) file.Delete();
                    }
                foreach (PrinterDTO dto in printerList)
                {
                    FileInfo file = new FileInfo(PrintFolderName + @"\" + dto.Name);
                    string dtoInfo = JsonSerializer.Serialize(dto);
                    using (StreamWriter sw = file.CreateText())
                        sw.Write(dtoInfo);
                }
                #endregion

                // 100초 마다 반복
                Thread.Sleep(TimeSpan.FromSeconds(100));
            }
        }

        public static void Start()
        {
            Thread taskThread = new Thread(Task);
            taskThread.Start();
        }

    }
}
