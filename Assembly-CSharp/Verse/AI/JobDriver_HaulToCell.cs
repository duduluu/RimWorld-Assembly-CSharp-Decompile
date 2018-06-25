﻿using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x02000A41 RID: 2625
	public class JobDriver_HaulToCell : JobDriver
	{
		// Token: 0x0400251F RID: 9503
		private bool forbiddenInitially;

		// Token: 0x04002520 RID: 9504
		private const TargetIndex HaulableInd = TargetIndex.A;

		// Token: 0x04002521 RID: 9505
		private const TargetIndex StoreCellInd = TargetIndex.B;

		// Token: 0x06003A3A RID: 14906 RVA: 0x001EE2C7 File Offset: 0x001EC6C7
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.forbiddenInitially, "forbiddenInitially", false, false);
		}

		// Token: 0x06003A3B RID: 14907 RVA: 0x001EE2E4 File Offset: 0x001EC6E4
		public override string GetReport()
		{
			IntVec3 cell = this.job.targetB.Cell;
			Thing thing = null;
			if (this.pawn.CurJob == this.job && this.pawn.carryTracker.CarriedThing != null)
			{
				thing = this.pawn.carryTracker.CarriedThing;
			}
			else if (base.TargetThingA != null && base.TargetThingA.Spawned)
			{
				thing = base.TargetThingA;
			}
			string result;
			if (thing == null)
			{
				result = "ReportHaulingUnknown".Translate();
			}
			else
			{
				string text = null;
				SlotGroup slotGroup = cell.GetSlotGroup(base.Map);
				if (slotGroup != null)
				{
					text = slotGroup.parent.SlotYielderLabel();
				}
				if (text != null)
				{
					result = "ReportHaulingTo".Translate(new object[]
					{
						thing.Label,
						text
					});
				}
				else
				{
					result = "ReportHauling".Translate(new object[]
					{
						thing.Label
					});
				}
			}
			return result;
		}

		// Token: 0x06003A3C RID: 14908 RVA: 0x001EE3EC File Offset: 0x001EC7EC
		public override bool TryMakePreToilReservations()
		{
			return this.pawn.Reserve(this.job.GetTarget(TargetIndex.B), this.job, 1, -1, null) && this.pawn.Reserve(this.job.GetTarget(TargetIndex.A), this.job, 1, -1, null);
		}

		// Token: 0x06003A3D RID: 14909 RVA: 0x001EE449 File Offset: 0x001EC849
		public override void Notify_Starting()
		{
			base.Notify_Starting();
			if (base.TargetThingA != null)
			{
				this.forbiddenInitially = base.TargetThingA.IsForbidden(this.pawn);
			}
			else
			{
				this.forbiddenInitially = false;
			}
		}

		// Token: 0x06003A3E RID: 14910 RVA: 0x001EE480 File Offset: 0x001EC880
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.B);
			if (!this.forbiddenInitially)
			{
				this.FailOnForbidden(TargetIndex.A);
			}
			Toil reserveTargetA = Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return reserveTargetA;
			Toil toilGoto = null;
			toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A).FailOn(delegate()
			{
				Pawn actor = toilGoto.actor;
				Job curJob = actor.jobs.curJob;
				if (curJob.haulMode == HaulMode.ToCellStorage)
				{
					Thing thing = curJob.GetTarget(TargetIndex.A).Thing;
					IntVec3 cell = actor.jobs.curJob.GetTarget(TargetIndex.B).Cell;
					if (!cell.IsValidStorageFor(this.Map, thing))
					{
						return true;
					}
				}
				return false;
			});
			yield return toilGoto;
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, true, false);
			if (this.job.haulOpportunisticDuplicates)
			{
				yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveTargetA, TargetIndex.A, TargetIndex.B, false, null);
			}
			Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
			yield return carryToCell;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, true);
			yield break;
		}
	}
}
