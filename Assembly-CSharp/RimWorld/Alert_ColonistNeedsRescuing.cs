﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	// Token: 0x0200078B RID: 1931
	public class Alert_ColonistNeedsRescuing : Alert_Critical
	{
		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06002AE0 RID: 10976 RVA: 0x0016A6CC File Offset: 0x00168ACC
		private IEnumerable<Pawn> ColonistsNeedingRescue
		{
			get
			{
				foreach (Pawn p in PawnsFinder.AllMaps_FreeColonistsSpawned)
				{
					if (Alert_ColonistNeedsRescuing.NeedsRescue(p))
					{
						yield return p;
					}
				}
				yield break;
			}
		}

		// Token: 0x06002AE1 RID: 10977 RVA: 0x0016A6F0 File Offset: 0x00168AF0
		public static bool NeedsRescue(Pawn p)
		{
			return p.Downed && !p.InBed() && !(p.ParentHolder is Pawn_CarryTracker) && (p.jobs.jobQueue == null || p.jobs.jobQueue.Count <= 0 || !p.jobs.jobQueue.Peek().job.CanBeginNow(p, false));
		}

		// Token: 0x06002AE2 RID: 10978 RVA: 0x0016A784 File Offset: 0x00168B84
		public override string GetLabel()
		{
			string result;
			if (this.ColonistsNeedingRescue.Count<Pawn>() == 1)
			{
				result = "ColonistNeedsRescue".Translate();
			}
			else
			{
				result = "ColonistsNeedRescue".Translate();
			}
			return result;
		}

		// Token: 0x06002AE3 RID: 10979 RVA: 0x0016A7C4 File Offset: 0x00168BC4
		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pawn pawn in this.ColonistsNeedingRescue)
			{
				stringBuilder.AppendLine("    " + pawn.LabelShort);
			}
			return string.Format("ColonistsNeedRescueDesc".Translate(), stringBuilder.ToString());
		}

		// Token: 0x06002AE4 RID: 10980 RVA: 0x0016A854 File Offset: 0x00168C54
		public override AlertReport GetReport()
		{
			return AlertReport.CulpritsAre(this.ColonistsNeedingRescue);
		}
	}
}
