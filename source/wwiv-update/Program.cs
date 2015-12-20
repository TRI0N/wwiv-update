using System;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;

namespace WWIVUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            // Fetch Latest Build Number For WWIV 5.1
            WebClient wc = new WebClient();
            string htmlString1 = wc.DownloadString("http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/");
            Match mTitle1 = Regex.Match(htmlString1, "(?:number.*?>)(?<buildNumber1>.*?)(?:<)");
            string htmlString2 = wc.DownloadString("https://build.wwivbbs.org/jenkins/job/wwiv_5.0.0/lastSuccessfulBuild/label=windows/");
            Match mTitle2 = Regex.Match(htmlString2, "(?:number.*?>)(?<buildNumber2>.*?)(?:<)");
            if (mTitle1.Success)
            {
                // Console Input or Update
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                string buildNumber1 = mTitle1.Groups[1].Value;
                string buildNumber2 = mTitle2.Groups[1].Value;
                Console.WriteLine(" ");
                Console.WriteLine("WWIV UPDATE v0.9.1 | ßeta");
                Console.WriteLine(" ");
                Console.WriteLine("WARNING! WWIV5Telnet, WWIV and WWIVnet MUST Be Closed Before Proceeding.");
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("WWIV 5.1 Latest Successful Build Number: " + buildNumber1);
                Console.WriteLine(" ");
                Console.WriteLine("WWIV 5.0 Latest Successful Build Number: " + buildNumber2);
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.Write("Enter Build Number or Press Enter To Use Latest 5.1: ");
                string useBuild = Console.ReadLine();
                Console.Clear();

                // Check for Running Instances of WWIV Programs
                // bbs.exe
                if (Process.GetProcessesByName("bbs").Length >= 1)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("WWIV BBS.EXE is Currently Running! Please Close and Try Again.");
                    Console.WriteLine(" ");
                    Console.WriteLine("Press Any Key To Restart WWIV Update...");
                    Console.ReadKey();
                    Main(args);
                }
                // WWIV5TelnetServer.exe
                if (Process.GetProcessesByName("WWIV5TelnetServer").Length >= 1)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("WWIV5TelnetServer is Currently Running! Please Close and Try Again.");
                    Console.WriteLine(" ");
                    Console.WriteLine("Press Any Key To Restart WWIV Update...");
                    Console.ReadKey();
                    Main(args);
                }
                // binkp networkb.exe
                if (Process.GetProcessesByName("networkb").Length >= 1)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("WWIV BINKP.CMD (WWIVnet) is Currently Running! Please Close and Try Again.");
                    Console.WriteLine(" ");
                    Console.WriteLine("Press Any Key To Restart WWIV Update...");
                    Console.ReadKey();
                    Main(args);
                }

                // Search For bbs.exe In Default Install Path
<<<<<<< HEAD
                Console.WriteLine(" ");
=======
                /*Console.WriteLine(" ");
>>>>>>> Beta
                Console.WriteLine("Searching for WWIV Working Directory...");
                Console.WriteLine(" ");
                string[] files = Directory.GetFiles(@"C:\wwiv", "bbs.exe", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        Console.WriteLine(file + "  Was Located In The Default WWIV Install Directory");
                        Console.WriteLine(" ");
                    }
                    else
                    {
                        Console.WriteLine(@"WWIV Is Not Installed In The Default Directory of C:\wwiv");
                        Console.WriteLine(" ");
                        Console.WriteLine("WWIV Update Cannot Proceed!");
                        Console.WriteLine(" ");
                        Console.WriteLine("Please Manually Update Your WWIV Install.");
                        Console.WriteLine(" ");
                        Console.WriteLine("Press Any Key To Exit WWIV Update.");
                        Console.ReadKey();
                        Environment.Exit(2);
                    }
                }
                Console.ReadKey();*/

                // Set Global Strings For Update
                string backupPath = @"C:\wwiv";
                string zipPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Documents\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_wwiv-backup.zip";
                string extractPath = @"C:\wwiv";
                string extractPath2 = Environment.GetEnvironmentVariable("SystemRoot") + @"\System32";
                string buildNumber3;
                {
                     if (useBuild == null || string.IsNullOrWhiteSpace(useBuild))
                     {
                         buildNumber3 = buildNumber1;
                    }
                     else
                     {
                        buildNumber3 = useBuild;
                    }
                }
                string remoteUri;
                {
                    if (buildNumber3 == buildNumber1)
                    {
                        remoteUri = "http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/artifact/";
                    }
                    else
                    {
                        remoteUri = "http://build.wwivbbs.org/jenkins/job/wwiv/" + buildNumber3 + "/label=windows/artifact/";
                    }
                }
                string fileName = "wwiv-build-win-" + buildNumber3 + ".zip", myStringWebResource = null;
                string updatePath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\wwiv-build-win-" + buildNumber3 + ".zip";
                string wwivChanges;
                {
                    if (buildNumber3 == buildNumber1)
                    {
                        wwivChanges = "http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/changes";
                    }
                    else
                    {
                        wwivChanges = "http://build.wwivbbs.org/jenkins/job/wwiv/" + buildNumber3 + "/label=windows/changes";
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
                // May look at ways to launch Local Node Via Telnet Server.
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
