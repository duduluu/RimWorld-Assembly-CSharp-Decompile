﻿using System;
using RimWorld.BaseGen;
using Verse;

namespace RimWorld
{
	// Token: 0x020003E4 RID: 996
	public class GenStep_FactionBase : GenStep_Scatterer
	{
		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x00092314 File Offset: 0x00090714
		public override int SeedPart
		{
			get
			{
				return 1806208471;
			}
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x00092330 File Offset: 0x00090730
		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			bool result;
			if (!base.CanScatterAt(c, map))
			{
				result = false;
			}
			else if (!c.Standable(map))
			{
				result = false;
			}
			else if (c.Roofed(map))
			{
				result = false;
			}
			else if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
			{
				result = false;
			}
			else
			{
				int min = GenStep_FactionBase.FactionBaseSizeRange.min;
				CellRect cellRect = new CellRect(c.x - min / 2, c.z - min / 2, min, min);
				result = cellRect.FullyContainedWithin(new CellRect(0, 0, map.Size.x, map.Size.z));
			}
			return result;
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x00092408 File Offset: 0x00090808
		protected override void ScatterAt(IntVec3 c, Map map, int stackCount = 1)
		{
			int randomInRange = GenStep_FactionBase.FactionBaseSizeRange.RandomInRange;
			int randomInRange2 = GenStep_FactionBase.FactionBaseSizeRange.RandomInRange;
			CellRect rect = new CellRect(c.x - randomInRange / 2, c.z - randomInRange2 / 2, randomInRange, randomInRange2);
			Faction faction;
			if (map.ParentFaction == null || map.ParentFaction == Faction.OfPlayer)
			{
				faction = Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Undefined);
			}
			else
			{
				faction = map.ParentFaction;
			}
			rect.ClipInsideMap(map);
			ResolveParams resolveParams = default(ResolveParams);
			resolveParams.rect = rect;
			resolveParams.faction = faction;
			BaseGen.globalSettings.map = map;
			BaseGen.globalSettings.minBuildings = 1;
			BaseGen.globalSettings.minBarracks = 1;
			BaseGen.symbolStack.Push("factionBase", resolveParams);
			BaseGen.Generate();
		}

		// Token: 0x04000A58 RID: 2648
		private static readonly IntRange FactionBaseSizeRange = new IntRange(34, 38);
	}
}
