﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000207 RID: 519
	public class ThoughtWorker_Ugly : ThoughtWorker
	{
		// Token: 0x060009D9 RID: 2521 RVA: 0x000585EC File Offset: 0x000569EC
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			ThoughtState result;
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				result = false;
			}
			else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
			{
				result = false;
			}
			else
			{
				int num = other.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
				if (num == -1)
				{
					result = ThoughtState.ActiveAtStage(0);
				}
				else if (num == -2)
				{
					result = ThoughtState.ActiveAtStage(1);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
	}
}
