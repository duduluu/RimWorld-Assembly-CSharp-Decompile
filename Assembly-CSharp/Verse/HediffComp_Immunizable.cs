﻿using System;
using System.Text;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000D21 RID: 3361
	[StaticConstructorOnStartup]
	public class HediffComp_Immunizable : HediffComp_SeverityPerDay
	{
		// Token: 0x04003239 RID: 12857
		private float severityPerDayNotImmuneRandomFactor = 1f;

		// Token: 0x0400323A RID: 12858
		private static readonly Texture2D IconImmune = ContentFinder<Texture2D>.Get("UI/Icons/Medical/IconImmune", true);

		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x06004A07 RID: 18951 RVA: 0x0026BBA4 File Offset: 0x00269FA4
		public HediffCompProperties_Immunizable Props
		{
			get
			{
				return (HediffCompProperties_Immunizable)this.props;
			}
		}

		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x06004A08 RID: 18952 RVA: 0x0026BBC4 File Offset: 0x00269FC4
		public override string CompLabelInBracketsExtra
		{
			get
			{
				string result;
				if (this.FullyImmune)
				{
					result = "DevelopedImmunityLower".Translate();
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x06004A09 RID: 18953 RVA: 0x0026BBF8 File Offset: 0x00269FF8
		public override string CompTipStringExtra
		{
			get
			{
				string result;
				if (base.Def.PossibleToDevelopImmunityNaturally() && !this.FullyImmune)
				{
					result = "Immunity".Translate() + ": " + (Mathf.Floor(this.Immunity * 100f) / 100f).ToStringPercent();
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x06004A0A RID: 18954 RVA: 0x0026BC60 File Offset: 0x0026A060
		public float Immunity
		{
			get
			{
				return base.Pawn.health.immunity.GetImmunity(base.Def);
			}
		}

		// Token: 0x17000BC2 RID: 3010
		// (get) Token: 0x06004A0B RID: 18955 RVA: 0x0026BC90 File Offset: 0x0026A090
		public bool FullyImmune
		{
			get
			{
				return this.Immunity >= 1f;
			}
		}

		// Token: 0x17000BC3 RID: 3011
		// (get) Token: 0x06004A0C RID: 18956 RVA: 0x0026BCB8 File Offset: 0x0026A0B8
		public override TextureAndColor CompStateIcon
		{
			get
			{
				TextureAndColor result;
				if (this.FullyImmune)
				{
					result = HediffComp_Immunizable.IconImmune;
				}
				else
				{
					result = TextureAndColor.None;
				}
				return result;
			}
		}

		// Token: 0x06004A0D RID: 18957 RVA: 0x0026BCED File Offset: 0x0026A0ED
		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			base.CompPostPostAdd(dinfo);
			this.severityPerDayNotImmuneRandomFactor = this.Props.severityPerDayNotImmuneRandomFactor.RandomInRange;
		}

		// Token: 0x06004A0E RID: 18958 RVA: 0x0026BD0D File Offset: 0x0026A10D
		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look<float>(ref this.severityPerDayNotImmuneRandomFactor, "severityPerDayNotImmuneRandomFactor", 1f, false);
		}

		// Token: 0x06004A0F RID: 18959 RVA: 0x0026BD2C File Offset: 0x0026A12C
		protected override float SeverityChangePerDay()
		{
			return (!this.FullyImmune) ? (this.Props.severityPerDayNotImmune * this.severityPerDayNotImmuneRandomFactor) : this.Props.severityPerDayImmune;
		}

		// Token: 0x06004A10 RID: 18960 RVA: 0x0026BD70 File Offset: 0x0026A170
		public override string CompDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.CompDebugString());
			if (this.severityPerDayNotImmuneRandomFactor != 1f)
			{
				stringBuilder.AppendLine("severityPerDayNotImmuneRandomFactor: " + this.severityPerDayNotImmuneRandomFactor.ToString("0.##"));
			}
			if (!base.Pawn.Dead)
			{
				ImmunityRecord immunityRecord = base.Pawn.health.immunity.GetImmunityRecord(base.Def);
				if (immunityRecord != null)
				{
					stringBuilder.AppendLine("immunity change per day: " + (immunityRecord.ImmunityChangePerTick(base.Pawn, true, this.parent) * 60000f).ToString("F3"));
					stringBuilder.AppendLine("  pawn immunity gain speed: " + StatDefOf.ImmunityGainSpeed.ValueToString(base.Pawn.GetStatValue(StatDefOf.ImmunityGainSpeed, true), ToStringNumberSense.Absolute));
				}
			}
			return stringBuilder.ToString();
		}
	}
}
