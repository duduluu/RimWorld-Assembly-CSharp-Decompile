﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000345 RID: 837
	public class IncidentWorker_RaidFriendly : IncidentWorker_Raid
	{
		// Token: 0x06000E4D RID: 3661 RVA: 0x0007A4A4 File Offset: 0x000788A4
		protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
		{
			IEnumerable<Faction> source = (from p in map.attackTargetsCache.TargetsHostileToColony
			select ((Thing)p).Faction).Distinct<Faction>();
			return base.FactionCanBeGroupSource(f, map, desperate) && !f.def.hidden && f.PlayerRelationKind == FactionRelationKind.Ally && (!source.Any<Faction>() || source.Any((Faction hf) => hf.HostileTo(f)));
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x0007A558 File Offset: 0x00078958
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
				result = ((from p in map.attackTargetsCache.TargetsHostileToColony
				where GenHostility.IsActiveThreatToPlayer(p)
				select p).Sum(delegate(IAttackTarget p)
				{
					Pawn pawn = p as Pawn;
					float result2;
					if (pawn != null)
					{
						result2 = pawn.kindDef.combatPower;
					}
					else
					{
						result2 = 0f;
					}
					return result2;
				}) > 120f);
			}
			return result;
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x0007A5E4 File Offset: 0x000789E4
		protected override bool TryResolveRaidFaction(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			bool result;
			if (parms.faction != null)
			{
				result = true;
			}
			else if (!base.CandidateFactions(map, false).Any<Faction>())
			{
				result = false;
			}
			else
			{
				parms.faction = base.CandidateFactions(map, false).RandomElementByWeight((Faction fac) => (float)fac.PlayerGoodwill + 120.000008f);
				result = true;
			}
			return result;
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x0007A661 File Offset: 0x00078A61
		protected override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
		{
			if (parms.raidStrategy == null)
			{
				parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
			}
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x0007A67F File Offset: 0x00078A7F
		protected override void ResolveRaidPoints(IncidentParms parms)
		{
			if (parms.points <= 0f)
			{
				parms.points = Mathf.Min(StorytellerUtility.DefaultThreatPointsNow(parms.target), 500f);
			}
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x0007A6B0 File Offset: 0x00078AB0
		protected override string GetLetterLabel(IncidentParms parms)
		{
			return parms.raidStrategy.letterLabelFriendly;
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x0007A6D0 File Offset: 0x00078AD0
		protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
		{
			string text = string.Format(parms.raidArrivalMode.textFriendly, parms.faction.def.pawnsPlural, parms.faction.Name);
			text += "\n\n";
			text += parms.raidStrategy.arrivalTextFriendly;
			Pawn pawn = pawns.Find((Pawn x) => x.Faction.leader == x);
			if (pawn != null)
			{
				text += "\n\n";
				text += "FriendlyRaidLeaderPresent".Translate(new object[]
				{
					pawn.Faction.def.pawnsPlural,
					pawn.LabelShort
				});
			}
			return text;
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x0007A79C File Offset: 0x00078B9C
		protected override LetterDef GetLetterDef()
		{
			return LetterDefOf.PositiveEvent;
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x0007A7B8 File Offset: 0x00078BB8
		protected override string GetRelatedPawnsInfoLetterText(IncidentParms parms)
		{
			return "LetterRelatedPawnsRaidFriendly".Translate(new object[]
			{
				Faction.OfPlayer.def.pawnsPlural,
				parms.faction.def.pawnsPlural
			});
		}
	}
}
