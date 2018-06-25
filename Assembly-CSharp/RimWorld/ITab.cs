﻿using System;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x02000847 RID: 2119
	public abstract class ITab : InspectTabBase
	{
		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06002FF9 RID: 12281 RVA: 0x001A1998 File Offset: 0x0019FD98
		protected object SelObject
		{
			get
			{
				return Find.Selector.SingleSelectedObject;
			}
		}

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06002FFA RID: 12282 RVA: 0x001A19B8 File Offset: 0x0019FDB8
		protected Thing SelThing
		{
			get
			{
				return Find.Selector.SingleSelectedThing;
			}
		}

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06002FFB RID: 12283 RVA: 0x001A19D8 File Offset: 0x0019FDD8
		protected Pawn SelPawn
		{
			get
			{
				return this.SelThing as Pawn;
			}
		}

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06002FFC RID: 12284 RVA: 0x001A19F8 File Offset: 0x0019FDF8
		private MainTabWindow_Inspect InspectPane
		{
			get
			{
				return (MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow;
			}
		}

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06002FFD RID: 12285 RVA: 0x001A1A1C File Offset: 0x0019FE1C
		protected override bool StillValid
		{
			get
			{
				return Find.MainTabsRoot.OpenTab == MainButtonDefOf.Inspect && ((MainTabWindow_Inspect)Find.MainTabsRoot.OpenTab.TabWindow).CurTabs.Contains(this);
			}
		}

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06002FFE RID: 12286 RVA: 0x001A1A68 File Offset: 0x0019FE68
		protected override float PaneTopY
		{
			get
			{
				return this.InspectPane.PaneTopY;
			}
		}

		// Token: 0x06002FFF RID: 12287 RVA: 0x001A1A88 File Offset: 0x0019FE88
		protected override void CloseTab()
		{
			this.InspectPane.CloseOpenTab();
		}
	}
}
