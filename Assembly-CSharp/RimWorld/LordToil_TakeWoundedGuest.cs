﻿using System;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x020001A0 RID: 416
	public class LordToil_TakeWoundedGuest : LordToil
	{
		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600089C RID: 2204 RVA: 0x00051A18 File Offset: 0x0004FE18
		public override bool AllowSatisfyLongNeeds
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600089D RID: 2205 RVA: 0x00051A30 File Offset: 0x0004FE30
		public override bool AllowSelfTend
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00051A48 File Offset: 0x0004FE48
		public override void UpdateAllDuties()
		{
			for (int i = 0; i < this.lord.ownedPawns.Count; i++)
			{
				this.lord.ownedPawns[i].mindState.duty = new PawnDuty(DutyDefOf.TakeWoundedGuest);
			}
		}
	}
}
