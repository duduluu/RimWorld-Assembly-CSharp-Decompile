﻿using System;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000545 RID: 1349
	public class WorldReachability
	{
		// Token: 0x04000ED2 RID: 3794
		private int[] fields;

		// Token: 0x04000ED3 RID: 3795
		private int nextFieldID;

		// Token: 0x04000ED4 RID: 3796
		private int impassableFieldID;

		// Token: 0x04000ED5 RID: 3797
		private int minValidFieldID;

		// Token: 0x0600193A RID: 6458 RVA: 0x000DBAC6 File Offset: 0x000D9EC6
		public WorldReachability()
		{
			this.fields = new int[Find.WorldGrid.TilesCount];
			this.nextFieldID = 1;
			this.InvalidateAllFields();
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x000DBAF1 File Offset: 0x000D9EF1
		public void ClearCache()
		{
			this.InvalidateAllFields();
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x000DBAFC File Offset: 0x000D9EFC
		public bool CanReach(Caravan c, int tile)
		{
			return this.CanReach(c.Tile, tile);
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x000DBB20 File Offset: 0x000D9F20
		public bool CanReach(int startTile, int destTile)
		{
			bool result;
			if (startTile < 0 || startTile >= this.fields.Length || destTile < 0 || destTile >= this.fields.Length)
			{
				result = false;
			}
			else if (this.fields[startTile] == this.impassableFieldID || this.fields[destTile] == this.impassableFieldID)
			{
				result = false;
			}
			else if (this.IsValidField(this.fields[startTile]) || this.IsValidField(this.fields[destTile]))
			{
				result = (this.fields[startTile] == this.fields[destTile]);
			}
			else
			{
				this.FloodFillAt(startTile);
				result = (this.fields[startTile] != this.impassableFieldID && this.fields[startTile] == this.fields[destTile]);
			}
			return result;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x000DBC04 File Offset: 0x000DA004
		private void InvalidateAllFields()
		{
			if (this.nextFieldID == 2147483646)
			{
				this.nextFieldID = 1;
			}
			this.minValidFieldID = this.nextFieldID;
			this.impassableFieldID = this.nextFieldID;
			this.nextFieldID++;
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x000DBC44 File Offset: 0x000DA044
		private bool IsValidField(int fieldID)
		{
			return fieldID >= this.minValidFieldID;
		}

		// Token: 0x06001940 RID: 6464 RVA: 0x000DBC68 File Offset: 0x000DA068
		private void FloodFillAt(int tile)
		{
			World world = Find.World;
			if (world.Impassable(tile))
			{
				this.fields[tile] = this.impassableFieldID;
			}
			else
			{
				Find.WorldFloodFiller.FloodFill(tile, (int x) => !world.Impassable(x), delegate(int x)
				{
					this.fields[x] = this.nextFieldID;
				}, int.MaxValue, null);
				this.nextFieldID++;
			}
		}
	}
}
