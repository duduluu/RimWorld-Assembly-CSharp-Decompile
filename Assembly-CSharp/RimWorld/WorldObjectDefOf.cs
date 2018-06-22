﻿using System;

namespace RimWorld
{
	// Token: 0x0200094A RID: 2378
	[DefOf]
	public static class WorldObjectDefOf
	{
		// Token: 0x06003652 RID: 13906 RVA: 0x001D0C81 File Offset: 0x001CF081
		static WorldObjectDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(WorldObjectDefOf));
		}

		// Token: 0x04002263 RID: 8803
		public static WorldObjectDef Caravan;

		// Token: 0x04002264 RID: 8804
		public static WorldObjectDef FactionBase;

		// Token: 0x04002265 RID: 8805
		public static WorldObjectDef AbandonedBase;

		// Token: 0x04002266 RID: 8806
		public static WorldObjectDef EscapeShip;

		// Token: 0x04002267 RID: 8807
		public static WorldObjectDef Ambush;

		// Token: 0x04002268 RID: 8808
		public static WorldObjectDef DestroyedFactionBase;

		// Token: 0x04002269 RID: 8809
		public static WorldObjectDef AttackedNonPlayerCaravan;

		// Token: 0x0400226A RID: 8810
		public static WorldObjectDef TravelingTransportPods;

		// Token: 0x0400226B RID: 8811
		public static WorldObjectDef RoutePlannerWaypoint;

		// Token: 0x0400226C RID: 8812
		public static WorldObjectDef Site;

		// Token: 0x0400226D RID: 8813
		public static WorldObjectDef PeaceTalks;

		// Token: 0x0400226E RID: 8814
		public static WorldObjectDef Debug_Arena;
	}
}
