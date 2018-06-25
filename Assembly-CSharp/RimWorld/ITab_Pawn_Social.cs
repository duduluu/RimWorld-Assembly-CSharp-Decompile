﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000859 RID: 2137
	public class ITab_Pawn_Social : ITab
	{
		// Token: 0x04001A3E RID: 6718
		public const float Width = 540f;

		// Token: 0x06003063 RID: 12387 RVA: 0x001A56BA File Offset: 0x001A3ABA
		public ITab_Pawn_Social()
		{
			this.size = new Vector2(540f, 510f);
			this.labelKey = "TabSocial";
			this.tutorTag = "Social";
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06003064 RID: 12388 RVA: 0x001A56F0 File Offset: 0x001A3AF0
		public override bool IsVisible
		{
			get
			{
				return this.SelPawnForSocialInfo.RaceProps.IsFlesh;
			}
		}

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06003065 RID: 12389 RVA: 0x001A5718 File Offset: 0x001A3B18
		private Pawn SelPawnForSocialInfo
		{
			get
			{
				Pawn result;
				if (base.SelPawn != null)
				{
					result = base.SelPawn;
				}
				else
				{
					Corpse corpse = base.SelThing as Corpse;
					if (corpse == null)
					{
						throw new InvalidOperationException("Social tab on non-pawn non-corpse " + base.SelThing);
					}
					result = corpse.InnerPawn;
				}
				return result;
			}
		}

		// Token: 0x06003066 RID: 12390 RVA: 0x001A5774 File Offset: 0x001A3B74
		protected override void FillTab()
		{
			SocialCardUtility.DrawSocialCard(new Rect(0f, 0f, this.size.x, this.size.y), this.SelPawnForSocialInfo);
		}
	}
}
