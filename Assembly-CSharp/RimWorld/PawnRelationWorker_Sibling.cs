﻿using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020004D2 RID: 1234
	public class PawnRelationWorker_Sibling : PawnRelationWorker
	{
		// Token: 0x060015FB RID: 5627 RVA: 0x000C30A4 File Offset: 0x000C14A4
		public override bool InRelation(Pawn me, Pawn other)
		{
			return me != other && (me.GetMother() != null && me.GetFather() != null && me.GetMother() == other.GetMother() && me.GetFather() == other.GetFather());
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x000C3108 File Offset: 0x000C1508
		public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			float num = 1f;
			float num2 = 1f;
			if (other.GetFather() != null || other.GetMother() != null)
			{
				num = ChildRelationUtility.ChanceOfBecomingChildOf(generated, other.GetFather(), other.GetMother(), new PawnGenerationRequest?(request), null, null);
			}
			else if (request.FixedMelanin != null)
			{
				num2 = ChildRelationUtility.GetMelaninSimilarityFactor(request.FixedMelanin.Value, other.story.melanin);
			}
			else
			{
				num2 = PawnSkinColors.GetMelaninCommonalityFactor(other.story.melanin);
			}
			float num3 = Mathf.Abs(generated.ageTracker.AgeChronologicalYearsFloat - other.ageTracker.AgeChronologicalYearsFloat);
			float num4 = 1f;
			if (num3 > 40f)
			{
				num4 = 0.2f;
			}
			else if (num3 > 10f)
			{
				num4 = 0.65f;
			}
			return num * num2 * num4 * base.BaseGenerationChanceFactor(generated, other, request);
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x000C3220 File Offset: 0x000C1620
		public override void CreateRelation(Pawn generated, Pawn other, ref PawnGenerationRequest request)
		{
			bool flag = other.GetMother() != null;
			bool flag2 = other.GetFather() != null;
			bool flag3 = Rand.Value < 0.85f;
			if (flag && LovePartnerRelationUtility.HasAnyLovePartner(other.GetMother()))
			{
				flag3 = false;
			}
			if (flag2 && LovePartnerRelationUtility.HasAnyLovePartner(other.GetFather()))
			{
				flag3 = false;
			}
			if (!flag)
			{
				Pawn newMother = PawnRelationWorker_Sibling.GenerateParent(generated, other, Gender.Female, request, flag3);
				other.SetMother(newMother);
			}
			generated.SetMother(other.GetMother());
			if (!flag2)
			{
				Pawn newFather = PawnRelationWorker_Sibling.GenerateParent(generated, other, Gender.Male, request, flag3);
				other.SetFather(newFather);
			}
			generated.SetFather(other.GetFather());
			if (!flag || !flag2)
			{
				bool flag4 = other.GetMother().story.traits.HasTrait(TraitDefOf.Gay) || other.GetFather().story.traits.HasTrait(TraitDefOf.Gay);
				if (flag4)
				{
					other.GetFather().relations.AddDirectRelation(PawnRelationDefOf.ExLover, other.GetMother());
				}
				else if (flag3)
				{
					other.GetFather().relations.AddDirectRelation(PawnRelationDefOf.Spouse, other.GetMother());
				}
				else
				{
					LovePartnerRelationUtility.GiveRandomExLoverOrExSpouseRelation(other.GetFather(), other.GetMother());
				}
			}
			PawnRelationWorker_Sibling.ResolveMyName(ref request, generated);
			PawnRelationWorker_Sibling.ResolveMySkinColor(ref request, generated);
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x000C3398 File Offset: 0x000C1798
		private static Pawn GenerateParent(Pawn generatedChild, Pawn existingChild, Gender genderToGenerate, PawnGenerationRequest childRequest, bool newlyGeneratedParentsWillBeSpousesIfNotGay)
		{
			float ageChronologicalYearsFloat = generatedChild.ageTracker.AgeChronologicalYearsFloat;
			float ageChronologicalYearsFloat2 = existingChild.ageTracker.AgeChronologicalYearsFloat;
			float num = (genderToGenerate != Gender.Male) ? 16f : 14f;
			float num2 = (genderToGenerate != Gender.Male) ? 45f : 50f;
			float num3 = (genderToGenerate != Gender.Male) ? 27f : 30f;
			float num4 = Mathf.Max(ageChronologicalYearsFloat, ageChronologicalYearsFloat2) + num;
			float maxChronologicalAge = num4 + (num2 - num);
			float midChronologicalAge = num4 + (num3 - num);
			float value;
			float value2;
			float value3;
			string last;
			PawnRelationWorker_Sibling.GenerateParentParams(num4, maxChronologicalAge, midChronologicalAge, num, generatedChild, existingChild, childRequest, out value, out value2, out value3, out last);
			bool allowGay = true;
			if (newlyGeneratedParentsWillBeSpousesIfNotGay && last.NullOrEmpty())
			{
				if (Rand.Value < 0.8f)
				{
					if (genderToGenerate == Gender.Male && existingChild.GetMother() != null && !existingChild.GetMother().story.traits.HasTrait(TraitDefOf.Gay))
					{
						last = ((NameTriple)existingChild.GetMother().Name).Last;
						allowGay = false;
					}
					else if (genderToGenerate == Gender.Female && existingChild.GetFather() != null && !existingChild.GetFather().story.traits.HasTrait(TraitDefOf.Gay))
					{
						last = ((NameTriple)existingChild.GetFather().Name).Last;
						allowGay = false;
					}
				}
			}
			Faction faction = existingChild.Faction;
			if (faction == null || faction.IsPlayer)
			{
				bool tryMedievalOrBetter = faction != null && faction.def.techLevel >= TechLevel.Medieval;
				if (!Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out faction, tryMedievalOrBetter, true, TechLevel.Undefined))
				{
					faction = Faction.OfAncients;
				}
			}
			PawnKindDef kindDef = existingChild.kindDef;
			Faction faction2 = faction;
			bool forceGenerateNewPawn = true;
			bool allowDead = true;
			bool allowDowned = true;
			bool canGeneratePawnRelations = false;
			Gender? fixedGender = new Gender?(genderToGenerate);
			float? fixedMelanin = new float?(value3);
			string fixedLastName = last;
			PawnGenerationRequest request = new PawnGenerationRequest(kindDef, faction2, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn, false, allowDead, allowDowned, canGeneratePawnRelations, false, 1f, false, allowGay, true, false, false, false, false, null, null, null, new float?(value), new float?(value2), fixedGender, fixedMelanin, fixedLastName);
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			if (!Find.WorldPawns.Contains(pawn))
			{
				Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
			}
			return pawn;
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x000C35FC File Offset: 0x000C19FC
		private static void GenerateParentParams(float minChronologicalAge, float maxChronologicalAge, float midChronologicalAge, float minBioAgeToHaveChildren, Pawn generatedChild, Pawn existingChild, PawnGenerationRequest childRequest, out float biologicalAge, out float chronologicalAge, out float melanin, out string lastName)
		{
			chronologicalAge = Rand.GaussianAsymmetric(midChronologicalAge, (midChronologicalAge - minChronologicalAge) / 2f, (maxChronologicalAge - midChronologicalAge) / 2f);
			chronologicalAge = Mathf.Clamp(chronologicalAge, minChronologicalAge, maxChronologicalAge);
			biologicalAge = Rand.Range(minBioAgeToHaveChildren, Mathf.Min(existingChild.RaceProps.lifeExpectancy, chronologicalAge));
			if (existingChild.GetFather() != null)
			{
				melanin = ParentRelationUtility.GetRandomSecondParentSkinColor(existingChild.GetFather().story.melanin, existingChild.story.melanin, childRequest.FixedMelanin);
			}
			else if (existingChild.GetMother() != null)
			{
				melanin = ParentRelationUtility.GetRandomSecondParentSkinColor(existingChild.GetMother().story.melanin, existingChild.story.melanin, childRequest.FixedMelanin);
			}
			else if (childRequest.FixedMelanin == null)
			{
				melanin = PawnSkinColors.GetRandomMelaninSimilarTo(existingChild.story.melanin, 0f, 1f);
			}
			else
			{
				float num = Mathf.Min(childRequest.FixedMelanin.Value, existingChild.story.melanin);
				float num2 = Mathf.Max(childRequest.FixedMelanin.Value, existingChild.story.melanin);
				if (Rand.Value < 0.5f)
				{
					melanin = PawnSkinColors.GetRandomMelaninSimilarTo(num, 0f, num);
				}
				else
				{
					melanin = PawnSkinColors.GetRandomMelaninSimilarTo(num2, num2, 1f);
				}
			}
			lastName = null;
			if (!ChildRelationUtility.DefinitelyHasNotBirthName(existingChild))
			{
				if (ChildRelationUtility.ChildWantsNameOfAnyParent(existingChild))
				{
					if (existingChild.GetMother() == null && existingChild.GetFather() == null)
					{
						if (Rand.Value < 0.5f)
						{
							lastName = ((NameTriple)existingChild.Name).Last;
						}
					}
					else
					{
						string last = ((NameTriple)existingChild.Name).Last;
						string b = null;
						if (existingChild.GetMother() != null)
						{
							b = ((NameTriple)existingChild.GetMother().Name).Last;
						}
						else if (existingChild.GetFather() != null)
						{
							b = ((NameTriple)existingChild.GetFather().Name).Last;
						}
						if (last != b)
						{
							lastName = last;
						}
					}
				}
			}
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x000C3858 File Offset: 0x000C1C58
		private static void ResolveMyName(ref PawnGenerationRequest request, Pawn generated)
		{
			if (request.FixedLastName == null)
			{
				if (ChildRelationUtility.ChildWantsNameOfAnyParent(generated))
				{
					if (Rand.Value < 0.5f)
					{
						request.SetFixedLastName(((NameTriple)generated.GetFather().Name).Last);
					}
					else
					{
						request.SetFixedLastName(((NameTriple)generated.GetMother().Name).Last);
					}
				}
			}
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x000C38D0 File Offset: 0x000C1CD0
		private static void ResolveMySkinColor(ref PawnGenerationRequest request, Pawn generated)
		{
			if (request.FixedMelanin == null)
			{
				request.SetFixedMelanin(ChildRelationUtility.GetRandomChildSkinColor(generated.GetFather().story.melanin, generated.GetMother().story.melanin));
			}
		}
	}
}
