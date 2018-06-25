﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005F1 RID: 1521
	public static class DaysUntilRotCalculator
	{
		// Token: 0x040011EF RID: 4591
		private static List<ThingCount> tmpThingCounts = new List<ThingCount>();

		// Token: 0x040011F0 RID: 4592
		private static List<ThingCount> tmpThingCountsFromTradeables = new List<ThingCount>();

		// Token: 0x040011F1 RID: 4593
		private static List<Pair<float, float>> tmpNutritions = new List<Pair<float, float>>();

		// Token: 0x040011F2 RID: 4594
		private static List<Thing> thingsInReverse = new List<Thing>();

		// Token: 0x040011F3 RID: 4595
		private static List<Pair<int, int>> tmpTicksToArrive = new List<Pair<int, int>>();

		// Token: 0x040011F4 RID: 4596
		public const float InfiniteDaysUntilRot = 600f;

		// Token: 0x06001E3A RID: 7738 RVA: 0x00104F34 File Offset: 0x00103334
		public static float ApproxDaysUntilRot(List<ThingCount> potentiallyFood, int tile, WorldPath path = null, float nextTileCostLeft = 0f, int caravanTicksPerMove = 3500)
		{
			DaysUntilRotCalculator.tmpTicksToArrive.Clear();
			if (path != null && path.Found)
			{
				CaravanArrivalTimeEstimator.EstimatedTicksToArriveToEvery(tile, path.LastNode, path, nextTileCostLeft, caravanTicksPerMove, Find.TickManager.TicksAbs, DaysUntilRotCalculator.tmpTicksToArrive);
			}
			DaysUntilRotCalculator.tmpNutritions.Clear();
			for (int i = 0; i < potentiallyFood.Count; i++)
			{
				ThingCount thingCount = potentiallyFood[i];
				if (thingCount.Count > 0 && thingCount.Thing.def.IsNutritionGivingIngestible)
				{
					CompRottable compRottable = thingCount.Thing.TryGetComp<CompRottable>();
					float first;
					if (compRottable != null && compRottable.Active)
					{
						first = (float)DaysUntilRotCalculator.ApproxTicksUntilRot_AssumeTimePassesBy(compRottable, tile, DaysUntilRotCalculator.tmpTicksToArrive) / 60000f;
					}
					else
					{
						first = 600f;
					}
					float second = thingCount.Thing.GetStatValue(StatDefOf.Nutrition, true) * (float)thingCount.Count;
					DaysUntilRotCalculator.tmpNutritions.Add(new Pair<float, float>(first, second));
				}
			}
			return GenMath.WeightedMedian(DaysUntilRotCalculator.tmpNutritions, 600f, 0.5f);
		}

		// Token: 0x06001E3B RID: 7739 RVA: 0x0010505C File Offset: 0x0010345C
		public static int ApproxTicksUntilRot_AssumeTimePassesBy(CompRottable rot, int tile, List<Pair<int, int>> ticksToArrive = null)
		{
			float num = 0f;
			int num2 = Find.TickManager.TicksAbs;
			while (num < 1f && (float)num2 < (float)Find.TickManager.TicksAbs + 3.606E+07f)
			{
				int tile2 = (!ticksToArrive.NullOrEmpty<Pair<int, int>>()) ? CaravanArrivalTimeEstimator.TileIllBeInAt(num2, ticksToArrive, Find.TickManager.TicksAbs) : tile;
				int num3 = Mathf.FloorToInt((float)rot.ApproxTicksUntilRotWhenAtTempOfTile(tile2, num2) * (1f - num));
				if (num3 <= 0)
				{
					break;
				}
				int num4 = Mathf.Min(num3, 26999);
				num += (float)num4 / (float)num3;
				num2 += num4;
			}
			return num2 - Find.TickManager.TicksAbs;
		}

		// Token: 0x06001E3C RID: 7740 RVA: 0x0010511C File Offset: 0x0010351C
		public static float ApproxDaysUntilRot(Caravan caravan)
		{
			return DaysUntilRotCalculator.ApproxDaysUntilRot(CaravanInventoryUtility.AllInventoryItems(caravan), caravan.Tile, caravan.pather.curPath, caravan.pather.nextTileCostLeft, caravan.TicksPerMove);
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x00105160 File Offset: 0x00103560
		public static float ApproxDaysUntilRot(List<Thing> potentiallyFood, int tile, WorldPath path = null, float nextTileCostLeft = 0f, int caravanTicksPerMove = 3500)
		{
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			for (int i = 0; i < potentiallyFood.Count; i++)
			{
				DaysUntilRotCalculator.tmpThingCounts.Add(new ThingCount(potentiallyFood[i], potentiallyFood[i].stackCount));
			}
			float result = DaysUntilRotCalculator.ApproxDaysUntilRot(DaysUntilRotCalculator.tmpThingCounts, tile, path, nextTileCostLeft, caravanTicksPerMove);
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			return result;
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x001051D8 File Offset: 0x001035D8
		public static float ApproxDaysUntilRot(List<TransferableOneWay> transferables, int tile, IgnorePawnsInventoryMode ignoreInventory, WorldPath path = null, float nextTileCostLeft = 0f, int caravanTicksPerMove = 3500)
		{
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			for (int i = 0; i < transferables.Count; i++)
			{
				TransferableOneWay transferableOneWay = transferables[i];
				if (transferableOneWay.HasAnyThing)
				{
					if (transferableOneWay.AnyThing is Pawn)
					{
						for (int j = 0; j < transferableOneWay.CountToTransfer; j++)
						{
							Pawn pawn = (Pawn)transferableOneWay.things[j];
							if (!InventoryCalculatorsUtility.ShouldIgnoreInventoryOf(pawn, ignoreInventory))
							{
								ThingOwner<Thing> innerContainer = pawn.inventory.innerContainer;
								for (int k = 0; k < innerContainer.Count; k++)
								{
									DaysUntilRotCalculator.tmpThingCounts.Add(new ThingCount(innerContainer[k], innerContainer[k].stackCount));
								}
							}
						}
					}
					else if (transferableOneWay.CountToTransfer > 0)
					{
						TransferableUtility.TransferNoSplit(transferableOneWay.things, transferableOneWay.CountToTransfer, delegate(Thing thing, int count)
						{
							DaysUntilRotCalculator.tmpThingCounts.Add(new ThingCount(thing, count));
						}, false, false);
					}
				}
			}
			float result = DaysUntilRotCalculator.ApproxDaysUntilRot(DaysUntilRotCalculator.tmpThingCounts, tile, path, nextTileCostLeft, caravanTicksPerMove);
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			return result;
		}

		// Token: 0x06001E3F RID: 7743 RVA: 0x00105328 File Offset: 0x00103728
		public static float ApproxDaysUntilRotLeftAfterTransfer(List<TransferableOneWay> transferables, int tile, IgnorePawnsInventoryMode ignoreInventory, WorldPath path = null, float nextTileCostLeft = 0f, int caravanTicksPerMove = 3500)
		{
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			for (int i = 0; i < transferables.Count; i++)
			{
				TransferableOneWay transferableOneWay = transferables[i];
				if (transferableOneWay.HasAnyThing)
				{
					if (transferableOneWay.AnyThing is Pawn)
					{
						for (int j = transferableOneWay.things.Count - 1; j >= transferableOneWay.CountToTransfer; j--)
						{
							Pawn pawn = (Pawn)transferableOneWay.things[j];
							if (!InventoryCalculatorsUtility.ShouldIgnoreInventoryOf(pawn, ignoreInventory))
							{
								ThingOwner<Thing> innerContainer = pawn.inventory.innerContainer;
								for (int k = 0; k < innerContainer.Count; k++)
								{
									DaysUntilRotCalculator.tmpThingCounts.Add(new ThingCount(innerContainer[k], innerContainer[k].stackCount));
								}
							}
						}
					}
					else if (transferableOneWay.MaxCount - transferableOneWay.CountToTransfer > 0)
					{
						DaysUntilRotCalculator.thingsInReverse.Clear();
						DaysUntilRotCalculator.thingsInReverse.AddRange(transferableOneWay.things);
						DaysUntilRotCalculator.thingsInReverse.Reverse();
						TransferableUtility.TransferNoSplit(DaysUntilRotCalculator.thingsInReverse, transferableOneWay.MaxCount - transferableOneWay.CountToTransfer, delegate(Thing thing, int count)
						{
							DaysUntilRotCalculator.tmpThingCounts.Add(new ThingCount(thing, count));
						}, false, false);
					}
				}
			}
			DaysUntilRotCalculator.thingsInReverse.Clear();
			float result = DaysUntilRotCalculator.ApproxDaysUntilRot(DaysUntilRotCalculator.tmpThingCounts, tile, path, nextTileCostLeft, caravanTicksPerMove);
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			return result;
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x001054C0 File Offset: 0x001038C0
		public static float ApproxDaysUntilRotLeftAfterTradeableTransfer(List<Thing> allCurrentThings, List<Tradeable> tradeables, int tile, IgnorePawnsInventoryMode ignoreInventory)
		{
			DaysUntilRotCalculator.tmpThingCountsFromTradeables.Clear();
			TransferableUtility.SimulateTradeableTransfer(allCurrentThings, tradeables, DaysUntilRotCalculator.tmpThingCountsFromTradeables);
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			for (int i = DaysUntilRotCalculator.tmpThingCountsFromTradeables.Count - 1; i >= 0; i--)
			{
				if (DaysUntilRotCalculator.tmpThingCountsFromTradeables[i].Count > 0)
				{
					Pawn pawn = DaysUntilRotCalculator.tmpThingCountsFromTradeables[i].Thing as Pawn;
					if (pawn != null)
					{
						if (!InventoryCalculatorsUtility.ShouldIgnoreInventoryOf(pawn, ignoreInventory))
						{
							ThingOwner<Thing> innerContainer = pawn.inventory.innerContainer;
							for (int j = 0; j < innerContainer.Count; j++)
							{
								DaysUntilRotCalculator.tmpThingCounts.Add(new ThingCount(innerContainer[j], innerContainer[j].stackCount));
							}
						}
					}
					else
					{
						DaysUntilRotCalculator.tmpThingCounts.Add(DaysUntilRotCalculator.tmpThingCountsFromTradeables[i]);
					}
				}
			}
			DaysUntilRotCalculator.tmpThingCountsFromTradeables.Clear();
			float result = DaysUntilRotCalculator.ApproxDaysUntilRot(DaysUntilRotCalculator.tmpThingCounts, tile, null, 0f, 3500);
			DaysUntilRotCalculator.tmpThingCounts.Clear();
			return result;
		}
	}
}
