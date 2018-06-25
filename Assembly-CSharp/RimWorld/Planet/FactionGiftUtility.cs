﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimWorld.Planet
{
	// Token: 0x0200055A RID: 1370
	[StaticConstructorOnStartup]
	public static class FactionGiftUtility
	{
		// Token: 0x04000F29 RID: 3881
		private static readonly Texture2D OfferGiftsCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/OfferGifts", true);

		// Token: 0x060019C5 RID: 6597 RVA: 0x000E08F0 File Offset: 0x000DECF0
		public static Command OfferGiftsCommand(Caravan caravan, Settlement settlement)
		{
			return new Command_Action
			{
				defaultLabel = "CommandOfferGifts".Translate(),
				defaultDesc = "CommandOfferGiftsDesc".Translate(),
				icon = FactionGiftUtility.OfferGiftsCommandTex,
				action = delegate()
				{
					Pawn playerNegotiator = BestCaravanPawnUtility.FindBestNegotiator(caravan);
					Find.WindowStack.Add(new Dialog_Trade(playerNegotiator, settlement, true));
				}
			};
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x000E0960 File Offset: 0x000DED60
		public static void GiveGift(List<Tradeable> tradeables, Faction giveTo, GlobalTargetInfo lookTarget)
		{
			int goodwillChange = FactionGiftUtility.GetGoodwillChange(tradeables, giveTo);
			for (int i = 0; i < tradeables.Count; i++)
			{
				if (tradeables[i].ActionToDo == TradeAction.PlayerSells)
				{
					tradeables[i].ResolveTrade();
				}
			}
			Faction ofPlayer = Faction.OfPlayer;
			int goodwillChange2 = goodwillChange;
			string reason = "GoodwillChangedReason_ReceivedGift".Translate();
			GlobalTargetInfo? lookTarget2 = new GlobalTargetInfo?(lookTarget);
			if (!giveTo.TryAffectGoodwillWith(ofPlayer, goodwillChange2, true, true, reason, lookTarget2))
			{
				FactionGiftUtility.SendGiftNotAppreciatedMessage(giveTo, lookTarget);
			}
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x000E09EC File Offset: 0x000DEDEC
		public static void GiveGift(List<ActiveDropPodInfo> pods, Settlement giveTo)
		{
			int goodwillChange = FactionGiftUtility.GetGoodwillChange(pods.Cast<IThingHolder>(), giveTo);
			for (int i = 0; i < pods.Count; i++)
			{
				ThingOwner innerContainer = pods[i].innerContainer;
				for (int j = innerContainer.Count - 1; j >= 0; j--)
				{
					FactionGiftUtility.GiveGiftInternal(innerContainer[j], innerContainer[j].stackCount, giveTo.Faction);
					if (j < innerContainer.Count)
					{
						innerContainer.RemoveAt(j);
					}
				}
			}
			Faction faction = giveTo.Faction;
			Faction ofPlayer = Faction.OfPlayer;
			int goodwillChange2 = goodwillChange;
			string reason = "GoodwillChangedReason_ReceivedGift".Translate();
			GlobalTargetInfo? lookTarget = new GlobalTargetInfo?(giveTo);
			if (!faction.TryAffectGoodwillWith(ofPlayer, goodwillChange2, true, true, reason, lookTarget))
			{
				FactionGiftUtility.SendGiftNotAppreciatedMessage(giveTo.Faction, giveTo);
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x000E0ACC File Offset: 0x000DEECC
		private static void GiveGiftInternal(Thing thing, int count, Faction giveTo)
		{
			Thing thing2 = thing.SplitOff(count);
			Pawn pawn = thing2 as Pawn;
			if (pawn != null)
			{
				pawn.SetFaction(giveTo, null);
			}
			thing2.DestroyOrPassToWorld(DestroyMode.Vanish);
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x000E0B00 File Offset: 0x000DEF00
		public static bool CheckCanCarryGift(List<Tradeable> tradeables, ITrader trader)
		{
			Pawn pawn = trader as Pawn;
			bool result;
			if (pawn == null)
			{
				result = true;
			}
			else
			{
				float num = 0f;
				float num2 = 0f;
				Lord lord = pawn.GetLord();
				if (lord != null)
				{
					for (int i = 0; i < lord.ownedPawns.Count; i++)
					{
						Pawn pawn2 = lord.ownedPawns[i];
						TraderCaravanRole traderCaravanRole = pawn2.GetTraderCaravanRole();
						if ((pawn2.RaceProps.Humanlike && traderCaravanRole != TraderCaravanRole.Guard) || traderCaravanRole == TraderCaravanRole.Carrier)
						{
							num += MassUtility.Capacity(pawn2, null);
							num2 += MassUtility.GearAndInventoryMass(pawn2);
						}
					}
				}
				else
				{
					num = MassUtility.Capacity(pawn, null);
					num2 = MassUtility.GearAndInventoryMass(pawn);
				}
				float num3 = 0f;
				for (int j = 0; j < tradeables.Count; j++)
				{
					if (tradeables[j].ActionToDo == TradeAction.PlayerSells)
					{
						int num4 = Mathf.Min(tradeables[j].CountToTransferToDestination, tradeables[j].CountHeldBy(Transactor.Colony));
						if (num4 > 0)
						{
							num3 += tradeables[j].AnyThing.GetStatValue(StatDefOf.Mass, true) * (float)num4;
						}
					}
				}
				if (num2 + num3 <= num)
				{
					result = true;
				}
				else
				{
					float num5 = num - num2;
					if (num5 <= 0f)
					{
						Messages.Message("MessageCantGiveGiftBecauseCantCarryEncumbered".Translate(), MessageTypeDefOf.RejectInput, false);
					}
					else
					{
						Messages.Message("MessageCantGiveGiftBecauseCantCarry".Translate(new object[]
						{
							num3.ToStringMass(),
							num5.ToStringMass()
						}), MessageTypeDefOf.RejectInput, false);
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x000E0CC4 File Offset: 0x000DF0C4
		public static int GetGoodwillChange(IEnumerable<IThingHolder> pods, Settlement giveTo)
		{
			float num = 0f;
			foreach (IThingHolder thingHolder in pods)
			{
				ThingOwner directlyHeldThings = thingHolder.GetDirectlyHeldThings();
				for (int i = 0; i < directlyHeldThings.Count; i++)
				{
					float priceFactorSell_TraderPriceType = (giveTo.TraderKind == null) ? 1f : giveTo.TraderKind.PriceTypeFor(directlyHeldThings[i].def, TradeAction.PlayerSells).PriceMultiplier();
					float tradePriceImprovementOffsetForPlayer = giveTo.TradePriceImprovementOffsetForPlayer;
					float pricePlayerSell = TradeUtility.GetPricePlayerSell(directlyHeldThings[i], priceFactorSell_TraderPriceType, 1f, tradePriceImprovementOffsetForPlayer);
					num += FactionGiftUtility.GetGoodwillChange(directlyHeldThings[i], directlyHeldThings[i].stackCount, pricePlayerSell, giveTo.Faction);
				}
			}
			return (int)num;
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x000E0DC4 File Offset: 0x000DF1C4
		public static int GetGoodwillChange(List<Tradeable> tradeables, Faction theirFaction)
		{
			float num = 0f;
			for (int i = 0; i < tradeables.Count; i++)
			{
				if (tradeables[i].ActionToDo == TradeAction.PlayerSells)
				{
					int count = Mathf.Min(tradeables[i].CountToTransferToDestination, tradeables[i].CountHeldBy(Transactor.Colony));
					num += FactionGiftUtility.GetGoodwillChange(tradeables[i].AnyThing, count, tradeables[i].GetPriceFor(TradeAction.PlayerSells), theirFaction);
				}
			}
			return (int)num;
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x000E0E54 File Offset: 0x000DF254
		private static float GetGoodwillChange(Thing anyThing, int count, float singlePrice, Faction theirFaction)
		{
			float result;
			if (count <= 0)
			{
				result = 0f;
			}
			else
			{
				float num = singlePrice * (float)count;
				Pawn pawn = anyThing as Pawn;
				if (pawn != null && pawn.IsPrisoner && pawn.Faction == theirFaction)
				{
					num *= 2f;
				}
				result = num / 25f;
			}
			return result;
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x000E0EB3 File Offset: 0x000DF2B3
		private static void SendGiftNotAppreciatedMessage(Faction giveTo, GlobalTargetInfo lookTarget)
		{
			Messages.Message("MessageGiftGivenButNotAppreciated".Translate(new object[]
			{
				giveTo.Name
			}).CapitalizeFirst(), lookTarget, MessageTypeDefOf.NegativeEvent, true);
		}
	}
}
