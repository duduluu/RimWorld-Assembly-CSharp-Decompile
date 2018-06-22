﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000146 RID: 326
	public class WorkGiver_GrowerHarvest : WorkGiver_Grower
	{
		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00045970 File Offset: 0x00043D70
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00045988 File Offset: 0x00043D88
		public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
		{
			Plant plant = c.GetPlant(pawn.Map);
			bool result;
			if (plant == null)
			{
				result = false;
			}
			else if (plant.IsForbidden(pawn))
			{
				result = false;
			}
			else if (!plant.HarvestableNow || plant.LifeStage != PlantLifeStage.Mature)
			{
				result = false;
			}
			else if (!plant.CanYieldNow())
			{
				result = false;
			}
			else
			{
				LocalTargetInfo target = plant;
				result = pawn.CanReserve(target, 1, -1, null, forced);
			}
			return result;
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x00045A20 File Offset: 0x00043E20
		public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
		{
			Job job = new Job(JobDefOf.Harvest);
			Map map = pawn.Map;
			Room room = c.GetRoom(map, RegionType.Set_Passable);
			float num = 0f;
			for (int i = 0; i < 40; i++)
			{
				IntVec3 intVec = c + GenRadial.RadialPattern[i];
				if (intVec.GetRoom(map, RegionType.Set_Passable) == room)
				{
					if (this.HasJobOnCell(pawn, intVec, false))
					{
						Plant plant = intVec.GetPlant(map);
						if (!(intVec != c) || plant.def == WorkGiver_Grower.CalculateWantedPlantDef(intVec, map))
						{
							num += plant.def.plant.harvestWork;
							if (intVec != c && num > 2400f)
							{
								break;
							}
							job.AddQueuedTarget(TargetIndex.A, plant);
						}
					}
				}
			}
			if (job.targetQueueA != null && job.targetQueueA.Count >= 3)
			{
				job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
			}
			return job;
		}
	}
}
