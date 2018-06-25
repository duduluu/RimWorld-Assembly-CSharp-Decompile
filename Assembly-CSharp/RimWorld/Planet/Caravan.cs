﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005D4 RID: 1492
	[StaticConstructorOnStartup]
	public class Caravan : WorldObject, IThingHolder, IIncidentTarget, ITrader, ILoadReferenceable
	{
		// Token: 0x0400115E RID: 4446
		private string nameInt;

		// Token: 0x0400115F RID: 4447
		public ThingOwner<Pawn> pawns;

		// Token: 0x04001160 RID: 4448
		public bool autoJoinable;

		// Token: 0x04001161 RID: 4449
		public Caravan_PathFollower pather;

		// Token: 0x04001162 RID: 4450
		public Caravan_GotoMoteRenderer gotoMote;

		// Token: 0x04001163 RID: 4451
		public Caravan_Tweener tweener;

		// Token: 0x04001164 RID: 4452
		public Caravan_TraderTracker trader;

		// Token: 0x04001165 RID: 4453
		public Caravan_ForageTracker forage;

		// Token: 0x04001166 RID: 4454
		public StoryState storyState;

		// Token: 0x04001167 RID: 4455
		private Material cachedMat;

		// Token: 0x04001168 RID: 4456
		private bool cachedImmobilized;

		// Token: 0x04001169 RID: 4457
		private int cachedImmobilizedForTicks = -99999;

		// Token: 0x0400116A RID: 4458
		private Pair<float, float> cachedDaysWorthOfFood;

		// Token: 0x0400116B RID: 4459
		private int cachedDaysWorthOfFoodForTicks = -99999;

		// Token: 0x0400116C RID: 4460
		public bool notifiedOutOfFood;

		// Token: 0x0400116D RID: 4461
		private const int ImmobilizedCacheDuration = 60;

		// Token: 0x0400116E RID: 4462
		private const int DaysWorthOfFoodCacheDuration = 3000;

		// Token: 0x0400116F RID: 4463
		private const int TendIntervalTicks = 2000;

		// Token: 0x04001170 RID: 4464
		private const int TryTakeScheduledDrugsIntervalTicks = 120;

		// Token: 0x04001171 RID: 4465
		private static readonly Texture2D SplitCommand = ContentFinder<Texture2D>.Get("UI/Commands/SplitCaravan", true);

		// Token: 0x04001172 RID: 4466
		private static readonly Color PlayerCaravanColor = new Color(1f, 0.863f, 0.33f);

		// Token: 0x06001D12 RID: 7442 RVA: 0x000FA270 File Offset: 0x000F8670
		public Caravan()
		{
			this.pawns = new ThingOwner<Pawn>(this, false, LookMode.Reference);
			this.pather = new Caravan_PathFollower(this);
			this.gotoMote = new Caravan_GotoMoteRenderer();
			this.tweener = new Caravan_Tweener(this);
			this.trader = new Caravan_TraderTracker(this);
			this.forage = new Caravan_ForageTracker(this);
			this.storyState = new StoryState(this);
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001D13 RID: 7443 RVA: 0x000FA2F0 File Offset: 0x000F86F0
		public List<Pawn> PawnsListForReading
		{
			get
			{
				return this.pawns.InnerListForReading;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001D14 RID: 7444 RVA: 0x000FA310 File Offset: 0x000F8710
		public override Material Material
		{
			get
			{
				if (this.cachedMat == null)
				{
					Color color;
					if (base.Faction == null)
					{
						color = Color.white;
					}
					else if (base.Faction.IsPlayer)
					{
						color = Caravan.PlayerCaravanColor;
					}
					else
					{
						color = base.Faction.Color;
					}
					this.cachedMat = MaterialPool.MatFrom(this.def.texture, ShaderDatabase.WorldOverlayTransparentLit, color, WorldMaterials.DynamicObjectRenderQueue);
				}
				return this.cachedMat;
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001D15 RID: 7445 RVA: 0x000FA39C File Offset: 0x000F879C
		// (set) Token: 0x06001D16 RID: 7446 RVA: 0x000FA3B7 File Offset: 0x000F87B7
		public string Name
		{
			get
			{
				return this.nameInt;
			}
			set
			{
				this.nameInt = value;
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06001D17 RID: 7447 RVA: 0x000FA3C4 File Offset: 0x000F87C4
		public override Vector3 DrawPos
		{
			get
			{
				return this.tweener.TweenedPos;
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001D18 RID: 7448 RVA: 0x000FA3E4 File Offset: 0x000F87E4
		public bool IsPlayerControlled
		{
			get
			{
				return base.Faction == Faction.OfPlayer;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06001D19 RID: 7449 RVA: 0x000FA408 File Offset: 0x000F8808
		public bool ImmobilizedByMass
		{
			get
			{
				bool result;
				if (Find.TickManager.TicksGame - this.cachedImmobilizedForTicks < 60)
				{
					result = this.cachedImmobilized;
				}
				else
				{
					this.cachedImmobilized = (this.MassUsage > this.MassCapacity);
					this.cachedImmobilizedForTicks = Find.TickManager.TicksGame;
					result = this.cachedImmobilized;
				}
				return result;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06001D1A RID: 7450 RVA: 0x000FA46C File Offset: 0x000F886C
		public Pair<float, float> DaysWorthOfFood
		{
			get
			{
				Pair<float, float> result;
				if (Find.TickManager.TicksGame - this.cachedDaysWorthOfFoodForTicks < 3000)
				{
					result = this.cachedDaysWorthOfFood;
				}
				else
				{
					this.cachedDaysWorthOfFood = new Pair<float, float>(DaysWorthOfFoodCalculator.ApproxDaysWorthOfFood(this), DaysUntilRotCalculator.ApproxDaysUntilRot(this));
					this.cachedDaysWorthOfFoodForTicks = Find.TickManager.TicksGame;
					result = this.cachedDaysWorthOfFood;
				}
				return result;
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001D1B RID: 7451 RVA: 0x000FA4D8 File Offset: 0x000F88D8
		public bool CantMove
		{
			get
			{
				return this.Resting || this.AllOwnersHaveMentalBreak || this.AllOwnersDowned || this.ImmobilizedByMass;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06001D1C RID: 7452 RVA: 0x000FA518 File Offset: 0x000F8918
		public float MassCapacity
		{
			get
			{
				return CollectionsMassCalculator.Capacity<Pawn>(this.PawnsListForReading, null);
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001D1D RID: 7453 RVA: 0x000FA53C File Offset: 0x000F893C
		public string MassCapacityExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				CollectionsMassCalculator.Capacity<Pawn>(this.PawnsListForReading, stringBuilder);
				return stringBuilder.ToString();
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06001D1E RID: 7454 RVA: 0x000FA56C File Offset: 0x000F896C
		public float MassUsage
		{
			get
			{
				return CollectionsMassCalculator.MassUsage<Pawn>(this.PawnsListForReading, IgnorePawnsInventoryMode.DontIgnore, false, false);
			}
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001D1F RID: 7455 RVA: 0x000FA590 File Offset: 0x000F8990
		public bool AllOwnersDowned
		{
			get
			{
				for (int i = 0; i < this.pawns.Count; i++)
				{
					if (this.IsOwner(this.pawns[i]) && !this.pawns[i].Downed)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001D20 RID: 7456 RVA: 0x000FA5F8 File Offset: 0x000F89F8
		public bool AllOwnersHaveMentalBreak
		{
			get
			{
				for (int i = 0; i < this.pawns.Count; i++)
				{
					if (this.IsOwner(this.pawns[i]) && !this.pawns[i].InMentalState)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06001D21 RID: 7457 RVA: 0x000FA660 File Offset: 0x000F8A60
		public bool Resting
		{
			get
			{
				return (!this.pather.Moving || this.pather.nextTile != this.pather.Destination || !Caravan_PathFollower.IsValidFinalPushDestination(this.pather.Destination) || Mathf.CeilToInt(this.pather.nextTileCostLeft / 1f) > 10000) && CaravanRestUtility.RestingNowAt(base.Tile);
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06001D22 RID: 7458 RVA: 0x000FA6E8 File Offset: 0x000F8AE8
		public int LeftRestTicks
		{
			get
			{
				int result;
				if (!this.Resting)
				{
					result = 0;
				}
				else
				{
					result = CaravanRestUtility.LeftRestTicksAt(base.Tile);
				}
				return result;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001D23 RID: 7459 RVA: 0x000FA71C File Offset: 0x000F8B1C
		public int LeftNonRestTicks
		{
			get
			{
				int result;
				if (this.Resting)
				{
					result = 0;
				}
				else
				{
					result = CaravanRestUtility.LeftNonRestTicksAt(base.Tile);
				}
				return result;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001D24 RID: 7460 RVA: 0x000FA750 File Offset: 0x000F8B50
		public override string Label
		{
			get
			{
				string label;
				if (this.nameInt != null)
				{
					label = this.nameInt;
				}
				else
				{
					label = base.Label;
				}
				return label;
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001D25 RID: 7461 RVA: 0x000FA784 File Offset: 0x000F8B84
		public int TicksPerMove
		{
			get
			{
				return CaravanTicksPerMoveUtility.GetTicksPerMove(this, null);
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001D26 RID: 7462 RVA: 0x000FA7A0 File Offset: 0x000F8BA0
		public override bool AppendFactionToInspectString
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001D27 RID: 7463 RVA: 0x000FA7B8 File Offset: 0x000F8BB8
		public float Visibility
		{
			get
			{
				return CaravanVisibilityCalculator.Visibility(this, null);
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001D28 RID: 7464 RVA: 0x000FA7D4 File Offset: 0x000F8BD4
		public string VisibilityExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				CaravanVisibilityCalculator.Visibility(this, stringBuilder);
				return stringBuilder.ToString();
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001D29 RID: 7465 RVA: 0x000FA800 File Offset: 0x000F8C00
		public string TicksPerMoveExplanation
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				CaravanTicksPerMoveUtility.GetTicksPerMove(this, stringBuilder);
				return stringBuilder.ToString();
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001D2A RID: 7466 RVA: 0x000FA82C File Offset: 0x000F8C2C
		public StoryState StoryState
		{
			get
			{
				return this.storyState;
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001D2B RID: 7467 RVA: 0x000FA848 File Offset: 0x000F8C48
		public GameConditionManager GameConditionManager
		{
			get
			{
				Log.ErrorOnce("Attempted to retrieve condition manager directly from caravan", 13291050, false);
				return null;
			}
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001D2C RID: 7468 RVA: 0x000FA870 File Offset: 0x000F8C70
		public float PlayerWealthForStoryteller
		{
			get
			{
				float result;
				if (!this.IsPlayerControlled)
				{
					result = 0f;
				}
				else
				{
					float num = 0f;
					for (int i = 0; i < this.pawns.Count; i++)
					{
						num += WealthWatcher.GetEquipmentApparelAndInventoryWealth(this.pawns[i]);
						if (this.pawns[i].RaceProps.Animal && this.pawns[i].Faction == Faction.OfPlayer)
						{
							num += this.pawns[i].MarketValue;
						}
					}
					result = num * 0.5f;
				}
				return result;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001D2D RID: 7469 RVA: 0x000FA924 File Offset: 0x000F8D24
		public IEnumerable<Pawn> PlayerPawnsForStoryteller
		{
			get
			{
				IEnumerable<Pawn> result;
				if (!this.IsPlayerControlled)
				{
					result = Enumerable.Empty<Pawn>();
				}
				else
				{
					result = from x in this.PawnsListForReading
					where x.Faction == Faction.OfPlayer
					select x;
				}
				return result;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001D2E RID: 7470 RVA: 0x000FA978 File Offset: 0x000F8D78
		public FloatRange IncidentPointsRandomFactorRange
		{
			get
			{
				return StorytellerUtility.CaravanPointsRandomFactorRange;
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001D2F RID: 7471 RVA: 0x000FA994 File Offset: 0x000F8D94
		public IEnumerable<Thing> AllThings
		{
			get
			{
				return CaravanInventoryUtility.AllInventoryItems(this).Concat(this.pawns);
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001D30 RID: 7472 RVA: 0x000FA9BC File Offset: 0x000F8DBC
		public TraderKindDef TraderKind
		{
			get
			{
				return this.trader.TraderKind;
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001D31 RID: 7473 RVA: 0x000FA9DC File Offset: 0x000F8DDC
		public IEnumerable<Thing> Goods
		{
			get
			{
				return this.trader.Goods;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001D32 RID: 7474 RVA: 0x000FA9FC File Offset: 0x000F8DFC
		public int RandomPriceFactorSeed
		{
			get
			{
				return this.trader.RandomPriceFactorSeed;
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001D33 RID: 7475 RVA: 0x000FAA1C File Offset: 0x000F8E1C
		public string TraderName
		{
			get
			{
				return this.trader.TraderName;
			}
		}

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001D34 RID: 7476 RVA: 0x000FAA3C File Offset: 0x000F8E3C
		public bool CanTradeNow
		{
			get
			{
				return this.trader.CanTradeNow;
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001D35 RID: 7477 RVA: 0x000FAA5C File Offset: 0x000F8E5C
		public float TradePriceImprovementOffsetForPlayer
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x000FAA78 File Offset: 0x000F8E78
		public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
		{
			return this.trader.ColonyThingsWillingToBuy(playerNegotiator);
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x000FAA99 File Offset: 0x000F8E99
		public void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			this.trader.GiveSoldThingToTrader(toGive, countToGive, playerNegotiator);
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x000FAAAA File Offset: 0x000F8EAA
		public void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			this.trader.GiveSoldThingToPlayer(toGive, countToGive, playerNegotiator);
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x000FAABC File Offset: 0x000F8EBC
		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.pawns.RemoveAll((Pawn x) => x.Destroyed);
			}
			Scribe_Values.Look<string>(ref this.nameInt, "name", null, false);
			Scribe_Deep.Look<ThingOwner<Pawn>>(ref this.pawns, "pawns", new object[]
			{
				this
			});
			Scribe_Values.Look<bool>(ref this.autoJoinable, "autoJoinable", false, false);
			Scribe_Deep.Look<Caravan_PathFollower>(ref this.pather, "pather", new object[]
			{
				this
			});
			Scribe_Deep.Look<Caravan_TraderTracker>(ref this.trader, "trader", new object[]
			{
				this
			});
			Scribe_Deep.Look<Caravan_ForageTracker>(ref this.forage, "forage", new object[]
			{
				this
			});
			Scribe_Deep.Look<StoryState>(ref this.storyState, "storyState", new object[]
			{
				this
			});
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				BackCompatibility.CaravanPostLoadInit(this);
			}
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x000FABBB File Offset: 0x000F8FBB
		public override void PostAdd()
		{
			base.PostAdd();
			Find.ColonistBar.MarkColonistsDirty();
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x000FABCE File Offset: 0x000F8FCE
		public override void PostRemove()
		{
			base.PostRemove();
			this.pather.StopDead();
			Find.ColonistBar.MarkColonistsDirty();
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x000FABEC File Offset: 0x000F8FEC
		public override void Tick()
		{
			base.Tick();
			this.CheckAnyNonWorldPawns();
			this.pather.PatherTick();
			this.tweener.TweenerTick();
			this.forage.ForageTrackerTick();
			CaravanPawnsNeedsUtility.TrySatisfyPawnsNeeds(this);
			if (this.IsHashIntervalTick(120))
			{
				CaravanDrugPolicyUtility.TryTakeScheduledDrugs(this);
			}
			if (this.IsHashIntervalTick(2000))
			{
				CaravanTendUtility.TryTendToRandomPawn(this);
			}
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x000FAC56 File Offset: 0x000F9056
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.tweener.ResetTweenedPosToRoot();
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x000FAC6A File Offset: 0x000F906A
		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			if (this.IsPlayerControlled && this.pather.curPath != null)
			{
				this.pather.curPath.DrawPath(this);
			}
			this.gotoMote.RenderMote();
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x000FACAC File Offset: 0x000F90AC
		public void AddPawn(Pawn p, bool addCarriedPawnToWorldPawnsIfAny)
		{
			if (p == null)
			{
				Log.Warning("Tried to add a null pawn to " + this, false);
			}
			else if (p.Dead)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to add ",
					p,
					" to ",
					this,
					", but this pawn is dead."
				}), false);
			}
			else
			{
				Pawn pawn = p.carryTracker.CarriedThing as Pawn;
				if (p.Spawned)
				{
					p.DeSpawn(DestroyMode.Vanish);
				}
				if (this.pawns.TryAdd(p, true))
				{
					if (this.ShouldAutoCapture(p))
					{
						p.guest.CapturedBy(base.Faction, null);
					}
					if (pawn != null)
					{
						p.carryTracker.innerContainer.Remove(pawn);
						if (this.ShouldAutoCapture(pawn))
						{
							pawn.guest.CapturedBy(base.Faction, p);
						}
						this.AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny);
						if (addCarriedPawnToWorldPawnsIfAny)
						{
							Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
						}
					}
				}
				else
				{
					Log.Error("Couldn't add pawn " + p + " to caravan.", false);
				}
			}
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x000FADDC File Offset: 0x000F91DC
		public void AddPawnOrItem(Thing thing, bool addCarriedPawnToWorldPawnsIfAny)
		{
			if (thing == null)
			{
				Log.Warning("Tried to add a null thing to " + this, false);
			}
			else
			{
				Pawn pawn = thing as Pawn;
				if (pawn != null)
				{
					this.AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny);
				}
				else
				{
					CaravanInventoryUtility.GiveThing(this, thing);
				}
			}
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x000FAE28 File Offset: 0x000F9228
		public bool ContainsPawn(Pawn p)
		{
			return this.pawns.Contains(p);
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x000FAE49 File Offset: 0x000F9249
		public void RemovePawn(Pawn p)
		{
			this.pawns.Remove(p);
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x000FAE59 File Offset: 0x000F9259
		public void RemoveAllPawns()
		{
			this.pawns.Clear();
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x000FAE68 File Offset: 0x000F9268
		public bool IsOwner(Pawn p)
		{
			return this.pawns.Contains(p) && CaravanUtility.IsOwner(p, base.Faction);
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x000FAEA0 File Offset: 0x000F92A0
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < this.pawns.Count; i++)
			{
				if (this.pawns[i].IsColonist)
				{
					num++;
				}
				else if (this.pawns[i].RaceProps.Animal)
				{
					num2++;
				}
				else if (this.pawns[i].IsPrisoner)
				{
					num3++;
				}
				if (this.pawns[i].Downed)
				{
					num4++;
				}
				if (this.pawns[i].InMentalState)
				{
					num5++;
				}
			}
			stringBuilder.Append("CaravanColonistsCount".Translate(new object[]
			{
				num,
				(num != 1) ? Faction.OfPlayer.def.pawnsPlural : Faction.OfPlayer.def.pawnSingular
			}));
			if (num2 == 1)
			{
				stringBuilder.Append(", " + "CaravanAnimal".Translate());
			}
			else if (num2 > 1)
			{
				stringBuilder.Append(", " + "CaravanAnimalsCount".Translate(new object[]
				{
					num2
				}));
			}
			if (num3 == 1)
			{
				stringBuilder.Append(", " + "CaravanPrisoner".Translate());
			}
			else if (num3 > 1)
			{
				stringBuilder.Append(", " + "CaravanPrisonersCount".Translate(new object[]
				{
					num3
				}));
			}
			stringBuilder.AppendLine();
			if (num5 > 0)
			{
				stringBuilder.Append("CaravanPawnsInMentalState".Translate(new object[]
				{
					num5
				}));
			}
			if (num4 > 0)
			{
				if (num5 > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("CaravanPawnsDowned".Translate(new object[]
				{
					num4
				}));
			}
			if (num5 > 0 || num4 > 0)
			{
				stringBuilder.AppendLine();
			}
			if (this.pather.Moving)
			{
				if (this.pather.ArrivalAction != null)
				{
					stringBuilder.Append(this.pather.ArrivalAction.ReportString);
				}
				else
				{
					stringBuilder.Append("CaravanTraveling".Translate());
				}
			}
			else
			{
				Settlement settlement = CaravanVisitUtility.SettlementVisitedNow(this);
				if (settlement != null)
				{
					stringBuilder.Append("CaravanVisiting".Translate(new object[]
					{
						settlement.Label
					}));
				}
				else
				{
					stringBuilder.Append("CaravanWaiting".Translate());
				}
			}
			if (this.pather.Moving)
			{
				float num6 = (float)CaravanArrivalTimeEstimator.EstimatedTicksToArrive(this, true) / 60000f;
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanEstimatedTimeToDestination".Translate(new object[]
				{
					num6.ToString("0.#")
				}));
			}
			if (this.AllOwnersDowned)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("AllCaravanMembersDowned".Translate());
			}
			else if (this.AllOwnersHaveMentalBreak)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("AllCaravanMembersMentalBreak".Translate());
			}
			else if (this.ImmobilizedByMass)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanImmobilizedByMass".Translate());
			}
			string text;
			if (CaravanPawnsNeedsUtility.AnyPawnOutOfFood(this, out text))
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanOutOfFood".Translate());
				if (!text.NullOrEmpty())
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(text);
					stringBuilder.Append(".");
				}
			}
			if (this.Resting)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanResting".Translate());
			}
			if (this.pather.Paused)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("CaravanPaused".Translate());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x000FB338 File Offset: 0x000F9738
		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.WorldSelector.SingleSelectedObject == this)
			{
				yield return new Gizmo_CaravanInfo(this);
			}
			foreach (Gizmo g in this.<GetGizmos>__BaseCallProxy0())
			{
				yield return g;
			}
			if (this.IsPlayerControlled)
			{
				if (Find.WorldSelector.SingleSelectedObject == this)
				{
					yield return SettleInEmptyTileUtility.SettleCommand(this);
				}
				if (Find.WorldSelector.SingleSelectedObject == this)
				{
					if (this.PawnsListForReading.Count((Pawn x) => x.IsColonist) >= 2)
					{
						yield return new Command_Action
						{
							defaultLabel = "CommandSplitCaravan".Translate(),
							defaultDesc = "CommandSplitCaravanDesc".Translate(),
							icon = Caravan.SplitCommand,
							action = delegate()
							{
								Find.WindowStack.Add(new Dialog_SplitCaravan(this));
							}
						};
					}
				}
				if (this.pather.Moving)
				{
					yield return new Command_Toggle
					{
						hotKey = KeyBindingDefOf.Misc1,
						isActive = (() => this.pather.Paused),
						toggleAction = delegate()
						{
							if (this.pather.Moving)
							{
								this.pather.Paused = !this.pather.Paused;
							}
						},
						defaultDesc = "CommandToggleCaravanPauseDesc".Translate(new object[]
						{
							2f.ToString("0.#"),
							0.3f.ToStringPercent()
						}),
						icon = TexCommand.PauseCaravan,
						defaultLabel = "CommandPauseCaravan".Translate()
					};
				}
				if (CaravanMergeUtility.ShouldShowMergeCommand)
				{
					yield return CaravanMergeUtility.MergeCommand(this);
				}
				foreach (Gizmo g2 in this.forage.GetGizmos())
				{
					yield return g2;
				}
				foreach (WorldObject wo in Find.WorldObjects.ObjectsAt(base.Tile))
				{
					foreach (Gizmo gizmo in wo.GetCaravanGizmos(this))
					{
						yield return gizmo;
					}
				}
			}
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "Dev: Mental break",
					action = delegate()
					{
						Pawn pawn;
						if ((from x in this.PawnsListForReading
						where x.RaceProps.Humanlike && !x.InMentalState
						select x).TryRandomElement(out pawn))
						{
							pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad, null, false, false, null, false);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Make random pawn hungry",
					action = delegate()
					{
						Pawn pawn;
						if ((from x in this.PawnsListForReading
						where x.needs.food != null
						select x).TryRandomElement(out pawn))
						{
							pawn.needs.food.CurLevelPercentage = 0f;
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Kill random pawn",
					action = delegate()
					{
						Pawn pawn;
						if (this.PawnsListForReading.TryRandomElement(out pawn))
						{
							pawn.Kill(null, null);
							Messages.Message("Dev: Killed " + pawn.LabelShort, this, MessageTypeDefOf.TaskCompletion, false);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Harm random pawn",
					action = delegate()
					{
						Pawn pawn;
						if (this.PawnsListForReading.TryRandomElement(out pawn))
						{
							DamageInfo dinfo = new DamageInfo(DamageDefOf.Scratch, 10f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
							pawn.TakeDamage(dinfo);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Down random pawn",
					action = delegate()
					{
						Pawn pawn;
						if ((from x in this.PawnsListForReading
						where !x.Downed
						select x).TryRandomElement(out pawn))
						{
							HealthUtility.DamageUntilDowned(pawn);
							Messages.Message("Dev: Downed " + pawn.LabelShort, this, MessageTypeDefOf.TaskCompletion, false);
						}
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "Dev: Teleport to destination",
					action = delegate()
					{
						base.Tile = this.pather.Destination;
						this.pather.StopDead();
					}
				};
			}
			yield break;
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x000FB364 File Offset: 0x000F9764
		public override IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptions(IEnumerable<IThingHolder> pods, CompLaunchable representative)
		{
			foreach (FloatMenuOption o in this.<GetTransportPodsFloatMenuOptions>__BaseCallProxy1(pods, representative))
			{
				yield return o;
			}
			foreach (FloatMenuOption f in TransportPodsArrivalAction_GiveToCaravan.GetFloatMenuOptions(representative, pods, this))
			{
				yield return f;
			}
			yield break;
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x000FB39C File Offset: 0x000F979C
		public void RecacheImmobilizedNow()
		{
			this.cachedImmobilizedForTicks = -99999;
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x000FB3AA File Offset: 0x000F97AA
		public void RecacheDaysWorthOfFood()
		{
			this.cachedDaysWorthOfFoodForTicks = -99999;
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x000FB3B8 File Offset: 0x000F97B8
		public virtual void Notify_MemberDied(Pawn member)
		{
			if (!this.PawnsListForReading.Any((Pawn x) => x != member && this.IsOwner(x)))
			{
				this.RemovePawn(member);
				if (base.Faction == Faction.OfPlayer)
				{
					Find.LetterStack.ReceiveLetter("LetterLabelAllCaravanColonistsDied".Translate(), "LetterAllCaravanColonistsDied".Translate(new object[]
					{
						this.Name
					}).CapitalizeFirst(), LetterDefOf.NegativeEvent, new GlobalTargetInfo(base.Tile), null, null);
				}
				Find.WorldObjects.Remove(this);
			}
			else
			{
				member.Strip();
				this.RemovePawn(member);
			}
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x000FB488 File Offset: 0x000F9888
		public virtual void Notify_Merged(List<Caravan> group)
		{
			this.notifiedOutOfFood = false;
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x000FB492 File Offset: 0x000F9892
		public virtual void Notify_StartedTrading()
		{
			this.notifiedOutOfFood = false;
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x000FB49C File Offset: 0x000F989C
		private void CheckAnyNonWorldPawns()
		{
			for (int i = this.pawns.Count - 1; i >= 0; i--)
			{
				if (!this.pawns[i].IsWorldPawn())
				{
					Log.Error("Caravan member " + this.pawns[i] + " is not a world pawn. Removing...", false);
					this.pawns.Remove(this.pawns[i]);
				}
			}
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x000FB51C File Offset: 0x000F991C
		private bool ShouldAutoCapture(Pawn p)
		{
			return CaravanUtility.ShouldAutoCapture(p, base.Faction);
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x000FB53D File Offset: 0x000F993D
		public void Notify_PawnRemoved(Pawn p)
		{
			Find.ColonistBar.MarkColonistsDirty();
			this.RecacheImmobilizedNow();
			this.RecacheDaysWorthOfFood();
		}

		// Token: 0x06001D50 RID: 7504 RVA: 0x000FB556 File Offset: 0x000F9956
		public void Notify_PawnAdded(Pawn p)
		{
			Find.ColonistBar.MarkColonistsDirty();
			this.RecacheImmobilizedNow();
			this.RecacheDaysWorthOfFood();
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x000FB56F File Offset: 0x000F996F
		public void Notify_DestinationOrPauseStatusChanged()
		{
			this.RecacheDaysWorthOfFood();
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x000FB578 File Offset: 0x000F9978
		public void Notify_Teleported()
		{
			this.tweener.ResetTweenedPosToRoot();
			this.pather.Notify_Teleported_Int();
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x000FB594 File Offset: 0x000F9994
		public ThingOwner GetDirectlyHeldThings()
		{
			return this.pawns;
		}

		// Token: 0x06001D54 RID: 7508 RVA: 0x000FB5AF File Offset: 0x000F99AF
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}
	}
}
