﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x020006A5 RID: 1701
	public class Building_Door : Building
	{
		// Token: 0x0400141D RID: 5149
		public CompPowerTrader powerComp;

		// Token: 0x0400141E RID: 5150
		private bool openInt = false;

		// Token: 0x0400141F RID: 5151
		private bool holdOpenInt = false;

		// Token: 0x04001420 RID: 5152
		private int lastFriendlyTouchTick = -9999;

		// Token: 0x04001421 RID: 5153
		protected int ticksUntilClose = 0;

		// Token: 0x04001422 RID: 5154
		protected int visualTicksOpen = 0;

		// Token: 0x04001423 RID: 5155
		private bool freePassageWhenClearedReachabilityCache;

		// Token: 0x04001424 RID: 5156
		private const float BaseDoorOpenTicks = 45f;

		// Token: 0x04001425 RID: 5157
		private const int AutomaticCloseDelayTicks = 60;

		// Token: 0x04001426 RID: 5158
		private const int ApproachCloseDelayTicks = 300;

		// Token: 0x04001427 RID: 5159
		private const int MaxTicksSinceFriendlyTouchToAutoClose = 301;

		// Token: 0x04001428 RID: 5160
		private const float VisualDoorOffsetStart = 0f;

		// Token: 0x04001429 RID: 5161
		private const float VisualDoorOffsetEnd = 0.45f;

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06002436 RID: 9270 RVA: 0x00136CF4 File Offset: 0x001350F4
		public bool Open
		{
			get
			{
				return this.openInt;
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06002437 RID: 9271 RVA: 0x00136D10 File Offset: 0x00135110
		public bool HoldOpen
		{
			get
			{
				return this.holdOpenInt;
			}
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06002438 RID: 9272 RVA: 0x00136D2C File Offset: 0x0013512C
		public bool FreePassage
		{
			get
			{
				return this.openInt && (this.holdOpenInt || !this.WillCloseSoon);
			}
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06002439 RID: 9273 RVA: 0x00136D6C File Offset: 0x0013516C
		public bool WillCloseSoon
		{
			get
			{
				bool result;
				if (!base.Spawned)
				{
					result = true;
				}
				else if (!this.openInt)
				{
					result = true;
				}
				else if (this.holdOpenInt)
				{
					result = false;
				}
				else if (this.ticksUntilClose > 0 && this.ticksUntilClose <= 60 && this.CanCloseAutomatically)
				{
					result = true;
				}
				else
				{
					for (int i = 0; i < 5; i++)
					{
						IntVec3 c = base.Position + GenAdj.CardinalDirectionsAndInside[i];
						if (c.InBounds(base.Map))
						{
							List<Thing> thingList = c.GetThingList(base.Map);
							for (int j = 0; j < thingList.Count; j++)
							{
								Pawn pawn = thingList[j] as Pawn;
								if (pawn != null && !pawn.HostileTo(this) && !pawn.Downed)
								{
									if (pawn.Position == base.Position || (pawn.pather.MovingNow && pawn.pather.nextCell == base.Position))
									{
										return true;
									}
								}
							}
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x0600243A RID: 9274 RVA: 0x00136ED4 File Offset: 0x001352D4
		public bool BlockedOpenMomentary
		{
			get
			{
				List<Thing> thingList = base.Position.GetThingList(base.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (thing.def.category == ThingCategory.Item || thing.def.category == ThingCategory.Pawn)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x0600243B RID: 9275 RVA: 0x00136F48 File Offset: 0x00135348
		public bool DoorPowerOn
		{
			get
			{
				return this.powerComp != null && this.powerComp.PowerOn;
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x0600243C RID: 9276 RVA: 0x00136F78 File Offset: 0x00135378
		public bool SlowsPawns
		{
			get
			{
				return !this.DoorPowerOn || this.TicksToOpenNow > 20;
			}
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x0600243D RID: 9277 RVA: 0x00136FA8 File Offset: 0x001353A8
		public int TicksToOpenNow
		{
			get
			{
				float num = 45f / this.GetStatValue(StatDefOf.DoorOpenSpeed, true);
				if (this.DoorPowerOn)
				{
					num *= 0.25f;
				}
				return Mathf.RoundToInt(num);
			}
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x0600243E RID: 9278 RVA: 0x00136FEC File Offset: 0x001353EC
		private bool CanCloseAutomatically
		{
			get
			{
				return this.DoorPowerOn && this.FriendlyTouchedRecently;
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x0600243F RID: 9279 RVA: 0x00137018 File Offset: 0x00135418
		private bool FriendlyTouchedRecently
		{
			get
			{
				return Find.TickManager.TicksGame < this.lastFriendlyTouchTick + 301;
			}
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06002440 RID: 9280 RVA: 0x00137048 File Offset: 0x00135448
		private int VisualTicksToOpen
		{
			get
			{
				return this.TicksToOpenNow;
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06002441 RID: 9281 RVA: 0x00137064 File Offset: 0x00135464
		public override bool FireBulwark
		{
			get
			{
				return !this.Open && base.FireBulwark;
			}
		}

		// Token: 0x06002442 RID: 9282 RVA: 0x0013708D File Offset: 0x0013548D
		public override void PostMake()
		{
			base.PostMake();
			this.powerComp = base.GetComp<CompPowerTrader>();
		}

		// Token: 0x06002443 RID: 9283 RVA: 0x001370A2 File Offset: 0x001354A2
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.ClearReachabilityCache(map);
			if (this.BlockedOpenMomentary)
			{
				this.DoorOpen(60);
			}
		}

		// Token: 0x06002444 RID: 9284 RVA: 0x001370D8 File Offset: 0x001354D8
		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			Map map = base.Map;
			base.DeSpawn(mode);
			this.ClearReachabilityCache(map);
		}

		// Token: 0x06002445 RID: 9285 RVA: 0x001370FC File Offset: 0x001354FC
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.openInt, "open", false, false);
			Scribe_Values.Look<bool>(ref this.holdOpenInt, "holdOpen", false, false);
			Scribe_Values.Look<int>(ref this.lastFriendlyTouchTick, "lastFriendlyTouchTick", 0, false);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				if (this.openInt)
				{
					this.visualTicksOpen = this.VisualTicksToOpen;
				}
			}
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x0013716A File Offset: 0x0013556A
		public override void SetFaction(Faction newFaction, Pawn recruiter = null)
		{
			base.SetFaction(newFaction, recruiter);
			if (base.Spawned)
			{
				this.ClearReachabilityCache(base.Map);
			}
		}

		// Token: 0x06002447 RID: 9287 RVA: 0x0013718C File Offset: 0x0013558C
		public override void Tick()
		{
			base.Tick();
			if (this.FreePassage != this.freePassageWhenClearedReachabilityCache)
			{
				this.ClearReachabilityCache(base.Map);
			}
			if (!this.openInt)
			{
				if (this.visualTicksOpen > 0)
				{
					this.visualTicksOpen--;
				}
				if ((Find.TickManager.TicksGame + this.thingIDNumber.HashOffset()) % 375 == 0)
				{
					GenTemperature.EqualizeTemperaturesThroughBuilding(this, 1f, false);
				}
			}
			else if (this.openInt)
			{
				if (this.visualTicksOpen < this.VisualTicksToOpen)
				{
					this.visualTicksOpen++;
				}
				if (!this.holdOpenInt)
				{
					if (base.Map.thingGrid.CellContains(base.Position, ThingCategory.Pawn))
					{
						this.ticksUntilClose = 60;
					}
					else
					{
						this.ticksUntilClose--;
						if (this.ticksUntilClose <= 0 && this.CanCloseAutomatically)
						{
							this.DoorTryClose();
						}
					}
				}
				if ((Find.TickManager.TicksGame + this.thingIDNumber.HashOffset()) % 22 == 0)
				{
					GenTemperature.EqualizeTemperaturesThroughBuilding(this, 1f, false);
				}
			}
		}

		// Token: 0x06002448 RID: 9288 RVA: 0x001372CF File Offset: 0x001356CF
		public void FriendlyTouched()
		{
			this.lastFriendlyTouchTick = Find.TickManager.TicksGame;
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x001372E4 File Offset: 0x001356E4
		public void Notify_PawnApproaching(Pawn p)
		{
			if (!p.HostileTo(this))
			{
				this.FriendlyTouched();
			}
			if (this.PawnCanOpen(p))
			{
				base.Map.fogGrid.Notify_PawnEnteringDoor(this, p);
				if (!this.SlowsPawns)
				{
					this.DoorOpen(300);
				}
			}
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x0013733C File Offset: 0x0013573C
		public bool CanPhysicallyPass(Pawn p)
		{
			return this.FreePassage || this.PawnCanOpen(p);
		}

		// Token: 0x0600244B RID: 9291 RVA: 0x00137368 File Offset: 0x00135768
		public virtual bool PawnCanOpen(Pawn p)
		{
			Lord lord = p.GetLord();
			return (lord != null && lord.LordJob != null && lord.LordJob.CanOpenAnyDoor(p)) || (p.IsWildMan() && !p.mindState.wildManEverReachedOutside) || base.Faction == null || (p.guest != null && p.guest.Released) || GenAI.MachinesLike(base.Faction, p);
		}

		// Token: 0x0600244C RID: 9292 RVA: 0x00137410 File Offset: 0x00135810
		public override bool BlocksPawn(Pawn p)
		{
			return !this.openInt && !this.PawnCanOpen(p);
		}

		// Token: 0x0600244D RID: 9293 RVA: 0x00137444 File Offset: 0x00135844
		protected void DoorOpen(int ticksToClose = 60)
		{
			this.ticksUntilClose = ticksToClose;
			if (!this.openInt)
			{
				this.openInt = true;
				if (this.DoorPowerOn)
				{
					this.def.building.soundDoorOpenPowered.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
				else
				{
					this.def.building.soundDoorOpenManual.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
			}
		}

		// Token: 0x0600244E RID: 9294 RVA: 0x001374D8 File Offset: 0x001358D8
		protected void DoorTryClose()
		{
			if (!this.holdOpenInt && !this.BlockedOpenMomentary)
			{
				this.openInt = false;
				if (this.DoorPowerOn)
				{
					this.def.building.soundDoorClosePowered.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
				else
				{
					this.def.building.soundDoorCloseManual.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
			}
		}

		// Token: 0x0600244F RID: 9295 RVA: 0x00137570 File Offset: 0x00135970
		public void StartManualOpenBy(Pawn opener)
		{
			this.DoorOpen(60);
		}

		// Token: 0x06002450 RID: 9296 RVA: 0x0013757B File Offset: 0x0013597B
		public void StartManualCloseBy(Pawn closer)
		{
			this.DoorTryClose();
		}

		// Token: 0x06002451 RID: 9297 RVA: 0x00137584 File Offset: 0x00135984
		public override void Draw()
		{
			base.Rotation = Building_Door.DoorRotationAt(base.Position, base.Map);
			float num = Mathf.Clamp01((float)this.visualTicksOpen / (float)this.VisualTicksToOpen);
			float d = 0.45f * num;
			for (int i = 0; i < 2; i++)
			{
				Vector3 vector = default(Vector3);
				Mesh mesh;
				if (i == 0)
				{
					vector = new Vector3(0f, 0f, -1f);
					mesh = MeshPool.plane10;
				}
				else
				{
					vector = new Vector3(0f, 0f, 1f);
					mesh = MeshPool.plane10Flip;
				}
				Rot4 rotation = base.Rotation;
				rotation.Rotate(RotationDirection.Clockwise);
				vector = rotation.AsQuat * vector;
				Vector3 vector2 = this.DrawPos;
				vector2.y = AltitudeLayer.DoorMoveable.AltitudeFor();
				vector2 += vector * d;
				Graphics.DrawMesh(mesh, vector2, base.Rotation.AsQuat, this.Graphic.MatAt(base.Rotation, null), 0);
			}
			base.Comps_PostDraw();
		}

		// Token: 0x06002452 RID: 9298 RVA: 0x001376A0 File Offset: 0x00135AA0
		private static int AlignQualityAgainst(IntVec3 c, Map map)
		{
			int result;
			if (!c.InBounds(map))
			{
				result = 0;
			}
			else if (!c.Walkable(map))
			{
				result = 9;
			}
			else
			{
				List<Thing> thingList = c.GetThingList(map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (typeof(Building_Door).IsAssignableFrom(thing.def.thingClass))
					{
						return 1;
					}
					Thing thing2 = thing as Blueprint;
					if (thing2 != null)
					{
						if (thing2.def.entityDefToBuild.passability == Traversability.Impassable)
						{
							return 9;
						}
						if (typeof(Building_Door).IsAssignableFrom(thing.def.thingClass))
						{
							return 1;
						}
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06002453 RID: 9299 RVA: 0x00137784 File Offset: 0x00135B84
		public static Rot4 DoorRotationAt(IntVec3 loc, Map map)
		{
			int num = 0;
			int num2 = 0;
			num += Building_Door.AlignQualityAgainst(loc + IntVec3.East, map);
			num += Building_Door.AlignQualityAgainst(loc + IntVec3.West, map);
			num2 += Building_Door.AlignQualityAgainst(loc + IntVec3.North, map);
			num2 += Building_Door.AlignQualityAgainst(loc + IntVec3.South, map);
			Rot4 result;
			if (num >= num2)
			{
				result = Rot4.North;
			}
			else
			{
				result = Rot4.East;
			}
			return result;
		}

		// Token: 0x06002454 RID: 9300 RVA: 0x00137804 File Offset: 0x00135C04
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo g in this.<GetGizmos>__BaseCallProxy0())
			{
				yield return g;
			}
			if (base.Faction == Faction.OfPlayer)
			{
				yield return new Command_Toggle
				{
					defaultLabel = "CommandToggleDoorHoldOpen".Translate(),
					defaultDesc = "CommandToggleDoorHoldOpenDesc".Translate(),
					hotKey = KeyBindingDefOf.Misc3,
					icon = TexCommand.HoldOpen,
					isActive = (() => this.holdOpenInt),
					toggleAction = delegate()
					{
						this.holdOpenInt = !this.holdOpenInt;
					}
				};
			}
			yield break;
		}

		// Token: 0x06002455 RID: 9301 RVA: 0x0013782E File Offset: 0x00135C2E
		private void ClearReachabilityCache(Map map)
		{
			map.reachability.ClearCache();
			this.freePassageWhenClearedReachabilityCache = this.FreePassage;
		}
	}
}
