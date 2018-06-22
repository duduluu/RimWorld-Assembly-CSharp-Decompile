﻿using System;

namespace RimWorld
{
	// Token: 0x020004F9 RID: 1273
	public static class JoyTunings
	{
		// Token: 0x04000D72 RID: 3442
		public const float BaseJoyGainPerHour = 0.36f;

		// Token: 0x04000D73 RID: 3443
		public const float ThreshLow = 0.15f;

		// Token: 0x04000D74 RID: 3444
		public const float ThreshSatisfied = 0.3f;

		// Token: 0x04000D75 RID: 3445
		public const float ThreshHigh = 0.7f;

		// Token: 0x04000D76 RID: 3446
		public const float ThreshVeryHigh = 0.85f;

		// Token: 0x04000D77 RID: 3447
		public const float BaseFallPerInterval = 0.0015f;

		// Token: 0x04000D78 RID: 3448
		public const float FallRateFactorWhenLow = 0.7f;

		// Token: 0x04000D79 RID: 3449
		public const float FallRateFactorWhenVeryLow = 0.4f;

		// Token: 0x04000D7A RID: 3450
		public const float ToleranceGainPerJoy = 0.65f;

		// Token: 0x04000D7B RID: 3451
		public const float ToleranceDropPerDay = 0.0833333358f;
	}
}