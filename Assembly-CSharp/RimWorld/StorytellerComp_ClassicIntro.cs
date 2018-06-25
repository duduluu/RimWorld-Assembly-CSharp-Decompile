﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x02000367 RID: 871
	public class StorytellerComp_ClassicIntro : StorytellerComp
	{
		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x000800A0 File Offset: 0x0007E4A0
		protected int IntervalsPassed
		{
			get
			{
				return Find.TickManager.TicksGame / 1000;
			}
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x000800C8 File Offset: 0x0007E4C8
		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			if (target != Find.Maps.Find((Map x) => x.IsPlayerHome))
			{
				yield break;
			}
			if (this.IntervalsPassed == 150)
			{
				IncidentDef inc = IncidentDefOf.VisitorGroup;
				if (inc.TargetAllowed(target))
				{
					yield return new FiringIncident(inc, this, null)
					{
						parms = 
						{
							target = target,
							points = (float)Rand.Range(40, 100)
						}
					};
				}
			}
			if (this.IntervalsPassed == 204)
			{
				IncidentCategoryDef threatCategory = (!Find.Storyteller.difficulty.allowIntroThreats) ? IncidentCategoryDefOf.Misc : IncidentCategoryDefOf.ThreatSmall;
				IncidentDef incDef;
				if ((from def in DefDatabase<IncidentDef>.AllDefs
				where def.TargetAllowed(target) && def.category == threatCategory
				select def).TryRandomElementByWeight(new Func<IncidentDef, float>(base.IncidentChanceFinal), out incDef))
				{
					yield return new FiringIncident(incDef, this, null)
					{
						parms = StorytellerUtility.DefaultParmsNow(incDef.category, target)
					};
				}
			}
			if (this.IntervalsPassed == 264)
			{
				IncidentDef incDef2;
				if ((from def in DefDatabase<IncidentDef>.AllDefs
				where def.TargetAllowed(target) && def.category == IncidentCategoryDefOf.Misc
				select def).TryRandomElementByWeight(new Func<IncidentDef, float>(base.IncidentChanceFinal), out incDef2))
				{
					yield return new FiringIncident(incDef2, this, null)
					{
						parms = StorytellerUtility.DefaultParmsNow(incDef2.category, target)
					};
				}
			}
			if (this.IntervalsPassed == 324)
			{
				IncidentDef inc2 = IncidentDefOf.RaidEnemy;
				if (!Find.Storyteller.difficulty.allowIntroThreats)
				{
					inc2 = (from def in DefDatabase<IncidentDef>.AllDefs
					where def.TargetAllowed(target) && def.category == IncidentCategoryDefOf.Misc
					select def).RandomElementByWeightWithFallback(new Func<IncidentDef, float>(base.IncidentChanceFinal), null);
				}
				if (inc2 != null && inc2.TargetAllowed(target))
				{
					yield return new FiringIncident(inc2, this, null)
					{
						parms = this.GenerateParms(inc2.category, target)
					};
				}
			}
			yield break;
		}
	}
}
