﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public static class NamePlayerFactionAndBaseUtility
	{
		private const float MinDaysPassedToNameFaction = 3f;

		private const float MinDaysPassedToNameFactionBase = 3f;

		private const int SoonTicks = 30000;

		[CompilerGenerated]
		private static Func<IAttackTarget, bool> <>f__am$cache0;

		public static bool CanNameFactionNow()
		{
			return NamePlayerFactionAndBaseUtility.CanNameFaction(Find.TickManager.TicksGame);
		}

		public static bool CanNameFactionBaseNow(Settlement factionBase)
		{
			return NamePlayerFactionAndBaseUtility.CanNameFactionBase(factionBase, Find.TickManager.TicksGame - factionBase.creationGameTicks);
		}

		public static bool CanNameFactionSoon()
		{
			return NamePlayerFactionAndBaseUtility.CanNameFaction(Find.TickManager.TicksGame + 30000);
		}

		public static bool CanNameFactionBaseSoon(Settlement factionBase)
		{
			return NamePlayerFactionAndBaseUtility.CanNameFactionBase(factionBase, Find.TickManager.TicksGame - factionBase.creationGameTicks + 30000);
		}

		private static bool CanNameFaction(int ticksPassed)
		{
			return !Faction.OfPlayer.HasName && (float)ticksPassed / 60000f >= 3f && NamePlayerFactionAndBaseUtility.CanNameAnythingNow();
		}

		private static bool CanNameFactionBase(Settlement factionBase, int ticksPassed)
		{
			return factionBase.Faction == Faction.OfPlayer && !factionBase.namedByPlayer && (float)ticksPassed / 60000f >= 3f && factionBase.HasMap && factionBase.Map.dangerWatcher.DangerRating != StoryDanger.High && factionBase.Map.mapPawns.FreeColonistsSpawnedCount != 0 && NamePlayerFactionAndBaseUtility.CanNameAnythingNow();
		}

		private static bool CanNameAnythingNow()
		{
			bool result;
			if (Find.AnyPlayerHomeMap == null || Find.CurrentMap == null || !Find.CurrentMap.IsPlayerHome || Find.GameEnder.gameEnding)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					if (maps[i].IsPlayerHome)
					{
						if (maps[i].mapPawns.FreeColonistsSpawnedCount >= 2)
						{
							flag = true;
						}
						if (!maps[i].attackTargetsCache.TargetsHostileToColony.Any((IAttackTarget x) => GenHostility.IsActiveThreatToPlayer(x)))
						{
							flag2 = true;
						}
					}
				}
				result = (flag && flag2);
			}
			return result;
		}

		[CompilerGenerated]
		private static bool <CanNameAnythingNow>m__0(IAttackTarget x)
		{
			return GenHostility.IsActiveThreatToPlayer(x);
		}
	}
}
