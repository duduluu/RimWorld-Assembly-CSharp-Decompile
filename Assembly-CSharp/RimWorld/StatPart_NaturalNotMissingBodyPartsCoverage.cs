﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020009B1 RID: 2481
	public class StatPart_NaturalNotMissingBodyPartsCoverage : StatPart
	{
		// Token: 0x06003798 RID: 14232 RVA: 0x001DA4F4 File Offset: 0x001D88F4
		public override void TransformValue(StatRequest req, ref float val)
		{
			float num;
			if (this.TryGetValue(req, out num))
			{
				val *= num;
			}
		}

		// Token: 0x06003799 RID: 14233 RVA: 0x001DA518 File Offset: 0x001D8918
		public override string ExplanationPart(StatRequest req)
		{
			float f;
			string result;
			if (this.TryGetValue(req, out f))
			{
				result = "StatsReport_MissingBodyParts".Translate() + ": x" + f.ToStringPercent();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600379A RID: 14234 RVA: 0x001DA55C File Offset: 0x001D895C
		private bool TryGetValue(StatRequest req, out float value)
		{
			return PawnOrCorpseStatUtility.TryGetPawnOrCorpseStat(req, (Pawn x) => x.health.hediffSet.GetCoverageOfNotMissingNaturalParts(x.RaceProps.body.corePart), (ThingDef x) => 1f, out value);
		}
	}
}
