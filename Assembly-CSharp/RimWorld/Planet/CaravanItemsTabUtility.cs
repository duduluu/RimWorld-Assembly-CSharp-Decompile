﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020008DC RID: 2268
	public static class CaravanItemsTabUtility
	{
		// Token: 0x06003406 RID: 13318 RVA: 0x001BD064 File Offset: 0x001BB464
		public static void DoRows(Vector2 size, List<TransferableImmutable> things, Caravan caravan, ref Vector2 scrollPosition, ref float scrollViewHeight)
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 0f, size.x, size.y).ContractedBy(10f);
			Rect viewRect = new Rect(0f, 0f, rect.width - 16f, scrollViewHeight);
			Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, true);
			float num = 0f;
			Widgets.ListSeparator(ref num, viewRect.width, "CaravanItems".Translate());
			if (things.Any<TransferableImmutable>())
			{
				for (int i = 0; i < things.Count; i++)
				{
					CaravanItemsTabUtility.DoRow(ref num, viewRect, rect, scrollPosition, things[i], caravan);
				}
			}
			else
			{
				Widgets.NoneLabel(ref num, viewRect.width, null);
			}
			if (Event.current.type == EventType.Layout)
			{
				scrollViewHeight = num + 30f;
			}
			Widgets.EndScrollView();
		}

		// Token: 0x06003407 RID: 13319 RVA: 0x001BD15C File Offset: 0x001BB55C
		public static Vector2 GetSize(List<TransferableImmutable> things, float paneTopY, bool doNeeds = true)
		{
			float num = 300f;
			num += 24f;
			num += 60f;
			Vector2 result;
			result.x = 103f + num + 16f;
			result.y = Mathf.Min(550f, paneTopY - 30f);
			return result;
		}

		// Token: 0x06003408 RID: 13320 RVA: 0x001BD1B4 File Offset: 0x001BB5B4
		private static void DoRow(ref float curY, Rect viewRect, Rect scrollOutRect, Vector2 scrollPosition, TransferableImmutable thing, Caravan caravan)
		{
			float num = scrollPosition.y - 30f;
			float num2 = scrollPosition.y + scrollOutRect.height;
			if (curY > num && curY < num2)
			{
				CaravanItemsTabUtility.DoRow(new Rect(0f, curY, viewRect.width, 30f), thing, caravan);
			}
			curY += 30f;
		}

		// Token: 0x06003409 RID: 13321 RVA: 0x001BD21C File Offset: 0x001BB61C
		private static void DoRow(Rect rect, TransferableImmutable thing, Caravan caravan)
		{
			GUI.BeginGroup(rect);
			Rect rect2 = rect.AtZero();
			if (thing.TotalStackCount != 1)
			{
				CaravanThingsTabUtility.DoAbandonSpecificCountButton(rect2, thing, caravan);
			}
			rect2.width -= 24f;
			CaravanThingsTabUtility.DoAbandonButton(rect2, thing, caravan);
			rect2.width -= 24f;
			Widgets.InfoCardButton(rect2.width - 24f, (rect.height - 24f) / 2f, thing.AnyThing);
			rect2.width -= 24f;
			Rect rect3 = rect2;
			rect3.xMin = rect3.xMax - 60f;
			CaravanThingsTabUtility.DrawMass(thing, rect3);
			rect2.width -= 60f;
			Widgets.DrawHighlightIfMouseover(rect2);
			Rect rect4 = new Rect(4f, (rect.height - 27f) / 2f, 27f, 27f);
			Widgets.ThingIcon(rect4, thing.AnyThing, 1f);
			Rect rect5 = new Rect(rect4.xMax + 4f, 0f, 300f, 30f);
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;
			Widgets.Label(rect5, thing.LabelCapWithTotalStackCount.Truncate(rect5.width, null));
			Text.Anchor = TextAnchor.UpperLeft;
			Text.WordWrap = true;
			GUI.EndGroup();
		}

		// Token: 0x04001C10 RID: 7184
		private const float RowHeight = 30f;

		// Token: 0x04001C11 RID: 7185
		private const float LabelColumnWidth = 300f;
	}
}