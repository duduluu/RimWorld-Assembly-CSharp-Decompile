﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000605 RID: 1541
	public abstract class Settlement_TraderTracker : IThingHolder, IExposable
	{
		// Token: 0x06001EEB RID: 7915 RVA: 0x0010AE49 File Offset: 0x00109249
		public Settlement_TraderTracker(Settlement settlement)
		{
			this.settlement = settlement;
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06001EEC RID: 7916 RVA: 0x0010AE6C File Offset: 0x0010926C
		protected virtual int RegenerateStockEveryDays
		{
			get
			{
				return 30;
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06001EED RID: 7917 RVA: 0x0010AE84 File Offset: 0x00109284
		public IThingHolder ParentHolder
		{
			get
			{
				return this.settlement;
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06001EEE RID: 7918 RVA: 0x0010AEA0 File Offset: 0x001092A0
		public List<Thing> StockListForReading
		{
			get
			{
				if (this.stock == null)
				{
					this.RegenerateStock();
				}
				return this.stock.InnerListForReading;
			}
		}

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06001EEF RID: 7919
		public abstract TraderKindDef TraderKind { get; }

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001EF0 RID: 7920 RVA: 0x0010AED4 File Offset: 0x001092D4
		public int RandomPriceFactorSeed
		{
			get
			{
				return Gen.HashCombineInt(this.settlement.ID, 1933327354);
			}
		}

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001EF1 RID: 7921 RVA: 0x0010AF00 File Offset: 0x00109300
		public virtual string TraderName
		{
			get
			{
				string result;
				if (this.settlement.Faction == null)
				{
					result = this.settlement.LabelCap;
				}
				else
				{
					result = "SettlementTrader".Translate(new object[]
					{
						this.settlement.LabelCap,
						this.settlement.Faction.Name
					});
				}
				return result;
			}
		}

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06001EF2 RID: 7922 RVA: 0x0010AF68 File Offset: 0x00109368
		public virtual bool CanTradeNow
		{
			get
			{
				return this.TraderKind != null && (this.stock == null || this.stock.InnerListForReading.Any((Thing x) => this.TraderKind.WillTrade(x.def)));
			}
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06001EF3 RID: 7923 RVA: 0x0010AFB8 File Offset: 0x001093B8
		public virtual float TradePriceImprovementOffsetForPlayer
		{
			get
			{
				return 0.02f;
			}
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x0010AFD4 File Offset: 0x001093D4
		public virtual void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.tmpSavedPawns.Clear();
				if (this.stock != null)
				{
					for (int i = this.stock.Count - 1; i >= 0; i--)
					{
						Pawn pawn = this.stock[i] as Pawn;
						if (pawn != null)
						{
							this.stock.Remove(pawn);
							this.tmpSavedPawns.Add(pawn);
						}
					}
				}
			}
			Scribe_Collections.Look<Pawn>(ref this.tmpSavedPawns, "tmpSavedPawns", LookMode.Reference, new object[0]);
			Scribe_Deep.Look<ThingOwner<Thing>>(ref this.stock, "stock", new object[0]);
			Scribe_Values.Look<int>(ref this.lastStockGenerationTicks, "lastStockGenerationTicks", 0, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit || Scribe.mode == LoadSaveMode.Saving)
			{
				for (int j = 0; j < this.tmpSavedPawns.Count; j++)
				{
					this.stock.TryAdd(this.tmpSavedPawns[j], false);
				}
				this.tmpSavedPawns.Clear();
			}
		}

		// Token: 0x06001EF5 RID: 7925 RVA: 0x0010B0F4 File Offset: 0x001094F4
		public virtual IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
		{
			Caravan caravan = playerNegotiator.GetCaravan();
			foreach (Thing item in CaravanInventoryUtility.AllInventoryItems(caravan))
			{
				yield return item;
			}
			List<Pawn> pawns = caravan.PawnsListForReading;
			for (int i = 0; i < pawns.Count; i++)
			{
				if (!caravan.IsOwner(pawns[i]))
				{
					yield return pawns[i];
				}
			}
			yield break;
		}

		// Token: 0x06001EF6 RID: 7926 RVA: 0x0010B120 File Offset: 0x00109520
		public virtual void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			if (this.stock == null)
			{
				this.RegenerateStock();
			}
			Caravan caravan = playerNegotiator.GetCaravan();
			Thing thing = toGive.SplitOff(countToGive);
			thing.PreTraded(TradeAction.PlayerSells, playerNegotiator, this.settlement);
			Pawn pawn = toGive as Pawn;
			if (pawn != null)
			{
				CaravanInventoryUtility.MoveAllInventoryToSomeoneElse(pawn, caravan.PawnsListForReading, null);
				if (!pawn.RaceProps.Humanlike)
				{
					if (!this.stock.TryAdd(pawn, false))
					{
						pawn.Destroy(DestroyMode.Vanish);
					}
				}
			}
			else if (!this.stock.TryAdd(thing, false))
			{
				thing.Destroy(DestroyMode.Vanish);
			}
		}

		// Token: 0x06001EF7 RID: 7927 RVA: 0x0010B1C8 File Offset: 0x001095C8
		public virtual void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			Caravan caravan = playerNegotiator.GetCaravan();
			Thing thing = toGive.SplitOff(countToGive);
			thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this.settlement);
			Pawn pawn = thing as Pawn;
			if (pawn != null)
			{
				caravan.AddPawn(pawn, true);
			}
			else
			{
				Pawn pawn2 = CaravanInventoryUtility.FindPawnToMoveInventoryTo(thing, caravan.PawnsListForReading, null, null);
				if (pawn2 == null)
				{
					Log.Error("Could not find any pawn to give sold thing to.", false);
					thing.Destroy(DestroyMode.Vanish);
				}
				else if (!pawn2.inventory.innerContainer.TryAdd(thing, true))
				{
					Log.Error("Could not add sold thing to inventory.", false);
					thing.Destroy(DestroyMode.Vanish);
				}
			}
		}

		// Token: 0x06001EF8 RID: 7928 RVA: 0x0010B268 File Offset: 0x00109668
		public virtual void TraderTrackerTick()
		{
			if (this.stock != null)
			{
				if (Find.TickManager.TicksGame - this.lastStockGenerationTicks > this.RegenerateStockEveryDays * 60000)
				{
					this.TryDestroyStock();
				}
				else
				{
					for (int i = this.stock.Count - 1; i >= 0; i--)
					{
						Pawn pawn = this.stock[i] as Pawn;
						if (pawn != null && pawn.Destroyed)
						{
							this.stock.Remove(pawn);
						}
					}
					for (int j = this.stock.Count - 1; j >= 0; j--)
					{
						Pawn pawn2 = this.stock[j] as Pawn;
						if (pawn2 != null && !pawn2.IsWorldPawn())
						{
							Log.Error("Faction base has non-world-pawns in its stock. Removing...", false);
							this.stock.Remove(pawn2);
						}
					}
				}
			}
		}

		// Token: 0x06001EF9 RID: 7929 RVA: 0x0010B360 File Offset: 0x00109760
		public void TryDestroyStock()
		{
			if (this.stock != null)
			{
				for (int i = this.stock.Count - 1; i >= 0; i--)
				{
					Thing thing = this.stock[i];
					this.stock.Remove(thing);
					if (!(thing is Pawn) && !thing.Destroyed)
					{
						thing.Destroy(DestroyMode.Vanish);
					}
				}
				this.stock = null;
			}
		}

		// Token: 0x06001EFA RID: 7930 RVA: 0x0010B3DC File Offset: 0x001097DC
		public bool ContainsPawn(Pawn p)
		{
			return this.stock != null && this.stock.Contains(p);
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x0010B410 File Offset: 0x00109810
		protected virtual void RegenerateStock()
		{
			this.TryDestroyStock();
			this.stock = new ThingOwner<Thing>(this);
			if (this.settlement.Faction == null || !this.settlement.Faction.IsPlayer)
			{
				ThingSetMakerParams parms = default(ThingSetMakerParams);
				parms.traderDef = this.TraderKind;
				parms.tile = new int?(this.settlement.Tile);
				parms.traderFaction = this.settlement.Faction;
				this.stock.TryAddRangeOrTransfer(ThingSetMakerDefOf.TraderStock.root.Generate(parms), true, false);
			}
			for (int i = 0; i < this.stock.Count; i++)
			{
				Pawn pawn = this.stock[i] as Pawn;
				if (pawn != null)
				{
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
				}
			}
			this.lastStockGenerationTicks = Find.TickManager.TicksGame;
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x0010B504 File Offset: 0x00109904
		public ThingOwner GetDirectlyHeldThings()
		{
			return this.stock;
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x0010B51F File Offset: 0x0010991F
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		// Token: 0x0400122B RID: 4651
		public Settlement settlement;

		// Token: 0x0400122C RID: 4652
		private ThingOwner<Thing> stock;

		// Token: 0x0400122D RID: 4653
		private int lastStockGenerationTicks = -1;

		// Token: 0x0400122E RID: 4654
		private const float DefaultTradePriceImprovement = 0.02f;

		// Token: 0x0400122F RID: 4655
		private List<Pawn> tmpSavedPawns = new List<Pawn>();
	}
}
