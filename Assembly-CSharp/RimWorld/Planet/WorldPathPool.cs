﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000542 RID: 1346
	public class WorldPathPool
	{
		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06001935 RID: 6453 RVA: 0x000DB624 File Offset: 0x000D9A24
		public static WorldPath NotFoundPath
		{
			get
			{
				return WorldPathPool.notFoundPathInt;
			}
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x000DB640 File Offset: 0x000D9A40
		public WorldPath GetEmptyWorldPath()
		{
			for (int i = 0; i < this.paths.Count; i++)
			{
				if (!this.paths[i].inUse)
				{
					this.paths[i].inUse = true;
					return this.paths[i];
				}
			}
			if (this.paths.Count > Find.WorldObjects.CaravansCount + 2 + (Find.WorldObjects.RoutePlannerWaypointsCount - 1))
			{
				Log.ErrorOnce("WorldPathPool leak: more paths than caravans. Force-recovering.", 664788, false);
				this.paths.Clear();
			}
			WorldPath worldPath = new WorldPath();
			this.paths.Add(worldPath);
			worldPath.inUse = true;
			return worldPath;
		}

		// Token: 0x04000ECC RID: 3788
		private List<WorldPath> paths = new List<WorldPath>(64);

		// Token: 0x04000ECD RID: 3789
		private static readonly WorldPath notFoundPathInt = WorldPath.NewNotFound();
	}
}