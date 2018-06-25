﻿using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x02000A3B RID: 2619
	public class JobDriver_AttackMelee : JobDriver
	{
		// Token: 0x0400250F RID: 9487
		private int numMeleeAttacksMade = 0;

		// Token: 0x06003A18 RID: 14872 RVA: 0x001EBBB4 File Offset: 0x001E9FB4
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.numMeleeAttacksMade, "numMeleeAttacksMade", 0, false);
		}

		// Token: 0x06003A19 RID: 14873 RVA: 0x001EBBD0 File Offset: 0x001E9FD0
		public override bool TryMakePreToilReservations()
		{
			IAttackTarget attackTarget = this.job.targetA.Thing as IAttackTarget;
			if (attackTarget != null)
			{
				this.pawn.Map.attackTargetReservationManager.Reserve(this.pawn, this.job, attackTarget);
			}
			return true;
		}

		// Token: 0x06003A1A RID: 14874 RVA: 0x001EBC24 File Offset: 0x001EA024
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_General.DoAtomic(delegate
			{
				Pawn pawn = this.job.targetA.Thing as Pawn;
				if (pawn != null && pawn.Downed && this.pawn.mindState.duty != null && this.pawn.mindState.duty.attackDownedIfStarving && this.pawn.Starving())
				{
					this.job.killIncappedTarget = true;
				}
			});
			yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
			yield return Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, delegate
			{
				Thing thing = this.job.GetTarget(TargetIndex.A).Thing;
				if (this.pawn.meleeVerbs.TryMeleeAttack(thing, this.job.verbToUse, false))
				{
					if (this.pawn.CurJob != null && this.pawn.jobs.curDriver == this)
					{
						this.numMeleeAttacksMade++;
						if (this.numMeleeAttacksMade >= this.job.maxNumMeleeAttacks)
						{
							base.EndJobWith(JobCondition.Succeeded);
						}
					}
				}
			}).FailOnDespawnedOrNull(TargetIndex.A);
			yield break;
		}

		// Token: 0x06003A1B RID: 14875 RVA: 0x001EBC50 File Offset: 0x001EA050
		public override void Notify_PatherFailed()
		{
			if (this.job.attackDoorIfTargetLost)
			{
				Thing thing;
				using (PawnPath pawnPath = base.Map.pathFinder.FindPath(this.pawn.Position, base.TargetA.Cell, TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
				{
					if (!pawnPath.Found)
					{
						return;
					}
					IntVec3 intVec;
					thing = pawnPath.FirstBlockingBuilding(out intVec, this.pawn);
				}
				if (thing != null)
				{
					this.job.targetA = thing;
					this.job.maxNumMeleeAttacks = Rand.RangeInclusive(2, 5);
					this.job.expiryInterval = Rand.Range(2000, 4000);
					return;
				}
			}
			base.Notify_PatherFailed();
		}

		// Token: 0x06003A1C RID: 14876 RVA: 0x001EBD40 File Offset: 0x001EA140
		public override bool IsContinuation(Job j)
		{
			return this.job.GetTarget(TargetIndex.A) == j.GetTarget(TargetIndex.A);
		}
	}
}
