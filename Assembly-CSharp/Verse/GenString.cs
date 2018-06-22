﻿using System;
using System.Collections.Generic;

namespace Verse
{
	// Token: 0x02000F49 RID: 3913
	public static class GenString
	{
		// Token: 0x06005EAE RID: 24238 RVA: 0x003032F0 File Offset: 0x003016F0
		static GenString()
		{
			for (int i = 0; i < 10000; i++)
			{
				GenString.numberStrings[i] = (i - 5000).ToString();
			}
		}

		// Token: 0x06005EAF RID: 24239 RVA: 0x00303344 File Offset: 0x00301744
		public static string ToStringCached(this int num)
		{
			string result;
			if (num < -4999)
			{
				result = num.ToString();
			}
			else if (num > 4999)
			{
				result = num.ToString();
			}
			else
			{
				result = GenString.numberStrings[num + 5000];
			}
			return result;
		}

		// Token: 0x06005EB0 RID: 24240 RVA: 0x003033A4 File Offset: 0x003017A4
		public static IEnumerable<string> SplitBy(this string str, int chunkLength)
		{
			if (str.NullOrEmpty())
			{
				yield break;
			}
			if (chunkLength < 1)
			{
				throw new ArgumentException();
			}
			for (int i = 0; i < str.Length; i += chunkLength)
			{
				if (chunkLength > str.Length - i)
				{
					chunkLength = str.Length - i;
				}
				yield return str.Substring(i, chunkLength);
			}
			yield break;
		}

		// Token: 0x04003E2D RID: 15917
		private static string[] numberStrings = new string[10000];
	}
}
