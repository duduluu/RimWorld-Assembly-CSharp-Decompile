﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace Verse
{
	// Token: 0x02000F51 RID: 3921
	public static class GenSpawn
	{
		// Token: 0x06005ECD RID: 24269 RVA: 0x00304A5C File Offset: 0x00302E5C
		public static Thing Spawn(ThingDef def, IntVec3 loc, Map map, WipeMode wipeMode = WipeMode.Vanish)
		{
			return GenSpawn.Spawn(ThingMaker.MakeThing(def, null), loc, map, wipeMode);
		}

		// Token: 0x06005ECE RID: 24270 RVA: 0x00304A80 File Offset: 0x00302E80
		public static Thing Spawn(Thing newThing, IntVec3 loc, Map map, WipeMode wipeMode = WipeMode.Vanish)
		{
			return GenSpawn.Spawn(newThing, loc, map, Rot4.North, wipeMode, false);
		}

		// Token: 0x06005ECF RID: 24271 RVA: 0x00304AA4 File Offset: 0x00302EA4
		public static Thing Spawn(Thing newThing, IntVec3 loc, Map map, Rot4 rot, WipeMode wipeMode = WipeMode.Vanish, bool respawningAfterLoad = false)
		{
			Thing result;
			if (map == null)
			{
				Log.Error("Tried to spawn " + newThing.ToStringSafe<Thing>() + " in a null map.", false);
				result = null;
			}
			else if (!loc.InBounds(map))
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to spawn ",
					newThing.ToStringSafe<Thing>(),
					" out of bounds at ",
					loc,
					"."
				}), false);
				result = null;
			}
			else
			{
				if (newThing.def.randomizeRotationOnSpawn)
				{
					rot = Rot4.Random;
				}
				CellRect occupiedRect = GenAdj.OccupiedRect(loc, rot, newThing.def.Size);
				if (!occupiedRect.InBounds(map))
				{
					Log.Error(string.Concat(new object[]
					{
						"Tried to spawn ",
						newThing.ToStringSafe<Thing>(),
						" out of bounds at ",
						loc,
						" (out of bounds because size is ",
						newThing.def.Size,
						")."
					}), false);
					result = null;
				}
				else if (newThing.Spawned)
				{
					Log.Error("Tried to spawn " + newThing + " but it's already spawned.", false);
					result = newThing;
				}
				else
				{
					if (wipeMode == WipeMode.Vanish)
					{
						GenSpawn.WipeExistingThings(loc, rot, newThing.def, map, DestroyMode.Vanish);
					}
					else if (wipeMode == WipeMode.FullRefund)
					{
						GenSpawn.WipeAndRefundExistingThings(loc, rot, newThing.def, map);
					}
					if (newThing.def.category == ThingCategory.Item)
					{
						foreach (IntVec3 intVec in occupiedRect)
						{
							foreach (Thing thing in intVec.GetThingList(map).ToList<Thing>())
							{
								if (thing != newThing)
								{
									if (thing.def.category == ThingCategory.Item)
									{
										thing.DeSpawn(DestroyMode.Vanish);
										if (!GenPlace.TryPlaceThing(thing, intVec, map, ThingPlaceMode.Near, null, (IntVec3 x) => !occupiedRect.Contains(x)))
										{
											thing.Destroy(DestroyMode.Vanish);
										}
									}
								}
							}
						}
					}
					newThing.Rotation = rot;
					newThing.Position = loc;
					if (newThing.holdingOwner != null)
					{
						newThing.holdingOwner.Remove(newThing);
					}
					newThing.SpawnSetup(map, respawningAfterLoad);
					if (newThing.Spawned && newThing.stackCount == 0)
					{
						Log.Error("Spawned thing with 0 stackCount: " + newThing, false);
						newThing.Destroy(DestroyMode.Vanish);
						result = null;
					}
					else
					{
						if (newThing.def.passability == Traversability.Impassable)
						{
							foreach (IntVec3 c in occupiedRect)
							{
								foreach (Thing thing2 in c.GetThingList(map).ToList<Thing>())
								{
									if (thing2 != newThing)
									{
										Pawn pawn = thing2 as Pawn;
										if (pawn != null)
										{
											pawn.pather.TryRecoverFromUnwalkablePosition(false);
										}
									}
								}
							}
						}
						result = newThing;
					}
				}
			}
			return result;
		}

		// Token: 0x06005ED0 RID: 24272 RVA: 0x00304E68 File Offset: 0x00303268
		public static void SpawnBuildingAsPossible(Building building, Map map, bool respawningAfterLoad = false)
		{
			bool flag = false;
			if (!building.OccupiedRect().InBounds(map))
			{
				flag = true;
			}
			else
			{
				foreach (IntVec3 c in building.OccupiedRect())
				{
					List<Thing> thingList = c.GetThingList(map);
					for (int i = 0; i < thingList.Count; i++)
					{
						if (thingList[i] is Pawn && building.def.passability == Traversability.Impassable)
						{
							flag = true;
							break;
						}
						if ((thingList[i].def.category == ThingCategory.Building || thingList[i].def.category == ThingCategory.Item) && GenSpawn.SpawningWipes(building.def, thingList[i].def))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			if (flag)
			{
				GenSpawn.Refund(building, map, CellRect.Empty);
			}
			else
			{
				GenSpawn.Spawn(building, building.Position, map, building.Rotation, WipeMode.FullRefund, respawningAfterLoad);
			}
		}

		// Token: 0x06005ED1 RID: 24273 RVA: 0x00304FC8 File Offset: 0x003033C8
		public static void Refund(Thing thing, Map map, CellRect avoidThisRect)
		{
			bool flag = false;
			if (thing.def.Minifiable)
			{
				MinifiedThing minifiedThing = thing.MakeMinified();
				if (GenPlace.TryPlaceThing(minifiedThing, thing.Position, map, ThingPlaceMode.Near, null, (IntVec3 x) => !avoidThisRect.Contains(x)))
				{
					flag = true;
				}
				else
				{
					minifiedThing.GetDirectlyHeldThings().Clear();
					minifiedThing.Destroy(DestroyMode.Vanish);
				}
			}
			if (!flag)
			{
				GenLeaving.DoLeavingsFor(thing, map, DestroyMode.Refund, thing.OccupiedRect(), (IntVec3 x) => !avoidThisRect.Contains(x));
				thing.Destroy(DestroyMode.Vanish);
			}
		}

		// Token: 0x06005ED2 RID: 24274 RVA: 0x00305064 File Offset: 0x00303464
		public static void WipeExistingThings(IntVec3 thingPos, Rot4 thingRot, BuildableDef thingDef, Map map, DestroyMode mode)
		{
			foreach (IntVec3 c in GenAdj.CellsOccupiedBy(thingPos, thingRot, thingDef.Size))
			{
				foreach (Thing thing in map.thingGrid.ThingsAt(c).ToList<Thing>())
				{
					if (GenSpawn.SpawningWipes(thingDef, thing.def))
					{
						thing.Destroy(mode);
					}
				}
			}
		}

		// Token: 0x06005ED3 RID: 24275 RVA: 0x00305130 File Offset: 0x00303530
		public static void WipeAndRefundExistingThings(IntVec3 thingPos, Rot4 thingRot, BuildableDef thingDef, Map map)
		{
			CellRect occupiedRect = GenAdj.OccupiedRect(thingPos, thingRot, thingDef.Size);
			foreach (IntVec3 intVec in occupiedRect)
			{
				foreach (Thing thing in intVec.GetThingList(map).ToList<Thing>())
				{
					if (GenSpawn.SpawningWipes(thingDef, thing.def))
					{
						if (thing.def.category == ThingCategory.Item)
						{
							thing.DeSpawn(DestroyMode.Vanish);
							if (!GenPlace.TryPlaceThing(thing, intVec, map, ThingPlaceMode.Near, null, (IntVec3 x) => !occupiedRect.Contains(x)))
							{
								thing.Destroy(DestroyMode.Vanish);
							}
						}
						else
						{
							GenSpawn.Refund(thing, map, occupiedRect);
						}
					}
				}
			}
		}

		// Token: 0x06005ED4 RID: 24276 RVA: 0x00305254 File Offset: 0x00303654
		public static bool WouldWipeAnythingWith(IntVec3 thingPos, Rot4 thingRot, BuildableDef thingDef, Map map, Predicate<Thing> predicate)
		{
			return GenSpawn.WouldWipeAnythingWith(GenAdj.OccupiedRect(thingPos, thingRot, thingDef.Size), thingDef, map, predicate);
		}

		// Token: 0x06005ED5 RID: 24277 RVA: 0x00305280 File Offset: 0x00303680
		public static bool WouldWipeAnythingWith(CellRect cellRect, BuildableDef thingDef, Map map, Predicate<Thing> predicate)
		{
			foreach (IntVec3 c in cellRect)
			{
				foreach (Thing thing in map.thingGrid.ThingsAt(c).ToList<Thing>())
				{
					if (GenSpawn.SpawningWipes(thingDef, thing.def) && predicate(thing))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005ED6 RID: 24278 RVA: 0x00305354 File Offset: 0x00303754
		public static bool SpawningWipes(BuildableDef newEntDef, BuildableDef oldEntDef)
		{
			ThingDef thingDef = newEntDef as ThingDef;
			ThingDef thingDef2 = oldEntDef as ThingDef;
			bool result;
			if (thingDef == null || thingDef2 == null)
			{
				result = false;
			}
			else if (thingDef.category == ThingCategory.Attachment || thingDef.category == ThingCategory.Mote || thingDef.category == ThingCategory.Filth || thingDef.category == ThingCategory.Projectile)
			{
				result = false;
			}
			else if (!thingDef2.destroyable)
			{
				result = false;
			}
			else if (thingDef.category == ThingCategory.Plant)
			{
				result = false;
			}
			else if (thingDef2.category == ThingCategory.Filth && thingDef.passability != Traversability.Standable)
			{
				result = true;
			}
			else if (thingDef2.category == ThingCategory.Item && thingDef.passability == Traversability.Impassable && thingDef.surfaceType == SurfaceType.None)
			{
				result = true;
			}
			else if (thingDef.EverTransmitsPower && thingDef2 == ThingDefOf.PowerConduit)
			{
				result = true;
			}
			else if (thingDef.IsFrame && GenSpawn.SpawningWipes(thingDef.entityDefToBuild, oldEntDef))
			{
				result = true;
			}
			else
			{
				BuildableDef buildableDef = GenConstruct.BuiltDefOf(thingDef);
				BuildableDef buildableDef2 = GenConstruct.BuiltDefOf(thingDef2);
				if (buildableDef == null || buildableDef2 == null)
				{
					result = false;
				}
				else
				{
					ThingDef thingDef3 = thingDef.entityDefToBuild as ThingDef;
					if (thingDef2.IsBlueprint)
					{
						if (thingDef.IsBlueprint)
						{
							if (thingDef3 != null && thingDef3.building != null && thingDef3.building.canPlaceOverWall && thingDef2.entityDefToBuild is ThingDef && (ThingDef)thingDef2.entityDefToBuild == ThingDefOf.Wall)
							{
								return true;
							}
							if (thingDef2.entityDefToBuild is TerrainDef)
							{
								if (thingDef.entityDefToBuild is ThingDef && ((ThingDef)thingDef.entityDefToBuild).coversFloor)
								{
									return true;
								}
								if (thingDef.entityDefToBuild is TerrainDef)
								{
									return true;
								}
							}
						}
						result = (thingDef2.entityDefToBuild == ThingDefOf.PowerConduit && thingDef.entityDefToBuild is ThingDef && (thingDef.entityDefToBuild as ThingDef).EverTransmitsPower);
					}
					else
					{
						if ((thingDef2.IsFrame || thingDef2.IsBlueprint) && thingDef2.entityDefToBuild is TerrainDef)
						{
							ThingDef thingDef4 = buildableDef as ThingDef;
							if (thingDef4 != null && !thingDef4.CoexistsWithFloors)
							{
								return true;
							}
						}
						if (thingDef2 == ThingDefOf.ActiveDropPod)
						{
							result = false;
						}
						else if (thingDef == ThingDefOf.ActiveDropPod)
						{
							if (thingDef2 == ThingDefOf.ActiveDropPod)
							{
								result = false;
							}
							else
							{
								if (thingDef2.category == ThingCategory.Building)
								{
									if (thingDef2.passability == Traversability.Impassable)
									{
										return true;
									}
								}
								result = false;
							}
						}
						else
						{
							if (thingDef.IsEdifice())
							{
								if (thingDef.BlockPlanting)
								{
									if (thingDef2.category == ThingCategory.Plant)
									{
										return true;
									}
								}
								if (!(buildableDef is TerrainDef))
								{
									if (buildableDef2.IsEdifice())
									{
										return true;
									}
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}
	}
}
