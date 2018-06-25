﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020009D2 RID: 2514
	public class Verb_BeatFire : Verb
	{
		// Token: 0x0400240F RID: 9231
		private const int DamageAmount = 32;

		// Token: 0x06003861 RID: 14433 RVA: 0x001E1EC0 File Offset: 0x001E02C0
		public Verb_BeatFire()
		{
			this.verbProps = NativeVerbPropertiesDatabase.VerbWithCategory(VerbCategory.BeatFire);
		}

		// Token: 0x06003862 RID: 14434 RVA: 0x001E1ED8 File Offset: 0x001E02D8
		protected override bool TryCastShot()
		{
			Fire fire = (Fire)this.currentTarget.Thing;
			Pawn casterPawn = base.CasterPawn;
			bool result;
			if (casterPawn.stances.FullBodyBusy)
			{
				result = false;
			}
			else
			{
				Thing thing = fire;
				DamageDef extinguish = DamageDefOf.Extinguish;
				float amount = 32f;
				Thing caster = this.caster;
				thing.TakeDamage(new DamageInfo(extinguish, amount, -1f, caster, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
				casterPawn.Drawer.Notify_MeleeAttackOn(fire);
				result = true;
			}
			return result;
		}
	}
}
