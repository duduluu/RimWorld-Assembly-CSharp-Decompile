﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x0200045B RID: 1115
	public class DeathActionWorker_BigExplosion : DeathActionWorker
	{
		// Token: 0x170002AD RID: 685
		// (get) Token: 0x0600138F RID: 5007 RVA: 0x000A93EC File Offset: 0x000A77EC
		public override RulePackDef DeathRules
		{
			get
			{
				return RulePackDefOf.Transition_DiedExplosive;
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06001390 RID: 5008 RVA: 0x000A9408 File Offset: 0x000A7808
		public override bool DangerousInMelee
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x000A9420 File Offset: 0x000A7820
		public override void PawnDied(Corpse corpse)
		{
			float radius;
			if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0)
			{
				radius = 1.9f;
			}
			else if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 1)
			{
				radius = 2.9f;
			}
			else
			{
				radius = 4.9f;
			}
			GenExplosion.DoExplosion(corpse.Position, corpse.Map, radius, DamageDefOf.Flame, corpse.InnerPawn, -1, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
		}
	}
}
