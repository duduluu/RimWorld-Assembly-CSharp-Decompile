﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x020006F3 RID: 1779
	public class ThingSetMaker_Nutrition : ThingSetMaker
	{
		// Token: 0x0400158A RID: 5514
		private int nextSeed;

		// Token: 0x060026B5 RID: 9909 RVA: 0x0014C200 File Offset: 0x0014A600
		public ThingSetMaker_Nutrition()
		{
			this.nextSeed = Rand.Int;
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x0014C214 File Offset: 0x0014A614
		protected override bool CanGenerateSub(ThingSetMakerParams parms)
		{
			bool result;
			if (!this.AllowedThingDefs(parms).Any<ThingDef>())
			{
				result = false;
			}
			else
			{
				IntRange? countRange = parms.countRange;
				if (countRange != null && parms.countRange.Value.max <= 0)
				{
					result = false;
				}
				else
				{
					FloatRange? totalNutritionRange = parms.totalNutritionRange;
					if (totalNutritionRange == null || parms.totalNutritionRange.Value.max <= 0f)
					{
						result = false;
					}
					else
					{
						float? maxTotalMass = parms.maxTotalMass;
						if (maxTotalMass != null && parms.maxTotalMass != 3.40282347E+38f)
						{
							IEnumerable<ThingDef> candidates = this.AllowedThingDefs(parms);
							TechLevel? techLevel = parms.techLevel;
							TechLevel stuffTechLevel = (techLevel == null) ? TechLevel.Undefined : techLevel.Value;
							float value = parms.maxTotalMass.Value;
							IntRange? countRange2 = parms.countRange;
							if (!ThingSetMakerUtility.PossibleToWeighNoMoreThan(candidates, stuffTechLevel, value, (countRange2 == null) ? 1 : parms.countRange.Value.min))
							{
								return false;
							}
						}
						float num;
						result = this.GeneratePossibleDefs(parms, out num, this.nextSeed).Any<ThingStuffPairWithQuality>();
					}
				}
			}
			return result;
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x0014C38C File Offset: 0x0014A78C
		protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
		{
			float? maxTotalMass = parms.maxTotalMass;
			float maxMass = (maxTotalMass == null) ? float.MaxValue : maxTotalMass.Value;
			float totalValue;
			List<ThingStuffPairWithQuality> list = this.GeneratePossibleDefs(parms, out totalValue, this.nextSeed);
			for (int i = 0; i < list.Count; i++)
			{
				outThings.Add(list[i].MakeThing());
			}
			ThingSetMakerByTotalStatUtility.IncreaseStackCountsToTotalValue(outThings, totalValue, (Thing x) => x.GetStatValue(StatDefOf.Nutrition, true), maxMass);
			this.nextSeed++;
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x0014C438 File Offset: 0x0014A838
		protected virtual IEnumerable<ThingDef> AllowedThingDefs(ThingSetMakerParams parms)
		{
			return ThingSetMakerUtility.GetAllowedThingDefs(parms);
		}

		// Token: 0x060026B9 RID: 9913 RVA: 0x0014C454 File Offset: 0x0014A854
		private List<ThingStuffPairWithQuality> GeneratePossibleDefs(ThingSetMakerParams parms, out float totalNutrition, int seed)
		{
			Rand.PushState(seed);
			List<ThingStuffPairWithQuality> result = this.GeneratePossibleDefs(parms, out totalNutrition);
			Rand.PopState();
			return result;
		}

		// Token: 0x060026BA RID: 9914 RVA: 0x0014C480 File Offset: 0x0014A880
		private List<ThingStuffPairWithQuality> GeneratePossibleDefs(ThingSetMakerParams parms, out float totalNutrition)
		{
			IEnumerable<ThingDef> enumerable = this.AllowedThingDefs(parms);
			List<ThingStuffPairWithQuality> result;
			if (!enumerable.Any<ThingDef>())
			{
				totalNutrition = 0f;
				result = new List<ThingStuffPairWithQuality>();
			}
			else
			{
				IntRange? countRange = parms.countRange;
				IntRange intRange = (countRange == null) ? new IntRange(1, int.MaxValue) : countRange.Value;
				FloatRange? totalNutritionRange = parms.totalNutritionRange;
				FloatRange floatRange = (totalNutritionRange == null) ? FloatRange.Zero : totalNutritionRange.Value;
				TechLevel? techLevel = parms.techLevel;
				TechLevel techLevel2 = (techLevel == null) ? TechLevel.Undefined : techLevel.Value;
				float? maxTotalMass = parms.maxTotalMass;
				float num = (maxTotalMass == null) ? float.MaxValue : maxTotalMass.Value;
				QualityGenerator? qualityGenerator = parms.qualityGenerator;
				QualityGenerator qualityGenerator2 = (qualityGenerator == null) ? QualityGenerator.BaseGen : qualityGenerator.Value;
				totalNutrition = floatRange.RandomInRange;
				int numMeats = enumerable.Count((ThingDef x) => x.IsMeat);
				int numLeathers = enumerable.Count((ThingDef x) => x.IsLeather);
				Func<ThingDef, float> func = (ThingDef x) => ThingSetMakerUtility.AdjustedBigCategoriesSelectionWeight(x, numMeats, numLeathers);
				IntRange countRange2 = intRange;
				float totalValue = totalNutrition;
				IEnumerable<ThingDef> allowed = enumerable;
				TechLevel techLevel3 = techLevel2;
				QualityGenerator qualityGenerator3 = qualityGenerator2;
				Func<ThingStuffPairWithQuality, float> getMinValue = (ThingStuffPairWithQuality x) => x.GetStatValue(StatDefOf.Nutrition);
				Func<ThingStuffPairWithQuality, float> getMaxValue = (ThingStuffPairWithQuality x) => x.GetStatValue(StatDefOf.Nutrition) * (float)x.thing.stackLimit;
				Func<ThingDef, float> weightSelector = func;
				float maxMass = num;
				result = ThingSetMakerByTotalStatUtility.GenerateDefsWithPossibleTotalValue(countRange2, totalValue, allowed, techLevel3, qualityGenerator3, getMinValue, getMaxValue, weightSelector, 100, maxMass);
			}
			return result;
		}

		// Token: 0x060026BB RID: 9915 RVA: 0x0014C660 File Offset: 0x0014AA60
		protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
		{
			TechLevel? techLevel2 = parms.techLevel;
			TechLevel techLevel = (techLevel2 == null) ? TechLevel.Undefined : techLevel2.Value;
			foreach (ThingDef t in this.AllowedThingDefs(parms))
			{
				float? maxTotalMass = parms.maxTotalMass;
				if (maxTotalMass != null && parms.maxTotalMass != 3.40282347E+38f)
				{
					float? maxTotalMass2 = parms.maxTotalMass;
					if (ThingSetMakerUtility.GetMinMass(t, techLevel) > maxTotalMass2)
					{
						continue;
					}
				}
				FloatRange? totalNutritionRange = parms.totalNutritionRange;
				if (totalNutritionRange == null || parms.totalNutritionRange.Value.max == 3.40282347E+38f || !t.IsNutritionGivingIngestible || t.ingestible.CachedNutrition <= parms.totalNutritionRange.Value.max)
				{
					yield return t;
				}
			}
			yield break;
		}
	}
}
