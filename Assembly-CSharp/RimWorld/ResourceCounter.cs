﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x0200042D RID: 1069
	public sealed class ResourceCounter
	{
		// Token: 0x04000B68 RID: 2920
		private Map map;

		// Token: 0x04000B69 RID: 2921
		private Dictionary<ThingDef, int> countedAmounts = new Dictionary<ThingDef, int>();

		// Token: 0x04000B6A RID: 2922
		private static List<ThingDef> resources = new List<ThingDef>();

		// Token: 0x060012A8 RID: 4776 RVA: 0x000A2177 File Offset: 0x000A0577
		public ResourceCounter(Map map)
		{
			this.map = map;
			this.ResetResourceCounts();
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x060012A9 RID: 4777 RVA: 0x000A2198 File Offset: 0x000A0598
		public int Silver
		{
			get
			{
				return this.GetCount(ThingDefOf.Silver);
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x060012AA RID: 4778 RVA: 0x000A21B8 File Offset: 0x000A05B8
		public float TotalHumanEdibleNutrition
		{
			get
			{
				float num = 0f;
				foreach (KeyValuePair<ThingDef, int> keyValuePair in this.countedAmounts)
				{
					if (keyValuePair.Key.IsNutritionGivingIngestible && keyValuePair.Key.ingestible.HumanEdible)
					{
						num += keyValuePair.Key.GetStatValueAbstract(StatDefOf.Nutrition, null) * (float)keyValuePair.Value;
					}
				}
				return num;
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x060012AB RID: 4779 RVA: 0x000A2264 File Offset: 0x000A0664
		public Dictionary<ThingDef, int> AllCountedAmounts
		{
			get
			{
				return this.countedAmounts;
			}
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x000A2280 File Offset: 0x000A0680
		public static void ResetDefs()
		{
			ResourceCounter.resources.Clear();
			ResourceCounter.resources.AddRange(from def in DefDatabase<ThingDef>.AllDefs
			where def.CountAsResource
			orderby def.resourceReadoutPriority descending
			select def);
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x000A22EC File Offset: 0x000A06EC
		public void ResetResourceCounts()
		{
			this.countedAmounts.Clear();
			for (int i = 0; i < ResourceCounter.resources.Count; i++)
			{
				this.countedAmounts.Add(ResourceCounter.resources[i], 0);
			}
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x000A233C File Offset: 0x000A073C
		public int GetCount(ThingDef rDef)
		{
			int result;
			int num;
			if (rDef.resourceReadoutPriority == ResourceCountPriority.Uncounted)
			{
				result = 0;
			}
			else if (this.countedAmounts.TryGetValue(rDef, out num))
			{
				result = num;
			}
			else
			{
				Log.Error("Looked for nonexistent key " + rDef + " in counted resources.", false);
				this.countedAmounts.Add(rDef, 0);
				result = 0;
			}
			return result;
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x000A23A4 File Offset: 0x000A07A4
		public int GetCountIn(ThingRequestGroup group)
		{
			int num = 0;
			foreach (KeyValuePair<ThingDef, int> keyValuePair in this.countedAmounts)
			{
				if (group.Includes(keyValuePair.Key))
				{
					num += keyValuePair.Value;
				}
			}
			return num;
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x000A2424 File Offset: 0x000A0824
		public int GetCountIn(ThingCategoryDef cat)
		{
			int num = 0;
			for (int i = 0; i < cat.childThingDefs.Count; i++)
			{
				num += this.GetCount(cat.childThingDefs[i]);
			}
			for (int j = 0; j < cat.childCategories.Count; j++)
			{
				if (!cat.childCategories[j].resourceReadoutRoot)
				{
					num += this.GetCountIn(cat.childCategories[j]);
				}
			}
			return num;
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x000A24B8 File Offset: 0x000A08B8
		public void ResourceCounterTick()
		{
			if (Find.TickManager.TicksGame % 204 == 0)
			{
				this.UpdateResourceCounts();
			}
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x000A24D8 File Offset: 0x000A08D8
		public void UpdateResourceCounts()
		{
			this.ResetResourceCounts();
			List<SlotGroup> allGroupsListForReading = this.map.haulDestinationManager.AllGroupsListForReading;
			for (int i = 0; i < allGroupsListForReading.Count; i++)
			{
				SlotGroup slotGroup = allGroupsListForReading[i];
				foreach (Thing outerThing in slotGroup.HeldThings)
				{
					Thing innerIfMinified = outerThing.GetInnerIfMinified();
					if (innerIfMinified.def.CountAsResource && this.ShouldCount(innerIfMinified))
					{
						Dictionary<ThingDef, int> dictionary;
						ThingDef def;
						(dictionary = this.countedAmounts)[def = innerIfMinified.def] = dictionary[def] + innerIfMinified.stackCount;
					}
				}
			}
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x000A25BC File Offset: 0x000A09BC
		private bool ShouldCount(Thing t)
		{
			return !t.IsNotFresh();
		}
	}
}
