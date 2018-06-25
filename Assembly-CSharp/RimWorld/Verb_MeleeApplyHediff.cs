﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020009D5 RID: 2517
	public class Verb_MeleeApplyHediff : Verb_MeleeAttack
	{
		// Token: 0x06003869 RID: 14441 RVA: 0x001E2950 File Offset: 0x001E0D50
		protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
		{
			DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
			DamageWorker.DamageResult result;
			if (this.tool == null)
			{
				Log.ErrorOnce("Attempted to apply melee hediff without a tool", 38381735, false);
				result = damageResult;
			}
			else
			{
				Pawn pawn = target.Thing as Pawn;
				if (pawn == null)
				{
					Log.ErrorOnce("Attempted to apply melee hediff without pawn target", 78330053, false);
					result = damageResult;
				}
				else
				{
					HediffSet hediffSet = pawn.health.hediffSet;
					BodyPartTagDef bodypartTagTarget = this.verbProps.bodypartTagTarget;
					foreach (BodyPartRecord part in hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, bodypartTagTarget))
					{
						damageResult.AddHediff(pawn.health.AddHediff(this.tool.hediff, part, null, null));
						damageResult.AddPart(pawn, part);
						damageResult.wounded = true;
					}
					result = damageResult;
				}
			}
			return result;
		}

		// Token: 0x0600386A RID: 14442 RVA: 0x001E2A5C File Offset: 0x001E0E5C
		public override bool IsUsableOn(Thing target)
		{
			return target is Pawn;
		}
	}
}
