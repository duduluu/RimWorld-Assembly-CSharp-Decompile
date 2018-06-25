﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x020000C0 RID: 192
	public class JobGiver_AITrashBuildingsDistant : ThinkNode_JobGiver
	{
		// Token: 0x04000297 RID: 663
		public bool attackAllInert;

		// Token: 0x0600047F RID: 1151 RVA: 0x00033688 File Offset: 0x00031A88
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_AITrashBuildingsDistant jobGiver_AITrashBuildingsDistant = (JobGiver_AITrashBuildingsDistant)base.DeepCopy(resolve);
			jobGiver_AITrashBuildingsDistant.attackAllInert = this.attackAllInert;
			return jobGiver_AITrashBuildingsDistant;
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x000336B8 File Offset: 0x00031AB8
		protected override Job TryGiveJob(Pawn pawn)
		{
			List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
			Job result;
			if (allBuildingsColonist.Count == 0)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < 75; i++)
				{
					Building building = allBuildingsColonist.RandomElement<Building>();
					if (TrashUtility.ShouldTrashBuilding(pawn, building, this.attackAllInert))
					{
						return TrashUtility.TrashJob(pawn, building);
					}
				}
				result = null;
			}
			return result;
		}
	}
}
