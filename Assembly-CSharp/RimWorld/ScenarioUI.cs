﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000836 RID: 2102
	public static class ScenarioUI
	{
		// Token: 0x040019B6 RID: 6582
		private static float editViewHeight;

		// Token: 0x06002F97 RID: 12183 RVA: 0x00197BB8 File Offset: 0x00195FB8
		public static void DrawScenarioInfo(Rect rect, Scenario scen, ref Vector2 infoScrollPosition)
		{
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();
			if (scen != null)
			{
				string fullInformationText = scen.GetFullInformationText();
				float width = rect.width - 16f;
				float height = 30f + Text.CalcHeight(fullInformationText, width) + 100f;
				Rect viewRect = new Rect(0f, 0f, width, height);
				Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect, true);
				Text.Font = GameFont.Medium;
				Rect rect2 = new Rect(0f, 0f, viewRect.width, 30f);
				Widgets.Label(rect2, scen.name);
				Text.Font = GameFont.Small;
				Rect rect3 = new Rect(0f, 30f, viewRect.width, viewRect.height - 30f);
				Widgets.Label(rect3, fullInformationText);
				Widgets.EndScrollView();
			}
		}

		// Token: 0x06002F98 RID: 12184 RVA: 0x00197C90 File Offset: 0x00196090
		public static void DrawScenarioEditInterface(Rect rect, Scenario scen, ref Vector2 infoScrollPosition)
		{
			Widgets.DrawMenuSection(rect);
			rect = rect.GetInnerRect();
			if (scen != null)
			{
				Rect viewRect = new Rect(0f, 0f, rect.width - 16f, ScenarioUI.editViewHeight);
				Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect, true);
				Rect rect2 = new Rect(0f, 0f, viewRect.width, 99999f);
				Listing_ScenEdit listing_ScenEdit = new Listing_ScenEdit(scen);
				listing_ScenEdit.ColumnWidth = rect2.width;
				listing_ScenEdit.Begin(rect2);
				listing_ScenEdit.Label("Title".Translate(), -1f, null);
				scen.name = listing_ScenEdit.TextEntry(scen.name, 1).TrimmedToLength(55);
				listing_ScenEdit.Label("Summary".Translate(), -1f, null);
				scen.summary = listing_ScenEdit.TextEntry(scen.summary, 2).TrimmedToLength(300);
				listing_ScenEdit.Label("Description".Translate(), -1f, null);
				scen.description = listing_ScenEdit.TextEntry(scen.description, 4).TrimmedToLength(1000);
				listing_ScenEdit.Gap(12f);
				foreach (ScenPart scenPart in scen.AllParts)
				{
					scenPart.DoEditInterface(listing_ScenEdit);
				}
				listing_ScenEdit.End();
				ScenarioUI.editViewHeight = listing_ScenEdit.CurHeight + 100f;
				Widgets.EndScrollView();
			}
		}
	}
}
