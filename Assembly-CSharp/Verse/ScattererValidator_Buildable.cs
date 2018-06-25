﻿using System;

namespace Verse
{
	// Token: 0x02000C5F RID: 3167
	public class ScattererValidator_Buildable : ScattererValidator
	{
		// Token: 0x04002FA6 RID: 12198
		public int radius = 1;

		// Token: 0x04002FA7 RID: 12199
		public TerrainAffordanceDef affordance;

		// Token: 0x060045AA RID: 17834 RVA: 0x0024D0CC File Offset: 0x0024B4CC
		public override bool Allows(IntVec3 c, Map map)
		{
			CellRect cellRect = CellRect.CenteredOn(c, this.radius);
			for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
			{
				for (int j = cellRect.minX; j <= cellRect.maxX; j++)
				{
					IntVec3 c2 = new IntVec3(j, 0, i);
					if (!c2.InBounds(map))
					{
						return false;
					}
					if (c2.InNoBuildEdgeArea(map))
					{
						return false;
					}
					if (this.affordance != null && !c2.GetTerrain(map).affordances.Contains(this.affordance))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
