﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020006FC RID: 1788
	public static class ThingSetMakerUtility
	{
		// Token: 0x06002701 RID: 9985 RVA: 0x0014FDD0 File Offset: 0x0014E1D0
		public static void Reset()
		{
			ThingSetMakerUtility.allGeneratableItems.Clear();
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (ThingSetMakerUtility.CanGenerate(thingDef))
				{
					ThingSetMakerUtility.allGeneratableItems.Add(thingDef);
				}
			}
			ThingSetMaker_Meteorite.Reset();
		}

		// Token: 0x06002702 RID: 9986 RVA: 0x0014FE50 File Offset: 0x0014E250
		public static bool CanGenerate(ThingDef thingDef)
		{
			return (thingDef.category == ThingCategory.Item || thingDef.Minifiable) && (thingDef.category != ThingCategory.Item || thingDef.EverHaulable) && !thingDef.isUnfinishedThing && !thingDef.IsCorpse && thingDef.PlayerAcquirable && thingDef.graphicData != null && !typeof(MinifiedThing).IsAssignableFrom(thingDef.thingClass);
		}

		// Token: 0x06002703 RID: 9987 RVA: 0x0014FEE4 File Offset: 0x0014E2E4
		public static IEnumerable<ThingDef> GetAllowedThingDefs(ThingSetMakerParams parms)
		{
			ThingSetMakerUtility.<GetAllowedThingDefs>c__AnonStorey0 <GetAllowedThingDefs>c__AnonStorey = new ThingSetMakerUtility.<GetAllowedThingDefs>c__AnonStorey0();
			<GetAllowedThingDefs>c__AnonStorey.parms = parms;
			ThingSetMakerUtility.<GetAllowedThingDefs>c__AnonStorey0 <GetAllowedThingDefs>c__AnonStorey2 = <GetAllowedThingDefs>c__AnonStorey;
			TechLevel? techLevel = <GetAllowedThingDefs>c__AnonStorey.parms.techLevel;
			<GetAllowedThingDefs>c__AnonStorey2.techLevel = ((techLevel == null) ? TechLevel.Undefined : techLevel.Value);
			IEnumerable<ThingDef> source = <GetAllowedThingDefs>c__AnonStorey.parms.filter.AllowedThingDefs;
			if (<GetAllowedThingDefs>c__AnonStorey.techLevel != TechLevel.Undefined)
			{
				source = from x in source
				where x.techLevel <= <GetAllowedThingDefs>c__AnonStorey.techLevel
				select x;
			}
			return source.Where(delegate(ThingDef x)
			{
				if (ThingSetMakerUtility.CanGenerate(x))
				{
					float? maxThingMarketValue = <GetAllowedThingDefs>c__AnonStorey.parms.maxThingMarketValue;
					if (maxThingMarketValue != null)
					{
						float? maxThingMarketValue2 = <GetAllowedThingDefs>c__AnonStorey.parms.maxThingMarketValue;
						if (!(x.BaseMarketValue <= maxThingMarketValue2))
						{
							goto IL_7E;
						}
					}
					return <GetAllowedThingDefs>c__AnonStorey.parms.validator == null || <GetAllowedThingDefs>c__AnonStorey.parms.validator(x);
				}
				IL_7E:
				return false;
			});
		}

		// Token: 0x06002704 RID: 9988 RVA: 0x0014FF74 File Offset: 0x0014E374
		public static void AssignQuality(Thing thing, QualityGenerator? qualityGenerator)
		{
			CompQuality compQuality = thing.TryGetComp<CompQuality>();
			if (compQuality != null)
			{
				QualityGenerator qualityGenerator2 = (qualityGenerator == null) ? QualityGenerator.BaseGen : qualityGenerator.Value;
				QualityCategory q = QualityUtility.GenerateQuality(qualityGenerator2);
				compQuality.SetQuality(q, ArtGenerationContext.Outsider);
			}
		}

		// Token: 0x06002705 RID: 9989 RVA: 0x0014FFBC File Offset: 0x0014E3BC
		public static float AdjustedBigCategoriesSelectionWeight(ThingDef d, int numMeats, int numLeathers)
		{
			float num = 1f;
			if (d.IsMeat)
			{
				num *= Mathf.Min(5f / (float)numMeats, 1f);
			}
			if (d.IsLeather)
			{
				num *= Mathf.Min(5f / (float)numLeathers, 1f);
			}
			return num;
		}

		// Token: 0x06002706 RID: 9990 RVA: 0x00150018 File Offset: 0x0014E418
		public static bool PossibleToWeighNoMoreThan(ThingDef t, float maxMass, IEnumerable<ThingDef> allowedStuff)
		{
			bool result;
			if (maxMass == 3.40282347E+38f || t.category == ThingCategory.Pawn)
			{
				result = true;
			}
			else if (maxMass < 0f)
			{
				result = false;
			}
			else if (t.MadeFromStuff)
			{
				foreach (ThingDef stuff in allowedStuff)
				{
					if (t.GetStatValueAbstract(StatDefOf.Mass, stuff) <= maxMass)
					{
						return true;
					}
				}
				result = false;
			}
			else
			{
				result = (t.GetStatValueAbstract(StatDefOf.Mass, null) <= maxMass);
			}
			return result;
		}

		// Token: 0x06002707 RID: 9991 RVA: 0x001500E0 File Offset: 0x0014E4E0
		public static bool TryGetRandomThingWhichCanWeighNoMoreThan(IEnumerable<ThingDef> candidates, TechLevel stuffTechLevel, float maxMass, out ThingStuffPair thingStuffPair)
		{
			ThingDef thingDef;
			bool result;
			if (!(from x in candidates
			where ThingSetMakerUtility.PossibleToWeighNoMoreThan(x, maxMass, GenStuff.AllowedStuffsFor(x, stuffTechLevel))
			select x).TryRandomElement(out thingDef))
			{
				thingStuffPair = default(ThingStuffPair);
				result = false;
			}
			else
			{
				ThingDef stuff;
				if (thingDef.MadeFromStuff)
				{
					if (!(from x in GenStuff.AllowedStuffsFor(thingDef, stuffTechLevel)
					where thingDef.GetStatValueAbstract(StatDefOf.Mass, x) <= maxMass
					select x).TryRandomElementByWeight((ThingDef x) => x.stuffProps.commonality, out stuff))
					{
						thingStuffPair = default(ThingStuffPair);
						return false;
					}
				}
				else
				{
					stuff = null;
				}
				thingStuffPair = new ThingStuffPair(thingDef, stuff, 1f);
				result = true;
			}
			return result;
		}

		// Token: 0x06002708 RID: 9992 RVA: 0x001501C0 File Offset: 0x0014E5C0
		public static bool PossibleToWeighNoMoreThan(IEnumerable<ThingDef> candidates, TechLevel stuffTechLevel, float maxMass, int count)
		{
			bool result;
			if (maxMass == 3.40282347E+38f || count <= 0)
			{
				result = true;
			}
			else if (maxMass < 0f)
			{
				result = false;
			}
			else
			{
				float num = float.MaxValue;
				foreach (ThingDef thingDef in candidates)
				{
					num = Mathf.Min(num, ThingSetMakerUtility.GetMinMass(thingDef, stuffTechLevel));
				}
				result = (num <= maxMass * (float)count);
			}
			return result;
		}

		// Token: 0x06002709 RID: 9993 RVA: 0x00150264 File Offset: 0x0014E664
		public static float GetMinMass(ThingDef thingDef, TechLevel stuffTechLevel)
		{
			float num = float.MaxValue;
			if (thingDef.MadeFromStuff)
			{
				foreach (ThingDef thingDef2 in GenStuff.AllowedStuffsFor(thingDef, stuffTechLevel))
				{
					if (thingDef2.stuffProps.commonality > 0f)
					{
						num = Mathf.Min(num, thingDef.GetStatValueAbstract(StatDefOf.Mass, thingDef2));
					}
				}
			}
			else
			{
				num = Mathf.Min(num, thingDef.GetStatValueAbstract(StatDefOf.Mass, null));
			}
			return num;
		}

		// Token: 0x0600270A RID: 9994 RVA: 0x00150318 File Offset: 0x0014E718
		public static float GetMinMarketValue(ThingDef thingDef, TechLevel stuffTechLevel)
		{
			float num = float.MaxValue;
			if (thingDef.MadeFromStuff)
			{
				foreach (ThingDef thingDef2 in GenStuff.AllowedStuffsFor(thingDef, stuffTechLevel))
				{
					if (thingDef2.stuffProps.commonality > 0f)
					{
						num = Mathf.Min(num, StatDefOf.MarketValue.Worker.GetValue(StatRequest.For(thingDef, thingDef2, QualityCategory.Awful), true));
					}
				}
			}
			else
			{
				num = Mathf.Min(num, StatDefOf.MarketValue.Worker.GetValue(StatRequest.For(thingDef, null, QualityCategory.Awful), true));
			}
			return num;
		}

		// Token: 0x040015AE RID: 5550
		public static List<ThingDef> allGeneratableItems = new List<ThingDef>();
	}
}