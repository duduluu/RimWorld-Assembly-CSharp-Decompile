﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000D39 RID: 3385
	public class ImmunityHandler : IExposable
	{
		// Token: 0x06004ABE RID: 19134 RVA: 0x00270620 File Offset: 0x0026EA20
		public ImmunityHandler(Pawn pawn)
		{
			this.pawn = pawn;
		}

		// Token: 0x06004ABF RID: 19135 RVA: 0x0027063B File Offset: 0x0026EA3B
		public void ExposeData()
		{
			Scribe_Collections.Look<ImmunityRecord>(ref this.immunityList, "imList", LookMode.Deep, new object[0]);
		}

		// Token: 0x06004AC0 RID: 19136 RVA: 0x00270658 File Offset: 0x0026EA58
		public float DiseaseContractChanceFactor(HediffDef diseaseDef, BodyPartRecord part = null)
		{
			HediffDef hediffDef = null;
			return this.DiseaseContractChanceFactor(diseaseDef, out hediffDef, part);
		}

		// Token: 0x06004AC1 RID: 19137 RVA: 0x0027067C File Offset: 0x0026EA7C
		public float DiseaseContractChanceFactor(HediffDef diseaseDef, out HediffDef immunityCause, BodyPartRecord part = null)
		{
			immunityCause = null;
			float result;
			if (!this.pawn.RaceProps.IsFlesh)
			{
				result = 0f;
			}
			else
			{
				List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					if (hediffs[i].def == diseaseDef && hediffs[i].Part == part)
					{
						return 0f;
					}
				}
				for (int j = 0; j < this.immunityList.Count; j++)
				{
					if (this.immunityList[j].hediffDef == diseaseDef)
					{
						immunityCause = this.immunityList[j].source;
						return Mathf.Lerp(1f, 0f, this.immunityList[j].immunity / 0.6f);
					}
				}
				result = 1f;
			}
			return result;
		}

		// Token: 0x06004AC2 RID: 19138 RVA: 0x0027078C File Offset: 0x0026EB8C
		public float GetImmunity(HediffDef def)
		{
			for (int i = 0; i < this.immunityList.Count; i++)
			{
				ImmunityRecord immunityRecord = this.immunityList[i];
				if (immunityRecord.hediffDef == def)
				{
					return immunityRecord.immunity;
				}
			}
			return 0f;
		}

		// Token: 0x06004AC3 RID: 19139 RVA: 0x002707EC File Offset: 0x0026EBEC
		internal void ImmunityHandlerTick()
		{
			List<ImmunityHandler.ImmunityInfo> list = this.NeededImmunitiesNow();
			for (int i = 0; i < list.Count; i++)
			{
				this.TryAddImmunityRecord(list[i].immunity, list[i].source);
			}
			for (int j = 0; j < this.immunityList.Count; j++)
			{
				ImmunityRecord immunityRecord = this.immunityList[j];
				Hediff firstHediffOfDef = this.pawn.health.hediffSet.GetFirstHediffOfDef(immunityRecord.hediffDef, false);
				immunityRecord.ImmunityTick(this.pawn, firstHediffOfDef != null, firstHediffOfDef);
				if (firstHediffOfDef == null && this.AnyHediffMakesFullyImmuneTo(immunityRecord.hediffDef))
				{
					immunityRecord.immunity = Mathf.Clamp(0.650000036f, immunityRecord.immunity, 1f);
				}
			}
			for (int k = this.immunityList.Count - 1; k >= 0; k--)
			{
				if (this.immunityList[k].immunity <= 0f)
				{
					bool flag = false;
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].immunity == this.immunityList[k].hediffDef)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.immunityList.RemoveAt(k);
					}
				}
			}
		}

		// Token: 0x06004AC4 RID: 19140 RVA: 0x00270984 File Offset: 0x0026ED84
		private List<ImmunityHandler.ImmunityInfo> NeededImmunitiesNow()
		{
			ImmunityHandler.tmpNeededImmunitiesNow.Clear();
			List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				Hediff hediff = hediffs[i];
				if (hediff.def.PossibleToDevelopImmunityNaturally())
				{
					ImmunityHandler.tmpNeededImmunitiesNow.Add(new ImmunityHandler.ImmunityInfo
					{
						immunity = hediff.def,
						source = hediff.def
					});
				}
				HediffStage curStage = hediff.CurStage;
				if (curStage != null && curStage.makeImmuneTo != null)
				{
					for (int j = 0; j < curStage.makeImmuneTo.Count; j++)
					{
						ImmunityHandler.tmpNeededImmunitiesNow.Add(new ImmunityHandler.ImmunityInfo
						{
							immunity = curStage.makeImmuneTo[j],
							source = hediff.def
						});
					}
				}
			}
			return ImmunityHandler.tmpNeededImmunitiesNow;
		}

		// Token: 0x06004AC5 RID: 19141 RVA: 0x00270A94 File Offset: 0x0026EE94
		private bool AnyHediffMakesFullyImmuneTo(HediffDef def)
		{
			List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				HediffStage curStage = hediffs[i].CurStage;
				if (curStage != null && curStage.makeImmuneTo != null)
				{
					for (int j = 0; j < curStage.makeImmuneTo.Count; j++)
					{
						if (curStage.makeImmuneTo[j] == def)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06004AC6 RID: 19142 RVA: 0x00270B34 File Offset: 0x0026EF34
		private void TryAddImmunityRecord(HediffDef def, HediffDef source)
		{
			if (def.CompProps<HediffCompProperties_Immunizable>() != null)
			{
				if (!this.ImmunityRecordExists(def))
				{
					ImmunityRecord immunityRecord = new ImmunityRecord();
					immunityRecord.hediffDef = def;
					immunityRecord.source = source;
					this.immunityList.Add(immunityRecord);
				}
			}
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x00270B84 File Offset: 0x0026EF84
		public ImmunityRecord GetImmunityRecord(HediffDef def)
		{
			for (int i = 0; i < this.immunityList.Count; i++)
			{
				if (this.immunityList[i].hediffDef == def)
				{
					return this.immunityList[i];
				}
			}
			return null;
		}

		// Token: 0x06004AC8 RID: 19144 RVA: 0x00270BE4 File Offset: 0x0026EFE4
		public bool ImmunityRecordExists(HediffDef def)
		{
			return this.GetImmunityRecord(def) != null;
		}

		// Token: 0x04003268 RID: 12904
		public Pawn pawn;

		// Token: 0x04003269 RID: 12905
		private List<ImmunityRecord> immunityList = new List<ImmunityRecord>();

		// Token: 0x0400326A RID: 12906
		private static List<ImmunityHandler.ImmunityInfo> tmpNeededImmunitiesNow = new List<ImmunityHandler.ImmunityInfo>();

		// Token: 0x02000D3A RID: 3386
		public struct ImmunityInfo
		{
			// Token: 0x0400326B RID: 12907
			public HediffDef immunity;

			// Token: 0x0400326C RID: 12908
			public HediffDef source;
		}
	}
}
