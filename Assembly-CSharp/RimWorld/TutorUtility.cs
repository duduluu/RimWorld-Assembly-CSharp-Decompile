﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public static class TutorUtility
	{
		public static bool BuildingOrBlueprintOrFrameCenterExists(IntVec3 c, Map map, ThingDef buildingDef)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing = thingList[i];
				if (!(thing.Position != c))
				{
					if (thing.def == buildingDef)
					{
						return true;
					}
					if (thing.def.entityDefToBuild == buildingDef)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static CellRect FindUsableRect(int width, int height, Map map, float minFertility = 0f, bool noItems = false)
		{
			IntVec3 center = map.Center;
			float num = 1f;
			CellRect cellRect;
			for (;;)
			{
				IntVec3 center2 = center + new IntVec3((int)Rand.Range(-num, num), 0, (int)Rand.Range(-num, num));
				cellRect = CellRect.CenteredOn(center2, width / 2);
				cellRect.Width = width;
				cellRect.Height = height;
				cellRect = cellRect.ExpandedBy(1);
				bool flag = true;
				CellRect.CellRectIterator iterator = cellRect.GetIterator();
				while (!iterator.Done())
				{
					IntVec3 intVec = iterator.Current;
					if (intVec.Fogged(map) || !intVec.Walkable(map) || !intVec.GetTerrain(map).affordances.Contains(TerrainAffordanceDefOf.Heavy) || intVec.GetTerrain(map).fertility < minFertility || intVec.GetZone(map) != null || TutorUtility.ContainsBlockingThing(intVec, map, noItems) || intVec.InNoBuildEdgeArea(map) || intVec.InNoZoneEdgeArea(map))
					{
						flag = false;
						break;
					}
					iterator.MoveNext();
				}
				if (flag)
				{
					break;
				}
				num += 0.25f;
			}
			return cellRect.ContractedBy(1);
		}

		private static bool ContainsBlockingThing(IntVec3 cell, Map map, bool noItems)
		{
			List<Thing> thingList = cell.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (thingList[i].def.category == ThingCategory.Building)
				{
					return true;
				}
				if (thingList[i] is Blueprint)
				{
					return true;
				}
				if (noItems && thingList[i].def.category == ThingCategory.Item)
				{
					return true;
				}
			}
			return false;
		}

		public static void DrawLabelOnThingOnGUI(Thing t, string label)
		{
			Vector2 vector = (t.DrawPos + new Vector3(0f, 0f, 0.5f)).MapToUIPosition();
			Vector2 vector2 = Text.CalcSize(label);
			Rect rect = new Rect(vector.x - vector2.x / 2f, vector.y - vector2.y / 2f, vector2.x, vector2.y);
			GUI.DrawTexture(rect, TexUI.GrayTextBG);
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, label);
			Text.Anchor = TextAnchor.UpperLeft;
		}

		public static void DrawLabelOnGUI(Vector3 mapPos, string label)
		{
			Vector2 vector = mapPos.MapToUIPosition();
			Vector2 vector2 = Text.CalcSize(label);
			Rect rect = new Rect(vector.x - vector2.x / 2f, vector.y - vector2.y / 2f, vector2.x, vector2.y);
			GUI.DrawTexture(rect, TexUI.GrayTextBG);
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, label);
			Text.Anchor = TextAnchor.UpperLeft;
		}

		public static void DrawCellRectOnGUI(CellRect cellRect, string label = null)
		{
			if (label != null)
			{
				Vector3 centerVector = cellRect.CenterVector3;
				TutorUtility.DrawLabelOnGUI(centerVector, label);
			}
		}

		public static void DrawCellRectUpdate(CellRect cellRect)
		{
			CellRect.CellRectIterator iterator = cellRect.GetIterator();
			while (!iterator.Done())
			{
				CellRenderer.RenderCell(iterator.Current, 0.5f);
				iterator.MoveNext();
			}
		}

		public static void DoModalDialogIfNotKnown(ConceptDef conc)
		{
			if (!PlayerKnowledgeDatabase.IsComplete(conc))
			{
				string helpTextAdjusted = conc.HelpTextAdjusted;
				Find.WindowStack.Add(new Dialog_MessageBox(helpTextAdjusted, null, null, null, null, null, false, null, null));
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(conc, KnowledgeAmount.Total);
			}
		}

		public static bool EventCellsMatchExactly(EventPack ep, List<IntVec3> targetCells)
		{
			if (ep.Cell.IsValid)
			{
				return targetCells.Count == 1 && ep.Cell == targetCells[0];
			}
			if (ep.Cells == null)
			{
				return false;
			}
			int num = 0;
			foreach (IntVec3 item in ep.Cells)
			{
				if (!targetCells.Contains(item))
				{
					return false;
				}
				num++;
			}
			return num == targetCells.Count;
		}

		public static bool EventCellsAreWithin(EventPack ep, List<IntVec3> targetCells)
		{
			if (ep.Cell.IsValid)
			{
				return targetCells.Contains(ep.Cell);
			}
			return ep.Cells != null && !ep.Cells.Any((IntVec3 c) => !targetCells.Contains(c));
		}

		[CompilerGenerated]
		private sealed class <EventCellsAreWithin>c__AnonStorey0
		{
			internal List<IntVec3> targetCells;

			public <EventCellsAreWithin>c__AnonStorey0()
			{
			}

			internal bool <>m__0(IntVec3 c)
			{
				return !this.targetCells.Contains(c);
			}
		}
	}
}
