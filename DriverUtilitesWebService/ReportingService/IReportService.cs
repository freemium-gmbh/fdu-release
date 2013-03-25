using System.Collections.Generic;
using System.ServiceModel;

namespace ReportingService
{
    [ServiceContract]
    public interface IReportService
    {
        [OperationContract]
        void SubmitNormalReport(string version, string action, string mac,
            string ipv6, string os, string hostName);

        [OperationContract]
        void SubmitBugReport(string version, string mac,
            string ipv6, string os, string hostName, string bugStackTrace,
            string bugType, string bugMessage, string bugUserInput, string bugTargetSite,
            string bugSource);

        [OperationContract]
        string[] GetLatestVersion();

		[OperationContract]
		Dictionary<string, string[]> GetDriverUpdates(Dictionary<string, string[]> driverScanResults);
    }
}
