﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000262 RID: 610
	public class CompProperties_Hibernatable : CompProperties
	{
		// Token: 0x040004C7 RID: 1223
		public float startupDays = 15f;

		// Token: 0x040004C8 RID: 1224
		public IncidentTargetTypeDef incidentTargetWhileStarting = null;

		// Token: 0x06000AA0 RID: 2720 RVA: 0x000600C1 File Offset: 0x0005E4C1
		public CompProperties_Hibernatable()
		{
			this.compClass = typeof(CompHibernatable);
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x000600EC File Offset: 0x0005E4EC
		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (string err in this.<ConfigErrors>__BaseCallProxy0(parentDef))
			{
				yield return err;
			}
			if (parentDef.tickerType != TickerType.Normal)
			{
				yield return string.Concat(new object[]
				{
					"CompHibernatable needs tickerType ",
					TickerType.Normal,
					", has ",
					parentDef.tickerType
				});
			}
			yield break;
		}
	}
}
