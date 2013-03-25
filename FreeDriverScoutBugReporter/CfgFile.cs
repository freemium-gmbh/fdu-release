using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace BugReporter
{
    public static class CfgFile
    {
        private static string cfgFilePath = /*GetInstallPath() +*/ "freemium.cfg";

        public static string CfgFilePath
        {
            get { return cfgFilePath; }
            set { cfgFilePath = value; }
        }

        static string GetInstallPath()
        {
            string installDir = Registry.CurrentUser.OpenSubKey("Freemium").GetValue("InstallDir").ToString();

            return installDir.EndsWith("\\") ? installDir : installDir + "\\";
        }

        /// <summary>
        /// Reads the given key from the config file and returns its value.
        /// </summary>
        /// <param name="key">Configuration entry key</param>
        /// <returns></returns>
        public static string Get(string key)
        {
            int dummy;
            return Get(key, out dummy);
        }

        private static string Get(string key, out int lineNum)
        {
            if (!File.Exists(CfgFilePath))
            {
                lineNum = -1;
                return null;
            }

            var reader = new StreamReader(CfgFilePath);

            try
            {
                string line = "";
                for (lineNum = 0; (line = reader.ReadLine()) != null; lineNum++)
                {
                    var entry = line.Split(new char[] { ' ', '=' });
                    if (entry.Length > 1 && entry[0] == key)
                        return entry[entry.Length - 1];
                }

            }
            finally
            {
                reader.Close();
            }

            lineNum = -1;
            return null;
        }

        /// <summary>
        /// Sets the given key and value in the config file.
        /// </summary>
        /// <param name="key">Configuration entry key</param>
        /// <param name="value">Configuration entry value</param>
        public static void Set(string key, string value)
        {
            int lineNum;
            if (Get(key, out lineNum) == null)
            {
                var writer = File.AppendText(CfgFilePath);
                writer.WriteLine(key + "=" + value);
                writer.Close();
            }
            else
            {
                var cfg = "";
                var reader = new StreamReader(CfgFilePath);

                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var entry = line.Split(new char[] { ' ', '=' });
                    if (entry[0] == key)
                        cfg += key + "=" + value + "\r\n";
                    else
                        cfg += line + "\r\n";
                }
                reader.Close();

                var writer = new StreamWriter(CfgFilePath);
                writer.Write(cfg);
                writer.Close();
            }
        }
    }
}
