﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020007C8 RID: 1992
	public class Designator_Cancel : Designator
	{
		// Token: 0x0400179E RID: 6046
		private static HashSet<Thing> seenThings = new HashSet<Thing>();

		// Token: 0x06002C10 RID: 11280 RVA: 0x00174FC0 File Offset: 0x001733C0
		public Designator_Cancel()
		{
			this.defaultLabel = "DesignatorCancel".Translate();
			this.defaultDesc = "DesignatorCancelDesc".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true);
			this.useMouseIcon = true;
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.soundSucceeded = SoundDefOf.Designate_Cancel;
			this.hotKey = KeyBindingDefOf.Designator_Cancel;
			this.tutorTag = "Cancel";
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06002C11 RID: 11281 RVA: 0x00175044 File Offset: 0x00173444
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06002C12 RID: 11282 RVA: 0x0017505C File Offset: 0x0017345C
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			AcceptanceReport result;
			if (!c.InBounds(base.Map))
			{
				result = false;
			}
			else if (this.CancelableDesignationsAt(c).Count<Designation>() > 0)
			{
				result = true;
			}
			else
			{
				List<Thing> thingList = c.GetThingList(base.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					if (this.CanDesignateThing(thingList[i]).Accepted)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06002C13 RID: 11283 RVA: 0x00175100 File Offset: 0x00173500
		public override void DesignateSingleCell(IntVec3 c)
		{
			foreach (Designation designation in this.CancelableDesignationsAt(c).ToList<Designation>())
			{
				if (designation.def.designateCancelable)
				{
					base.Map.designationManager.RemoveDesignation(designation);
				}
			}
			List<Thing> thingList = c.GetThingList(base.Map);
			for (int i = thingList.Count - 1; i >= 0; i--)
			{
				if (this.CanDesignateThing(thingList[i]).Accepted)
				{
					this.DesignateThing(thingList[i]);
				}
			}
		}

		// Token: 0x06002C14 RID: 11284 RVA: 0x001751D0 File Offset: 0x001735D0
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			if (base.Map.designationManager.DesignationOn(t) != null)
			{
				foreach (Designation designation in base.Map.designationManager.AllDesignationsOn(t))
				{
					if (designation.def.designateCancelable)
					{
						return true;
					}
				}
			}
			AcceptanceReport result;
			if (t.def.mineable && base.Map.designationManager.DesignationAt(t.Position, DesignationDefOf.Mine) != null)
			{
				result = true;
			}
			else
			{
				result = (t.Faction == Faction.OfPlayer && (t is Frame || t is Blueprint));
			}
			return result;
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x001752D8 File Offset: 0x001736D8
		public override void DesignateThing(Thing t)
		{
			if (t is Frame || t is Blueprint)
			{
				t.Destroy(DestroyMode.Cancel);
			}
			else
			{
				base.Map.designationManager.RemoveAllDesignationsOn(t, true);
				if (t.def.mineable)
				{
					Designation designation = base.Map.designationManager.DesignationAt(t.Position, DesignationDefOf.Mine);
					if (designation != null)
					{
						base.Map.designationManager.RemoveDesignation(designation);
					}
				}
			}
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x00175361 File Offset: 0x00173761
		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}

		// Token: 0x06002C17 RID: 11287 RVA: 0x0017536C File Offset: 0x0017376C
		private IEnumerable<Designation> CancelableDesignationsAt(IntVec3 c)
		{
			return from x in base.Map.designationManager.AllDesignationsAt(c)
			where x.def != DesignationDefOf.Plan
			select x;
		}

		// Token: 0x06002C18 RID: 11288 RVA: 0x001753B4 File Offset: 0x001737B4
		public override void RenderHighlight(List<IntVec3> dragCells)
		{
			Designator_Cancel.seenThings.Clear();
			for (int i = 0; i < dragCells.Count; i++)
			{
				if (base.Map.designationManager.HasMapDesignationAt(dragCells[i]))
				{
					Graphics.DrawMesh(MeshPool.plane10, dragCells[i].ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays.AltitudeFor()), Quaternion.identity, DesignatorUtility.DragHighlightCellMat, 0);
				}
				List<Thing> thingList = dragCells[i].GetThingList(base.Map);
				for (int j = 0; j < thingList.Count; j++)
				{
					Thing thing = thingList[j];
					if (!Designator_Cancel.seenThings.Contains(thing) && this.CanDesignateThing(thing).Accepted)
					{
						Vector3 drawPos = thing.DrawPos;
						drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor();
						Graphics.DrawMesh(MeshPool.plane10, drawPos, Quaternion.identity, DesignatorUtility.DragHighlightThingMat, 0);
						Designator_Cancel.seenThings.Add(thing);
					}
				}
			}
			Designator_Cancel.seenThings.Clear();
		}
	}
}
