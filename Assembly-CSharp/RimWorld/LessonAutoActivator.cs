﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020008B5 RID: 2229
	public static class LessonAutoActivator
	{
		// Token: 0x04001B88 RID: 7048
		private static Dictionary<ConceptDef, float> opportunities = new Dictionary<ConceptDef, float>();

		// Token: 0x04001B89 RID: 7049
		private static float timeSinceLastLesson = 10000f;

		// Token: 0x04001B8A RID: 7050
		private static List<ConceptDef> alertingConcepts = new List<ConceptDef>();

		// Token: 0x04001B8B RID: 7051
		private const float MapStartGracePeriod = 8f;

		// Token: 0x04001B8C RID: 7052
		private const float KnowledgeDecayRate = 0.00015f;

		// Token: 0x04001B8D RID: 7053
		private const float OpportunityDecayRate = 0.4f;

		// Token: 0x04001B8E RID: 7054
		private const float OpportunityMaxDesireAdd = 60f;

		// Token: 0x04001B8F RID: 7055
		private const int CheckInterval = 15;

		// Token: 0x04001B90 RID: 7056
		private const float MaxLessonInterval = 900f;

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x060032EF RID: 13039 RVA: 0x001B6DC8 File Offset: 0x001B51C8
		private static float SecondsSinceLesson
		{
			get
			{
				return LessonAutoActivator.timeSinceLastLesson;
			}
		}

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x060032F0 RID: 13040 RVA: 0x001B6DE4 File Offset: 0x001B51E4
		private static float RelaxDesire
		{
			get
			{
				return 100f - LessonAutoActivator.SecondsSinceLesson * 0.111111112f;
			}
		}

		// Token: 0x060032F1 RID: 13041 RVA: 0x001B6E0A File Offset: 0x001B520A
		public static void Reset()
		{
			LessonAutoActivator.alertingConcepts.Clear();
		}

		// Token: 0x060032F2 RID: 13042 RVA: 0x001B6E17 File Offset: 0x001B5217
		public static void TeachOpportunity(ConceptDef conc, OpportunityType opp)
		{
			LessonAutoActivator.TeachOpportunity(conc, null, opp);
		}

		// Token: 0x060032F3 RID: 13043 RVA: 0x001B6E24 File Offset: 0x001B5224
		public static void TeachOpportunity(ConceptDef conc, Thing subject, OpportunityType opp)
		{
			if (TutorSystem.AdaptiveTrainingEnabled && !PlayerKnowledgeDatabase.IsComplete(conc))
			{
				float value = 999f;
				switch (opp)
				{
				case OpportunityType.GoodToKnow:
					value = 60f;
					break;
				case OpportunityType.Important:
					value = 80f;
					break;
				case OpportunityType.Critical:
					value = 100f;
					break;
				default:
					Log.Error("Unknown need", false);
					break;
				}
				LessonAutoActivator.opportunities[conc] = value;
				if (opp >= OpportunityType.Important || Find.Tutor.learningReadout.ActiveConceptsCount < 4)
				{
					LessonAutoActivator.TryInitiateLesson(conc);
				}
			}
		}

		// Token: 0x060032F4 RID: 13044 RVA: 0x001B6EC8 File Offset: 0x001B52C8
		public static void Notify_KnowledgeDemonstrated(ConceptDef conc)
		{
			if (PlayerKnowledgeDatabase.IsComplete(conc))
			{
				LessonAutoActivator.opportunities[conc] = 0f;
			}
		}

		// Token: 0x060032F5 RID: 13045 RVA: 0x001B6EE8 File Offset: 0x001B52E8
		public static void LessonAutoActivatorUpdate()
		{
			if (TutorSystem.AdaptiveTrainingEnabled && Current.Game != null && !Find.Tutor.learningReadout.ShowAllMode)
			{
				LessonAutoActivator.timeSinceLastLesson += RealTime.realDeltaTime;
				if (Current.ProgramState == ProgramState.Playing)
				{
					if (Time.timeSinceLevelLoad < 8f || Find.WindowStack.SecondsSinceClosedGameStartDialog < 8f || Find.TickManager.NotPlaying)
					{
						return;
					}
				}
				for (int i = LessonAutoActivator.alertingConcepts.Count - 1; i >= 0; i--)
				{
					if (PlayerKnowledgeDatabase.IsComplete(LessonAutoActivator.alertingConcepts[i]))
					{
						LessonAutoActivator.alertingConcepts.RemoveAt(i);
					}
				}
				if (Time.frameCount % 15 == 0 && Find.ActiveLesson.Current == null)
				{
					for (int j = 0; j < DefDatabase<ConceptDef>.AllDefsListForReading.Count; j++)
					{
						ConceptDef conceptDef = DefDatabase<ConceptDef>.AllDefsListForReading[j];
						if (!PlayerKnowledgeDatabase.IsComplete(conceptDef))
						{
							float num = PlayerKnowledgeDatabase.GetKnowledge(conceptDef);
							num -= 0.00015f * Time.deltaTime * 15f;
							if (num < 0f)
							{
								num = 0f;
							}
							PlayerKnowledgeDatabase.SetKnowledge(conceptDef, num);
							if (conceptDef.opportunityDecays)
							{
								float num2 = LessonAutoActivator.GetOpportunity(conceptDef);
								num2 -= 0.4f * Time.deltaTime * 15f;
								if (num2 < 0f)
								{
									num2 = 0f;
								}
								LessonAutoActivator.opportunities[conceptDef] = num2;
							}
						}
					}
					if (Find.Tutor.learningReadout.ActiveConceptsCount < 3)
					{
						ConceptDef conceptDef2 = LessonAutoActivator.MostDesiredConcept();
						if (conceptDef2 != null)
						{
							float desire = LessonAutoActivator.GetDesire(conceptDef2);
							if (desire > 0.1f && LessonAutoActivator.RelaxDesire < desire)
							{
								LessonAutoActivator.TryInitiateLesson(conceptDef2);
							}
						}
					}
					else
					{
						LessonAutoActivator.SetLastLessonTimeToNow();
					}
				}
			}
		}

		// Token: 0x060032F6 RID: 13046 RVA: 0x001B70E4 File Offset: 0x001B54E4
		private static ConceptDef MostDesiredConcept()
		{
			float num = -9999f;
			ConceptDef result = null;
			List<ConceptDef> allDefsListForReading = DefDatabase<ConceptDef>.AllDefsListForReading;
			int i = 0;
			while (i < allDefsListForReading.Count)
			{
				ConceptDef conceptDef = allDefsListForReading[i];
				float desire = LessonAutoActivator.GetDesire(conceptDef);
				if (desire > num)
				{
					if (!conceptDef.needsOpportunity || LessonAutoActivator.GetOpportunity(conceptDef) >= 0.1f)
					{
						if (PlayerKnowledgeDatabase.GetKnowledge(conceptDef) <= 0.15f)
						{
							num = desire;
							result = conceptDef;
						}
					}
				}
				IL_72:
				i++;
				continue;
				goto IL_72;
			}
			return result;
		}

		// Token: 0x060032F7 RID: 13047 RVA: 0x001B7180 File Offset: 0x001B5580
		private static float GetDesire(ConceptDef conc)
		{
			float result;
			if (PlayerKnowledgeDatabase.IsComplete(conc))
			{
				result = 0f;
			}
			else if (Find.Tutor.learningReadout.IsActive(conc))
			{
				result = 0f;
			}
			else if (Current.ProgramState != conc.gameMode)
			{
				result = 0f;
			}
			else if (conc.needsOpportunity && LessonAutoActivator.GetOpportunity(conc) < 0.1f)
			{
				result = 0f;
			}
			else
			{
				float num = 0f;
				num += conc.priority;
				num += LessonAutoActivator.GetOpportunity(conc) / 100f * 60f;
				num *= 1f - PlayerKnowledgeDatabase.GetKnowledge(conc);
				result = num;
			}
			return result;
		}

		// Token: 0x060032F8 RID: 13048 RVA: 0x001B7240 File Offset: 0x001B5640
		private static float GetOpportunity(ConceptDef conc)
		{
			float num;
			float result;
			if (LessonAutoActivator.opportunities.TryGetValue(conc, out num))
			{
				result = num;
			}
			else
			{
				LessonAutoActivator.opportunities[conc] = 0f;
				result = 0f;
			}
			return result;
		}

		// Token: 0x060032F9 RID: 13049 RVA: 0x001B7284 File Offset: 0x001B5684
		private static void TryInitiateLesson(ConceptDef conc)
		{
			if (Find.Tutor.learningReadout.TryActivateConcept(conc))
			{
				LessonAutoActivator.SetLastLessonTimeToNow();
			}
		}

		// Token: 0x060032FA RID: 13050 RVA: 0x001B72A1 File Offset: 0x001B56A1
		private static void SetLastLessonTimeToNow()
		{
			LessonAutoActivator.timeSinceLastLesson = 0f;
		}

		// Token: 0x060032FB RID: 13051 RVA: 0x001B72AE File Offset: 0x001B56AE
		public static void Notify_TutorialEnding()
		{
			LessonAutoActivator.SetLastLessonTimeToNow();
		}

		// Token: 0x060032FC RID: 13052 RVA: 0x001B72B8 File Offset: 0x001B56B8
		public static string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("RelaxDesire: " + LessonAutoActivator.RelaxDesire);
			foreach (ConceptDef conceptDef in from co in DefDatabase<ConceptDef>.AllDefs
			orderby LessonAutoActivator.GetDesire(co) descending
			select co)
			{
				if (PlayerKnowledgeDatabase.IsComplete(conceptDef))
				{
					stringBuilder.AppendLine(conceptDef.defName + " complete");
				}
				else
				{
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						conceptDef.defName,
						"\n   know ",
						PlayerKnowledgeDatabase.GetKnowledge(conceptDef).ToString("F3"),
						"\n   need ",
						LessonAutoActivator.opportunities[conceptDef].ToString("F3"),
						"\n   des ",
						LessonAutoActivator.GetDesire(conceptDef).ToString("F3")
					}));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060032FD RID: 13053 RVA: 0x001B7408 File Offset: 0x001B5808
		public static void DebugForceInitiateBestLessonNow()
		{
			LessonAutoActivator.TryInitiateLesson((from def in DefDatabase<ConceptDef>.AllDefs
			orderby LessonAutoActivator.GetDesire(def) descending
			select def).First<ConceptDef>());
		}
	}
}
