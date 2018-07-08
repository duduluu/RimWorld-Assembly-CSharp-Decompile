﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	public class IncidentWorker_TraderCaravanArrival : IncidentWorker_NeutralGroup
	{
		public IncidentWorker_TraderCaravanArrival()
		{
		}

		protected override PawnGroupKindDef PawnGroupKindDef
		{
			get
			{
				return PawnGroupKindDefOf.Trader;
			}
		}

		protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
		{
			return base.FactionCanBeGroupSource(f, map, desperate) && f.def.caravanTraderKinds.Any<TraderKindDef>();
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			bool result;
			if (!base.CanFireNowSub(parms))
			{
				result = false;
			}
			else
			{
				Map map = (Map)parms.target;
				result = (parms.faction == null || !base.AnyBlockingHostileLord(map, parms.faction));
			}
			return result;
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			bool result;
			if (!base.TryResolveParms(parms))
			{
				result = false;
			}
			else if (parms.faction.HostileTo(Faction.OfPlayer))
			{
				result = false;
			}
			else
			{
				List<Pawn> list = base.SpawnPawns(parms);
				if (list.Count == 0)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].needs != null && list[i].needs.food != null)
						{
							list[i].needs.food.CurLevel = list[i].needs.food.MaxLevel;
						}
					}
					TraderKindDef traderKindDef = null;
					for (int j = 0; j < list.Count; j++)
					{
						Pawn pawn = list[j];
						if (pawn.TraderKind != null)
						{
							traderKindDef = pawn.TraderKind;
							break;
						}
					}
					string label = "LetterLabelTraderCaravanArrival".Translate(new object[]
					{
						parms.faction.Name,
						traderKindDef.label
					}).CapitalizeFirst();
					string text = "LetterTraderCaravanArrival".Translate(new object[]
					{
						parms.faction.Name,
						traderKindDef.label
					}).CapitalizeFirst();
					PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(list, ref label, ref text, "LetterRelatedPawnsNeutralGroup".Translate(new object[]
					{
						Faction.OfPlayer.def.pawnsPlural
					}), true, true);
					Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, list[0], parms.faction, null);
					IntVec3 chillSpot;
					RCellFinder.TryFindRandomSpotJustOutsideColony(list[0], out chillSpot);
					LordJob_TradeWithColony lordJob = new LordJob_TradeWithColony(parms.faction, chillSpot);
					LordMaker.MakeNewLord(parms.faction, lordJob, map, list);
					result = true;
				}
			}
			return result;
		}

		protected override void ResolveParmsPoints(IncidentParms parms)
		{
			parms.points = TraderCaravanUtility.GenerateGuardPoints();
		}
	}
}
