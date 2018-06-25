﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x0200045C RID: 1116
	public class DeathActionWorker_SmallExplosion : DeathActionWorker
	{
		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06001393 RID: 5011 RVA: 0x000A94B4 File Offset: 0x000A78B4
		public override RulePackDef DeathRules
		{
			get
			{
				return RulePackDefOf.Transition_DiedExplosive;
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06001394 RID: 5012 RVA: 0x000A94D0 File Offset: 0x000A78D0
		public override bool DangerousInMelee
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x000A94E8 File Offset: 0x000A78E8
		public override void PawnDied(Corpse corpse)
		{
			GenExplosion.DoExplosion(corpse.Position, corpse.Map, 1.9f, DamageDefOf.Flame, corpse.InnerPawn, -1, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
		}
	}
}
