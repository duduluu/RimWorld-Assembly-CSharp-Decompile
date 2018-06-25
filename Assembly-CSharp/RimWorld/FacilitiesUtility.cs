﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x0200097A RID: 2426
	public static class FacilitiesUtility
	{
		// Token: 0x04002337 RID: 9015
		private const float MaxDistToLinkToFacilityEver = 10f;

		// Token: 0x04002338 RID: 9016
		private static int RegionsToSearch = (1 + 2 * Mathf.CeilToInt(0.8333333f)) * (1 + 2 * Mathf.CeilToInt(0.8333333f));

		// Token: 0x04002339 RID: 9017
		private static HashSet<Region> visited = new HashSet<Region>();

		// Token: 0x0400233A RID: 9018
		private static HashSet<Thing> processed = new HashSet<Thing>();

		// Token: 0x0400233B RID: 9019
		private static bool working;

		// Token: 0x0600368A RID: 13962 RVA: 0x001D1764 File Offset: 0x001CFB64
		public static void NotifyFacilitiesAboutChangedLOSBlockers(List<Region> affectedRegions)
		{
			if (affectedRegions.Any<Region>())
			{
				if (FacilitiesUtility.working)
				{
					Log.Warning("Tried to update facilities while already updating.", false);
				}
				else
				{
					FacilitiesUtility.working = true;
					try
					{
						FacilitiesUtility.visited.Clear();
						FacilitiesUtility.processed.Clear();
						int facilitiesToProcess = affectedRegions[0].Map.listerThings.ThingsInGroup(ThingRequestGroup.Facility).Count;
						int affectedByFacilitiesToProcess = affectedRegions[0].Map.listerThings.ThingsInGroup(ThingRequestGroup.AffectedByFacilities).Count;
						int facilitiesProcessed = 0;
						int affectedByFacilitiesProcessed = 0;
						if (facilitiesToProcess > 0 && affectedByFacilitiesToProcess > 0)
						{
							for (int i = 0; i < affectedRegions.Count; i++)
							{
								if (!FacilitiesUtility.visited.Contains(affectedRegions[i]))
								{
									RegionTraverser.BreadthFirstTraverse(affectedRegions[i], (Region from, Region r) => !FacilitiesUtility.visited.Contains(r), delegate(Region x)
									{
										FacilitiesUtility.visited.Add(x);
										List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
										for (int j = 0; j < list.Count; j++)
										{
											if (!FacilitiesUtility.processed.Contains(list[j]))
											{
												FacilitiesUtility.processed.Add(list[j]);
												CompFacility compFacility = list[j].TryGetComp<CompFacility>();
												CompAffectedByFacilities compAffectedByFacilities = list[j].TryGetComp<CompAffectedByFacilities>();
												if (compFacility != null)
												{
													compFacility.Notify_LOSBlockerSpawnedOrDespawned();
													facilitiesProcessed++;
												}
												if (compAffectedByFacilities != null)
												{
													compAffectedByFacilities.Notify_LOSBlockerSpawnedOrDespawned();
													affectedByFacilitiesProcessed++;
												}
											}
										}
										return facilitiesProcessed >= facilitiesToProcess && affectedByFacilitiesProcessed >= affectedByFacilitiesToProcess;
									}, FacilitiesUtility.RegionsToSearch, RegionType.Set_Passable);
									if (facilitiesProcessed >= facilitiesToProcess && affectedByFacilitiesProcessed >= affectedByFacilitiesToProcess)
									{
										break;
									}
								}
							}
						}
					}
					finally
					{
						FacilitiesUtility.working = false;
						FacilitiesUtility.visited.Clear();
						FacilitiesUtility.processed.Clear();
					}
				}
			}
		}
	}
}
