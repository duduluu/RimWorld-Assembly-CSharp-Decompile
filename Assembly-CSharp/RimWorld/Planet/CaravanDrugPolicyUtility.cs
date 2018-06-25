﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005D7 RID: 1495
	public static class CaravanDrugPolicyUtility
	{
		// Token: 0x06001D69 RID: 7529 RVA: 0x000FD0E8 File Offset: 0x000FB4E8
		public static void TryTakeScheduledDrugs(Caravan caravan)
		{
			List<Pawn> pawnsListForReading = caravan.PawnsListForReading;
			for (int i = 0; i < pawnsListForReading.Count; i++)
			{
				CaravanDrugPolicyUtility.TryTakeScheduledDrugs(pawnsListForReading[i], caravan);
			}
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x000FD124 File Offset: 0x000FB524
		private static void TryTakeScheduledDrugs(Pawn pawn, Caravan caravan)
		{
			if (pawn.drugs != null)
			{
				DrugPolicy currentPolicy = pawn.drugs.CurrentPolicy;
				for (int i = 0; i < currentPolicy.Count; i++)
				{
					if (pawn.drugs.ShouldTryToTakeScheduledNow(currentPolicy[i].drug))
					{
						Thing drug;
						Pawn drugOwner;
						if (CaravanInventoryUtility.TryGetThingOfDef(caravan, currentPolicy[i].drug, out drug, out drugOwner))
						{
							CaravanPawnsNeedsUtility.IngestDrug(pawn, drug, drugOwner, caravan);
						}
					}
				}
			}
		}
	}
}
