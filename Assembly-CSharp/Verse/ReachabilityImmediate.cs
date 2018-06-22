﻿using System;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000C82 RID: 3202
	public static class ReachabilityImmediate
	{
		// Token: 0x06004631 RID: 17969 RVA: 0x0025004C File Offset: 0x0024E44C
		public static bool CanReachImmediate(IntVec3 start, LocalTargetInfo target, Map map, PathEndMode peMode, Pawn pawn)
		{
			bool result;
			if (!target.IsValid)
			{
				result = false;
			}
			else
			{
				target = (LocalTargetInfo)GenPath.ResolvePathMode(pawn, target.ToTargetInfo(map), ref peMode);
				if (target.HasThing)
				{
					Thing thing = target.Thing;
					if (!thing.Spawned)
					{
						if (pawn != null)
						{
							if (pawn.carryTracker.innerContainer.Contains(thing))
							{
								return true;
							}
							if (pawn.inventory.innerContainer.Contains(thing))
							{
								return true;
							}
							if (pawn.apparel != null && pawn.apparel.Contains(thing))
							{
								return true;
							}
							if (pawn.equipment != null && pawn.equipment.Contains(thing))
							{
								return true;
							}
						}
						return false;
					}
					if (thing.Map != map)
					{
						return false;
					}
				}
				if (!target.HasThing || (target.Thing.def.size.x == 1 && target.Thing.def.size.z == 1))
				{
					if (start == target.Cell)
					{
						return true;
					}
				}
				else if (start.IsInside(target.Thing))
				{
					return true;
				}
				result = (peMode == PathEndMode.Touch && TouchPathEndModeUtility.IsAdjacentOrInsideAndAllowedToTouch(start, target, map));
			}
			return result;
		}

		// Token: 0x06004632 RID: 17970 RVA: 0x002501FC File Offset: 0x0024E5FC
		public static bool CanReachImmediate(this Pawn pawn, LocalTargetInfo target, PathEndMode peMode)
		{
			return pawn.Spawned && ReachabilityImmediate.CanReachImmediate(pawn.Position, target, pawn.Map, peMode, pawn);
		}

		// Token: 0x06004633 RID: 17971 RVA: 0x00250238 File Offset: 0x0024E638
		public static bool CanReachImmediateNonLocal(this Pawn pawn, TargetInfo target, PathEndMode peMode)
		{
			return pawn.Spawned && (target.Map == null || target.Map == pawn.Map) && pawn.CanReachImmediate((LocalTargetInfo)target, peMode);
		}

		// Token: 0x06004634 RID: 17972 RVA: 0x00250294 File Offset: 0x0024E694
		public static bool CanReachImmediate(IntVec3 start, CellRect rect, Map map, PathEndMode peMode, Pawn pawn)
		{
			IntVec3 c = rect.ClosestCellTo(start);
			return ReachabilityImmediate.CanReachImmediate(start, c, map, peMode, pawn);
		}
	}
}
