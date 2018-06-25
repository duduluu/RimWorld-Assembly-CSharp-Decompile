﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000C71 RID: 3185
	public class PlaceWorker_DeepDrill : PlaceWorker_ShowDeepResources
	{
		// Token: 0x060045E3 RID: 17891 RVA: 0x0024E0EC File Offset: 0x0024C4EC
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
		{
			AcceptanceReport result;
			if (DeepDrillUtility.GetNextResource(loc, map) == null)
			{
				result = "MustPlaceOnDrillable".Translate();
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
