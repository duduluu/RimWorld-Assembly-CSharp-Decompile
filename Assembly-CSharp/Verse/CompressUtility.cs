﻿using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;

namespace Verse
{
	// Token: 0x02000ED7 RID: 3799
	public static class CompressUtility
	{
		// Token: 0x06005A00 RID: 23040 RVA: 0x002E3C20 File Offset: 0x002E2020
		public static byte[] Compress(byte[] input)
		{
			MemoryStream memoryStream = new MemoryStream();
			DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress);
			deflateStream.Write(input, 0, input.Length);
			deflateStream.Close();
			return memoryStream.ToArray();
		}

		// Token: 0x06005A01 RID: 23041 RVA: 0x002E3C5C File Offset: 0x002E205C
		public static byte[] Decompress(byte[] input)
		{
			MemoryStream stream = new MemoryStream(input);
			DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
			List<byte[]> list = null;
			byte[] array;
			int num;
			for (;;)
			{
				array = new byte[65536];
				num = deflateStream.Read(array, 0, array.Length);
				if (num < array.Length && list == null)
				{
					break;
				}
				if (num < array.Length)
				{
					goto Block_3;
				}
				if (list == null)
				{
					list = new List<byte[]>();
				}
				list.Add(array);
			}
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, num);
			return array2;
			Block_3:
			byte[] array3 = new byte[num + list.Count * array.Length];
			for (int i = 0; i < list.Count; i++)
			{
				Array.ConstrainedCopy(list[i], 0, array3, i * array.Length, array.Length);
			}
			Array.ConstrainedCopy(array, 0, array3, list.Count * array.Length, num);
			return array3;
		}
	}
}
