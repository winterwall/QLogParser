using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSUtil;
using QLogParser.Services.Messages;
using QLogParser.Services;
using System.IO;
using System.Diagnostics;

namespace QLogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("**** QLogParser V0.1 for WSM Subscribe Google Analytics Cookies ****");
            Console.WriteLine("");
            Console.WriteLine("Enter q to exit.");
            Console.WriteLine("");
            Console.WriteLine("Please enter log file folder path (example: C:\\Log):");

            string logFolderPath = Console.ReadLine();

            while (logFolderPath != "q")
            {
                // Check log file folder path
                bool pathOK = false;
                while (!pathOK)
                {
                    try
                    {
                        string[] filePaths = Directory.GetFiles(@logFolderPath, "*.log");
                        if (filePaths.Count() == 0)
                        {
                            Console.WriteLine("Cannot find log file under: " + logFolderPath);
                            Console.WriteLine("Enter again:");
                            logFolderPath = Console.ReadLine();
                        }
                        else
                        {
                            pathOK = true;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid folder path: " + logFolderPath);
                        Console.WriteLine("Enter again:");
                        logFolderPath = Console.ReadLine();
                    }
                }

                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(@logFolderPath);

                string resultFile = logFolderPath + "\\" + "GoogleAnalytics" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

                StreamWriter SW = new StreamWriter(resultFile, true);

                SW.WriteLine("cs-username\tCampaignSource\tCampaignName\tCampaignContent");
                SW.Close();

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                foreach (System.IO.FileInfo file in dir.GetFiles("*.log*"))
                {
                    Console.WriteLine("Parsing {0}", file.Name, file.Length);
                    ParseLog(logFolderPath, file.Name, resultFile);
                    Console.WriteLine("Success.");
                }

                stopWatch.Stop();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value. 
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                Console.WriteLine("");
                Console.WriteLine("Finished!");
                Console.WriteLine("RunTime " + elapsedTime);
                Console.WriteLine("");
                Console.WriteLine("Enter next folder path or q to exit.");
                Console.WriteLine("");
                logFolderPath = Console.ReadLine();
            }
        }

        public static void ParseLog(string folderPath, string fileName, string resultFile)
        {
            try
            {
                LogQueryClass logQuery = new LogQueryClass();
                COMIISW3CInputContextClass inputFormatIIS = new COMIISW3CInputContextClass();

                string filePath = folderPath + "\\" + fileName;

                // Log Parser query
                string strQuery = @"SELECT DISTINCT cs-username, cs(Cookie) FROM '" + filePath + @"'
                        WHERE cs-uri-stem LIKE '%subscribe%'
                        and cs-uri-stem Not LIKE '%thank%'
                        and cs-method LIKE 'POST'
                        and cs-username IS NOT NULL 
                        and cs(Cookie) LIKE '%utma%'";

                // Parser result set
                ILogRecordset results = logQuery.Execute(strQuery, inputFormatIIS);

                StreamWriter SW = new StreamWriter(resultFile, true);

                while (!results.atEnd())
                {
                    SW.WriteLine(String.Format("{0}\t{1}", results.getRecord().getValue("cs-username"), GetGACookies(results.getRecord().getValue("cs(Cookie)"))));
                    results.moveNext();
                }
                SW.Close();
            }
            catch
            {
                Console.WriteLine(folderPath + "\\" + fileName + " parsing error!");
                Console.ReadLine();
            }
        }

        public static string GetGACookies(string cookies)
        {
            GoogleAnalyticsCookies googleAnalyticsCookies = new GoogleAnalyticsCookies();

            GoogleAnalyticsCookiesService _googleAnalyticsCookiesService = new GoogleAnalyticsCookiesService();

            googleAnalyticsCookies = _googleAnalyticsCookiesService.ParseGoogleAnalyticsCookies(cookies);

            string result = googleAnalyticsCookies.CampaignSource + "\t" + googleAnalyticsCookies.CampaignName + "\t" + googleAnalyticsCookies.CampaignContent;

            return result;
        }
    }
}
