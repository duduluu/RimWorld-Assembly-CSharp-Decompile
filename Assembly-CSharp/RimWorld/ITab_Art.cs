﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000848 RID: 2120
	public class ITab_Art : ITab
	{
		// Token: 0x040019F5 RID: 6645
		private static string cachedImageDescription;

		// Token: 0x040019F6 RID: 6646
		private static CompArt cachedImageSource;

		// Token: 0x040019F7 RID: 6647
		private static TaleReference cachedTaleRef;

		// Token: 0x040019F8 RID: 6648
		private static readonly Vector2 WinSize = new Vector2(400f, 300f);

		// Token: 0x06003000 RID: 12288 RVA: 0x001A1A96 File Offset: 0x0019FE96
		public ITab_Art()
		{
			this.size = ITab_Art.WinSize;
			this.labelKey = "TabArt";
			this.tutorTag = "Art";
		}

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x06003001 RID: 12289 RVA: 0x001A1AC0 File Offset: 0x0019FEC0
		private CompArt SelectedCompArt
		{
			get
			{
				Thing thing = Find.Selector.SingleSelectedThing;
				MinifiedThing minifiedThing = thing as MinifiedThing;
				if (minifiedThing != null)
				{
					thing = minifiedThing.InnerThing;
				}
				CompArt result;
				if (thing == null)
				{
					result = null;
				}
				else
				{
					result = thing.TryGetComp<CompArt>();
				}
				return result;
			}
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x06003002 RID: 12290 RVA: 0x001A1B08 File Offset: 0x0019FF08
		public override bool IsVisible
		{
			get
			{
				return this.SelectedCompArt != null && this.SelectedCompArt.Active;
			}
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x001A1B38 File Offset: 0x0019FF38
		protected override void FillTab()
		{
			Rect rect = new Rect(0f, 0f, ITab_Art.WinSize.x, ITab_Art.WinSize.y).ContractedBy(10f);
			Rect rect2 = rect;
			Text.Font = GameFont.Medium;
			Widgets.Label(rect2, this.SelectedCompArt.Title);
			if (ITab_Art.cachedImageSource != this.SelectedCompArt || ITab_Art.cachedTaleRef != this.SelectedCompArt.TaleRef)
			{
				ITab_Art.cachedImageDescription = this.SelectedCompArt.GenerateImageDescription();
				ITab_Art.cachedImageSource = this.SelectedCompArt;
				ITab_Art.cachedTaleRef = this.SelectedCompArt.TaleRef;
			}
			Rect rect3 = rect;
			rect3.yMin += 35f;
			Text.Font = GameFont.Small;
			Widgets.Label(rect3, ITab_Art.cachedImageDescription);
		}
	}
}
