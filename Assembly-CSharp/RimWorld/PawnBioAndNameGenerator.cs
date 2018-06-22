﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace RimWorld
{
	// Token: 0x02000487 RID: 1159
	public static class PawnBioAndNameGenerator
	{
		// Token: 0x06001474 RID: 5236 RVA: 0x000B35A0 File Offset: 0x000B19A0
		public static void GiveAppropriateBioAndNameTo(Pawn pawn, string requiredLastName, FactionDef factionType)
		{
			if (Rand.Value < 0.25f || pawn.kindDef.factionLeader)
			{
				bool flag = PawnBioAndNameGenerator.TryGiveSolidBioTo(pawn, requiredLastName, factionType);
				if (flag)
				{
					return;
				}
			}
			PawnBioAndNameGenerator.GiveShuffledBioTo(pawn, factionType, requiredLastName);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x000B35EC File Offset: 0x000B19EC
		private static void GiveShuffledBioTo(Pawn pawn, FactionDef factionType, string requiredLastName)
		{
			pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, requiredLastName);
			PawnBioAndNameGenerator.FillBackstorySlotShuffled(pawn, BackstorySlot.Childhood, ref pawn.story.childhood, factionType);
			if (pawn.ageTracker.AgeBiologicalYearsFloat >= 20f)
			{
				PawnBioAndNameGenerator.FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, factionType);
			}
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x000B3644 File Offset: 0x000B1A44
		private static void FillBackstorySlotShuffled(Pawn pawn, BackstorySlot slot, ref Backstory backstory, FactionDef factionType)
		{
			IEnumerable<Backstory> source = from bs in BackstoryDatabase.ShuffleableBackstoryList(slot, factionType.backstoryCategory).TakeRandom(20)
			where slot != BackstorySlot.Adulthood || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables)
			select bs;
			if (PawnBioAndNameGenerator.<>f__mg$cache0 == null)
			{
				PawnBioAndNameGenerator.<>f__mg$cache0 = new Func<Backstory, float>(PawnBioAndNameGenerator.BackstorySelectionWeight);
			}
			if (!source.TryRandomElementByWeight(PawnBioAndNameGenerator.<>f__mg$cache0, out backstory))
			{
				Log.Error(string.Concat(new object[]
				{
					"No shuffled ",
					slot,
					" found for ",
					pawn.ToStringSafe<Pawn>(),
					" of ",
					factionType.ToStringSafe<FactionDef>(),
					". Defaulting."
				}), false);
				backstory = (from kvp in BackstoryDatabase.allBackstories
				where kvp.Value.slot == slot
				select kvp).RandomElement<KeyValuePair<string, Backstory>>().Value;
			}
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x000B3738 File Offset: 0x000B1B38
		private static bool TryGiveSolidBioTo(Pawn pawn, string requiredLastName, FactionDef factionType)
		{
			PawnBio pawnBio = PawnBioAndNameGenerator.TryGetRandomUnusedSolidBioFor(factionType.backstoryCategory, pawn.kindDef, pawn.gender, requiredLastName);
			bool result;
			if (pawnBio == null)
			{
				result = false;
			}
			else
			{
				if (pawnBio.name.First == "Tynan" && pawnBio.name.Last == "Sylvester")
				{
					if (Rand.Value < 0.5f)
					{
						pawnBio = PawnBioAndNameGenerator.TryGetRandomUnusedSolidBioFor(factionType.backstoryCategory, pawn.kindDef, pawn.gender, requiredLastName);
					}
				}
				if (pawnBio == null)
				{
					result = false;
				}
				else
				{
					pawn.Name = pawnBio.name;
					pawn.story.childhood = pawnBio.childhood;
					if (pawn.ageTracker.AgeBiologicalYearsFloat >= 20f)
					{
						pawn.story.adulthood = pawnBio.adulthood;
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x000B3824 File Offset: 0x000B1C24
		private static PawnBio TryGetRandomUnusedSolidBioFor(string backstoryCategory, PawnKindDef kind, Gender gender, string requiredLastName)
		{
			NameTriple prefName = null;
			if (Rand.Value < 0.5f)
			{
				prefName = Prefs.RandomPreferredName();
				if (prefName != null && (prefName.UsedThisGame || (requiredLastName != null && prefName.Last != requiredLastName)))
				{
					prefName = null;
				}
			}
			PawnBio result;
			for (;;)
			{
				result = null;
				IEnumerable<PawnBio> source = SolidBioDatabase.allBios.TakeRandom(20).Where(delegate(PawnBio bio)
				{
					if (bio.gender != GenderPossibility.Either)
					{
						if (gender == Gender.Male && bio.gender != GenderPossibility.Male)
						{
							return false;
						}
						if (gender == Gender.Female && bio.gender != GenderPossibility.Female)
						{
							return false;
						}
					}
					return (requiredLastName.NullOrEmpty() || !(bio.name.Last != requiredLastName)) && (prefName == null || bio.name.Equals(prefName)) && (!kind.factionLeader || bio.pirateKing) && bio.adulthood.spawnCategories.Contains(backstoryCategory) && !bio.name.UsedThisGame;
				});
				if (PawnBioAndNameGenerator.<>f__mg$cache1 == null)
				{
					PawnBioAndNameGenerator.<>f__mg$cache1 = new Func<PawnBio, float>(PawnBioAndNameGenerator.BioSelectionWeight);
				}
				if (source.TryRandomElementByWeight(PawnBioAndNameGenerator.<>f__mg$cache1, out result) || prefName == null)
				{
					break;
				}
				prefName = null;
			}
			return result;
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x000B392C File Offset: 0x000B1D2C
		public static NameTriple TryGetRandomUnusedSolidName(Gender gender, string requiredLastName = null)
		{
			NameTriple nameTriple = null;
			if (Rand.Value < 0.5f)
			{
				nameTriple = Prefs.RandomPreferredName();
				if (nameTriple != null && (nameTriple.UsedThisGame || (requiredLastName != null && nameTriple.Last != requiredLastName)))
				{
					nameTriple = null;
				}
			}
			List<NameTriple> listForGender = PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Either);
			List<NameTriple> list = (gender != Gender.Male) ? PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Female) : PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Male);
			float num = ((float)listForGender.Count + 0.1f) / ((float)(listForGender.Count + list.Count) + 0.1f);
			List<NameTriple> list2;
			if (Rand.Value < num)
			{
				list2 = listForGender;
			}
			else
			{
				list2 = list;
			}
			NameTriple result;
			if (list2.Count == 0)
			{
				Log.Error("Empty solid pawn name list for gender: " + gender + ".", false);
				result = null;
			}
			else if (nameTriple != null && list2.Contains(nameTriple))
			{
				result = nameTriple;
			}
			else
			{
				list2.Shuffle<NameTriple>();
				NameTriple nameTriple2 = (from name in list2
				where (requiredLastName == null || !(name.Last != requiredLastName)) && !name.UsedThisGame
				select name).FirstOrDefault<NameTriple>();
				result = nameTriple2;
			}
			return result;
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x000B3A68 File Offset: 0x000B1E68
		public static Name GeneratePawnName(Pawn pawn, NameStyle style = NameStyle.Full, string forcedLastName = null)
		{
			Name result;
			if (style == NameStyle.Full)
			{
				RulePackDef nameGenerator = pawn.RaceProps.GetNameGenerator(pawn.gender);
				if (nameGenerator != null)
				{
					string name = NameGenerator.GenerateName(nameGenerator, (string x) => !new NameSingle(x, false).UsedThisGame, false, null, null);
					result = new NameSingle(name, false);
				}
				else if (pawn.Faction != null && pawn.Faction.def.pawnNameMaker != null)
				{
					string rawName = NameGenerator.GenerateName(pawn.Faction.def.pawnNameMaker, delegate(string x)
					{
						NameTriple nameTriple4 = NameTriple.FromString(x);
						nameTriple4.ResolveMissingPieces(forcedLastName);
						return !nameTriple4.UsedThisGame;
					}, false, null, null);
					NameTriple nameTriple = NameTriple.FromString(rawName);
					nameTriple.CapitalizeNick();
					nameTriple.ResolveMissingPieces(forcedLastName);
					result = nameTriple;
				}
				else if (pawn.RaceProps.nameCategory != PawnNameCategory.NoName)
				{
					if (Rand.Value < 0.5f)
					{
						NameTriple nameTriple2 = PawnBioAndNameGenerator.TryGetRandomUnusedSolidName(pawn.gender, forcedLastName);
						if (nameTriple2 != null)
						{
							return nameTriple2;
						}
					}
					result = PawnBioAndNameGenerator.GeneratePawnName_Shuffled(pawn, forcedLastName);
				}
				else
				{
					Log.Error("No name making method for " + pawn, false);
					NameTriple nameTriple3 = NameTriple.FromString(pawn.def.label);
					nameTriple3.ResolveMissingPieces(null);
					result = nameTriple3;
				}
			}
			else
			{
				if (style != NameStyle.Numeric)
				{
					throw new InvalidOperationException();
				}
				int num = 1;
				string text;
				for (;;)
				{
					text = pawn.KindLabel + " " + num.ToString();
					if (!NameUseChecker.NameSingleIsUsed(text))
					{
						break;
					}
					num++;
				}
				result = new NameSingle(text, true);
			}
			return result;
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x000B3C30 File Offset: 0x000B2030
		private static NameTriple GeneratePawnName_Shuffled(Pawn pawn, string forcedLastName = null)
		{
			PawnNameCategory pawnNameCategory = pawn.RaceProps.nameCategory;
			if (pawnNameCategory == PawnNameCategory.NoName)
			{
				Log.Message("Can't create a name of type NoName. Defaulting to HumanStandard.", false);
				pawnNameCategory = PawnNameCategory.HumanStandard;
			}
			NameBank nameBank = PawnNameDatabaseShuffled.BankOf(pawnNameCategory);
			string name = nameBank.GetName(PawnNameSlot.First, pawn.gender, true);
			string text;
			if (forcedLastName != null)
			{
				text = forcedLastName;
			}
			else
			{
				text = nameBank.GetName(PawnNameSlot.Last, Gender.None, true);
			}
			int num = 0;
			string nick;
			do
			{
				num++;
				if (Rand.Value < 0.15f)
				{
					Gender gender = pawn.gender;
					if (Rand.Value < 0.5f)
					{
						gender = Gender.None;
					}
					nick = nameBank.GetName(PawnNameSlot.Nick, gender, true);
				}
				else if (Rand.Value < 0.5f)
				{
					nick = name;
				}
				else
				{
					nick = text;
				}
			}
			while (num < 50 && NameUseChecker.AllPawnsNamesEverUsed.Any(delegate(Name x)
			{
				NameTriple nameTriple = x as NameTriple;
				return nameTriple != null && nameTriple.Nick == nick;
			}));
			return new NameTriple(name, nick, text);
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x000B3D40 File Offset: 0x000B2140
		private static float BackstorySelectionWeight(Backstory bs)
		{
			return PawnBioAndNameGenerator.SelectionWeightFactorFromWorkTagsDisabled(bs.workDisables);
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x000B3D60 File Offset: 0x000B2160
		private static float BioSelectionWeight(PawnBio bio)
		{
			return PawnBioAndNameGenerator.SelectionWeightFactorFromWorkTagsDisabled(bio.adulthood.workDisables | bio.childhood.workDisables);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x000B3D94 File Offset: 0x000B2194
		private static float SelectionWeightFactorFromWorkTagsDisabled(WorkTags wt)
		{
			float num = 1f;
			if ((wt & WorkTags.ManualDumb) != WorkTags.None)
			{
				num *= 0.4f;
			}
			if ((wt & WorkTags.ManualSkilled) != WorkTags.None)
			{
				num *= 1f;
			}
			if ((wt & WorkTags.Violent) != WorkTags.None)
			{
				num *= 0.5f;
			}
			if ((wt & WorkTags.Caring) != WorkTags.None)
			{
				num *= 0.9f;
			}
			if ((wt & WorkTags.Social) != WorkTags.None)
			{
				num *= 0.5f;
			}
			if ((wt & WorkTags.Intellectual) != WorkTags.None)
			{
				num *= 0.35f;
			}
			if ((wt & WorkTags.Firefighting) != WorkTags.None)
			{
				num *= 0.7f;
			}
			return num;
		}

		// Token: 0x04000C4A RID: 3146
		private const float MinAgeForAdulthood = 20f;

		// Token: 0x04000C4B RID: 3147
		private const float SolidBioChance = 0.25f;

		// Token: 0x04000C4C RID: 3148
		private const float SolidNameChance = 0.5f;

		// Token: 0x04000C4D RID: 3149
		private const float TryPreferredNameChance_Bio = 0.5f;

		// Token: 0x04000C4E RID: 3150
		private const float TryPreferredNameChance_Name = 0.5f;

		// Token: 0x04000C4F RID: 3151
		private const float ShuffledNicknameChance = 0.15f;

		// Token: 0x04000C50 RID: 3152
		[CompilerGenerated]
		private static Func<Backstory, float> <>f__mg$cache0;

		// Token: 0x04000C51 RID: 3153
		[CompilerGenerated]
		private static Func<PawnBio, float> <>f__mg$cache1;
	}
}
