﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;

namespace Verse
{
	public sealed class TerrainGrid : IExposable
	{
		private Map map;

		public TerrainDef[] topGrid;

		private TerrainDef[] underGrid;

		public TerrainGrid(Map map)
		{
			this.map = map;
			this.ResetGrids();
		}

		public void ResetGrids()
		{
			this.topGrid = new TerrainDef[this.map.cellIndices.NumGridCells];
			this.underGrid = new TerrainDef[this.map.cellIndices.NumGridCells];
		}

		public TerrainDef TerrainAt(int ind)
		{
			return this.topGrid[ind];
		}

		public TerrainDef TerrainAt(IntVec3 c)
		{
			return this.topGrid[this.map.cellIndices.CellToIndex(c)];
		}

		public TerrainDef UnderTerrainAt(int ind)
		{
			return this.underGrid[ind];
		}

		public TerrainDef UnderTerrainAt(IntVec3 c)
		{
			return this.underGrid[this.map.cellIndices.CellToIndex(c)];
		}

		public void SetTerrain(IntVec3 c, TerrainDef newTerr)
		{
			if (newTerr == null)
			{
				Log.Error("Tried to set terrain at " + c + " to null.", false);
				return;
			}
			if (Current.ProgramState == ProgramState.Playing)
			{
				Designation designation = this.map.designationManager.DesignationAt(c, DesignationDefOf.SmoothFloor);
				if (designation != null)
				{
					designation.Delete();
				}
			}
			int num = this.map.cellIndices.CellToIndex(c);
			if (newTerr.layerable)
			{
				if (this.underGrid[num] == null)
				{
					if (this.topGrid[num].passability != Traversability.Impassable)
					{
						this.underGrid[num] = this.topGrid[num];
					}
					else
					{
						this.underGrid[num] = TerrainDefOf.Sand;
					}
				}
			}
			else
			{
				this.underGrid[num] = null;
			}
			this.topGrid[num] = newTerr;
			this.DoTerrainChangedEffects(c);
		}

		public void SetUnderTerrain(IntVec3 c, TerrainDef newTerr)
		{
			if (!c.InBounds(this.map))
			{
				Log.Error("Tried to set terrain out of bounds at " + c, false);
				return;
			}
			int num = this.map.cellIndices.CellToIndex(c);
			this.underGrid[num] = newTerr;
		}

		public void RemoveTopLayer(IntVec3 c, bool doLeavings = true)
		{
			int num = this.map.cellIndices.CellToIndex(c);
			if (doLeavings)
			{
				GenLeaving.DoLeavingsFor(this.topGrid[num], c, this.map);
			}
			if (this.underGrid[num] != null)
			{
				this.topGrid[num] = this.underGrid[num];
				this.underGrid[num] = null;
				this.DoTerrainChangedEffects(c);
			}
		}

		public bool CanRemoveTopLayerAt(IntVec3 c)
		{
			int num = this.map.cellIndices.CellToIndex(c);
			return this.topGrid[num].Removable && this.underGrid[num] != null;
		}

		private void DoTerrainChangedEffects(IntVec3 c)
		{
			this.map.mapDrawer.MapMeshDirty(c, MapMeshFlag.Terrain, true, false);
			List<Thing> thingList = c.GetThingList(this.map);
			for (int i = thingList.Count - 1; i >= 0; i--)
			{
				if (thingList[i].def.category == ThingCategory.Plant && this.map.fertilityGrid.FertilityAt(c) < thingList[i].def.plant.fertilityMin)
				{
					thingList[i].Destroy(DestroyMode.Vanish);
				}
				else if (thingList[i].def.category == ThingCategory.Filth && !this.TerrainAt(c).acceptFilth)
				{
					thingList[i].Destroy(DestroyMode.Vanish);
				}
				else if ((thingList[i].def.IsBlueprint || thingList[i].def.IsFrame) && !GenConstruct.CanBuildOnTerrain(thingList[i].def.entityDefToBuild, thingList[i].Position, this.map, thingList[i].Rotation, null))
				{
					thingList[i].Destroy(DestroyMode.Cancel);
				}
			}
			this.map.pathGrid.RecalculatePerceivedPathCostAt(c);
			Region regionAt_NoRebuild_InvalidAllowed = this.map.regionGrid.GetRegionAt_NoRebuild_InvalidAllowed(c);
			if (regionAt_NoRebuild_InvalidAllowed != null && regionAt_NoRebuild_InvalidAllowed.Room != null)
			{
				regionAt_NoRebuild_InvalidAllowed.Room.Notify_TerrainChanged();
			}
		}

		public void ExposeData()
		{
			this.ExposeTerrainGrid(this.topGrid, "topGrid");
			this.ExposeTerrainGrid(this.underGrid, "underGrid");
		}

		public void Notify_TerrainBurned(IntVec3 c)
		{
			TerrainDef terrain = c.GetTerrain(this.map);
			this.Notify_TerrainDestroyed(c);
			if (terrain.burnedDef != null)
			{
				this.SetTerrain(c, terrain.burnedDef);
			}
		}

		public void Notify_TerrainDestroyed(IntVec3 c)
		{
			if (!this.CanRemoveTopLayerAt(c))
			{
				return;
			}
			TerrainDef terrainDef = this.TerrainAt(c);
			this.RemoveTopLayer(c, false);
			if (terrainDef.destroyBuildingsOnDestroyed)
			{
				Building firstBuilding = c.GetFirstBuilding(this.map);
				if (firstBuilding != null)
				{
					firstBuilding.Kill(null, null);
				}
			}
			if (terrainDef.destroyEffectWater != null && this.TerrainAt(c) != null && this.TerrainAt(c).IsWater)
			{
				Effecter effecter = terrainDef.destroyEffectWater.Spawn();
				effecter.Trigger(new TargetInfo(c, this.map, false), new TargetInfo(c, this.map, false));
				effecter.Cleanup();
			}
			else if (terrainDef.destroyEffect != null)
			{
				Effecter effecter2 = terrainDef.destroyEffect.Spawn();
				effecter2.Trigger(new TargetInfo(c, this.map, false), new TargetInfo(c, this.map, false));
				effecter2.Cleanup();
			}
		}

		private void ExposeTerrainGrid(TerrainDef[] grid, string label)
		{
			Dictionary<ushort, TerrainDef> terrainDefsByShortHash = new Dictionary<ushort, TerrainDef>();
			foreach (TerrainDef terrainDef in DefDatabase<TerrainDef>.AllDefs)
			{
				terrainDefsByShortHash.Add(terrainDef.shortHash, terrainDef);
			}
			Func<IntVec3, ushort> shortReader = delegate(IntVec3 c)
			{
				TerrainDef terrainDef2 = grid[this.map.cellIndices.CellToIndex(c)];
				return (terrainDef2 == null) ? 0 : terrainDef2.shortHash;
			};
			Action<IntVec3, ushort> shortWriter = delegate(IntVec3 c, ushort val)
			{
				TerrainDef terrainDef2 = terrainDefsByShortHash.TryGetValue(val, null);
				if (terrainDef2 == null && val != 0)
				{
					TerrainDef terrainDef3 = BackCompatibility.BackCompatibleTerrainWithShortHash(val);
					if (terrainDef3 == null)
					{
						Log.Error(string.Concat(new object[]
						{
							"Did not find terrain def with short hash ",
							val,
							" for cell ",
							c,
							"."
						}), false);
						terrainDef3 = TerrainDefOf.Soil;
					}
					terrainDef2 = terrainDef3;
					terrainDefsByShortHash.Add(val, terrainDef3);
				}
				grid[this.map.cellIndices.CellToIndex(c)] = terrainDef2;
			};
			MapExposeUtility.ExposeUshort(this.map, shortReader, shortWriter, label);
		}

		public string DebugStringAt(IntVec3 c)
		{
			if (c.InBounds(this.map))
			{
				TerrainDef terrain = c.GetTerrain(this.map);
				TerrainDef terrainDef = this.underGrid[this.map.cellIndices.CellToIndex(c)];
				return "top: " + ((terrain == null) ? "null" : terrain.defName) + ", under=" + ((terrainDef == null) ? "null" : terrainDef.defName);
			}
			return "out of bounds";
		}

		[CompilerGenerated]
		private sealed class <ExposeTerrainGrid>c__AnonStorey0
		{
			internal TerrainDef[] grid;

			internal Dictionary<ushort, TerrainDef> terrainDefsByShortHash;

			internal TerrainGrid $this;

			public <ExposeTerrainGrid>c__AnonStorey0()
			{
			}

			internal ushort <>m__0(IntVec3 c)
			{
				TerrainDef terrainDef = this.grid[this.$this.map.cellIndices.CellToIndex(c)];
				return (terrainDef == null) ? 0 : terrainDef.shortHash;
			}

			internal void <>m__1(IntVec3 c, ushort val)
			{
				TerrainDef terrainDef = this.terrainDefsByShortHash.TryGetValue(val, null);
				if (terrainDef == null && val != 0)
				{
					TerrainDef terrainDef2 = BackCompatibility.BackCompatibleTerrainWithShortHash(val);
					if (terrainDef2 == null)
					{
						Log.Error(string.Concat(new object[]
						{
							"Did not find terrain def with short hash ",
							val,
							" for cell ",
							c,
							"."
						}), false);
						terrainDef2 = TerrainDefOf.Soil;
					}
					terrainDef = terrainDef2;
					this.terrainDefsByShortHash.Add(val, terrainDef2);
				}
				this.grid[this.$this.map.cellIndices.CellToIndex(c)] = terrainDef;
			}
		}
	}
}
