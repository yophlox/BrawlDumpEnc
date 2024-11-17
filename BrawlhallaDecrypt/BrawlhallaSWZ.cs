using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace BrawlhallaDumper
{
	// Token: 0x02000002 RID: 2
	public static class BrawlhallaSWZ
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static string[] Decrypt(Stream input, uint globalKey = 124416110U)
		{
			BrawlhallaSWZ.ReadUInt32BE(input);
			uint num = BrawlhallaSWZ.ReadUInt32BE(input);
			Console.WriteLine(string.Format(" | Seed: {0}", num));
			WELL512 well = new WELL512(num ^ globalKey);
			uint num2 = 771006925U;
			uint num3 = globalKey % 31U + 5U;
			int num4 = 0;
			while ((long)num4 < (long)((ulong)num3))
			{
				num2 ^= well.NextUInt();
				num4++;
			}
			List<string> list = new List<string>();
			while (input.Position != input.Length)
			{
				string item;
				if (BrawlhallaSWZ.ReadStringEntry(input, well, out item))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020E4 File Offset: 0x000002E4
		public static byte[] Encrypt(uint seed, uint globalKey = 124416110U, params string[] stringEntries)
		{
			WELL512 well = new WELL512(seed ^ globalKey);
			uint num = 771006925U;
			uint num2 = globalKey % 31U + 5U;
			int num3 = 0;
			while ((long)num3 < (long)((ulong)num2))
			{
				num ^= well.NextUInt();
				num3++;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(4096))
			{
				BrawlhallaSWZ.WriteUInt32BE(memoryStream, num);
				BrawlhallaSWZ.WriteUInt32BE(memoryStream, seed);
				foreach (string s in stringEntries)
				{
					BrawlhallaSWZ.WriteStringEntry(Encoding.UTF8.GetBytes(s), well, memoryStream);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002194 File Offset: 0x00000394
		private static bool ReadStringEntry(Stream input, WELL512 rand, out string result)
		{
			uint num = BrawlhallaSWZ.ReadUInt32BE(input) ^ rand.NextUInt();
			BrawlhallaSWZ.ReadUInt32BE(input);
			rand.NextUInt();
			BrawlhallaSWZ.ReadUInt32BE(input);
			if ((ulong)num + (ulong)input.Position > (ulong)input.Length)
			{
				result = null;
				return false;
			}
			byte[] array = new byte[num];
			input.Read(array, 0, array.Length);
			uint v = rand.NextUInt();
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				int num3 = num2 & 15;
				byte[] array2 = array;
				int num4 = num2;
				int num5 = num4;
				array2[num5] ^= (byte)((255U << num3 & rand.NextUInt()) >> num3);
				v = ((uint)array[num2] ^ BrawlhallaSWZ.RotateRight(v, num2 % 7 + 1));
				num2++;
			}
			byte[] bytes = ZlibStream.UncompressBuffer(array);
			result = Encoding.UTF8.GetString(bytes);
			return true;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002258 File Offset: 0x00000458
		private static void WriteStringEntry(byte[] input, WELL512 rand, Stream output)
		{
			byte[] array = ZlibStream.CompressBuffer(input);
			uint value = (uint)(array.Length ^ (int)rand.NextUInt());
			uint value2 = (uint)(input.Length ^ (int)rand.NextUInt());
			uint num = rand.NextUInt();
			for (int i = 0; i < array.Length; i++)
			{
				num = ((uint)array[i] ^ BrawlhallaSWZ.RotateRight(num, i % 7 + 1));
				int num2 = i & 15;
				byte[] array2 = array;
				int num3 = i;
				int num4 = num3;
				array2[num4] ^= (byte)((255U << num2 & rand.NextUInt()) >> num2);
			}
			BrawlhallaSWZ.WriteUInt32BE(output, value);
			BrawlhallaSWZ.WriteUInt32BE(output, value2);
			BrawlhallaSWZ.WriteUInt32BE(output, num);
			output.Write(array, 0, array.Length);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000022FE File Offset: 0x000004FE
		private static uint RotateRight(uint v, int bits)
		{
			return v >> bits | v << 32 - bits;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002310 File Offset: 0x00000510
		private static uint ReadUInt32BE(Stream stream)
		{
			byte[] array = new byte[4];
			stream.Read(array, 0, 4);
			return (uint)((int)array[3] | (int)array[2] << 8 | (int)array[1] << 16 | (int)array[0] << 24);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002348 File Offset: 0x00000548
		private static void WriteUInt32BE(Stream stream, uint value)
		{
			byte[] buffer = new byte[]
			{
				(byte)(value >> 24 & 255U),
				(byte)(value >> 16 & 255U),
				(byte)(value >> 8 & 255U),
				(byte)(value & 255U)
			};
			stream.Write(buffer, 0, 4);
		}
	}
}
