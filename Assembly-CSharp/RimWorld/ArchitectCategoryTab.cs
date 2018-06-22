﻿using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000843 RID: 2115
	public class ArchitectCategoryTab
	{
		// Token: 0x06002FE6 RID: 12262 RVA: 0x001A0E59 File Offset: 0x0019F259
		public ArchitectCategoryTab(DesignationCategoryDef def)
		{
			this.def = def;
		}

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06002FE7 RID: 12263 RVA: 0x001A0E6C File Offset: 0x0019F26C
		public static Rect InfoRect
		{
			get
			{
				return new Rect(0f, (float)(UI.screenHeight - 35) - ((MainTabWindow_Architect)MainButtonDefOf.Architect.TabWindow).WinHeight - 270f, 200f, 270f);
			}
		}

		// Token: 0x06002FE8 RID: 12264 RVA: 0x001A0EBC File Offset: 0x0019F2BC
		public void DesignationTabOnGUI()
		{
			if (Find.DesignatorManager.SelectedDesignator != null)
			{
				Find.DesignatorManager.SelectedDesignator.DoExtraGuiControls(0f, (float)(UI.screenHeight - 35) - ((MainTabWindow_Architect)MainButtonDefOf.Architect.TabWindow).WinHeight - 270f);
			}
			float startX = 210f;
			Gizmo selectedDesignator;
			GizmoGridDrawer.DrawGizmoGrid(this.def.ResolvedAllowedDesignators.Cast<Gizmo>(), startX, out selectedDesignator);
			if (selectedDesignator == null && Find.DesignatorManager.SelectedDesignator != null)
			{
				selectedDesignator = Find.DesignatorManager.SelectedDesignator;
			}
			this.DoInfoBox(ArchitectCategoryTab.InfoRect, (Designator)selectedDesignator);
		}

		// Token: 0x06002FE9 RID: 12265 RVA: 0x001A0F60 File Offset: 0x0019F360
		protected void DoInfoBox(Rect infoRect, Designator designator)
		{
			Find.WindowStack.ImmediateWindow(32520, infoRect, WindowLayer.GameUI, delegate
			{
				if (designator != null)
				{
					Rect position = infoRect.AtZero().ContractedBy(7f);
					GUI.BeginGroup(position);
					Rect rect = new Rect(0f, 0f, position.width - designator.PanelReadoutTitleExtraRightMargin, 999f);
					Text.Font = GameFont.Small;
					Widgets.Label(rect, designator.LabelCap);
					float num = Mathf.Max(24f, Text.CalcHeight(designator.LabelCap, rect.width));
					designator.DrawPanelReadout(ref num, position.width);
					Rect rect2 = new Rect(0f, num, position.width, position.height - num);
					string desc = designator.Desc;
					GenText.SetTextSizeToFit(desc, rect2);
					Widgets.Label(rect2, desc);
					GUI.EndGroup();
				}
			}, true, false, 1f);
		}

		// Token: 0x040019EC RID: 6636
		public DesignationCategoryDef def;

		// Token: 0x040019ED RID: 6637
		public const float InfoRectHeight = 270f;
	}
}
