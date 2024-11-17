using System;
using System.IO;

namespace BrawlhallaDumper
{
	// Token: 0x02000003 RID: 3
	internal class Program
	{
		// Token: 0x06000008 RID: 8 RVA: 0x0000239C File Offset: 0x0000059C
		private static void Main(string[] args)
		{
			uint globalKey;
			if (uint.TryParse(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.txt")).Trim(), out globalKey))
			{
				foreach (string text in args)
				{
					string folderName = Path.GetFileNameWithoutExtension(text);
					Directory.CreateDirectory(folderName);

					using (FileStream fileStream = File.OpenRead(text))
					{
						Console.Write(text);
						uint seed = BrawlhallaSWZ.ReadUInt32BE(fileStream);
						fileStream.Position = 0;
						File.WriteAllText("seed.txt", seed.ToString());

						string[] array = BrawlhallaSWZ.Decrypt(fileStream, globalKey);
						for (int j = 0; j < array.Length; j++)
						{
							string text2 = array[j];
							string path;
							if (text2[0] == '<')
							{
								if (text2.Substring(0, 10) == "<LevelDesc")
								{
									path = text2.Split(new string[]
									{
										"LevelName=\""
									}, StringSplitOptions.None)[1].Split(new char[]
									{
										'"'
									})[0] + ".xml";
								}
								else
								{
									path = text2.Substring(1, text2.IndexOf('>') - 1) + ".xml";
								}
							}
							else
							{
								path = text2.Substring(0, text2.IndexOf('\n')) + ".csv";
							}
							File.WriteAllText(Path.Combine(folderName, path), array[j]);
						}
					}
				}
				Console.WriteLine();
				Console.WriteLine("Done!");
				Console.ReadLine();
			}
		}
	}
}
