﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	// Token: 0x02000358 RID: 856
	[HasDebugOutput]
	public class StoryIntender_Population : IExposable
	{
		// Token: 0x06000ED3 RID: 3795 RVA: 0x0007D770 File Offset: 0x0007BB70
		public StoryIntender_Population()
		{
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x0007D784 File Offset: 0x0007BB84
		public StoryIntender_Population(Storyteller teller)
		{
			this.teller = teller;
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x0007D7A0 File Offset: 0x0007BBA0
		private StorytellerDef Def
		{
			get
			{
				return this.teller.def;
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000ED6 RID: 3798 RVA: 0x0007D7C0 File Offset: 0x0007BBC0
		private int TimeSinceLastGain
		{
			get
			{
				return Find.TickManager.TicksGame - this.lastPopGainTime;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000ED7 RID: 3799 RVA: 0x0007D7E8 File Offset: 0x0007BBE8
		public virtual float PopulationIntent
		{
			get
			{
				return StoryIntender_Population.CalculatePopulationIntent(this.Def, this.AdjustedPopulation, this.TimeSinceLastGain);
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000ED8 RID: 3800 RVA: 0x0007D814 File Offset: 0x0007BC14
		public float AdjustedPopulation
		{
			get
			{
				float num = 0f;
				num += (float)PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.Count<Pawn>();
				return num + (float)PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_PrisonersOfColony.Count<Pawn>() * 0.5f;
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000ED9 RID: 3801 RVA: 0x0007D854 File Offset: 0x0007BC54
		public string DebugReadout
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("IntenderPopulation");
				stringBuilder.AppendLine("   AdjustedPopulation: " + this.AdjustedPopulation);
				stringBuilder.AppendLine("   PopulationIntent: " + this.PopulationIntent);
				stringBuilder.AppendLine("   lastPopGainTime: " + this.lastPopGainTime);
				stringBuilder.AppendLine("   TimeSinceLastGain: " + this.TimeSinceLastGain);
				return stringBuilder.ToString();
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x0007D8F1 File Offset: 0x0007BCF1
		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.lastPopGainTime, "lastPopGainTime", 0, false);
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x0007D906 File Offset: 0x0007BD06
		public void Notify_PopulationGained()
		{
			if (Current.ProgramState == ProgramState.Playing)
			{
				this.lastPopGainTime = Find.TickManager.TicksGame;
			}
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x0007D924 File Offset: 0x0007BD24
		private static float CalculatePopulationIntent(StorytellerDef def, float curPop, int ticksSinceGain)
		{
			float num = def.populationIntentFromPopCurve.Evaluate(curPop);
			if (num > 0f)
			{
				float x = (float)ticksSinceGain / 60000f;
				num *= def.populationIntentFromTimeCurve.Evaluate(x);
			}
			return num;
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0007D96C File Offset: 0x0007BD6C
		[DebugOutput]
		public void PopulationIntents()
		{
			List<float> list = new List<float>();
			for (int i = 0; i < 30; i++)
			{
				list.Add((float)i);
			}
			List<float> list2 = new List<float>();
			for (int j = 0; j < 40; j += 2)
			{
				list2.Add((float)j);
			}
			DebugTables.MakeTablesDialog<float, float>(list2, (float ds) => "d-" + ds.ToString("F0"), list, (float rv) => rv.ToString("F2"), (float ds, float p) => StoryIntender_Population.CalculatePopulationIntent(this.Def, p, (int)(ds * 60000f)).ToString("F2"), "pop");
		}

		// Token: 0x0400091E RID: 2334
		public Storyteller teller;

		// Token: 0x0400091F RID: 2335
		private int lastPopGainTime = -600000;
	}
}
