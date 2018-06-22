﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x0200040D RID: 1037
	public class GenStep_PreciousLump : GenStep_ScatterLumpsMineable
	{
		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060011D4 RID: 4564 RVA: 0x0009AFD4 File Offset: 0x000993D4
		public override int SeedPart
		{
			get
			{
				return 1634184421;
			}
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0009AFF0 File Offset: 0x000993F0
		public override void Generate(Map map)
		{
			this.forcedDefToScatter = this.mineables.RandomElementByWeight((ThingOption x) => x.weight).thingDef;
			this.count = 1;
			float randomInRange = this.totalValueRange.RandomInRange;
			float baseMarketValue = this.forcedDefToScatter.building.mineableThing.BaseMarketValue;
			this.forcedLumpSize = Mathf.Max(Mathf.RoundToInt(randomInRange / ((float)this.forcedDefToScatter.building.mineableYield * baseMarketValue)), 1);
			base.Generate(map);
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x0009B088 File Offset: 0x00099488
		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			return map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false));
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x0009B0B4 File Offset: 0x000994B4
		protected override void ScatterAt(IntVec3 c, Map map, int stackCount = 1)
		{
			base.ScatterAt(c, map, stackCount);
			int minX = this.recentLumpCells.Min((IntVec3 x) => x.x);
			int minZ = this.recentLumpCells.Min((IntVec3 x) => x.z);
			int maxX = this.recentLumpCells.Max((IntVec3 x) => x.x);
			int maxZ = this.recentLumpCells.Max((IntVec3 x) => x.z);
			CellRect var = CellRect.FromLimits(minX, minZ, maxX, maxZ);
			MapGenerator.SetVar<CellRect>("RectOfInterest", var);
		}

		// Token: 0x04000AD9 RID: 2777
		public List<ThingOption> mineables;

		// Token: 0x04000ADA RID: 2778
		public FloatRange totalValueRange = new FloatRange(1000f, 2000f);
	}
}
