﻿using System;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000132 RID: 306
	public abstract class WorkGiver_Warden : WorkGiver_Scanner
	{
		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600064D RID: 1613 RVA: 0x00042080 File Offset: 0x00040480
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x0004209C File Offset: 0x0004049C
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.OnCell;
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000420B4 File Offset: 0x000404B4
		protected bool ShouldTakeCareOfPrisoner(Pawn warden, Thing prisoner)
		{
			Pawn pawn = prisoner as Pawn;
			return pawn != null && pawn.IsPrisonerOfColony && pawn.guest.PrisonerIsSecure && pawn.Spawned && !pawn.InAggroMentalState && !prisoner.IsForbidden(warden) && !pawn.IsFormingCaravan() && warden.CanReserveAndReach(pawn, PathEndMode.OnCell, warden.NormalMaxDanger(), 1, -1, null, false);
		}
	}
}
