﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000220 RID: 544
	public class ThoughtWorker_ColonistLeftUnburied : ThoughtWorker
	{
		// Token: 0x06000A0B RID: 2571 RVA: 0x00059424 File Offset: 0x00057824
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			ThoughtState result;
			if (p.Faction != Faction.OfPlayer)
			{
				result = false;
			}
			else
			{
				List<Thing> list = p.Map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Corpse));
				for (int i = 0; i < list.Count; i++)
				{
					Corpse corpse = (Corpse)list[i];
					if ((float)corpse.Age > 90000f && corpse.InnerPawn.Faction == Faction.OfPlayer && corpse.InnerPawn.def.race.Humanlike && !corpse.IsInAnyStorage())
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}
	}
}
