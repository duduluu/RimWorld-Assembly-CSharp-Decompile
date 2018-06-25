﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020001F5 RID: 501
	public class Thought_OpinionOfMyLover : Thought_Situational
	{
		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060009B2 RID: 2482 RVA: 0x00057954 File Offset: 0x00055D54
		public override string LabelCap
		{
			get
			{
				DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(this.pawn, false);
				return string.Format(base.CurStage.label, directPawnRelation.def.GetGenderSpecificLabel(directPawnRelation.otherPawn), directPawnRelation.otherPawn.LabelShort).CapitalizeFirst();
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060009B3 RID: 2483 RVA: 0x000579A8 File Offset: 0x00055DA8
		protected override float BaseMoodOffset
		{
			get
			{
				float num = 0.1f * (float)this.pawn.relations.OpinionOf(LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(this.pawn, false).otherPawn);
				float result;
				if (num < 0f)
				{
					result = Mathf.Min(num, -1f);
				}
				else
				{
					result = Mathf.Max(num, 1f);
				}
				return result;
			}
		}
	}
}
