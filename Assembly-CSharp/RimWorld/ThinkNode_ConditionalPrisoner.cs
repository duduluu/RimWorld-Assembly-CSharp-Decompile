﻿using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x020001BB RID: 443
	public class ThinkNode_ConditionalPrisoner : ThinkNode_Conditional
	{
		// Token: 0x0600092B RID: 2347 RVA: 0x00055E64 File Offset: 0x00054264
		protected override bool Satisfied(Pawn pawn)
		{
			return pawn.IsPrisoner;
		}
	}
}
