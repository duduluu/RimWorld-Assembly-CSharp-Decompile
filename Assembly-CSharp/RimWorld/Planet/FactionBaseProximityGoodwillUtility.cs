﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000557 RID: 1367
	public static class FactionBaseProximityGoodwillUtility
	{
		// Token: 0x04000F19 RID: 3865
		private static List<Pair<FactionBase, int>> tmpGoodwillOffsets = new List<Pair<FactionBase, int>>();

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x060019A0 RID: 6560 RVA: 0x000DEBF4 File Offset: 0x000DCFF4
		public static int MaxDist
		{
			get
			{
				return Mathf.RoundToInt(DiplomacyTuning.Goodwill_PerQuadrumFromSettlementProximity.Last<CurvePoint>().x);
			}
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x000DEC20 File Offset: 0x000DD020
		public static void CheckFactionBaseProximityGoodwillChange()
		{
			if (Find.TickManager.TicksGame != 0 && Find.TickManager.TicksGame % 900000 == 0)
			{
				List<FactionBase> factionBases = Find.WorldObjects.FactionBases;
				FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Clear();
				for (int i = 0; i < factionBases.Count; i++)
				{
					FactionBase factionBase = factionBases[i];
					if (factionBase.Faction == Faction.OfPlayer)
					{
						FactionBaseProximityGoodwillUtility.AppendProximityGoodwillOffsets(factionBase.Tile, FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets, true, false);
					}
				}
				if (FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Any<Pair<FactionBase, int>>())
				{
					FactionBaseProximityGoodwillUtility.SortProximityGoodwillOffsets(FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets);
					List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
					bool flag = false;
					string text = "LetterFactionBaseProximity".Translate() + "\n\n" + FactionBaseProximityGoodwillUtility.ProximityGoodwillOffsetsToString(FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets);
					for (int j = 0; j < allFactionsListForReading.Count; j++)
					{
						Faction faction = allFactionsListForReading[j];
						if (faction != Faction.OfPlayer)
						{
							int num = 0;
							for (int k = 0; k < FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Count; k++)
							{
								if (FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets[k].First.Faction == faction)
								{
									num += FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets[k].Second;
								}
							}
							FactionRelationKind playerRelationKind = faction.PlayerRelationKind;
							if (faction.TryAffectGoodwillWith(Faction.OfPlayer, num, false, false, null, null))
							{
								flag = true;
								faction.TryAppendRelationKindChangedInfo(ref text, playerRelationKind, faction.PlayerRelationKind, null);
							}
						}
					}
					if (flag)
					{
						Find.LetterStack.ReceiveLetter("LetterLabelFactionBaseProximity".Translate(), text, LetterDefOf.NegativeEvent, null);
					}
				}
			}
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x000DEE00 File Offset: 0x000DD200
		public static void AppendProximityGoodwillOffsets(int tile, List<Pair<FactionBase, int>> outOffsets, bool ignoreIfAlreadyMinGoodwill, bool ignorePermanentlyHostile)
		{
			int maxDist = FactionBaseProximityGoodwillUtility.MaxDist;
			List<FactionBase> factionBases = Find.WorldObjects.FactionBases;
			for (int i = 0; i < factionBases.Count; i++)
			{
				FactionBase factionBase = factionBases[i];
				if (factionBase.Faction != null)
				{
					if (factionBase.Faction != Faction.OfPlayer)
					{
						if (!ignorePermanentlyHostile || !factionBase.Faction.def.permanentEnemy)
						{
							if (!ignoreIfAlreadyMinGoodwill || factionBase.Faction.PlayerGoodwill != -100)
							{
								int num = Find.WorldGrid.TraversalDistanceBetween(tile, factionBase.Tile, false, maxDist);
								if (num != 2147483647)
								{
									int num2 = Mathf.RoundToInt(DiplomacyTuning.Goodwill_PerQuadrumFromSettlementProximity.Evaluate((float)num));
									if (num2 != 0)
									{
										outOffsets.Add(new Pair<FactionBase, int>(factionBase, num2));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x000DEEF0 File Offset: 0x000DD2F0
		public static void SortProximityGoodwillOffsets(List<Pair<FactionBase, int>> offsets)
		{
			offsets.SortBy((Pair<FactionBase, int> x) => x.First.Faction.loadID, (Pair<FactionBase, int> x) => -Mathf.Abs(x.Second));
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x000DEF40 File Offset: 0x000DD340
		public static string ProximityGoodwillOffsetsToString(List<Pair<FactionBase, int>> offsets)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < offsets.Count; i++)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("  - " + offsets[i].First.LabelCap + ": " + "ProximitySingleGoodwillChange".Translate(new object[]
				{
					offsets[i].Second.ToStringWithSign(),
					offsets[i].First.Faction.Name
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x000DF000 File Offset: 0x000DD400
		public static void CheckConfirmSettle(int tile, Action settleAction)
		{
			FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Clear();
			FactionBaseProximityGoodwillUtility.AppendProximityGoodwillOffsets(tile, FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets, false, true);
			if (FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Any<Pair<FactionBase, int>>())
			{
				FactionBaseProximityGoodwillUtility.SortProximityGoodwillOffsets(FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets);
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmSettleNearFactionBase".Translate(new object[]
				{
					FactionBaseProximityGoodwillUtility.MaxDist - 1,
					15
				}) + "\n\n" + FactionBaseProximityGoodwillUtility.ProximityGoodwillOffsetsToString(FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets), settleAction, false, null));
			}
			else
			{
				settleAction();
			}
		}
	}
}
