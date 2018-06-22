﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000C7A RID: 3194
	public class PlaceWorker_TurretTop : PlaceWorker
	{
		// Token: 0x06004601 RID: 17921 RVA: 0x0024E73C File Offset: 0x0024CB3C
		public override void DrawGhost(ThingDef def, IntVec3 loc, Rot4 rot, Color ghostCol)
		{
			Graphic baseGraphic = GraphicDatabase.Get<Graphic_Single>(def.building.turretTopGraphicPath, ShaderDatabase.Cutout, new Vector2(def.building.turretTopDrawSize, def.building.turretTopDrawSize), Color.white);
			Graphic graphic = GhostUtility.GhostGraphicFor(baseGraphic, def, ghostCol);
			graphic.DrawFromDef(GenThing.TrueCenter(loc, rot, def.Size, AltitudeLayer.MetaOverlays.AltitudeFor()), rot, def, 0f);
		}
	}
}