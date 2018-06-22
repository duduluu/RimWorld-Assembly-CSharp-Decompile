﻿using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse.AI.Group
{
	// Token: 0x02000A00 RID: 2560
	public class TransitionAction_EndAttackBuildingJobs : TransitionAction
	{
		// Token: 0x06003978 RID: 14712 RVA: 0x001E7A58 File Offset: 0x001E5E58
		public override void DoAction(Transition trans)
		{
			List<Pawn> ownedPawns = trans.target.lord.ownedPawns;
			for (int i = 0; i < ownedPawns.Count; i++)
			{
				Pawn pawn = ownedPawns[i];
				if (pawn.jobs != null && pawn.jobs.curJob != null && pawn.jobs.curJob.def == JobDefOf.AttackMelee && pawn.jobs.curJob.targetA.Thing is Building)
				{
					pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
				}
			}
		}
	}
}