﻿using System;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020008A3 RID: 2211
	public static class TargetHighlighter
	{
		// Token: 0x04001B21 RID: 6945
		private static List<Vector3> arrowPositions = new List<Vector3>();

		// Token: 0x04001B22 RID: 6946
		private static List<Pair<Vector3, float>> circleOverlays = new List<Pair<Vector3, float>>();

		// Token: 0x0600329A RID: 12954 RVA: 0x001B42A0 File Offset: 0x001B26A0
		public static void Highlight(GlobalTargetInfo target, bool arrow = true, bool colonistBar = true, bool circleOverlay = false)
		{
			if (target.IsValid)
			{
				if (arrow)
				{
					GlobalTargetInfo adjustedTarget = CameraJumper.GetAdjustedTarget(target);
					if (adjustedTarget.IsMapTarget && adjustedTarget.Map != null && adjustedTarget.Map == Find.CurrentMap)
					{
						Vector3 centerVector = ((TargetInfo)adjustedTarget).CenterVector3;
						if (!TargetHighlighter.arrowPositions.Contains(centerVector))
						{
							TargetHighlighter.arrowPositions.Add(centerVector);
						}
					}
				}
				if (colonistBar)
				{
					if (target.Thing is Pawn)
					{
						Find.ColonistBar.Highlight((Pawn)target.Thing);
					}
					else if (target.Thing is Corpse)
					{
						Find.ColonistBar.Highlight(((Corpse)target.Thing).InnerPawn);
					}
					else if (target.WorldObject is Caravan)
					{
						Caravan caravan = (Caravan)target.WorldObject;
						if (caravan != null)
						{
							for (int i = 0; i < caravan.pawns.Count; i++)
							{
								Find.ColonistBar.Highlight(caravan.pawns[i]);
							}
						}
					}
				}
				if (circleOverlay)
				{
					if (target.Thing != null && target.Thing.Spawned && target.Thing.Map == Find.CurrentMap)
					{
						Pawn pawn = target.Thing as Pawn;
						float num;
						if (pawn != null)
						{
							if (pawn.RaceProps.Humanlike)
							{
								num = 1.6f;
							}
							else
							{
								num = pawn.ageTracker.CurLifeStage.bodySizeFactor * pawn.ageTracker.CurKindLifeStage.bodyGraphicData.drawSize.y;
								num = Mathf.Max(num, 1f);
							}
						}
						else
						{
							num = (float)Mathf.Max(target.Thing.def.size.x, target.Thing.def.size.z);
						}
						Pair<Vector3, float> item = new Pair<Vector3, float>(target.Thing.DrawPos, num * 0.5f);
						if (!TargetHighlighter.circleOverlays.Contains(item))
						{
							TargetHighlighter.circleOverlays.Add(item);
						}
					}
				}
			}
		}

		// Token: 0x0600329B RID: 12955 RVA: 0x001B450C File Offset: 0x001B290C
		public static void TargetHighlighterUpdate()
		{
			for (int i = 0; i < TargetHighlighter.arrowPositions.Count; i++)
			{
				GenDraw.DrawArrowPointingAt(TargetHighlighter.arrowPositions[i], false);
			}
			TargetHighlighter.arrowPositions.Clear();
			for (int j = 0; j < TargetHighlighter.circleOverlays.Count; j++)
			{
				GenDraw.DrawCircleOutline(TargetHighlighter.circleOverlays[j].First, TargetHighlighter.circleOverlays[j].Second);
			}
			TargetHighlighter.circleOverlays.Clear();
		}
	}
}
