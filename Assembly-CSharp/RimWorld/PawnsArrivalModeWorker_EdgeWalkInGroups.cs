﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x0200049E RID: 1182
	public class PawnsArrivalModeWorker_EdgeWalkInGroups : PawnsArrivalModeWorker
	{
		// Token: 0x06001520 RID: 5408 RVA: 0x000B9CF0 File Offset: 0x000B80F0
		public override void Arrive(List<Pawn> pawns, IncidentParms parms)
		{
			Map map = (Map)parms.target;
			List<Pair<List<Pawn>, IntVec3>> list = PawnsArrivalModeWorkerUtility.SplitIntoRandomGroupsNearMapEdge(pawns, map, false);
			PawnsArrivalModeWorkerUtility.SetPawnGroupsInfo(parms, list);
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list[i].First.Count; j++)
				{
					IntVec3 loc = CellFinder.RandomClosewalkCellNear(list[i].Second, map, 8, null);
					GenSpawn.Spawn(list[i].First[j], loc, map, parms.spawnRotation, WipeMode.Vanish, false);
				}
			}
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x000B9D9C File Offset: 0x000B819C
		public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
		{
			parms.spawnRotation = Rot4.Random;
			return true;
		}
	}
}
