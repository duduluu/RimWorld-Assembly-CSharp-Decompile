﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000140 RID: 320
	public class WorkGiver_FeedPatient : WorkGiver_Scanner
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x0004498C File Offset: 0x00042D8C
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x000449A8 File Offset: 0x00042DA8
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.ClosestTouch;
			}
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x000449C0 File Offset: 0x00042DC0
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x000449D8 File Offset: 0x00042DD8
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			bool result;
			if (pawn2 == null || pawn2 == pawn)
			{
				result = false;
			}
			else if (this.def.feedHumanlikesOnly && !pawn2.RaceProps.Humanlike)
			{
				result = false;
			}
			else if (this.def.feedAnimalsOnly && !pawn2.RaceProps.Animal)
			{
				result = false;
			}
			else if (pawn2.needs.food == null || pawn2.needs.food.CurLevelPercentage > pawn2.needs.food.PercentageThreshHungry + 0.02f)
			{
				result = false;
			}
			else if (!FeedPatientUtility.ShouldBeFed(pawn2))
			{
				result = false;
			}
			else
			{
				LocalTargetInfo target = t;
				Thing thing;
				ThingDef thingDef;
				if (!pawn.CanReserve(target, 1, -1, null, forced))
				{
					result = false;
				}
				else if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn2, pawn2.needs.food.CurCategory == HungerCategory.Starving, out thing, out thingDef, false, true, false, true, false, false, false))
				{
					JobFailReason.Is("NoFood".Translate(), null);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x00044B14 File Offset: 0x00042F14
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = (Pawn)t;
			Thing thing;
			ThingDef thingDef;
			Job result;
			if (FoodUtility.TryFindBestFoodSourceFor(pawn, pawn2, pawn2.needs.food.CurCategory == HungerCategory.Starving, out thing, out thingDef, false, true, false, true, false, false, false))
			{
				float nutrition = FoodUtility.GetNutrition(thing, thingDef);
				result = new Job(JobDefOf.FeedPatient)
				{
					targetA = thing,
					targetB = pawn2,
					count = FoodUtility.WillIngestStackCountOf(pawn2, thingDef, nutrition)
				};
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
