﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000E19 RID: 3609
	[HasDebugOutput]
	internal static class DebugOutputsInfection
	{
		// Token: 0x060052FC RID: 21244 RVA: 0x002A8FF0 File Offset: 0x002A73F0
		private static List<Pawn> GenerateDoctorArray()
		{
			PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.Colonist, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, (Pawn p) => !p.story.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) && p.health.hediffSet.hediffs.Count == 0, null, null, null, null, null, null, null);
			List<Pawn> list = new List<Pawn>();
			for (int i = 0; i <= 20; i++)
			{
				Pawn pawn = PawnGenerator.GeneratePawn(request);
				pawn.skills.GetSkill(SkillDefOf.Medicine).Level = i;
				list.Add(pawn);
			}
			return list;
		}

		// Token: 0x060052FD RID: 21245 RVA: 0x002A90C0 File Offset: 0x002A74C0
		private static IEnumerable<HediffDef> InfectionList()
		{
			return from hediff in DefDatabase<HediffDef>.AllDefs
			where hediff.tendable && hediff.HasComp(typeof(HediffComp_TendDuration)) && hediff.HasComp(typeof(HediffComp_Immunizable)) && hediff.lethalSeverity > 0f
			select hediff;
		}

		// Token: 0x060052FE RID: 21246 RVA: 0x002A90FC File Offset: 0x002A74FC
		[DebugOutput]
		public static void Infections()
		{
			Func<DebugOutputsInfection.InfectionLuck, float> ilc = delegate(DebugOutputsInfection.InfectionLuck il)
			{
				float result = 1f;
				if (il == DebugOutputsInfection.InfectionLuck.Bad)
				{
					result = 0.8f;
				}
				if (il == DebugOutputsInfection.InfectionLuck.Good)
				{
					result = 1.2f;
				}
				return result;
			};
			Func<Func<DebugOutputsInfection.InfectionLuck, float>, string> stringizeWithLuck = (Func<DebugOutputsInfection.InfectionLuck, float> func) => string.Format("{0:F2} / {1:F2}", func(DebugOutputsInfection.InfectionLuck.Bad), func(DebugOutputsInfection.InfectionLuck.Good));
			Func<HediffDef, bool> isAnimal = (HediffDef d) => d.defName.Contains("Animal");
			Func<HediffDef, float> revealSeverity = (HediffDef d) => d.stages.First((HediffStage s) => s.becomeVisible).minSeverity;
			Func<HediffDef, float> baseSeverityIncrease = (HediffDef d) => d.CompProps<HediffCompProperties_Immunizable>().severityPerDayNotImmune;
			Func<HediffDef, DebugOutputsInfection.InfectionLuck, float> baseImmunityIncrease = (HediffDef d, DebugOutputsInfection.InfectionLuck il) => d.CompProps<HediffCompProperties_Immunizable>().immunityPerDaySick * ilc(il);
			Func<HediffDef, float, float> tendedSeverityIncrease = (HediffDef d, float tend) => baseSeverityIncrease(d) + d.CompProps<HediffCompProperties_TendDuration>().severityPerDayTended * tend;
			Func<HediffDef, DebugOutputsInfection.InfectionLuck, bool, float> immunityIncrease = delegate(HediffDef d, DebugOutputsInfection.InfectionLuck il, bool bedridden)
			{
				float b = (!isAnimal(d)) ? ThingDefOf.Bed.GetStatValueAbstract(StatDefOf.ImmunityGainSpeedFactor, null) : 1f;
				float num = Mathf.Lerp(1f, b, (!bedridden) ? 0.3f : 1f);
				float num2 = num * StatDefOf.ImmunityGainSpeed.GetStatPart<StatPart_Resting>().factor;
				return baseImmunityIncrease(d, il) * num2;
			};
			Func<HediffDef, DebugOutputsInfection.InfectionLuck, float> immunityOnReveal = (HediffDef d, DebugOutputsInfection.InfectionLuck il) => revealSeverity(d) / baseSeverityIncrease(d) * immunityIncrease(d, il, false);
			Func<HediffDef, DebugOutputsInfection.InfectionLuck, float, float> immunityOnLethality = delegate(HediffDef d, DebugOutputsInfection.InfectionLuck il, float tend)
			{
				float result;
				if (tendedSeverityIncrease(d, tend) <= 0f)
				{
					result = float.PositiveInfinity;
				}
				else
				{
					result = immunityOnReveal(d, il) + (d.lethalSeverity - revealSeverity(d)) / tendedSeverityIncrease(d, tend) * immunityIncrease(d, il, true);
				}
				return result;
			};
			List<TableDataGetter<HediffDef>> list = new List<TableDataGetter<HediffDef>>();
			list.Add(new TableDataGetter<HediffDef>("defName", (HediffDef d) => d.defName + ((!d.stages.Any((HediffStage stage) => stage.capMods.Any((PawnCapacityModifier cap) => cap.capacity == PawnCapacityDefOf.BloodFiltration))) ? "" : " (inaccurate)")));
			list.Add(new TableDataGetter<HediffDef>("lethal\nseverity", (HediffDef d) => d.lethalSeverity.ToString("F2")));
			list.Add(new TableDataGetter<HediffDef>("base\nseverity\nincrease", (HediffDef d) => baseSeverityIncrease(d).ToString("F2")));
			list.Add(new TableDataGetter<HediffDef>("base\nimmunity\nincrease", (HediffDef d) => stringizeWithLuck((DebugOutputsInfection.InfectionLuck il) => baseImmunityIncrease(d, il))));
			list.Add(new TableDataGetter<HediffDef>("immunity\non reveal", (HediffDef d) => stringizeWithLuck((DebugOutputsInfection.InfectionLuck il) => immunityOnReveal(d, il))));
			List<Pawn> source = DebugOutputsInfection.GenerateDoctorArray();
			float tendquality;
			for (tendquality = 0f; tendquality <= 1.01f; tendquality += 0.1f)
			{
				tendquality = Mathf.Clamp01(tendquality);
				Pawn arg = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, null) >= Mathf.Clamp01(tendquality - 0.25f), null);
				Pawn arg2 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineHerbal) >= Mathf.Clamp01(tendquality - 0.25f), null);
				Pawn arg3 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineIndustrial) >= Mathf.Clamp01(tendquality - 0.25f), null);
				Pawn arg4 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineUltratech) >= Mathf.Clamp01(tendquality - 0.25f), null);
				Pawn arg5 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, null) >= tendquality, null);
				Pawn arg6 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineHerbal) >= tendquality, null);
				Pawn arg7 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineIndustrial) >= tendquality, null);
				Pawn arg8 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineUltratech) >= tendquality, null);
				Pawn arg9 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, null) >= Mathf.Clamp01(tendquality + 0.25f), null);
				Pawn arg10 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineHerbal) >= Mathf.Clamp01(tendquality + 0.25f), null);
				Pawn arg11 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineIndustrial) >= Mathf.Clamp01(tendquality + 0.25f), null);
				Pawn arg12 = source.FirstOrFallback((Pawn doc) => TendUtility.CalculateBaseTendQuality(doc, null, ThingDefOf.MedicineUltratech) >= Mathf.Clamp01(tendquality + 0.25f), null);
				Func<Pawn, Pawn, Pawn, string> func2 = delegate(Pawn low, Pawn exp, Pawn high)
				{
					string arg13 = (low == null) ? "X" : low.skills.GetSkill(SkillDefOf.Medicine).Level.ToString();
					string arg14 = (exp == null) ? "X" : exp.skills.GetSkill(SkillDefOf.Medicine).Level.ToString();
					string arg15 = (high == null) ? "X" : high.skills.GetSkill(SkillDefOf.Medicine).Level.ToString();
					return string.Format("{0}-{1}-{2}", arg13, arg14, arg15);
				};
				string text = func2(arg, arg5, arg9);
				string text2 = func2(arg2, arg6, arg10);
				string text3 = func2(arg3, arg7, arg11);
				string text4 = func2(arg4, arg8, arg12);
				float tq = tendquality;
				list.Add(new TableDataGetter<HediffDef>(string.Format("survival chance at\ntend quality {0}\n\ndoc skill needed:\nno meds:  {1}\nherbal:  {2}\nnormal:  {3}\nglitter:  {4}", new object[]
				{
					tq.ToStringPercent(),
					text,
					text2,
					text3,
					text4
				}), delegate(HediffDef d)
				{
					float num = immunityOnLethality(d, DebugOutputsInfection.InfectionLuck.Bad, tq);
					float num2 = immunityOnLethality(d, DebugOutputsInfection.InfectionLuck.Good, tq);
					string result;
					if (num == float.PositiveInfinity)
					{
						result = float.PositiveInfinity.ToString();
					}
					else
					{
						result = Mathf.Clamp01((num2 - 1f) / (num2 - num)).ToStringPercent();
					}
					return result;
				}));
			}
			DebugTables.MakeTablesDialog<HediffDef>(DebugOutputsInfection.InfectionList(), list.ToArray());
		}

		// Token: 0x060052FF RID: 21247 RVA: 0x002A9500 File Offset: 0x002A7900
		[DebugOutput]
		public static void InfectionSimulator()
		{
			LongEventHandler.QueueLongEvent(DebugOutputsInfection.InfectionSimulatorWorker(), "Simulating . . .", null);
		}

		// Token: 0x06005300 RID: 21248 RVA: 0x002A9514 File Offset: 0x002A7914
		private static IEnumerable InfectionSimulatorWorker()
		{
			int trials = 2;
			List<Pawn> doctors = DebugOutputsInfection.GenerateDoctorArray();
			List<int> testSkill = new List<int>
			{
				4,
				10,
				16
			};
			List<ThingDef> testMedicine = new List<ThingDef>
			{
				null,
				ThingDefOf.MedicineHerbal,
				ThingDefOf.MedicineIndustrial,
				ThingDefOf.MedicineUltratech
			};
			PawnGenerationRequest pawngen = new PawnGenerationRequest(PawnKindDefOf.Colonist, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
			int originalTicks = Find.TickManager.TicksGame;
			List<DebugOutputsInfection.InfectionSimRow> results = new List<DebugOutputsInfection.InfectionSimRow>();
			int totalTests = DebugOutputsInfection.InfectionList().Count<HediffDef>() * testMedicine.Count<ThingDef>() * testSkill.Count<int>() * trials;
			int currentTest = 0;
			foreach (HediffDef hediff in DebugOutputsInfection.InfectionList())
			{
				foreach (ThingDef meds in testMedicine)
				{
					foreach (int skill in testSkill)
					{
						DebugOutputsInfection.InfectionSimRow result = default(DebugOutputsInfection.InfectionSimRow);
						result.illness = hediff;
						result.skill = skill;
						result.medicine = meds;
						Pawn doctor = doctors[skill];
						for (int i = 0; i < trials; i++)
						{
							Pawn patient = PawnGenerator.GeneratePawn(pawngen);
							int startTicks = Find.TickManager.TicksGame;
							patient.health.AddHediff(result.illness, null, null, null);
							Hediff activeHediff = patient.health.hediffSet.GetFirstHediffOfDef(result.illness, false);
							while (!patient.Dead && patient.health.hediffSet.HasHediff(result.illness, false))
							{
								if (activeHediff.TendableNow(false))
								{
									activeHediff.Tended(TendUtility.CalculateBaseTendQuality(doctor, patient, meds), 0);
									result.medicineUsed += 1f;
								}
								foreach (Hediff hediff2 in patient.health.hediffSet.GetHediffsTendable())
								{
									hediff2.Tended(TendUtility.CalculateBaseTendQuality(doctor, patient, meds), 0);
								}
								Find.TickManager.DebugSetTicksGame(Find.TickManager.TicksGame + 1);
								patient.health.HealthTick();
								if (Find.TickManager.TicksGame % 900 == 0)
								{
									yield return null;
								}
							}
							if (patient.Dead)
							{
								result.deathChance += 1f;
							}
							else
							{
								result.recoveryTimeDays += (Find.TickManager.TicksGame - startTicks).TicksToDays();
							}
							currentTest++;
							LongEventHandler.SetCurrentEventText(string.Format("Simulating ({0}/{1})", currentTest, totalTests));
							yield return null;
						}
						result.recoveryTimeDays /= (float)trials - result.deathChance;
						result.deathChance /= (float)trials;
						result.medicineUsed /= (float)trials;
						results.Add(result);
					}
				}
			}
			IEnumerable<DebugOutputsInfection.InfectionSimRow> dataSources = results;
			TableDataGetter<DebugOutputsInfection.InfectionSimRow>[] array = new TableDataGetter<DebugOutputsInfection.InfectionSimRow>[6];
			array[0] = new TableDataGetter<DebugOutputsInfection.InfectionSimRow>("defName", (DebugOutputsInfection.InfectionSimRow isr) => isr.illness.defName);
			array[1] = new TableDataGetter<DebugOutputsInfection.InfectionSimRow>("meds", (DebugOutputsInfection.InfectionSimRow isr) => (isr.medicine == null) ? "(none)" : isr.medicine.defName);
			array[2] = new TableDataGetter<DebugOutputsInfection.InfectionSimRow>("skill", (DebugOutputsInfection.InfectionSimRow isr) => isr.skill.ToString());
			array[3] = new TableDataGetter<DebugOutputsInfection.InfectionSimRow>("death chance", (DebugOutputsInfection.InfectionSimRow isr) => isr.deathChance.ToStringPercent());
			array[4] = new TableDataGetter<DebugOutputsInfection.InfectionSimRow>("recovery time (days)", (DebugOutputsInfection.InfectionSimRow isr) => isr.recoveryTimeDays.ToString("F1"));
			array[5] = new TableDataGetter<DebugOutputsInfection.InfectionSimRow>("medicine used", (DebugOutputsInfection.InfectionSimRow isr) => isr.medicineUsed.ToString());
			DebugTables.MakeTablesDialog<DebugOutputsInfection.InfectionSimRow>(dataSources, array);
			Find.TickManager.DebugSetTicksGame(originalTicks);
			yield break;
		}

		// Token: 0x02000E1A RID: 3610
		private enum InfectionLuck
		{
			// Token: 0x04003679 RID: 13945
			Bad,
			// Token: 0x0400367A RID: 13946
			Normal,
			// Token: 0x0400367B RID: 13947
			Good
		}

		// Token: 0x02000E1B RID: 3611
		private struct InfectionSimRow
		{
			// Token: 0x0400367C RID: 13948
			public HediffDef illness;

			// Token: 0x0400367D RID: 13949
			public int skill;

			// Token: 0x0400367E RID: 13950
			public ThingDef medicine;

			// Token: 0x0400367F RID: 13951
			public float deathChance;

			// Token: 0x04003680 RID: 13952
			public float recoveryTimeDays;

			// Token: 0x04003681 RID: 13953
			public float medicineUsed;
		}
	}
}
