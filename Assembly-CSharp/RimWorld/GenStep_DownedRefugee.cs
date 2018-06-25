﻿using System;
using RimWorld.Planet;
using Verse;

namespace RimWorld
{
	// Token: 0x0200040B RID: 1035
	public class GenStep_DownedRefugee : GenStep_Scatterer
	{
		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060011C7 RID: 4551 RVA: 0x0009AA60 File Offset: 0x00098E60
		public override int SeedPart
		{
			get
			{
				return 931842770;
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x0009AA7C File Offset: 0x00098E7C
		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			return base.CanScatterAt(c, map) && c.Standable(map);
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x0009AAA8 File Offset: 0x00098EA8
		protected override void ScatterAt(IntVec3 loc, Map map, int count = 1)
		{
			DownedRefugeeComp component = map.Parent.GetComponent<DownedRefugeeComp>();
			Pawn pawn;
			if (component != null && component.pawn.Any)
			{
				pawn = component.pawn.Take(component.pawn[0]);
			}
			else
			{
				pawn = DownedRefugeeQuestUtility.GenerateRefugee(map.Tile);
			}
			GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
			pawn.mindState.willJoinColonyIfRescued = true;
			MapGenerator.rootsToUnfog.Add(loc);
		}
	}
}
