﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020002B4 RID: 692
	public class PawnGroupKindDef : Def
	{
		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000B9B RID: 2971 RVA: 0x00068AA0 File Offset: 0x00066EA0
		public PawnGroupKindWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (PawnGroupKindWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		// Token: 0x040006AE RID: 1710
		public Type workerClass = typeof(PawnGroupKindWorker);

		// Token: 0x040006AF RID: 1711
		[Unsaved]
		private PawnGroupKindWorker workerInt;
	}
}
