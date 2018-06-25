﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000103 RID: 259
	public class JoyGiver_ViewArt : JoyGiver
	{
		// Token: 0x040002E4 RID: 740
		private static List<Thing> candidates = new List<Thing>();

		// Token: 0x06000571 RID: 1393 RVA: 0x0003B250 File Offset: 0x00039650
		public override Job TryGiveJob(Pawn pawn)
		{
			bool allowedOutside = JoyUtility.EnjoyableOutsideNow(pawn, null);
			Job result;
			try
			{
				JoyGiver_ViewArt.candidates.AddRange(pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Art).Where(delegate(Thing thing)
				{
					bool result2;
					if (thing.Faction != Faction.OfPlayer || thing.IsForbidden(pawn) || (!allowedOutside && !thing.Position.Roofed(thing.Map)) || !pawn.CanReserveAndReach(thing, PathEndMode.Touch, Danger.None, 1, -1, null, false) || !thing.IsPoliticallyProper(pawn))
					{
						result2 = false;
					}
					else
					{
						CompArt compArt = thing.TryGetComp<CompArt>();
						if (compArt == null)
						{
							Log.Error("No CompArt on thing being considered for viewing: " + thing, false);
							result2 = false;
						}
						else if (!compArt.CanShowArt || !compArt.Props.canBeEnjoyedAsArt)
						{
							result2 = false;
						}
						else
						{
							Room room = thing.GetRoom(RegionType.Set_Passable);
							if (room == null)
							{
								result2 = false;
							}
							else
							{
								if (room.Role == RoomRoleDefOf.Bedroom || room.Role == RoomRoleDefOf.Barracks || room.Role == RoomRoleDefOf.PrisonCell || room.Role == RoomRoleDefOf.PrisonBarracks || room.Role == RoomRoleDefOf.Hospital)
								{
									if (pawn.ownership == null || pawn.ownership.OwnedRoom == null || pawn.ownership.OwnedRoom != room)
									{
										return false;
									}
								}
								result2 = true;
							}
						}
					}
					return result2;
				}));
				Thing t;
				if (!JoyGiver_ViewArt.candidates.TryRandomElementByWeight((Thing target) => Mathf.Max(target.GetStatValue(StatDefOf.Beauty, true), 0.5f), out t))
				{
					result = null;
				}
				else
				{
					result = new Job(this.def.jobDef, t);
				}
			}
			finally
			{
				JoyGiver_ViewArt.candidates.Clear();
			}
			return result;
		}
	}
}
