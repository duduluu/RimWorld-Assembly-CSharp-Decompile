﻿using System;

namespace RimWorld.BaseGen
{
	// Token: 0x02000397 RID: 919
	public class SymbolResolver_BasePart_Indoors_Leaf_Brewery : SymbolResolver
	{
		// Token: 0x040009F9 RID: 2553
		private const float MaxCoverage = 0.08f;

		// Token: 0x06001004 RID: 4100 RVA: 0x00086F94 File Offset: 0x00085394
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp) && BaseGen.globalSettings.basePart_barracksResolved >= BaseGen.globalSettings.minBarracks && BaseGen.globalSettings.basePart_breweriesCoverage + (float)rp.rect.Area / (float)BaseGen.globalSettings.mainRect.Area < 0.08f && (rp.faction == null || rp.faction.def.techLevel >= TechLevel.Medieval);
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x0008703C File Offset: 0x0008543C
		public override void Resolve(ResolveParams rp)
		{
			BaseGen.symbolStack.Push("brewery", rp);
			BaseGen.globalSettings.basePart_breweriesCoverage += (float)rp.rect.Area / (float)BaseGen.globalSettings.mainRect.Area;
		}
	}
}
