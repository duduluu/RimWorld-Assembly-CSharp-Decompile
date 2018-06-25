﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000159 RID: 345
	public class WorkGiver_Refuel_Turret : WorkGiver_Refuel
	{
		// Token: 0x17000114 RID: 276
		// (get) Token: 0x0600071A RID: 1818 RVA: 0x000484C0 File Offset: 0x000468C0
		public override JobDef JobStandard
		{
			get
			{
				return JobDefOf.RearmTurret;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x000484DC File Offset: 0x000468DC
		public override JobDef JobAtomic
		{
			get
			{
				return JobDefOf.RearmTurretAtomic;
			}
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x000484F8 File Offset: 0x000468F8
		public override bool CanRefuelThing(Thing t)
		{
			return t is Building_Turret;
		}
	}
}
