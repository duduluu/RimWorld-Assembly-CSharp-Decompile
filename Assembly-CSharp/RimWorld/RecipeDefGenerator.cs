﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	// Token: 0x02000237 RID: 567
	internal static class RecipeDefGenerator
	{
		// Token: 0x06000A3B RID: 2619 RVA: 0x0005A9C8 File Offset: 0x00058DC8
		public static IEnumerable<RecipeDef> ImpliedRecipeDefs()
		{
			foreach (RecipeDef r in RecipeDefGenerator.DefsFromRecipeMakers().Concat(RecipeDefGenerator.DrugAdministerDefs()))
			{
				yield return r;
			}
			yield break;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x0005A9EC File Offset: 0x00058DEC
		private static IEnumerable<RecipeDef> DefsFromRecipeMakers()
		{
			foreach (ThingDef def in from d in DefDatabase<ThingDef>.AllDefs
			where d.recipeMaker != null
			select d)
			{
				RecipeMakerProperties rm = def.recipeMaker;
				RecipeDef r = new RecipeDef();
				r.defName = "Make_" + def.defName;
				r.label = "RecipeMake".Translate(new object[]
				{
					def.label
				});
				r.jobString = "RecipeMakeJobString".Translate(new object[]
				{
					def.label
				});
				r.modContentPack = def.modContentPack;
				r.workAmount = (float)rm.workAmount;
				r.workSpeedStat = rm.workSpeedStat;
				r.efficiencyStat = rm.efficiencyStat;
				if (def.MadeFromStuff)
				{
					IngredientCount ingredientCount = new IngredientCount();
					ingredientCount.SetBaseCount((float)def.costStuffCount);
					ingredientCount.filter.SetAllowAllWhoCanMake(def);
					r.ingredients.Add(ingredientCount);
					r.fixedIngredientFilter.SetAllowAllWhoCanMake(def);
					r.productHasIngredientStuff = true;
				}
				if (def.costList != null)
				{
					foreach (ThingDefCountClass thingDefCountClass in def.costList)
					{
						IngredientCount ingredientCount2 = new IngredientCount();
						ingredientCount2.SetBaseCount((float)thingDefCountClass.count);
						ingredientCount2.filter.SetAllow(thingDefCountClass.thingDef, true);
						r.ingredients.Add(ingredientCount2);
					}
				}
				r.defaultIngredientFilter = rm.defaultIngredientFilter;
				r.products.Add(new ThingDefCountClass(def, rm.productCount));
				r.targetCountAdjustment = rm.targetCountAdjustment;
				r.skillRequirements = rm.skillRequirements.ListFullCopyOrNull<SkillRequirement>();
				r.workSkill = rm.workSkill;
				r.workSkillLearnFactor = rm.workSkillLearnPerTick;
				r.unfinishedThingDef = rm.unfinishedThingDef;
				r.recipeUsers = rm.recipeUsers.ListFullCopyOrNull<ThingDef>();
				r.effectWorking = rm.effectWorking;
				r.soundWorking = rm.soundWorking;
				r.researchPrerequisite = rm.researchPrerequisite;
				r.factionPrerequisiteTags = rm.factionPrerequisiteTags;
				yield return r;
			}
			yield break;
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x0005AA10 File Offset: 0x00058E10
		private static IEnumerable<RecipeDef> DrugAdministerDefs()
		{
			foreach (ThingDef def in from d in DefDatabase<ThingDef>.AllDefs
			where d.IsDrug
			select d)
			{
				RecipeDef r = new RecipeDef();
				r.defName = "Administer_" + def.defName;
				r.label = "RecipeAdminister".Translate(new object[]
				{
					def.label
				});
				r.jobString = "RecipeAdministerJobString".Translate(new object[]
				{
					def.label
				});
				r.workerClass = typeof(Recipe_AdministerIngestible);
				r.targetsBodyPart = false;
				r.anesthetize = false;
				r.surgerySuccessChanceFactor = 99999f;
				r.modContentPack = def.modContentPack;
				r.workAmount = (float)def.ingestible.baseIngestTicks;
				IngredientCount ic = new IngredientCount();
				ic.SetBaseCount(1f);
				ic.filter.SetAllow(def, true);
				r.ingredients.Add(ic);
				r.fixedIngredientFilter.SetAllow(def, true);
				r.recipeUsers = new List<ThingDef>();
				foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs.Where((ThingDef d) => d.category == ThingCategory.Pawn && d.race.IsFlesh))
				{
					r.recipeUsers.Add(item);
				}
				yield return r;
			}
			yield break;
		}
	}
}
