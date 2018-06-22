﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000978 RID: 2424
	public static class FacilitiesUtility
	{
		// Token: 0x06003686 RID: 13958 RVA: 0x001D1350 File Offset: 0x001CF750
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

		// Token: 0x0400232F RID: 9007
		private const float MaxDistToLinkToFacilityEver = 10f;

		// Token: 0x04002330 RID: 9008
		private static int RegionsToSearch = (1 + 2 * Mathf.CeilToInt(0.8333333f)) * (1 + 2 * Mathf.CeilToInt(0.8333333f));

		// Token: 0x04002331 RID: 9009
		private static HashSet<Region> visited = new HashSet<Region>();

		// Token: 0x04002332 RID: 9010
		private static HashSet<Thing> processed = new HashSet<Thing>();

		// Token: 0x04002333 RID: 9011
		private static bool working;
	}
}
