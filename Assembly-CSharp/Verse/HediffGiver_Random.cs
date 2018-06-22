﻿using System;

namespace Verse
{
	// Token: 0x02000D35 RID: 3381
	public class HediffGiver_Random : HediffGiver
	{
		// Token: 0x06004A8C RID: 19084 RVA: 0x0026E282 File Offset: 0x0026C682
		public override void OnIntervalPassed(Pawn pawn, Hediff cause)
		{
			if (Rand.MTBEventOccurs(this.mtbDays, 60000f, 60f))
			{
				if (base.TryApply(pawn, null))
				{
					base.SendLetter(pawn, cause);
				}
			}
		}

		// Token: 0x04003257 RID: 12887
		public float mtbDays = 0f;
	}
}
