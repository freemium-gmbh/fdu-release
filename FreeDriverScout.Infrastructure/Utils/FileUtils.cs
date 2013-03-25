using System.IO;
using Ionic.Zip;

namespace FreeDriverScout.Utils
{
	static class FileUtils
	{
		/// <summary>
		/// Unzips the zip package whose path is given by <code>zipPath</code> to <code>destPath</code>.
		/// </summary>
		/// <param name="zipPath">The path to a zip package</param>
		/// <param name="destDir">The path of the folder in which to extract the zip</param>
		public static void Unzip(string zipPath, string destDir)
		{
			// Make sure destination directory exists
			if (!Directory.Exists(destDir))
			{
				Directory.CreateDirectory(destDir);
			}

			// Extract zip package to destination directory
			ZipFile zipPackage = ZipFile.Read(zipPath);
			zipPackage.ExtractAll(destDir);
		}
	}
}
