﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000165 RID: 357
	public class WorkGiver_Flick : WorkGiver_Scanner
	{
		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000755 RID: 1877 RVA: 0x00049394 File Offset: 0x00047794
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x000493AC File Offset: 0x000477AC
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x000493C4 File Offset: 0x000477C4
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			List<Designation> desList = pawn.Map.designationManager.allDesignations;
			for (int i = 0; i < desList.Count; i++)
			{
				if (desList[i].def == DesignationDefOf.Flick)
				{
					yield return desList[i].target.Thing;
				}
			}
			yield break;
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x000493F0 File Offset: 0x000477F0
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			bool result;
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Flick) == null)
			{
				result = false;
			}
			else
			{
				LocalTargetInfo target = t;
				result = pawn.CanReserve(target, 1, -1, null, forced);
			}
			return result;
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x0004944C File Offset: 0x0004784C
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(JobDefOf.Flick, t);
		}
	}
}
