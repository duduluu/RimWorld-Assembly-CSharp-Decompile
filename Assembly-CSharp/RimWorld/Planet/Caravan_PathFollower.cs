﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005EB RID: 1515
	public class Caravan_PathFollower : IExposable
	{
		// Token: 0x040011BF RID: 4543
		private Caravan caravan;

		// Token: 0x040011C0 RID: 4544
		private bool moving;

		// Token: 0x040011C1 RID: 4545
		private bool paused;

		// Token: 0x040011C2 RID: 4546
		public int nextTile = -1;

		// Token: 0x040011C3 RID: 4547
		public int previousTileForDrawingIfInDoubt = -1;

		// Token: 0x040011C4 RID: 4548
		public float nextTileCostLeft = 0f;

		// Token: 0x040011C5 RID: 4549
		public float nextTileCostTotal = 1f;

		// Token: 0x040011C6 RID: 4550
		private int destTile;

		// Token: 0x040011C7 RID: 4551
		private CaravanArrivalAction arrivalAction;

		// Token: 0x040011C8 RID: 4552
		public WorldPath curPath;

		// Token: 0x040011C9 RID: 4553
		public int lastPathedTargetTile;

		// Token: 0x040011CA RID: 4554
		public const int MaxMoveTicks = 120000;

		// Token: 0x040011CB RID: 4555
		private const int MaxCheckAheadNodes = 20;

		// Token: 0x040011CC RID: 4556
		private const int MinCostWalk = 50;

		// Token: 0x040011CD RID: 4557
		private const int MinCostAmble = 60;

		// Token: 0x040011CE RID: 4558
		public const float DefaultPathCostToPayPerTick = 1f;

		// Token: 0x040011CF RID: 4559
		public const int FinalNoRestPushMaxDurationTicks = 10000;

		// Token: 0x06001DF9 RID: 7673 RVA: 0x00102457 File Offset: 0x00100857
		public Caravan_PathFollower(Caravan caravan)
		{
			this.caravan = caravan;
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001DFA RID: 7674 RVA: 0x0010248C File Offset: 0x0010088C
		public int Destination
		{
			get
			{
				return this.destTile;
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001DFB RID: 7675 RVA: 0x001024A8 File Offset: 0x001008A8
		public bool Moving
		{
			get
			{
				return this.moving && this.caravan.Spawned;
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001DFC RID: 7676 RVA: 0x001024D8 File Offset: 0x001008D8
		public bool MovingNow
		{
			get
			{
				return this.Moving && !this.Paused && !this.caravan.CantMove;
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001DFD RID: 7677 RVA: 0x00102514 File Offset: 0x00100914
		public CaravanArrivalAction ArrivalAction
		{
			get
			{
				return (!this.Moving) ? null : this.arrivalAction;
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001DFE RID: 7678 RVA: 0x00102540 File Offset: 0x00100940
		// (set) Token: 0x06001DFF RID: 7679 RVA: 0x0010256C File Offset: 0x0010096C
		public bool Paused
		{
			get
			{
				return this.Moving && this.paused;
			}
			set
			{
				if (value != this.paused)
				{
					if (!value)
					{
						this.paused = false;
					}
					else if (!this.Moving)
					{
						Log.Error("Tried to pause caravan movement of " + this.caravan.ToStringSafe<Caravan>() + " but it's not moving.", false);
					}
					else
					{
						this.paused = true;
					}
					this.caravan.Notify_DestinationOrPauseStatusChanged();
				}
			}
		}

		// Token: 0x06001E00 RID: 7680 RVA: 0x001025E0 File Offset: 0x001009E0
		public void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.moving, "moving", true, false);
			Scribe_Values.Look<bool>(ref this.paused, "paused", false, false);
			Scribe_Values.Look<int>(ref this.nextTile, "nextTile", 0, false);
			Scribe_Values.Look<int>(ref this.previousTileForDrawingIfInDoubt, "previousTileForDrawingIfInDoubt", 0, false);
			Scribe_Values.Look<float>(ref this.nextTileCostLeft, "nextTileCostLeft", 0f, false);
			Scribe_Values.Look<float>(ref this.nextTileCostTotal, "nextTileCostTotal", 0f, false);
			Scribe_Values.Look<int>(ref this.destTile, "destTile", 0, false);
			Scribe_Deep.Look<CaravanArrivalAction>(ref this.arrivalAction, "arrivalAction", new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit && Current.ProgramState != ProgramState.Entry)
			{
				if (this.moving)
				{
					this.StartPath(this.destTile, this.arrivalAction, true, false);
				}
			}
		}

		// Token: 0x06001E01 RID: 7681 RVA: 0x001026C0 File Offset: 0x00100AC0
		public void StartPath(int destTile, CaravanArrivalAction arrivalAction, bool repathImmediately = false, bool resetPauseStatus = true)
		{
			this.caravan.autoJoinable = false;
			if (resetPauseStatus)
			{
				this.paused = false;
			}
			if (arrivalAction == null || arrivalAction.StillValid(this.caravan, destTile))
			{
				if (!this.IsPassable(this.caravan.Tile))
				{
					if (!this.TryRecoverFromUnwalkablePosition())
					{
						return;
					}
				}
				if (this.moving && this.curPath != null && this.destTile == destTile)
				{
					this.arrivalAction = arrivalAction;
				}
				else if (!this.caravan.CanReach(destTile))
				{
					this.PatherFailed();
				}
				else
				{
					this.destTile = destTile;
					this.arrivalAction = arrivalAction;
					this.caravan.Notify_DestinationOrPauseStatusChanged();
					if (this.nextTile < 0 || !this.IsNextTilePassable())
					{
						this.nextTile = this.caravan.Tile;
						this.nextTileCostLeft = 0f;
						this.previousTileForDrawingIfInDoubt = -1;
					}
					if (this.AtDestinationPosition())
					{
						this.PatherArrived();
					}
					else
					{
						if (this.curPath != null)
						{
							this.curPath.ReleaseToPool();
						}
						this.curPath = null;
						this.moving = true;
						if (repathImmediately)
						{
							bool flag = this.TrySetNewPath();
							if (flag && this.nextTileCostLeft <= 0f && this.moving)
							{
								this.TryEnterNextPathTile();
							}
						}
					}
				}
			}
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x00102844 File Offset: 0x00100C44
		public void StopDead()
		{
			if (this.curPath != null)
			{
				this.curPath.ReleaseToPool();
			}
			this.curPath = null;
			this.moving = false;
			this.paused = false;
			this.nextTile = this.caravan.Tile;
			this.previousTileForDrawingIfInDoubt = -1;
			this.arrivalAction = null;
			this.nextTileCostLeft = 0f;
			this.caravan.Notify_DestinationOrPauseStatusChanged();
		}

		// Token: 0x06001E03 RID: 7683 RVA: 0x001028B4 File Offset: 0x00100CB4
		public void PatherTick()
		{
			if (this.moving && this.arrivalAction != null && !this.arrivalAction.StillValid(this.caravan, this.Destination))
			{
				string failMessage = this.arrivalAction.StillValid(this.caravan, this.Destination).FailMessage;
				Messages.Message("MessageCaravanArrivalActionNoLongerValid".Translate(new object[]
				{
					this.caravan.Name
				}).CapitalizeFirst() + ((failMessage == null) ? "" : (" " + failMessage)), this.caravan, MessageTypeDefOf.NegativeEvent, true);
				this.StopDead();
			}
			if (!this.caravan.CantMove && !this.paused)
			{
				if (this.nextTileCostLeft > 0f)
				{
					this.nextTileCostLeft -= this.CostToPayThisTick();
				}
				else if (this.moving)
				{
					this.TryEnterNextPathTile();
				}
			}
		}

		// Token: 0x06001E04 RID: 7684 RVA: 0x001029D3 File Offset: 0x00100DD3
		public void Notify_Teleported_Int()
		{
			this.StopDead();
		}

		// Token: 0x06001E05 RID: 7685 RVA: 0x001029DC File Offset: 0x00100DDC
		private bool IsPassable(int tile)
		{
			return !Find.World.Impassable(tile);
		}

		// Token: 0x06001E06 RID: 7686 RVA: 0x00102A00 File Offset: 0x00100E00
		public bool IsNextTilePassable()
		{
			return this.IsPassable(this.nextTile);
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x00102A24 File Offset: 0x00100E24
		private bool TryRecoverFromUnwalkablePosition()
		{
			int num;
			bool result;
			if (GenWorldClosest.TryFindClosestTile(this.caravan.Tile, (int t) => this.IsPassable(t), out num, 2147483647, true))
			{
				Log.Warning(string.Concat(new object[]
				{
					this.caravan,
					" on unwalkable tile ",
					this.caravan.Tile,
					". Teleporting to ",
					num
				}), false);
				this.caravan.Tile = num;
				this.caravan.Notify_Teleported();
				result = true;
			}
			else
			{
				Find.WorldObjects.Remove(this.caravan);
				Log.Error(string.Concat(new object[]
				{
					this.caravan,
					" on unwalkable tile ",
					this.caravan.Tile,
					". Could not find walkable position nearby. Removed."
				}), false);
				result = false;
			}
			return result;
		}

		// Token: 0x06001E08 RID: 7688 RVA: 0x00102B18 File Offset: 0x00100F18
		private void PatherArrived()
		{
			CaravanArrivalAction caravanArrivalAction = this.arrivalAction;
			this.StopDead();
			if (caravanArrivalAction != null && caravanArrivalAction.StillValid(this.caravan, this.caravan.Tile))
			{
				caravanArrivalAction.Arrived(this.caravan);
			}
			else if (this.caravan.IsPlayerControlled && !this.caravan.VisibleToCameraNow())
			{
				Messages.Message("MessageCaravanArrivedAtDestination".Translate(new object[]
				{
					this.caravan.Label
				}).CapitalizeFirst(), this.caravan, MessageTypeDefOf.TaskCompletion, true);
			}
		}

		// Token: 0x06001E09 RID: 7689 RVA: 0x00102BC4 File Offset: 0x00100FC4
		private void PatherFailed()
		{
			this.StopDead();
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x00102BD0 File Offset: 0x00100FD0
		private void TryEnterNextPathTile()
		{
			if (!this.IsNextTilePassable())
			{
				this.PatherFailed();
			}
			else
			{
				this.caravan.Tile = this.nextTile;
				if (this.NeedNewPath())
				{
					if (!this.TrySetNewPath())
					{
						return;
					}
				}
				if (this.AtDestinationPosition())
				{
					this.PatherArrived();
				}
				else if (this.curPath.NodesLeftCount == 0)
				{
					Log.Error(this.caravan + " ran out of path nodes. Force-arriving.", false);
					this.PatherArrived();
				}
				else
				{
					this.SetupMoveIntoNextTile();
				}
			}
		}

		// Token: 0x06001E0B RID: 7691 RVA: 0x00102C74 File Offset: 0x00101074
		private void SetupMoveIntoNextTile()
		{
			if (this.curPath.NodesLeftCount < 2)
			{
				Log.Error(string.Concat(new object[]
				{
					this.caravan,
					" at ",
					this.caravan.Tile,
					" ran out of path nodes while pathing to ",
					this.destTile,
					"."
				}), false);
				this.PatherFailed();
			}
			else
			{
				this.nextTile = this.curPath.ConsumeNextNode();
				this.previousTileForDrawingIfInDoubt = -1;
				if (Find.World.Impassable(this.nextTile))
				{
					Log.Error(string.Concat(new object[]
					{
						this.caravan,
						" entering ",
						this.nextTile,
						" which is unwalkable."
					}), false);
				}
				int num = this.CostToMove(this.caravan.Tile, this.nextTile);
				this.nextTileCostTotal = (float)num;
				this.nextTileCostLeft = (float)num;
			}
		}

		// Token: 0x06001E0C RID: 7692 RVA: 0x00102D80 File Offset: 0x00101180
		private int CostToMove(int start, int end)
		{
			return Caravan_PathFollower.CostToMove(this.caravan, start, end, null);
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x00102DAC File Offset: 0x001011AC
		public static int CostToMove(Caravan caravan, int start, int end, int? ticksAbs = null)
		{
			return Caravan_PathFollower.CostToMove(caravan.TicksPerMove, start, end, ticksAbs, false, null, null);
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x00102DD4 File Offset: 0x001011D4
		public static int CostToMove(int caravanTicksPerMove, int start, int end, int? ticksAbs = null, bool perceivedStatic = false, StringBuilder explanation = null, string caravanTicksPerMoveExplanation = null)
		{
			int result;
			if (start == end)
			{
				result = 0;
			}
			else
			{
				if (explanation != null)
				{
					explanation.Append(caravanTicksPerMoveExplanation);
					explanation.AppendLine();
				}
				StringBuilder stringBuilder = (explanation == null) ? null : new StringBuilder();
				float num;
				if (perceivedStatic && explanation == null)
				{
					num = Find.WorldPathGrid.PerceivedMovementDifficultyAt(end);
				}
				else
				{
					num = WorldPathGrid.CalculatedMovementDifficultyAt(end, perceivedStatic, ticksAbs, stringBuilder);
				}
				float roadMovementDifficultyMultiplier = Find.WorldGrid.GetRoadMovementDifficultyMultiplier(start, end, stringBuilder);
				if (explanation != null)
				{
					explanation.AppendLine();
					explanation.Append("TileMovementDifficulty".Translate() + ":");
					explanation.AppendLine();
					explanation.Append(stringBuilder.ToString().Indented("  "));
					explanation.AppendLine();
					explanation.Append("  = " + (num * roadMovementDifficultyMultiplier).ToString("0.#"));
				}
				int num2 = (int)((float)caravanTicksPerMove * num * roadMovementDifficultyMultiplier);
				num2 = Mathf.Clamp(num2, 1, 120000);
				if (explanation != null)
				{
					explanation.AppendLine();
					explanation.AppendLine();
					explanation.Append("FinalCaravanMovementSpeed".Translate() + ":");
					int num3 = Mathf.CeilToInt((float)num2 / 1f);
					explanation.AppendLine();
					explanation.Append(string.Concat(new string[]
					{
						"  ",
						(60000f / (float)caravanTicksPerMove).ToString("0.#"),
						" / ",
						(num * roadMovementDifficultyMultiplier).ToString("0.#"),
						" = ",
						(60000f / (float)num3).ToString("0.#"),
						" ",
						"TilesPerDay".Translate()
					}));
				}
				result = num2;
			}
			return result;
		}

		// Token: 0x06001E0F RID: 7695 RVA: 0x00102FC8 File Offset: 0x001013C8
		public static bool IsValidFinalPushDestination(int tile)
		{
			List<WorldObject> allWorldObjects = Find.WorldObjects.AllWorldObjects;
			for (int i = 0; i < allWorldObjects.Count; i++)
			{
				if (allWorldObjects[i].Tile == tile && !(allWorldObjects[i] is Caravan))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x0010302C File Offset: 0x0010142C
		private float CostToPayThisTick()
		{
			float num = 1f;
			if (DebugSettings.fastCaravans)
			{
				num = 100f;
			}
			if (num < this.nextTileCostTotal / 120000f)
			{
				num = this.nextTileCostTotal / 120000f;
			}
			return num;
		}

		// Token: 0x06001E11 RID: 7697 RVA: 0x00103078 File Offset: 0x00101478
		private bool TrySetNewPath()
		{
			WorldPath worldPath = this.GenerateNewPath();
			bool result;
			if (!worldPath.Found)
			{
				this.PatherFailed();
				result = false;
			}
			else
			{
				if (this.curPath != null)
				{
					this.curPath.ReleaseToPool();
				}
				this.curPath = worldPath;
				result = true;
			}
			return result;
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x001030CC File Offset: 0x001014CC
		private WorldPath GenerateNewPath()
		{
			int num = (!this.moving || this.nextTile < 0 || !this.IsNextTilePassable()) ? this.caravan.Tile : this.nextTile;
			this.lastPathedTargetTile = this.destTile;
			WorldPath worldPath = Find.WorldPathFinder.FindPath(num, this.destTile, this.caravan, null);
			if (worldPath.Found && num != this.caravan.Tile)
			{
				if (worldPath.NodesLeftCount >= 2 && worldPath.Peek(1) == this.caravan.Tile)
				{
					worldPath.ConsumeNextNode();
					if (this.moving)
					{
						this.previousTileForDrawingIfInDoubt = this.nextTile;
						this.nextTile = this.caravan.Tile;
						this.nextTileCostLeft = this.nextTileCostTotal - this.nextTileCostLeft;
					}
				}
				else
				{
					worldPath.AddNodeAtStart(this.caravan.Tile);
				}
			}
			return worldPath;
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x001031E0 File Offset: 0x001015E0
		private bool AtDestinationPosition()
		{
			return this.caravan.Tile == this.destTile;
		}

		// Token: 0x06001E14 RID: 7700 RVA: 0x00103208 File Offset: 0x00101608
		private bool NeedNewPath()
		{
			bool result;
			if (!this.moving)
			{
				result = false;
			}
			else if (this.curPath == null || !this.curPath.Found || this.curPath.NodesLeftCount == 0)
			{
				result = true;
			}
			else
			{
				int num = 0;
				while (num < 20 && num < this.curPath.NodesLeftCount)
				{
					int tileID = this.curPath.Peek(num);
					if (Find.World.Impassable(tileID))
					{
						return true;
					}
					num++;
				}
				result = false;
			}
			return result;
		}
	}
}
