﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005C2 RID: 1474
	public class WorldGenStep_Roads : WorldGenStep
	{
		// Token: 0x040010F7 RID: 4343
		private static readonly FloatRange ExtraRoadNodesPer100kTiles = new FloatRange(30f, 50f);

		// Token: 0x040010F8 RID: 4344
		private static readonly IntRange RoadDistanceFromSettlement = new IntRange(-4, 4);

		// Token: 0x040010F9 RID: 4345
		private const float ChanceExtraNonSpanningTreeLink = 0.015f;

		// Token: 0x040010FA RID: 4346
		private const float ChanceHideSpanningTreeLink = 0.1f;

		// Token: 0x040010FB RID: 4347
		private const float ChanceWorldObjectReclusive = 0.05f;

		// Token: 0x040010FC RID: 4348
		private const int PotentialSpanningTreeLinksPerSettlement = 8;

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06001C56 RID: 7254 RVA: 0x000F3B34 File Offset: 0x000F1F34
		public override int SeedPart
		{
			get
			{
				return 1538475135;
			}
		}

		// Token: 0x06001C57 RID: 7255 RVA: 0x000F3B4E File Offset: 0x000F1F4E
		public override void GenerateFresh(string seed)
		{
			this.GenerateRoadEndpoints();
			Rand.PushState();
			Rand.Seed = GenText.StableStringHash(seed);
			this.GenerateRoadNetwork();
			Rand.PopState();
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x000F3B72 File Offset: 0x000F1F72
		public override void GenerateWithoutWorldData(string seed)
		{
			Rand.PushState();
			Rand.Seed = GenText.StableStringHash(seed);
			this.GenerateRoadNetwork();
			Rand.PopState();
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x000F3B90 File Offset: 0x000F1F90
		private void GenerateRoadEndpoints()
		{
			List<int> list = (from wo in Find.WorldObjects.AllWorldObjects
			where Rand.Value > 0.05f
			select wo.Tile).ToList<int>();
			int num = GenMath.RoundRandom((float)Find.WorldGrid.TilesCount / 100000f * WorldGenStep_Roads.ExtraRoadNodesPer100kTiles.RandomInRange);
			for (int i = 0; i < num; i++)
			{
				list.Add(TileFinder.RandomFactionBaseTileFor(null, false, null));
			}
			List<int> list2 = new List<int>();
			for (int j = 0; j < list.Count; j++)
			{
				int num2 = Mathf.Max(0, WorldGenStep_Roads.RoadDistanceFromSettlement.RandomInRange);
				int num3 = list[j];
				for (int k = 0; k < num2; k++)
				{
					Find.WorldGrid.GetTileNeighbors(num3, list2);
					num3 = list2.RandomElement<int>();
				}
				if (Find.WorldReachability.CanReach(list[j], num3))
				{
					list[j] = num3;
				}
			}
			list = list.Distinct<int>().ToList<int>();
			Find.World.genData.roadNodes = list;
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x000F3CEC File Offset: 0x000F20EC
		private void GenerateRoadNetwork()
		{
			Find.WorldPathGrid.RecalculateAllPerceivedPathCosts(new int?(0));
			List<WorldGenStep_Roads.Link> linkProspective = this.GenerateProspectiveLinks(Find.World.genData.roadNodes);
			List<WorldGenStep_Roads.Link> linkFinal = this.GenerateFinalLinks(linkProspective, Find.World.genData.roadNodes.Count);
			this.DrawLinksOnWorld(linkFinal, Find.World.genData.roadNodes);
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x000F3D54 File Offset: 0x000F2154
		private List<WorldGenStep_Roads.Link> GenerateProspectiveLinks(List<int> indexToTile)
		{
			Dictionary<int, int> tileToIndexLookup = new Dictionary<int, int>();
			for (int i = 0; i < indexToTile.Count; i++)
			{
				tileToIndexLookup[indexToTile[i]] = i;
			}
			List<WorldGenStep_Roads.Link> linkProspective = new List<WorldGenStep_Roads.Link>();
			List<int> list = new List<int>();
			int srcIndex;
			for (srcIndex = 0; srcIndex < indexToTile.Count; srcIndex++)
			{
				int srcTile = indexToTile[srcIndex];
				list.Clear();
				list.Add(srcTile);
				int found = 0;
				Find.WorldPathFinder.FloodPathsWithCost(list, (int src, int dst) => Caravan_PathFollower.CostToMove(3500, src, dst, null, true, null, null), null, delegate(int tile, float distance)
				{
					if (tile != srcTile && tileToIndexLookup.ContainsKey(tile))
					{
						found++;
						linkProspective.Add(new WorldGenStep_Roads.Link
						{
							distance = distance,
							indexA = srcIndex,
							indexB = tileToIndexLookup[tile]
						});
					}
					return found >= 8;
				});
			}
			linkProspective.Sort((WorldGenStep_Roads.Link lhs, WorldGenStep_Roads.Link rhs) => lhs.distance.CompareTo(rhs.distance));
			return linkProspective;
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x000F3E98 File Offset: 0x000F2298
		private List<WorldGenStep_Roads.Link> GenerateFinalLinks(List<WorldGenStep_Roads.Link> linkProspective, int endpointCount)
		{
			List<WorldGenStep_Roads.Connectedness> list = new List<WorldGenStep_Roads.Connectedness>();
			for (int i = 0; i < endpointCount; i++)
			{
				list.Add(new WorldGenStep_Roads.Connectedness());
			}
			List<WorldGenStep_Roads.Link> list2 = new List<WorldGenStep_Roads.Link>();
			int j = 0;
			while (j < linkProspective.Count)
			{
				WorldGenStep_Roads.Link prospective = linkProspective[j];
				if (list[prospective.indexA].Group() != list[prospective.indexB].Group())
				{
					goto IL_AF;
				}
				if (Rand.Value <= 0.015f)
				{
					if (!list2.Any((WorldGenStep_Roads.Link link) => link.indexB == prospective.indexA && link.indexA == prospective.indexB))
					{
						goto IL_AF;
					}
				}
				IL_146:
				j++;
				continue;
				IL_AF:
				if (Rand.Value > 0.1f)
				{
					list2.Add(prospective);
				}
				if (list[prospective.indexA].Group() != list[prospective.indexB].Group())
				{
					WorldGenStep_Roads.Connectedness parent = new WorldGenStep_Roads.Connectedness();
					list[prospective.indexA].Group().parent = parent;
					list[prospective.indexB].Group().parent = parent;
				}
				goto IL_146;
			}
			return list2;
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x000F4008 File Offset: 0x000F2408
		private void DrawLinksOnWorld(List<WorldGenStep_Roads.Link> linkFinal, List<int> indexToTile)
		{
			foreach (WorldGenStep_Roads.Link link in linkFinal)
			{
				WorldPath worldPath = Find.WorldPathFinder.FindPath(indexToTile[link.indexA], indexToTile[link.indexB], null, null);
				List<int> nodesReversed = worldPath.NodesReversed;
				RoadDef roadDef = (from rd in DefDatabase<RoadDef>.AllDefsListForReading
				where !rd.ancientOnly
				select rd).RandomElementWithFallback(null);
				for (int i = 0; i < nodesReversed.Count - 1; i++)
				{
					Find.WorldGrid.OverlayRoad(nodesReversed[i], nodesReversed[i + 1], roadDef);
				}
				worldPath.ReleaseToPool();
			}
		}

		// Token: 0x020005C3 RID: 1475
		private struct Link
		{
			// Token: 0x04001102 RID: 4354
			public float distance;

			// Token: 0x04001103 RID: 4355
			public int indexA;

			// Token: 0x04001104 RID: 4356
			public int indexB;
		}

		// Token: 0x020005C4 RID: 1476
		private class Connectedness
		{
			// Token: 0x04001105 RID: 4357
			public WorldGenStep_Roads.Connectedness parent;

			// Token: 0x06001C65 RID: 7269 RVA: 0x000F41D8 File Offset: 0x000F25D8
			public WorldGenStep_Roads.Connectedness Group()
			{
				WorldGenStep_Roads.Connectedness result;
				if (this.parent == null)
				{
					result = this;
				}
				else
				{
					result = this.parent.Group();
				}
				return result;
			}
		}
	}
}
