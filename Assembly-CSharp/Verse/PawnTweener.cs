﻿using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000CE7 RID: 3303
	public class PawnTweener
	{
		// Token: 0x04003144 RID: 12612
		private Pawn pawn;

		// Token: 0x04003145 RID: 12613
		private Vector3 tweenedPos = new Vector3(0f, 0f, 0f);

		// Token: 0x04003146 RID: 12614
		private int lastDrawFrame = -1;

		// Token: 0x04003147 RID: 12615
		private Vector3 lastTickSpringPos;

		// Token: 0x04003148 RID: 12616
		private const float SpringTightness = 0.09f;

		// Token: 0x060048C1 RID: 18625 RVA: 0x002631CB File Offset: 0x002615CB
		public PawnTweener(Pawn pawn)
		{
			this.pawn = pawn;
		}

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x060048C2 RID: 18626 RVA: 0x002631FC File Offset: 0x002615FC
		public Vector3 TweenedPos
		{
			get
			{
				return this.tweenedPos;
			}
		}

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x060048C3 RID: 18627 RVA: 0x00263218 File Offset: 0x00261618
		public Vector3 LastTickTweenedVelocity
		{
			get
			{
				return this.TweenedPos - this.lastTickSpringPos;
			}
		}

		// Token: 0x060048C4 RID: 18628 RVA: 0x00263240 File Offset: 0x00261640
		public void PreDrawPosCalculation()
		{
			if (this.lastDrawFrame != RealTime.frameCount)
			{
				if (this.lastDrawFrame < RealTime.frameCount - 1)
				{
					this.ResetTweenedPosToRoot();
				}
				else
				{
					this.lastTickSpringPos = this.tweenedPos;
					float tickRateMultiplier = Find.TickManager.TickRateMultiplier;
					if (tickRateMultiplier < 5f)
					{
						Vector3 a = this.TweenedPosRoot() - this.tweenedPos;
						float num = 0.09f * (RealTime.deltaTime * 60f * tickRateMultiplier);
						if (RealTime.deltaTime > 0.05f)
						{
							num = Mathf.Min(num, 1f);
						}
						this.tweenedPos += a * num;
					}
					else
					{
						this.tweenedPos = this.TweenedPosRoot();
					}
				}
				this.lastDrawFrame = RealTime.frameCount;
			}
		}

		// Token: 0x060048C5 RID: 18629 RVA: 0x0026331F File Offset: 0x0026171F
		public void ResetTweenedPosToRoot()
		{
			this.tweenedPos = this.TweenedPosRoot();
			this.lastTickSpringPos = this.tweenedPos;
		}

		// Token: 0x060048C6 RID: 18630 RVA: 0x0026333C File Offset: 0x0026173C
		private Vector3 TweenedPosRoot()
		{
			Vector3 result;
			if (!this.pawn.Spawned)
			{
				result = this.pawn.Position.ToVector3Shifted();
			}
			else
			{
				float num = this.MovedPercent();
				result = this.pawn.pather.nextCell.ToVector3Shifted() * num + this.pawn.Position.ToVector3Shifted() * (1f - num) + PawnCollisionTweenerUtility.PawnCollisionPosOffsetFor(this.pawn);
			}
			return result;
		}

		// Token: 0x060048C7 RID: 18631 RVA: 0x002633D0 File Offset: 0x002617D0
		private float MovedPercent()
		{
			float result;
			if (!this.pawn.pather.Moving)
			{
				result = 0f;
			}
			else if (this.pawn.stances.FullBodyBusy)
			{
				result = 0f;
			}
			else if (this.pawn.pather.BuildingBlockingNextPathCell() != null)
			{
				result = 0f;
			}
			else if (this.pawn.pather.NextCellDoorToManuallyOpen() != null)
			{
				result = 0f;
			}
			else if (this.pawn.pather.WillCollideWithPawnOnNextPathCell())
			{
				result = 0f;
			}
			else
			{
				result = 1f - this.pawn.pather.nextCellCostLeft / this.pawn.pather.nextCellCostTotal;
			}
			return result;
		}
	}
}
