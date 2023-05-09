using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Printing;
using System.Text.RegularExpressions;

namespace Danomaly.GetPrintStatus
{
    public class PrintUtil
    {
        /// <summary>
        /// 프린터 목록을 가져옵니다.
        /// </summary>
        /// <returns>프린터 목록</returns>
        public static string[] GetPrintQueues()
        {
            // Read Ignore File
            string ignoreFilePath = "ignore.inf";
            string[] ignorePatterns = File.ReadAllText(ignoreFilePath).Split('\n');
            for (int i = 0; i < ignorePatterns.Length; i++)
                if (ignorePatterns[i].Contains("\r"))
                    ignorePatterns[i] = ignorePatterns[i].Replace("\r", string.Empty);

            List<string> printers = new List<string>();
            // 로컬 프린트 서버를 생성합니다.
            using (PrintServer printServer = new PrintServer())
            {
                // 연결된 프린터 목록을 가져옵니다.
                PrintQueueCollection printQueues = printServer.GetPrintQueues();
                foreach (PrintQueue printer in printQueues)
                {
                    bool isIgnore = false;

                    foreach (string ignorePattern in ignorePatterns)
                    {
                        if (ignorePattern != null && ignorePattern != string.Empty && printer.Name.ToUpper().Contains(ignorePattern))
                        {
                            isIgnore = true;
                            break;
                        }
                    }

                    if (isIgnore == false)
                    {
                        printers.Add(printer.Name);
                    }
                }
            }

            return printers.ToArray();
        }

        /// <summary>
        /// 프린터의 상태를 가져옵니다.
        /// 하지만 해당 프린터가 USB로 연결되어 있지 않으면 상태를 정확하게 알수 없습니다.
        /// </summary>
        /// <param name="printerName">프린터이름</param>
        /// <param name="status">프린터상태</param>
        /// <returns>프린터 사용가능 여부</returns>
        public static bool GetPrinterStatus(string printerName, out string status)
        {
            bool useYN = true;
            status = string.Empty;
            using (PrintServer printServer = new PrintServer())
            {
                try
                {
                    PrintQueue printQueue = printServer.GetPrintQueue(printerName);
                    PrintQueueStatus printStatus = printQueue.QueueStatus;
                    status = printStatus.ToString();
                    if (printStatus == PrintQueueStatus.None)
                    {
                        string ipPattern = @"(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])";
                        string ip = Regex.Match(printQueue.QueuePort.Name, ipPattern).Value;

                        #region SyncThru 웹서버에 통신
                        if (ip != string.Empty)
                        {
                            string url = "http://" + ip;

                            try
                            {
                                #region 1. http://[프린터IP주소]에 통신
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                                request.Method = "GET";
                                request.Timeout = 10 * 1000;
                                HttpStatusCode statusCode;
                                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                                    statusCode = resp.StatusCode;
                                #endregion

                                #region 2. http://[프린터IP주소]에 통신이 가능하면 sws url에서 프린터 상태 가져오기
                                string responseText = string.Empty;
                                if (statusCode == HttpStatusCode.OK)
                                {
                                    if (url[url.Length - 1] != '/')
                                        url += '/';
                                    url += "sws/app/information/home/home.json";
                                    request = (HttpWebRequest)WebRequest.Create(url);
                                    request.Method = "GET";
                                    request.Timeout = 10 * 1000;
                                    using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                                        if (resp.StatusCode == HttpStatusCode.OK)
                                        {
                                            Stream respStream = resp.GetResponseStream();
                                            using (StreamReader sr = new StreamReader(respStream))
                                                responseText = sr.ReadToEnd();
                                        }
                                }
                                #endregion

                                #region 3. 응답 분석
                                if (responseText != string.Empty)
                                {
                                    // responseText = responseText.Replace(" ", "");
                                    JObject jObject = JObject.Parse(responseText);
                                    if (jObject["status"].Value<Int16>("hrDeviceStatus") >= 0 && jObject["status"].Value<Int16>("hrDeviceStatus") <= 2)
                                        useYN = true;
                                    else
                                        useYN = false;
                                    status = jObject["status"].Value<string>("status1");
                                    if (jObject["status"].Value<string>("status2") != string.Empty)
                                        status += "\r\n" + jObject["status"].Value<string>("status2");
                                    if (jObject["status"].Value<string>("status3") != string.Empty)
                                        status += "\r\n" + jObject["status"].Value<string>("status3");
                                    if (jObject["status"].Value<string>("status4") != string.Empty)
                                        status += "\r\n" + jObject["status"].Value<string>("status4");

                                    while (status.Contains("  "))
                                        status = status.Replace("  ", " ");
                                    if (status.Contains("\n"))
                                        status = status.Replace("\n", string.Empty);
                                    if (status.Contains("\r"))
                                        status = status.Replace("\r", string.Empty);
                                    status = status.Trim();
                                }
                                #endregion
                            }
                            catch (WebException)
                            {
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        foreach (PrintQueueStatus disableStatus in DisAblePrinterStatus)
                        {
                            if (printStatus == disableStatus)
                            {
                                useYN = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception printerEx)
                {
                    useYN = false;
                    status = printerEx.Message;
                }
            }
            return useYN;
        }

        /// <summary>
        /// 대기열 추가가 불가능한 정도의 에러 목록
        /// </summary>
        private static PrintQueueStatus[] DisAblePrinterStatus = new PrintQueueStatus[]
            {
                PrintQueueStatus.Offline,
                PrintQueueStatus.PagePunt,
                PrintQueueStatus.Error,
                PrintQueueStatus.OutOfMemory,
                PrintQueueStatus.PaperJam,
                PrintQueueStatus.PaperOut,
                PrintQueueStatus.PaperProblem,
                PrintQueueStatus.UserIntervention,
            };

    }
}
