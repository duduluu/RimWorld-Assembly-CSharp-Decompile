﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020009AF RID: 2479
	public class StatPart_MaxChanceIfRotting : StatPart
	{
		// Token: 0x0600378E RID: 14222 RVA: 0x001DA1D7 File Offset: 0x001D85D7
		public override void TransformValue(StatRequest req, ref float val)
		{
			if (this.IsRotting(req))
			{
				val = 1f;
			}
		}

		// Token: 0x0600378F RID: 14223 RVA: 0x001DA1F0 File Offset: 0x001D85F0
		public override string ExplanationPart(StatRequest req)
		{
			string result;
			if (this.IsRotting(req))
			{
				result = "StatsReport_NotFresh".Translate() + ": " + 1f.ToStringPercent();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06003790 RID: 14224 RVA: 0x001DA238 File Offset: 0x001D8638
		private bool IsRotting(StatRequest req)
		{
			return req.HasThing && req.Thing.GetRotStage() != RotStage.Fresh;
		}
	}
}
