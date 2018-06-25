﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000734 RID: 1844
	public class CompShearable : CompHasGatherableBodyResource
	{
		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x060028AF RID: 10415 RVA: 0x0015B7D0 File Offset: 0x00159BD0
		protected override int GatherResourcesIntervalDays
		{
			get
			{
				return this.Props.shearIntervalDays;
			}
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x060028B0 RID: 10416 RVA: 0x0015B7F0 File Offset: 0x00159BF0
		protected override int ResourceAmount
		{
			get
			{
				return this.Props.woolAmount;
			}
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x060028B1 RID: 10417 RVA: 0x0015B810 File Offset: 0x00159C10
		protected override ThingDef ResourceDef
		{
			get
			{
				return this.Props.woolDef;
			}
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x060028B2 RID: 10418 RVA: 0x0015B830 File Offset: 0x00159C30
		protected override string SaveKey
		{
			get
			{
				return "woolGrowth";
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x060028B3 RID: 10419 RVA: 0x0015B84C File Offset: 0x00159C4C
		public CompProperties_Shearable Props
		{
			get
			{
				return (CompProperties_Shearable)this.props;
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x060028B4 RID: 10420 RVA: 0x0015B86C File Offset: 0x00159C6C
		protected override bool Active
		{
			get
			{
				bool result;
				if (!base.Active)
				{
					result = false;
				}
				else
				{
					Pawn pawn = this.parent as Pawn;
					result = (pawn == null || pawn.ageTracker.CurLifeStage.shearable);
				}
				return result;
			}
		}

		// Token: 0x060028B5 RID: 10421 RVA: 0x0015B8C4 File Offset: 0x00159CC4
		public override string CompInspectStringExtra()
		{
			string result;
			if (!this.Active)
			{
				result = null;
			}
			else
			{
				result = "WoolGrowth".Translate() + ": " + base.Fullness.ToStringPercent();
			}
			return result;
		}
	}
}
