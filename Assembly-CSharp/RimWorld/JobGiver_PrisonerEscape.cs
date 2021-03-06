﻿using System;
using System.Runtime.CompilerServices;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_PrisonerEscape : ThinkNode_JobGiver
	{
		private const int MaxRegionsToCheckWhenEscapingThroughOpenDoors = 25;

		[CompilerGenerated]
		private static RegionEntryPredicate <>f__am$cache0;

		public JobGiver_PrisonerEscape()
		{
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			IntVec3 c;
			if (this.ShouldStartEscaping(pawn) && RCellFinder.TryFindBestExitSpot(pawn, out c, TraverseMode.ByPawn))
			{
				if (!pawn.guest.Released)
				{
					Messages.Message("MessagePrisonerIsEscaping".Translate(new object[]
					{
						pawn.LabelShort
					}), pawn, MessageTypeDefOf.ThreatSmall, true);
					Find.TickManager.slower.SignalForceNormalSpeed();
				}
				return new Job(JobDefOf.Goto, c)
				{
					exitMapOnArrival = true
				};
			}
			return null;
		}

		private bool ShouldStartEscaping(Pawn pawn)
		{
			if (!pawn.guest.IsPrisoner || pawn.guest.HostFaction != Faction.OfPlayer || !pawn.guest.PrisonerIsSecure)
			{
				return false;
			}
			Room room = pawn.GetRoom(RegionType.Set_Passable);
			if (room.TouchesMapEdge)
			{
				return true;
			}
			bool found = false;
			RegionTraverser.BreadthFirstTraverse(room.Regions[0], (Region from, Region reg) => reg.door == null || reg.door.FreePassage, delegate(Region reg)
			{
				if (reg.Room.TouchesMapEdge)
				{
					found = true;
					return true;
				}
				return false;
			}, 25, RegionType.Set_Passable);
			return found;
		}

		[CompilerGenerated]
		private static bool <ShouldStartEscaping>m__0(Region from, Region reg)
		{
			return reg.door == null || reg.door.FreePassage;
		}

		[CompilerGenerated]
		private sealed class <ShouldStartEscaping>c__AnonStorey0
		{
			internal bool found;

			public <ShouldStartEscaping>c__AnonStorey0()
			{
			}

			internal bool <>m__0(Region reg)
			{
				if (reg.Room.TouchesMapEdge)
				{
					this.found = true;
					return true;
				}
				return false;
			}
		}
	}
}
