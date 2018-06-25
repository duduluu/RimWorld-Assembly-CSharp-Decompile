﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000211 RID: 529
	public class ThoughtWorker_Hot : ThoughtWorker
	{
		// Token: 0x060009ED RID: 2541 RVA: 0x00058C70 File Offset: 0x00057070
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			float statValue = p.GetStatValue(StatDefOf.ComfyTemperatureMax, true);
			float ambientTemperature = p.AmbientTemperature;
			float num = ambientTemperature - statValue;
			ThoughtState result;
			if (num <= 0f)
			{
				result = ThoughtState.Inactive;
			}
			else if (num < 10f)
			{
				result = ThoughtState.ActiveAtStage(0);
			}
			else if (num < 20f)
			{
				result = ThoughtState.ActiveAtStage(1);
			}
			else if (num < 30f)
			{
				result = ThoughtState.ActiveAtStage(2);
			}
			else
			{
				result = ThoughtState.ActiveAtStage(3);
			}
			return result;
		}
	}
}
