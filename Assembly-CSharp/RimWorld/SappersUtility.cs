﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x0200018A RID: 394
	public static class SappersUtility
	{
		// Token: 0x0600082D RID: 2093 RVA: 0x0004EBC0 File Offset: 0x0004CFC0
		public static bool IsGoodSapper(Pawn p)
		{
			return p.kindDef.canBeSapper && SappersUtility.HasBuildingDestroyerWeapon(p) && SappersUtility.CanMineReasonablyFast(p);
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0004EBFC File Offset: 0x0004CFFC
		public static bool IsGoodBackupSapper(Pawn p)
		{
			return p.kindDef.canBeSapper && SappersUtility.CanMineReasonablyFast(p);
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0004EC2C File Offset: 0x0004D02C
		private static bool CanMineReasonablyFast(Pawn p)
		{
			return p.RaceProps.Humanlike && !p.skills.GetSkill(SkillDefOf.Mining).TotallyDisabled && !StatDefOf.MiningSpeed.Worker.IsDisabledFor(p) && p.skills.GetSkill(SkillDefOf.Mining).Level >= 4;
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x0004ECA0 File Offset: 0x0004D0A0
		public static bool HasBuildingDestroyerWeapon(Pawn p)
		{
			bool result;
			if (p.equipment == null || p.equipment.Primary == null)
			{
				result = false;
			}
			else
			{
				List<Verb> allVerbs = p.equipment.Primary.GetComp<CompEquippable>().AllVerbs;
				for (int i = 0; i < allVerbs.Count; i++)
				{
					if (allVerbs[i].verbProps.ai_IsBuildingDestroyer)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}
	}
}