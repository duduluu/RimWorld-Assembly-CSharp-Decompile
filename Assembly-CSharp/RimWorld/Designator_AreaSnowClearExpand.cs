﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020007C6 RID: 1990
	public class Designator_AreaSnowClearExpand : Designator_AreaSnowClear
	{
		// Token: 0x06002C0E RID: 11278 RVA: 0x00174EF8 File Offset: 0x001732F8
		public Designator_AreaSnowClearExpand() : base(DesignateMode.Add)
		{
			this.defaultLabel = "DesignatorAreaSnowClearExpand".Translate();
			this.defaultDesc = "DesignatorAreaSnowClearExpandDesc".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/SnowClearAreaOn", true);
			this.soundDragSustain = SoundDefOf.Designate_DragAreaAdd;
			this.soundDragChanged = null;
			this.soundSucceeded = SoundDefOf.Designate_AreaAdd;
		}
	}
}
