﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000167 RID: 359
	public class WorkGiver_VisitSickPawn : WorkGiver_Scanner
	{
		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x00049684 File Offset: 0x00047A84
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000761 RID: 1889 RVA: 0x0004969C File Offset: 0x00047A9C
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x000496B8 File Offset: 0x00047AB8
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !InteractionUtility.CanInitiateInteraction(pawn);
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x000496D8 File Offset: 0x00047AD8
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			return pawn2 != null && SickPawnVisitUtility.CanVisit(pawn, pawn2, JoyCategory.VeryLow);
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0004970C File Offset: 0x00047B0C
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = (Pawn)t;
			return new Job(JobDefOf.VisitSickPawn, pawn2, SickPawnVisitUtility.FindChair(pawn, pawn2))
			{
				ignoreJoyTimeAssignment = true
			};
		}
	}
}
