﻿using System;
using RimWorld;

namespace Verse
{
	// Token: 0x02000B22 RID: 2850
	public abstract class DeathActionWorker
	{
		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x06003ECA RID: 16074 RVA: 0x000A93B0 File Offset: 0x000A77B0
		public virtual RulePackDef DeathRules
		{
			get
			{
				return RulePackDefOf.Transition_Died;
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x06003ECB RID: 16075 RVA: 0x000A93CC File Offset: 0x000A77CC
		public virtual bool DangerousInMelee
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003ECC RID: 16076
		public abstract void PawnDied(Corpse corpse);
	}
}
