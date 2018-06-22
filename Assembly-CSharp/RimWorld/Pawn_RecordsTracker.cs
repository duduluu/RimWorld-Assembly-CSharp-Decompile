﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000516 RID: 1302
	public class Pawn_RecordsTracker : IExposable
	{
		// Token: 0x060017A4 RID: 6052 RVA: 0x000CECC8 File Offset: 0x000CD0C8
		public Pawn_RecordsTracker(Pawn pawn)
		{
			this.pawn = pawn;
			Rand.PushState();
			Rand.Seed = pawn.thingIDNumber * 681;
			this.storyRelevanceBonus = Rand.Range(0f, 100f);
			Rand.PopState();
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x060017A5 RID: 6053 RVA: 0x000CED48 File Offset: 0x000CD148
		public float StoryRelevance
		{
			get
			{
				return (float)this.storyRelevance + this.storyRelevanceBonus;
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x060017A6 RID: 6054 RVA: 0x000CED6C File Offset: 0x000CD16C
		public Battle BattleActive
		{
			get
			{
				Battle result;
				if (this.battleExitTick < Find.TickManager.TicksGame)
				{
					result = null;
				}
				else if (this.battleActive == null)
				{
					result = null;
				}
				else
				{
					while (this.battleActive.AbsorbedBy != null)
					{
						this.battleActive = this.battleActive.AbsorbedBy;
					}
					result = this.battleActive;
				}
				return result;
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x060017A7 RID: 6055 RVA: 0x000CEDE0 File Offset: 0x000CD1E0
		public int LastBattleTick
		{
			get
			{
				return this.battleExitTick;
			}
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x000CEDFB File Offset: 0x000CD1FB
		public void RecordsTick()
		{
			if (!this.pawn.Dead)
			{
				if (this.pawn.IsHashIntervalTick(80))
				{
					this.RecordsTickUpdate(80);
					this.battleActive = this.BattleActive;
				}
			}
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x000CEE3B File Offset: 0x000CD23B
		public void RecordsTickMothballed(int interval)
		{
			this.RecordsTickUpdate(interval);
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x000CEE48 File Offset: 0x000CD248
		private void RecordsTickUpdate(int interval)
		{
			List<RecordDef> allDefsListForReading = DefDatabase<RecordDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				if (allDefsListForReading[i].type == RecordType.Time)
				{
					if (allDefsListForReading[i].Worker.ShouldMeasureTimeNow(this.pawn))
					{
						DefMap<RecordDef, float> defMap;
						RecordDef def;
						(defMap = this.records)[def = allDefsListForReading[i]] = defMap[def] + (float)interval;
					}
				}
			}
			this.storyRelevance *= Math.Pow(0.20000000298023224, (double)(0 * interval));
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x000CEEE4 File Offset: 0x000CD2E4
		public void Increment(RecordDef def)
		{
			if (def.type != RecordType.Int)
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to increment record \"",
					def.defName,
					"\" whose record type is \"",
					def.type,
					"\"."
				}), false);
			}
			else
			{
				this.records[def] = Mathf.Round(this.records[def] + 1f);
			}
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x000CEF68 File Offset: 0x000CD368
		public void AddTo(RecordDef def, float value)
		{
			if (def.type == RecordType.Int)
			{
				this.records[def] = Mathf.Round(this.records[def] + Mathf.Round(value));
			}
			else if (def.type == RecordType.Float)
			{
				DefMap<RecordDef, float> defMap;
				(defMap = this.records)[def] = defMap[def] + value;
			}
			else
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to add value to record \"",
					def.defName,
					"\" whose record type is \"",
					def.type,
					"\"."
				}), false);
			}
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x000CF01C File Offset: 0x000CD41C
		public float GetValue(RecordDef def)
		{
			float num = this.records[def];
			float result;
			if (def.type == RecordType.Int || def.type == RecordType.Time)
			{
				result = Mathf.Round(num);
			}
			else
			{
				result = num;
			}
			return result;
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x000CF064 File Offset: 0x000CD464
		public int GetAsInt(RecordDef def)
		{
			return Mathf.RoundToInt(this.records[def]);
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x000CF08A File Offset: 0x000CD48A
		public void AccumulateStoryEvent(StoryEventDef def)
		{
			this.storyRelevance += (double)def.importance;
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x000CF0A1 File Offset: 0x000CD4A1
		public void EnterBattle(Battle battle)
		{
			this.battleActive = battle;
			this.battleExitTick = Find.TickManager.TicksGame + 5000;
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x000CF0C4 File Offset: 0x000CD4C4
		public void ExposeData()
		{
			this.battleActive = this.BattleActive;
			Scribe_Deep.Look<DefMap<RecordDef, float>>(ref this.records, "records", new object[0]);
			Scribe_Values.Look<double>(ref this.storyRelevance, "storyRelevance", 0.0, false);
			Scribe_References.Look<Battle>(ref this.battleActive, "battleActive", false);
			Scribe_Values.Look<int>(ref this.battleExitTick, "battleExitTick", 0, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				BackCompatibility.RecordsTrackerPostLoadInit(this);
			}
		}

		// Token: 0x04000DF0 RID: 3568
		public Pawn pawn;

		// Token: 0x04000DF1 RID: 3569
		private DefMap<RecordDef, float> records = new DefMap<RecordDef, float>();

		// Token: 0x04000DF2 RID: 3570
		private double storyRelevance = 0.0;

		// Token: 0x04000DF3 RID: 3571
		private Battle battleActive = null;

		// Token: 0x04000DF4 RID: 3572
		private int battleExitTick = 0;

		// Token: 0x04000DF5 RID: 3573
		private float storyRelevanceBonus = 0f;

		// Token: 0x04000DF6 RID: 3574
		private const int UpdateTimeRecordsIntervalTicks = 80;

		// Token: 0x04000DF7 RID: 3575
		private const float StoryRelevanceBonusRange = 100f;

		// Token: 0x04000DF8 RID: 3576
		private const float StoryRelevanceMultiplierPerYear = 0.2f;
	}
}
