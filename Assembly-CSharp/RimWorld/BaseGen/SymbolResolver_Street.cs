﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld.BaseGen
{
	// Token: 0x020003D3 RID: 979
	public class SymbolResolver_Street : SymbolResolver
	{
		// Token: 0x060010DC RID: 4316 RVA: 0x0008FD44 File Offset: 0x0008E144
		public override void Resolve(ResolveParams rp)
		{
			bool? streetHorizontal = rp.streetHorizontal;
			bool flag = (streetHorizontal == null) ? (rp.rect.Width >= rp.rect.Height) : streetHorizontal.Value;
			int width = (!flag) ? rp.rect.Width : rp.rect.Height;
			TerrainDef floorDef = rp.floorDef ?? BaseGenUtility.RandomBasicFloorDef(rp.faction, false);
			this.CalculateStreet(rp.rect, flag, floorDef);
			this.FillStreetGaps(flag, width);
			this.RemoveShortStreetParts(flag, width);
			this.SpawnFloor(rp.rect, flag, floorDef);
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x0008FE00 File Offset: 0x0008E200
		private void CalculateStreet(CellRect rect, bool horizontal, TerrainDef floorDef)
		{
			SymbolResolver_Street.street.Clear();
			int num = (!horizontal) ? rect.Height : rect.Width;
			for (int i = 0; i < num; i++)
			{
				if (horizontal)
				{
					SymbolResolver_Street.street.Add(this.CausesStreet(new IntVec3(rect.minX + i, 0, rect.minZ - 1), floorDef) && this.CausesStreet(new IntVec3(rect.minX + i, 0, rect.maxZ + 1), floorDef));
				}
				else
				{
					SymbolResolver_Street.street.Add(this.CausesStreet(new IntVec3(rect.minX - 1, 0, rect.minZ + i), floorDef) && this.CausesStreet(new IntVec3(rect.maxX + 1, 0, rect.minZ + i), floorDef));
				}
			}
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0008FEF8 File Offset: 0x0008E2F8
		private void FillStreetGaps(bool horizontal, int width)
		{
			int num = -1;
			for (int i = 0; i < SymbolResolver_Street.street.Count; i++)
			{
				if (SymbolResolver_Street.street[i])
				{
					num = i;
				}
				else if (num != -1 && i - num <= width)
				{
					for (int j = i + 1; j < i + width + 1; j++)
					{
						if (j >= SymbolResolver_Street.street.Count)
						{
							break;
						}
						if (SymbolResolver_Street.street[j])
						{
							SymbolResolver_Street.street[i] = true;
							break;
						}
					}
				}
			}
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x0008FFA8 File Offset: 0x0008E3A8
		private void RemoveShortStreetParts(bool horizontal, int width)
		{
			for (int i = 0; i < SymbolResolver_Street.street.Count; i++)
			{
				if (SymbolResolver_Street.street[i])
				{
					int num = 0;
					for (int j = i; j < SymbolResolver_Street.street.Count; j++)
					{
						if (!SymbolResolver_Street.street[j])
						{
							break;
						}
						num++;
					}
					int num2 = 0;
					for (int k = i; k >= 0; k--)
					{
						if (!SymbolResolver_Street.street[k])
						{
							break;
						}
						num2++;
					}
					int num3 = num2 + num - 1;
					if (num3 < width)
					{
						SymbolResolver_Street.street[i] = false;
					}
				}
			}
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0009007C File Offset: 0x0008E47C
		private void SpawnFloor(CellRect rect, bool horizontal, TerrainDef floorDef)
		{
			Map map = BaseGen.globalSettings.map;
			TerrainGrid terrainGrid = map.terrainGrid;
			CellRect.CellRectIterator iterator = rect.GetIterator();
			while (!iterator.Done())
			{
				IntVec3 c = iterator.Current;
				if ((horizontal && SymbolResolver_Street.street[c.x - rect.minX]) || (!horizontal && SymbolResolver_Street.street[c.z - rect.minZ]))
				{
					terrainGrid.SetTerrain(c, floorDef);
				}
				iterator.MoveNext();
			}
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00090118 File Offset: 0x0008E518
		private bool CausesStreet(IntVec3 c, TerrainDef floorDef)
		{
			Map map = BaseGen.globalSettings.map;
			bool result;
			if (!c.InBounds(map))
			{
				result = false;
			}
			else
			{
				Building edifice = c.GetEdifice(map);
				result = ((edifice != null && edifice.def == ThingDefOf.Wall) || c.GetDoor(map) != null || c.GetTerrain(map) == floorDef);
			}
			return result;
		}

		// Token: 0x04000A43 RID: 2627
		private static List<bool> street = new List<bool>();
	}
}