using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Routine
{
	public class GZipStreamStringHelper
	{
		public static string Zip(string value)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(value);
			var memoryStream = new MemoryStream();
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
			{
				gZipStream.Write(buffer, 0, buffer.Length);
			}

			memoryStream.Position = 0;

			var compressedData = new byte[memoryStream.Length];
			memoryStream.Read(compressedData, 0, compressedData.Length);

			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

			//Transform byte[] unzip data to string
			System.Text.StringBuilder sB = new System.Text.StringBuilder(gZipBuffer.Length);
			//Read the number of bytes GZipStream red and do not a for each bytes in
			//resultByteArray;
			for (int i = 0; i < gZipBuffer.Length; i++)
			{
				sB.Append((char)gZipBuffer[i]);
			}

			return sB.ToString();
		}

		public static string UnZip(string value)
		{
			//Transform string into byte[]
			byte[] gZipBuffer = new byte[value.Length];
			int indexBA = 0;
			foreach (char item in value.ToCharArray())
			{
				gZipBuffer[indexBA++] = (byte)item;
			}

			using (var memoryStream = new MemoryStream())
			{
				int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
				memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

				var buffer = new byte[dataLength];

				memoryStream.Position = 0;
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					gZipStream.Read(buffer, 0, buffer.Length);
				}

				return Encoding.UTF8.GetString(buffer);
			}
		}
	}
}
