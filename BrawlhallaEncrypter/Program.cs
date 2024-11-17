using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BrawlhallaDumper
{
	// Token: 0x02000003 RID: 3
	internal class Program
	{
		// Token: 0x06000008 RID: 8 RVA: 0x0000239C File Offset: 0x0000059C
		private static void Main(string[] args)
		{
			Console.Write("Please give the seed: ");
			uint seed;
			uint globalKey;
			if (uint.TryParse(Console.ReadLine(), out seed) && uint.TryParse(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.txt")).Trim(), out globalKey))
			{
				string dumpPath = "Dump";
				if (!Directory.Exists(dumpPath))
				{
					Console.WriteLine("Error: 'Dump' folder not found!");
					Console.ReadLine();
					return;
				}

				Directory.CreateDirectory("Encrypt");
				string[] files = Directory.GetFiles(dumpPath);
				string[] stringEntries = files.Select(x => File.ReadAllText(x, Encoding.UTF8)).ToArray();
				byte[] bytes = BrawlhallaSWZ.Encrypt(seed, globalKey, stringEntries);
				File.WriteAllBytes(Path.Combine("Encrypt", "Encrypted.swz"), bytes);
				Console.WriteLine();
				Console.WriteLine("Done!");
				Console.ReadLine();
			}
		}
	}
}
