﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld.Planet
{
	// Token: 0x020005DC RID: 1500
	[StaticConstructorOnStartup]
	public static class CaravanFormingUtility
	{
		// Token: 0x0400118D RID: 4493
		private static readonly Texture2D RemoveFromCaravanCommand = ContentFinder<Texture2D>.Get("UI/Commands/RemoveFromCaravan", true);

		// Token: 0x0400118E RID: 4494
		private static readonly Texture2D AddToCaravanCommand = ContentFinder<Texture2D>.Get("UI/Commands/AddToCaravan", true);

		// Token: 0x0400118F RID: 4495
		private static List<Thing> tmpReachableItems = new List<Thing>();

		// Token: 0x04001190 RID: 4496
		private static List<Pawn> tmpSendablePawns = new List<Pawn>();

		// Token: 0x04001191 RID: 4497
		private static List<ThingCount> tmpCaravanPawns = new List<ThingCount>();

		// Token: 0x06001D87 RID: 7559 RVA: 0x000FE87A File Offset: 0x000FCC7A
		public static void FormAndCreateCaravan(IEnumerable<Pawn> pawns, Faction faction, int exitFromTile, int directionTile, int destinationTile)
		{
			CaravanExitMapUtility.ExitMapAndCreateCaravan(pawns, faction, exitFromTile, directionTile, destinationTile, true);
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x000FE88C File Offset: 0x000FCC8C
		public static void StartFormingCaravan(List<Pawn> pawns, Faction faction, List<TransferableOneWay> transferables, IntVec3 meetingPoint, IntVec3 exitSpot, int startingTile, int destinationTile)
		{
			if (startingTile < 0)
			{
				Log.Error("Can't start forming caravan because startingTile is invalid.", false);
			}
			else if (!pawns.Any<Pawn>())
			{
				Log.Error("Can't start forming caravan with 0 pawns.", false);
			}
			else
			{
				if (pawns.Any((Pawn x) => x.Downed))
				{
					Log.Warning("Forming a caravan with a downed pawn. This shouldn't happen because we have to create a Lord.", false);
				}
				List<TransferableOneWay> list = transferables.ToList<TransferableOneWay>();
				list.RemoveAll((TransferableOneWay x) => x.CountToTransfer <= 0 || !x.HasAnyThing || x.AnyThing is Pawn);
				for (int i = 0; i < pawns.Count; i++)
				{
					Lord lord = pawns[i].GetLord();
					if (lord != null)
					{
						lord.Notify_PawnLost(pawns[i], PawnLostCondition.ForcedToJoinOtherLord);
					}
				}
				LordJob_FormAndSendCaravan lordJob = new LordJob_FormAndSendCaravan(list, meetingPoint, exitSpot, startingTile, destinationTile);
				LordMaker.MakeNewLord(Faction.OfPlayer, lordJob, pawns[0].MapHeld, pawns);
				for (int j = 0; j < pawns.Count; j++)
				{
					Pawn pawn = pawns[j];
					if (pawn.Spawned)
					{
						pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
					}
				}
			}
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x000FE9CF File Offset: 0x000FCDCF
		public static void StopFormingCaravan(Lord lord)
		{
			CaravanFormingUtility.SetToUnloadEverything(lord);
			lord.lordManager.RemoveLord(lord);
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x000FE9E4 File Offset: 0x000FCDE4
		public static void RemovePawnFromCaravan(Pawn pawn, Lord lord)
		{
			bool flag = false;
			for (int i = 0; i < lord.ownedPawns.Count; i++)
			{
				Pawn pawn2 = lord.ownedPawns[i];
				if (pawn2 != pawn && CaravanUtility.IsOwner(pawn2, Faction.OfPlayer))
				{
					flag = true;
					break;
				}
			}
			bool flag2 = true;
			string text = "MessagePawnLostWhileFormingCaravan".Translate(new object[]
			{
				pawn.LabelIndefinite()
			}).CapitalizeFirst();
			if (!flag)
			{
				CaravanFormingUtility.StopFormingCaravan(lord);
				text = text + " " + "MessagePawnLostWhileFormingCaravan_AllLost".Translate();
			}
			else
			{
				pawn.inventory.UnloadEverything = true;
				if (lord.ownedPawns.Contains(pawn))
				{
					lord.Notify_PawnLost(pawn, PawnLostCondition.ForcedByPlayerAction);
					flag2 = false;
				}
			}
			if (flag2)
			{
				Messages.Message(text, pawn, MessageTypeDefOf.NegativeEvent, true);
			}
		}

		// Token: 0x06001D8B RID: 7563 RVA: 0x000FEAD0 File Offset: 0x000FCED0
		public static void Notify_FormAndSendCaravanLordFailed(Lord lord)
		{
			CaravanFormingUtility.SetToUnloadEverything(lord);
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x000FEADC File Offset: 0x000FCEDC
		private static void SetToUnloadEverything(Lord lord)
		{
			for (int i = 0; i < lord.ownedPawns.Count; i++)
			{
				lord.ownedPawns[i].inventory.UnloadEverything = true;
			}
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x000FEB20 File Offset: 0x000FCF20
		public static List<Thing> AllReachableColonyItems(Map map, bool allowEvenIfOutsideHomeArea = false, bool allowEvenIfReserved = false, bool canMinify = false)
		{
			CaravanFormingUtility.tmpReachableItems.Clear();
			List<Thing> allThings = map.listerThings.AllThings;
			for (int i = 0; i < allThings.Count; i++)
			{
				Thing thing = allThings[i];
				bool flag = canMinify && thing.def.Minifiable;
				if ((flag || thing.def.category == ThingCategory.Item) && (allowEvenIfOutsideHomeArea || map.areaManager.Home[thing.Position] || thing.IsInAnyStorage()) && (!thing.Position.Fogged(thing.Map) && (allowEvenIfReserved || !map.reservationManager.IsReservedByAnyoneOf(thing, Faction.OfPlayer))) && (flag || thing.def.EverHaulable))
				{
					CaravanFormingUtility.tmpReachableItems.Add(thing);
				}
			}
			return CaravanFormingUtility.tmpReachableItems;
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x000FEC2C File Offset: 0x000FD02C
		public static List<Pawn> AllSendablePawns(Map map, bool allowEvenIfDownedOrInMentalState = false, bool allowEvenIfPrisonerNotSecure = false, bool allowCapturableDownedPawns = false)
		{
			CaravanFormingUtility.tmpSendablePawns.Clear();
			List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				Pawn pawn = allPawnsSpawned[i];
				if (allowEvenIfDownedOrInMentalState || (!pawn.Downed && !pawn.InMentalState))
				{
					if (pawn.Faction == Faction.OfPlayer || pawn.IsPrisonerOfColony || (allowCapturableDownedPawns && pawn.Downed && CaravanUtility.ShouldAutoCapture(pawn, Faction.OfPlayer)))
					{
						if ((allowEvenIfPrisonerNotSecure || !pawn.IsPrisoner || pawn.guest.PrisonerIsSecure) && (pawn.GetLord() == null || pawn.GetLord().LordJob is LordJob_VoluntarilyJoinable))
						{
							CaravanFormingUtility.tmpSendablePawns.Add(pawn);
						}
					}
				}
			}
			return CaravanFormingUtility.tmpSendablePawns;
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x000FED34 File Offset: 0x000FD134
		public static IEnumerable<Gizmo> GetGizmos(Pawn pawn)
		{
			if (pawn.IsFormingCaravan())
			{
				Lord lord = pawn.GetLord();
				yield return new Command_Action
				{
					defaultLabel = "CommandCancelFormingCaravan".Translate(),
					defaultDesc = "CommandCancelFormingCaravanDesc".Translate(),
					icon = TexCommand.ClearPrioritizedWork,
					activateSound = SoundDefOf.Tick_Low,
					action = delegate()
					{
						CaravanFormingUtility.StopFormingCaravan(lord);
					},
					hotKey = KeyBindingDefOf.Designator_Cancel
				};
				yield return new Command_Action
				{
					defaultLabel = "CommandRemoveFromCaravan".Translate(),
					defaultDesc = "CommandRemoveFromCaravanDesc".Translate(),
					icon = CaravanFormingUtility.RemoveFromCaravanCommand,
					action = delegate()
					{
						CaravanFormingUtility.RemovePawnFromCaravan(pawn, lord);
					},
					hotKey = KeyBindingDefOf.Misc6
				};
			}
			else if (Dialog_FormCaravan.AllSendablePawns(pawn.Map, false).Contains(pawn))
			{
				bool anyCaravanToJoin = false;
				for (int i = 0; i < pawn.Map.lordManager.lords.Count; i++)
				{
					Lord lord2 = pawn.Map.lordManager.lords[i];
					if (lord2.faction == Faction.OfPlayer && lord2.LordJob is LordJob_FormAndSendCaravan)
					{
						anyCaravanToJoin = true;
						break;
					}
				}
				if (anyCaravanToJoin)
				{
					yield return new Command_Action
					{
						defaultLabel = "CommandAddToCaravan".Translate(),
						defaultDesc = "CommandAddToCaravanDesc".Translate(),
						icon = CaravanFormingUtility.AddToCaravanCommand,
						action = delegate()
						{
							List<Lord> list = new List<Lord>();
							for (int j = 0; j < pawn.Map.lordManager.lords.Count; j++)
							{
								Lord lord3 = pawn.Map.lordManager.lords[j];
								if (lord3.faction == Faction.OfPlayer && lord3.LordJob is LordJob_FormAndSendCaravan)
								{
									list.Add(lord3);
								}
							}
							if (list.Count != 0)
							{
								if (list.Count == 1)
								{
									CaravanFormingUtility.LateJoinFormingCaravan(pawn, list[0]);
									SoundDefOf.Click.PlayOneShotOnCamera(null);
								}
								else
								{
									List<FloatMenuOption> list2 = new List<FloatMenuOption>();
									for (int k = 0; k < list.Count; k++)
									{
										Lord caravanLocal = list[k];
										string label = "Caravan".Translate() + " " + (k + 1);
										list2.Add(new FloatMenuOption(label, delegate()
										{
											if (pawn.Spawned && pawn.Map.lordManager.lords.Contains(caravanLocal) && Dialog_FormCaravan.AllSendablePawns(pawn.Map, false).Contains(pawn))
											{
												CaravanFormingUtility.LateJoinFormingCaravan(pawn, caravanLocal);
											}
										}, MenuOptionPriority.Default, null, null, 0f, null, null));
									}
									Find.WindowStack.Add(new FloatMenu(list2));
								}
							}
						},
						hotKey = KeyBindingDefOf.Misc7
					};
				}
			}
			yield break;
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x000FED60 File Offset: 0x000FD160
		private static void LateJoinFormingCaravan(Pawn pawn, Lord lord)
		{
			Lord lord2 = pawn.GetLord();
			if (lord2 != null)
			{
				lord2.Notify_PawnLost(pawn, PawnLostCondition.ForcedToJoinOtherLord);
			}
			lord.AddPawn(pawn);
			if (pawn.Spawned)
			{
				pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
			}
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x000FEDA4 File Offset: 0x000FD1A4
		public static bool IsFormingCaravan(this Pawn p)
		{
			Lord lord = p.GetLord();
			return lord != null && lord.LordJob is LordJob_FormAndSendCaravan;
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x000FEDD8 File Offset: 0x000FD1D8
		public static float CapacityLeft(LordJob_FormAndSendCaravan lordJob)
		{
			float num = CollectionsMassCalculator.MassUsageTransferables(lordJob.transferables, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, false, false);
			CaravanFormingUtility.tmpCaravanPawns.Clear();
			for (int i = 0; i < lordJob.lord.ownedPawns.Count; i++)
			{
				Pawn pawn = lordJob.lord.ownedPawns[i];
				CaravanFormingUtility.tmpCaravanPawns.Add(new ThingCount(pawn, pawn.stackCount));
			}
			num += CollectionsMassCalculator.MassUsage(CaravanFormingUtility.tmpCaravanPawns, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, false, false);
			float num2 = CollectionsMassCalculator.Capacity(CaravanFormingUtility.tmpCaravanPawns, null);
			CaravanFormingUtility.tmpCaravanPawns.Clear();
			return num2 - num;
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x000FEE7C File Offset: 0x000FD27C
		public static string AppendOverweightInfo(string text, float capacityLeft)
		{
			if (capacityLeft < 0f)
			{
				text = text + " (" + "OverweightLower".Translate() + ")";
			}
			return text;
		}
	}
}
