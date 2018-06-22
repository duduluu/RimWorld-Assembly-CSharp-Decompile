﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020007CB RID: 1995
	public class Designator_Forbid : Designator
	{
		// Token: 0x06002C3C RID: 11324 RVA: 0x00175A7C File Offset: 0x00173E7C
		public Designator_Forbid()
		{
			this.defaultLabel = "DesignatorForbid".Translate();
			this.defaultDesc = "DesignatorForbidDesc".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt", true);
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.Designate_Claim;
			this.hotKey = KeyBindingDefOf.Command_ItemForbid;
			this.hasDesignateAllFloatMenuOption = true;
			this.designateAllLabel = "ForbidAllItems".Translate();
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06002C3D RID: 11325 RVA: 0x00175B0C File Offset: 0x00173F0C
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06002C3E RID: 11326 RVA: 0x00175B24 File Offset: 0x00173F24
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			AcceptanceReport result;
			if (!c.InBounds(base.Map) || c.Fogged(base.Map))
			{
				result = false;
			}
			else if (!c.GetThingList(base.Map).Any((Thing t) => this.CanDesignateThing(t).Accepted))
			{
				result = "MessageMustDesignateForbiddable".Translate();
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06002C3F RID: 11327 RVA: 0x00175BA4 File Offset: 0x00173FA4
		public override void DesignateSingleCell(IntVec3 c)
		{
			List<Thing> thingList = c.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (this.CanDesignateThing(thingList[i]).Accepted)
				{
					this.DesignateThing(thingList[i]);
				}
			}
		}

		// Token: 0x06002C40 RID: 11328 RVA: 0x00175C00 File Offset: 0x00174000
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			AcceptanceReport result;
			if (t.def.category != ThingCategory.Item)
			{
				result = false;
			}
			else
			{
				CompForbiddable compForbiddable = t.TryGetComp<CompForbiddable>();
				result = (compForbiddable != null && !compForbiddable.Forbidden);
			}
			return result;
		}

		// Token: 0x06002C41 RID: 11329 RVA: 0x00175C50 File Offset: 0x00174050
		public override void DesignateThing(Thing t)
		{
			t.SetForbidden(true, false);
		}
	}
}