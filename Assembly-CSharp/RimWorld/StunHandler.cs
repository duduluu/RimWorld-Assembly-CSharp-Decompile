﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020006CE RID: 1742
	public class StunHandler : IExposable
	{
		// Token: 0x04001514 RID: 5396
		public Thing parent;

		// Token: 0x04001515 RID: 5397
		private int stunTicksLeft = 0;

		// Token: 0x04001516 RID: 5398
		private Mote moteStun = null;

		// Token: 0x04001517 RID: 5399
		private int EMPAdaptedTicksLeft = 0;

		// Token: 0x04001518 RID: 5400
		public const float StunDurationTicksPerDamage = 30f;

		// Token: 0x060025B0 RID: 9648 RVA: 0x00142FA3 File Offset: 0x001413A3
		public StunHandler(Thing parent)
		{
			this.parent = parent;
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x060025B1 RID: 9649 RVA: 0x00142FC8 File Offset: 0x001413C8
		public bool Stunned
		{
			get
			{
				return this.stunTicksLeft > 0;
			}
		}

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x060025B2 RID: 9650 RVA: 0x00142FE8 File Offset: 0x001413E8
		private int EMPAdaptationTicksDuration
		{
			get
			{
				Pawn pawn = this.parent as Pawn;
				int result;
				if (pawn != null && pawn.RaceProps.IsMechanoid)
				{
					result = 2200;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x0014302B File Offset: 0x0014142B
		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.stunTicksLeft, "stunTicksLeft", 0, false);
			Scribe_Values.Look<int>(ref this.EMPAdaptedTicksLeft, "EMPAdaptedTicksLeft", 0, false);
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x00143054 File Offset: 0x00141454
		public void StunHandlerTick()
		{
			if (this.EMPAdaptedTicksLeft > 0)
			{
				this.EMPAdaptedTicksLeft--;
			}
			if (this.stunTicksLeft > 0)
			{
				this.stunTicksLeft--;
				if (this.moteStun == null || this.moteStun.Destroyed)
				{
					this.moteStun = MoteMaker.MakeStunOverlay(this.parent);
				}
				Pawn pawn = this.parent as Pawn;
				if (pawn != null && pawn.Downed)
				{
					this.stunTicksLeft = 0;
				}
				if (this.moteStun != null)
				{
					this.moteStun.Maintain();
				}
			}
		}

		// Token: 0x060025B5 RID: 9653 RVA: 0x00143100 File Offset: 0x00141500
		public void Notify_DamageApplied(DamageInfo dinfo, bool affectedByEMP)
		{
			Pawn pawn = this.parent as Pawn;
			if (pawn == null || (!pawn.Downed && !pawn.Dead))
			{
				if (dinfo.Def == DamageDefOf.Stun)
				{
					this.StunFor(Mathf.RoundToInt(dinfo.Amount * 30f), dinfo.Instigator);
				}
				else if (dinfo.Def == DamageDefOf.EMP && affectedByEMP)
				{
					if (this.EMPAdaptedTicksLeft <= 0)
					{
						this.StunFor(Mathf.RoundToInt(dinfo.Amount * 30f), dinfo.Instigator);
						this.EMPAdaptedTicksLeft = this.EMPAdaptationTicksDuration;
					}
					else
					{
						Vector3 loc = new Vector3((float)this.parent.Position.x + 1f, (float)this.parent.Position.y, (float)this.parent.Position.z + 1f);
						MoteMaker.ThrowText(loc, this.parent.Map, "Adapted".Translate(), Color.white, -1f);
					}
				}
			}
		}

		// Token: 0x060025B6 RID: 9654 RVA: 0x00143240 File Offset: 0x00141640
		public void StunFor(int ticks, Thing instigator)
		{
			this.stunTicksLeft = Mathf.Max(this.stunTicksLeft, ticks);
			Find.BattleLog.Add(new BattleLogEntry_Event(this.parent, RulePackDefOf.Event_Stun, instigator));
		}
	}
}
