using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Routine;

namespace ReportingService
{
    public class ReportService : IReportService
    {
        private ReportsDBEntities db = new ReportsDBEntities();

        public void SubmitNormalReport(string version, string action, string mac,
            string ip, string os, string hostName)
        {
            // Add report
            var report = new NormalReport
            {
                Version = version,
                Action = action,
                Date = DateTime.Now,
                MAC = mac,
                IP = ip,
                OS = os,
                HostName = hostName
            };
            db.NormalReports.AddObject(report);
            
            // Update statistics
            var statisticsEntry = db.Statistics.FirstOrDefault(s => s.Version == version && s.Type == action);
            if (statisticsEntry != null)
            {
                statisticsEntry.Count++;
            }
            else
            {
                var statistic = new Statistic
                {
                    Version = version,
                    Type = action,
                    Count = 1
                };
                db.Statistics.AddObject(statistic);
            }
            
            // Save changes
            try
            {
                db.SaveChanges();
            }
            catch { }
        }

        public void SubmitBugReport(string version, string mac,
            string ip, string os, string hostName, string bugStackTrace,
            string bugType, string bugMessage, string bugUserInput, string bugTargetSite,
            string bugSource)
        {
            // Add bug report
            var report = new BugReport
            {
                Version = version,
                Date = DateTime.Now,
                MAC = mac,
                IP = ip,
                OS = os,
                HostName = hostName,
                BugStackTrace = GZipStreamStringHelper.UnZip(bugStackTrace),
                BugType = bugType,
                BugMessage = bugMessage,
                BugUserInput = bugUserInput,
                BugTargetSite = bugTargetSite,
                BugSource = bugSource
            };
            db.BugReports.AddObject(report);

            // Update statistics
            var statisticsEntry = db.Statistics.FirstOrDefault(s => s.Version == version && s.Type == "Bug");
            if (statisticsEntry != null)
            {
                statisticsEntry.Count++;
            }
            else
            {
                var statistic = new Statistic
                {
                    Version = version,
                    Type = "Bug",
                    Count = 1
                };
                db.Statistics.AddObject(statistic);
            }

            // Save changes
            db.SaveChanges();
        }

        public string[] GetLatestVersion()
        {
            var query = db.Versions.OrderByDescending(v => v.Date).FirstOrDefault();

            return new string[] { query.Number, query.DownloadLink };
        }

		public Dictionary<string, string[]> GetDriverUpdates(Dictionary<string, string[]> driverScanResults)
        {
            Dictionary<string, string[]> driverUpdates = new Dictionary<string, string[]>();

			try
			{
				string[] deviceIds = driverScanResults.Keys.ToArray<string>();
				string[] hardwareIds = driverScanResults.Values.Select(v => v[1]).ToArray<string>();
				string[] compatIds = driverScanResults.Values.Select(v => v[2]).ToArray<string>();

				dynamic results = (from device in db.Devices
								   join driver in db.Drivers on device.Id equals driver.Device_Id
                                   where deviceIds.Contains(device.DeviceID)
                                         || hardwareIds.Contains(device.HardwareID)
                                         || compatIds.Contains(device.CompatibleID)
								   select new { device, driver }).OrderByDescending(r => r.driver.VersionDate).ToList<dynamic>();

				// If device found in db, check if driver update needed for it
				foreach (dynamic result in results)
				{
					if (result.driver.Version != driverScanResults[result.device.DeviceID][0])
					{
						driverUpdates.Add(
							result.device.DeviceID,
							new string[3] {
								 result.driver.VersionDate.ToShortDateString(),
								 result.driver.DownloadLink,
                                 result.driver.InstallCommand
							 }
						);
					}
				}

				// Remove it
				//driverUpdates.Add(
				//    @"ROOT\*ISATAP\0000",
				//    new string[2] {
				//     "date",
				//     @"http://dl.dropbox.com/u/38404707/WPFDataStreaming.zip"
				// });
			}
			catch { }

            return driverUpdates;
        }
    }
    
    }

