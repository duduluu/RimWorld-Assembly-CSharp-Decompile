﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000128 RID: 296
	public class WorkGiver_ConstructDeliverResourcesToFrames : WorkGiver_ConstructDeliverResources
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000617 RID: 1559 RVA: 0x00041328 File Offset: 0x0003F728
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.BuildingFrame);
			}
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00041344 File Offset: 0x0003F744
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job result;
			if (t.Faction != pawn.Faction)
			{
				result = null;
			}
			else
			{
				Frame frame = t as Frame;
				if (frame == null)
				{
					result = null;
				}
				else if (GenConstruct.FirstBlockingThing(frame, pawn) != null)
				{
					result = GenConstruct.HandleBlockingThingJob(frame, pawn, forced);
				}
				else
				{
					bool checkConstructionSkill = this.def.workType == WorkTypeDefOf.Construction;
					if (!GenConstruct.CanConstruct(frame, pawn, checkConstructionSkill, forced))
					{
						result = null;
					}
					else
					{
						result = base.ResourceDeliverJobFor(pawn, frame, true);
					}
				}
			}
			return result;
		}
	}
}
