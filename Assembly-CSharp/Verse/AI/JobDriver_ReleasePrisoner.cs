﻿using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x0200007B RID: 123
	public class JobDriver_ReleasePrisoner : JobDriver
	{
		// Token: 0x04000230 RID: 560
		private const TargetIndex PrisonerInd = TargetIndex.A;

		// Token: 0x04000231 RID: 561
		private const TargetIndex ReleaseCellInd = TargetIndex.B;

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000349 RID: 841 RVA: 0x000246C0 File Offset: 0x00022AC0
		private Pawn Prisoner
		{
			get
			{
				return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x000246F0 File Offset: 0x00022AF0
		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.Prisoner, this.job, 1, -1, null);
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00024724 File Offset: 0x00022B24
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.B);
			this.FailOn(() => ((Pawn)((Thing)this.GetActor().CurJob.GetTarget(TargetIndex.A))).guest.interactionMode != PrisonerInteractionModeDefOf.Release);
			this.FailOnDowned(TargetIndex.A);
			this.FailOnAggroMentalState(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOn(() => !this.Prisoner.IsPrisonerOfColony || !this.Prisoner.guest.PrisonerIsSecure).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
			Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
			yield return carryToCell;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, false);
			Toil setReleased = new Toil();
			setReleased.initAction = delegate()
			{
				Pawn actor = setReleased.actor;
				Job curJob = actor.jobs.curJob;
				Pawn p = curJob.targetA.Thing as Pawn;
				GenGuest.PrisonerRelease(p);
			};
			yield return setReleased;
			yield break;
		}
	}
}
