using System;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;

namespace WWIVUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            // Fetch Latest Build Number
            WebClient wc = new WebClient();
            string htmlString = wc.DownloadString("http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/");
            Match mTitle = Regex.Match(htmlString, "(?:number.*?>)(?<buildNumber>.*?)(?:<)");
            if (mTitle.Success)
            {
                // Console Input or Update
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                string buildNumber = mTitle.Groups[1].Value;
                Console.WriteLine("WWIV UPDATE v0.8 | ßeta");
                Console.WriteLine(" ");
                Console.WriteLine("WARNING! WWIV5TelNet, WWIV and WWIVnet MUST Be Closed Before Proceeding.");
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("Latest Successful Build Number: " + buildNumber);
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.Write("Enter Build Number or Press Enter To Use Latest: ");
                string useBuild = Console.ReadLine();
                Console.Clear();

                // Set Global Strings For Update
                string backupPath = @"C:\wwiv";
                string zipPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Documents\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_wwiv-backup.zip";
                string extractPath = @"C:\wwiv";
                string extractPath2 = Environment.GetEnvironmentVariable("SystemRoot") + @"\System32";
                string buildNumber2;
                {
                     if (useBuild == null || string.IsNullOrWhiteSpace(useBuild))
                     {
                         buildNumber2 = buildNumber;
                    }
                     else
                     {
                        buildNumber2 = useBuild;
                    }
                }
                string remoteUri;
                {
                    if (buildNumber2 == buildNumber)
                    {
                        remoteUri = "http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/artifact/";
                    }
                    else
                    {
                        remoteUri = "http://build.wwivbbs.org/jenkins/job/wwiv/" + buildNumber2 + "/label=windows/artifact/";
                    }
                }
                string fileName = "wwiv-build-win-" + buildNumber2 + ".zip", myStringWebResource = null;
                string updatePath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\wwiv-build-win-" + buildNumber2 + ".zip";
                string wwivChanges;
                {
                    if (buildNumber2 == buildNumber)
                    {
                        wwivChanges = "http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/changes";
                    }
                    else
                    {
                        wwivChanges = "http://build.wwivbbs.org/jenkins/job/wwiv/" + buildNumber2 + "/label=windows/changes";
                    }
                }

                // Create WWIV Backup File With Unique Name
                ZipFile.CreateFromDirectory(backupPath, zipPath);

                // Fetch Latest Sucessful Build
                WebClient myWebClient = new WebClient();
                myStringWebResource = remoteUri + fileName;
                Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
                myWebClient.DownloadFile(myStringWebResource, Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\" + fileName);
                Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
                Console.WriteLine(" ");
                Console.WriteLine("Fetch Update Complete | Press Any Key to Update WWIV...");
                Console.WriteLine(" ");
                Console.ReadKey();

                // Patch Existing WWIV Install
                using (ZipArchive archive = ZipFile.OpenRead(updatePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);
                        }
                        if (entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);
                        }
                        if (entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(extractPath2, entry.FullName), true);
                        }
                    }
                }
                Console.WriteLine("WWIV Update Complete | Press Any Key to Launch WWIV and Exit Update...");
                Console.ReadKey();

                // Launch WWIV, WWIVnet and Latest Changes in Browser.
                Environment.CurrentDirectory = @"C:\wwiv";

                //Launch Telnet Server
                ProcessStartInfo telNet = new ProcessStartInfo("WWIV5TelnetServer.exe");
                telNet.WindowStyle = ProcessWindowStyle.Minimized;
                Process.Start(telNet);

                // Launch Local BBS Node 1 with Networking
                // TODO This Refuses to Load. Will Investigate.
                // Process.Start("bbs.exe -N1 -M");
                // Process.Start("bbs.exe", "-N1 -M");

                // Launch binkp.cmd for WWIVnet
                ProcessStartInfo binkP = new ProcessStartInfo("binkp.cmd");
                binkP.WindowStyle = ProcessWindowStyle.Minimized;
                Process.Start(binkP);

                //Launch Latest Realse Changes into Default Browser
                Process.Start(wwivChanges);
            }
        }
    }
}
