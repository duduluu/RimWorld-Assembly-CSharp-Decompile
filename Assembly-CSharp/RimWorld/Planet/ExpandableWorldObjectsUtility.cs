﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x0200059B RID: 1435
	public static class ExpandableWorldObjectsUtility
	{
		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06001B63 RID: 7011 RVA: 0x000EC8F4 File Offset: 0x000EACF4
		public static float TransitionPct
		{
			get
			{
				float result;
				if (!Find.PlaySettings.showExpandingIcons)
				{
					result = 0f;
				}
				else
				{
					result = ExpandableWorldObjectsUtility.transitionPct;
				}
				return result;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06001B64 RID: 7012 RVA: 0x000EC928 File Offset: 0x000EAD28
		public static float ExpandMoreTransitionPct
		{
			get
			{
				float result;
				if (!Find.PlaySettings.showExpandingIcons)
				{
					result = 0f;
				}
				else
				{
					result = ExpandableWorldObjectsUtility.expandMoreTransitionPct;
				}
				return result;
			}
		}

		// Token: 0x06001B65 RID: 7013 RVA: 0x000EC95C File Offset: 0x000EAD5C
		public static void ExpandableWorldObjectsUpdate()
		{
			float num = Time.deltaTime * 3f;
			if (Find.WorldCameraDriver.CurrentZoom <= WorldCameraZoomRange.VeryClose)
			{
				ExpandableWorldObjectsUtility.transitionPct -= num;
			}
			else
			{
				ExpandableWorldObjectsUtility.transitionPct += num;
			}
			ExpandableWorldObjectsUtility.transitionPct = Mathf.Clamp01(ExpandableWorldObjectsUtility.transitionPct);
			float num2 = Time.deltaTime * 4f;
			if (Find.WorldCameraDriver.CurrentZoom <= WorldCameraZoomRange.Far)
			{
				ExpandableWorldObjectsUtility.expandMoreTransitionPct -= num2;
			}
			else
			{
				ExpandableWorldObjectsUtility.expandMoreTransitionPct += num2;
			}
			ExpandableWorldObjectsUtility.expandMoreTransitionPct = Mathf.Clamp01(ExpandableWorldObjectsUtility.expandMoreTransitionPct);
		}

		// Token: 0x06001B66 RID: 7014 RVA: 0x000EC9FC File Offset: 0x000EADFC
		public static void ExpandableWorldObjectsOnGUI()
		{
			if (ExpandableWorldObjectsUtility.TransitionPct != 0f)
			{
				ExpandableWorldObjectsUtility.tmpWorldObjects.Clear();
				ExpandableWorldObjectsUtility.tmpWorldObjects.AddRange(Find.WorldObjects.AllWorldObjects);
				ExpandableWorldObjectsUtility.SortByExpandingIconPriority(ExpandableWorldObjectsUtility.tmpWorldObjects);
				WorldTargeter worldTargeter = Find.WorldTargeter;
				List<WorldObject> worldObjectsUnderMouse = null;
				if (worldTargeter.IsTargeting)
				{
					worldObjectsUnderMouse = GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
				}
				for (int i = 0; i < ExpandableWorldObjectsUtility.tmpWorldObjects.Count; i++)
				{
					WorldObject worldObject = ExpandableWorldObjectsUtility.tmpWorldObjects[i];
					if (worldObject.def.expandingIcon)
					{
						if (!worldObject.HiddenBehindTerrainNow())
						{
							Color expandingIconColor = worldObject.ExpandingIconColor;
							expandingIconColor.a = ExpandableWorldObjectsUtility.TransitionPct;
							if (worldTargeter.IsTargetedNow(worldObject, worldObjectsUnderMouse))
							{
								float num = GenMath.LerpDouble(-1f, 1f, 0.7f, 1f, Mathf.Sin(Time.time * 8f));
								expandingIconColor.r *= num;
								expandingIconColor.g *= num;
								expandingIconColor.b *= num;
							}
							GUI.color = expandingIconColor;
							GUI.DrawTexture(ExpandableWorldObjectsUtility.ExpandedIconScreenRect(worldObject), worldObject.ExpandingIcon);
						}
					}
				}
				ExpandableWorldObjectsUtility.tmpWorldObjects.Clear();
				GUI.color = Color.white;
			}
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x000ECB5C File Offset: 0x000EAF5C
		public static Rect ExpandedIconScreenRect(WorldObject o)
		{
			Vector2 vector = o.ScreenPos();
			float num;
			if (o.ExpandMore)
			{
				num = Mathf.Lerp(30f, 40.5f, ExpandableWorldObjectsUtility.ExpandMoreTransitionPct);
			}
			else
			{
				num = 30f;
			}
			return new Rect(vector.x - num / 2f, vector.y - num / 2f, num, num);
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x000ECBCC File Offset: 0x000EAFCC
		public static bool IsExpanded(WorldObject o)
		{
			return ExpandableWorldObjectsUtility.TransitionPct > 0.5f && o.def.expandingIcon;
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x000ECC00 File Offset: 0x000EB000
		public static void GetExpandedWorldObjectUnderMouse(Vector2 mousePos, List<WorldObject> outList)
		{
			outList.Clear();
			Vector2 point = mousePos;
			point.y = (float)UI.screenHeight - point.y;
			List<WorldObject> allWorldObjects = Find.WorldObjects.AllWorldObjects;
			for (int i = 0; i < allWorldObjects.Count; i++)
			{
				WorldObject worldObject = allWorldObjects[i];
				if (ExpandableWorldObjectsUtility.IsExpanded(worldObject))
				{
					if (ExpandableWorldObjectsUtility.ExpandedIconScreenRect(worldObject).Contains(point))
					{
						if (!worldObject.HiddenBehindTerrainNow())
						{
							outList.Add(worldObject);
						}
					}
				}
			}
			ExpandableWorldObjectsUtility.SortByExpandingIconPriority(outList);
			outList.Reverse();
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x000ECCA4 File Offset: 0x000EB0A4
		private static void SortByExpandingIconPriority(List<WorldObject> worldObjects)
		{
			worldObjects.SortBy(delegate(WorldObject x)
			{
				float num = x.ExpandingIconPriority;
				if (x.Faction != null && x.Faction.IsPlayer)
				{
					num += 0.001f;
				}
				return num;
			}, (WorldObject x) => x.ID);
		}

		// Token: 0x04001024 RID: 4132
		private static float transitionPct;

		// Token: 0x04001025 RID: 4133
		private static float expandMoreTransitionPct;

		// Token: 0x04001026 RID: 4134
		private static List<WorldObject> tmpWorldObjects = new List<WorldObject>();

		// Token: 0x04001027 RID: 4135
		private const float WorldObjectIconSize = 30f;

		// Token: 0x04001028 RID: 4136
		private const float ExpandMoreWorldObjectIconSizeFactor = 1.35f;

		// Token: 0x04001029 RID: 4137
		private const float TransitionSpeed = 3f;

		// Token: 0x0400102A RID: 4138
		private const float ExpandMoreTransitionSpeed = 4f;
	}
}