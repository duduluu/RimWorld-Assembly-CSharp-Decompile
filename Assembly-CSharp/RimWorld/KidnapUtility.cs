﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000564 RID: 1380
	public static class KidnapUtility
	{
		// Token: 0x06001A13 RID: 6675 RVA: 0x000E285C File Offset: 0x000E0C5C
		public static bool IsKidnapped(this Pawn pawn)
		{
			List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
			for (int i = 0; i < allFactionsListForReading.Count; i++)
			{
				if (allFactionsListForReading[i].kidnapped.KidnappedPawnsListForReading.Contains(pawn))
				{
					return true;
				}
			}
			return false;
		}
	}
}
