﻿using System;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000E99 RID: 3737
	public static class GhostDrawer
	{
		// Token: 0x0600585D RID: 22621 RVA: 0x002D4C14 File Offset: 0x002D3014
		public static void DrawGhostThing(IntVec3 center, Rot4 rot, ThingDef thingDef, Graphic baseGraphic, Color ghostCol, AltitudeLayer drawAltitude)
		{
			if (baseGraphic == null)
			{
				baseGraphic = thingDef.graphic;
			}
			Graphic graphic = GhostUtility.GhostGraphicFor(baseGraphic, thingDef, ghostCol);
			Vector3 loc = GenThing.TrueCenter(center, rot, thingDef.Size, drawAltitude.AltitudeFor());
			graphic.DrawFromDef(loc, rot, thingDef, 0f);
			for (int i = 0; i < thingDef.comps.Count; i++)
			{
				thingDef.comps[i].DrawGhost(center, rot, thingDef, ghostCol, drawAltitude);
			}
			if (thingDef.PlaceWorkers != null)
			{
				for (int j = 0; j < thingDef.PlaceWorkers.Count; j++)
				{
					thingDef.PlaceWorkers[j].DrawGhost(thingDef, center, rot, ghostCol);
				}
			}
		}
	}
}
