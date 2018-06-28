﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	public static class FactionBaseProximityGoodwillUtility
	{
		private static List<Pair<Settlement, int>> tmpGoodwillOffsets = new List<Pair<Settlement, int>>();

		[CompilerGenerated]
		private static Func<Pair<Settlement, int>, int> <>f__am$cache0;

		[CompilerGenerated]
		private static Func<Pair<Settlement, int>, int> <>f__am$cache1;

		public static int MaxDist
		{
			get
			{
				return Mathf.RoundToInt(DiplomacyTuning.Goodwill_PerQuadrumFromSettlementProximity.Last<CurvePoint>().x);
			}
		}

		public static void CheckFactionBaseProximityGoodwillChange()
		{
			if (Find.TickManager.TicksGame != 0 && Find.TickManager.TicksGame % 900000 == 0)
			{
				List<Settlement> settlements = Find.WorldObjects.Settlements;
				FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Clear();
				for (int i = 0; i < settlements.Count; i++)
				{
					Settlement settlement = settlements[i];
					if (settlement.Faction == Faction.OfPlayer)
					{
						FactionBaseProximityGoodwillUtility.AppendProximityGoodwillOffsets(settlement.Tile, FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets, true, false);
					}
				}
				if (FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Any<Pair<Settlement, int>>())
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

		public static void AppendProximityGoodwillOffsets(int tile, List<Pair<Settlement, int>> outOffsets, bool ignoreIfAlreadyMinGoodwill, bool ignorePermanentlyHostile)
		{
			int maxDist = FactionBaseProximityGoodwillUtility.MaxDist;
			List<Settlement> settlements = Find.WorldObjects.Settlements;
			for (int i = 0; i < settlements.Count; i++)
			{
				Settlement settlement = settlements[i];
				if (settlement.Faction != null)
				{
					if (settlement.Faction != Faction.OfPlayer)
					{
						if (!ignorePermanentlyHostile || !settlement.Faction.def.permanentEnemy)
						{
							if (!ignoreIfAlreadyMinGoodwill || settlement.Faction.PlayerGoodwill != -100)
							{
								int num = Find.WorldGrid.TraversalDistanceBetween(tile, settlement.Tile, false, maxDist);
								if (num != 2147483647)
								{
									int num2 = Mathf.RoundToInt(DiplomacyTuning.Goodwill_PerQuadrumFromSettlementProximity.Evaluate((float)num));
									if (num2 != 0)
									{
										outOffsets.Add(new Pair<Settlement, int>(settlement, num2));
									}
								}
							}
						}
					}
				}
			}
		}

		public static void SortProximityGoodwillOffsets(List<Pair<Settlement, int>> offsets)
		{
			offsets.SortBy((Pair<Settlement, int> x) => x.First.Faction.loadID, (Pair<Settlement, int> x) => -Mathf.Abs(x.Second));
		}

		public static string ProximityGoodwillOffsetsToString(List<Pair<Settlement, int>> offsets)
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

		public static void CheckConfirmSettle(int tile, Action settleAction)
		{
			FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Clear();
			FactionBaseProximityGoodwillUtility.AppendProximityGoodwillOffsets(tile, FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets, false, true);
			if (FactionBaseProximityGoodwillUtility.tmpGoodwillOffsets.Any<Pair<Settlement, int>>())
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

		// Note: this type is marked as 'beforefieldinit'.
		static FactionBaseProximityGoodwillUtility()
		{
		}

		[CompilerGenerated]
		private static int <SortProximityGoodwillOffsets>m__0(Pair<Settlement, int> x)
		{
			return x.First.Faction.loadID;
		}

		[CompilerGenerated]
		private static int <SortProximityGoodwillOffsets>m__1(Pair<Settlement, int> x)
		{
			return -Mathf.Abs(x.Second);
		}
	}
}
