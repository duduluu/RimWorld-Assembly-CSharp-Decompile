﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x02000192 RID: 402
	public class LordToil_HiveRelatedData : LordToilData
	{
		// Token: 0x04000388 RID: 904
		public Dictionary<Pawn, Hive> assignedHives = new Dictionary<Pawn, Hive>();

		// Token: 0x06000854 RID: 2132 RVA: 0x0004FC34 File Offset: 0x0004E034
		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.assignedHives.RemoveAll((KeyValuePair<Pawn, Hive> x) => x.Key.Destroyed);
			}
			Scribe_Collections.Look<Pawn, Hive>(ref this.assignedHives, "assignedHives", LookMode.Reference, LookMode.Reference);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.assignedHives.RemoveAll((KeyValuePair<Pawn, Hive> x) => x.Value == null);
			}
		}
	}
}
