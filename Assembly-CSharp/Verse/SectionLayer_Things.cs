﻿using System;
using System.Collections.Generic;

namespace Verse
{
	// Token: 0x02000C54 RID: 3156
	public abstract class SectionLayer_Things : SectionLayer
	{
		// Token: 0x04002F82 RID: 12162
		protected bool requireAddToMapMesh;

		// Token: 0x06004577 RID: 17783 RVA: 0x0008537D File Offset: 0x0008377D
		public SectionLayer_Things(Section section) : base(section)
		{
		}

		// Token: 0x06004578 RID: 17784 RVA: 0x00085387 File Offset: 0x00083787
		public override void DrawLayer()
		{
			if (DebugViewSettings.drawThingsPrinted)
			{
				base.DrawLayer();
			}
		}

		// Token: 0x06004579 RID: 17785 RVA: 0x000853A0 File Offset: 0x000837A0
		public override void Regenerate()
		{
			base.ClearSubMeshes(MeshParts.All);
			foreach (IntVec3 c in this.section.CellRect)
			{
				List<Thing> list = base.Map.thingGrid.ThingsListAt(c);
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					Thing thing = list[i];
					if (thing.def.drawerType != DrawerType.None)
					{
						if (thing.def.drawerType != DrawerType.RealtimeOnly || !this.requireAddToMapMesh)
						{
							if (thing.def.hideAtSnowDepth >= 1f || base.Map.snowGrid.GetDepth(thing.Position) <= thing.def.hideAtSnowDepth)
							{
								if (thing.Position.x == c.x && thing.Position.z == c.z)
								{
									this.TakePrintFrom(thing);
								}
							}
						}
					}
				}
			}
			base.FinalizeMesh(MeshParts.All);
		}

		// Token: 0x0600457A RID: 17786
		protected abstract void TakePrintFrom(Thing t);
	}
}
