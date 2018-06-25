﻿using System;
using System.Collections.Generic;

namespace Verse
{
	// Token: 0x02000F04 RID: 3844
	public static class ThingCountUtility
	{
		// Token: 0x06005C2A RID: 23594 RVA: 0x002EE6B8 File Offset: 0x002ECAB8
		public static int CountOf(List<ThingCount> list, Thing thing)
		{
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Thing == thing)
				{
					num += list[i].Count;
				}
			}
			return num;
		}

		// Token: 0x06005C2B RID: 23595 RVA: 0x002EE714 File Offset: 0x002ECB14
		public static void AddToList(List<ThingCount> list, Thing thing, int countToAdd)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Thing == thing)
				{
					list[i] = list[i].WithCount(list[i].Count + countToAdd);
					return;
				}
			}
			list.Add(new ThingCount(thing, countToAdd));
		}
	}
}
