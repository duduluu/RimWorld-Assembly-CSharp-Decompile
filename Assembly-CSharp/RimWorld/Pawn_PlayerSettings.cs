﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x02000521 RID: 1313
	public class Pawn_PlayerSettings : IExposable
	{
		// Token: 0x04000E24 RID: 3620
		private Pawn pawn;

		// Token: 0x04000E25 RID: 3621
		private Area areaAllowedInt = null;

		// Token: 0x04000E26 RID: 3622
		public int joinTick = -1;

		// Token: 0x04000E27 RID: 3623
		private Pawn master = null;

		// Token: 0x04000E28 RID: 3624
		public bool followDrafted = true;

		// Token: 0x04000E29 RID: 3625
		public bool followFieldwork = true;

		// Token: 0x04000E2A RID: 3626
		public bool animalsReleased = false;

		// Token: 0x04000E2B RID: 3627
		public MedicalCareCategory medCare = MedicalCareCategory.NoMeds;

		// Token: 0x04000E2C RID: 3628
		public HostilityResponseMode hostilityResponse = HostilityResponseMode.Flee;

		// Token: 0x04000E2D RID: 3629
		public bool selfTend = false;

		// Token: 0x04000E2E RID: 3630
		public int displayOrder;

		// Token: 0x060017ED RID: 6125 RVA: 0x000D16F0 File Offset: 0x000CFAF0
		public Pawn_PlayerSettings(Pawn pawn)
		{
			this.pawn = pawn;
			if (Current.ProgramState == ProgramState.Playing)
			{
				this.joinTick = Find.TickManager.TicksGame;
			}
			else
			{
				this.joinTick = 0;
			}
			this.Notify_FactionChanged();
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060017EE RID: 6126 RVA: 0x000D1778 File Offset: 0x000CFB78
		// (set) Token: 0x060017EF RID: 6127 RVA: 0x000D1794 File Offset: 0x000CFB94
		public Pawn Master
		{
			get
			{
				return this.master;
			}
			set
			{
				if (this.master != value)
				{
					if (value != null && !this.pawn.training.HasLearned(TrainableDefOf.Obedience))
					{
						Log.ErrorOnce("Attempted to set master for non-obedient pawn", 73908573, false);
					}
					else
					{
						bool flag = ThinkNode_ConditionalShouldFollowMaster.ShouldFollowMaster(this.pawn);
						this.master = value;
						if (this.pawn.Spawned && (flag || ThinkNode_ConditionalShouldFollowMaster.ShouldFollowMaster(this.pawn)))
						{
							this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
						}
					}
				}
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060017F0 RID: 6128 RVA: 0x000D1834 File Offset: 0x000CFC34
		public Area EffectiveAreaRestrictionInPawnCurrentMap
		{
			get
			{
				Area result;
				if (this.areaAllowedInt != null && this.areaAllowedInt.Map != this.pawn.MapHeld)
				{
					result = null;
				}
				else
				{
					result = this.EffectiveAreaRestriction;
				}
				return result;
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x060017F1 RID: 6129 RVA: 0x000D187C File Offset: 0x000CFC7C
		public Area EffectiveAreaRestriction
		{
			get
			{
				Area result;
				if (!this.RespectsAllowedArea)
				{
					result = null;
				}
				else
				{
					result = this.areaAllowedInt;
				}
				return result;
			}
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x060017F2 RID: 6130 RVA: 0x000D18AC File Offset: 0x000CFCAC
		// (set) Token: 0x060017F3 RID: 6131 RVA: 0x000D18C7 File Offset: 0x000CFCC7
		public Area AreaRestriction
		{
			get
			{
				return this.areaAllowedInt;
			}
			set
			{
				this.areaAllowedInt = value;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x000D18D4 File Offset: 0x000CFCD4
		public bool RespectsAllowedArea
		{
			get
			{
				return this.pawn.GetLord() == null && this.pawn.Faction == Faction.OfPlayer && this.pawn.HostFaction == null;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060017F5 RID: 6133 RVA: 0x000D1928 File Offset: 0x000CFD28
		public bool RespectsMaster
		{
			get
			{
				return this.Master != null && this.pawn.Faction == Faction.OfPlayer && this.Master.Faction == this.pawn.Faction;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060017F6 RID: 6134 RVA: 0x000D1980 File Offset: 0x000CFD80
		public Pawn RespectedMaster
		{
			get
			{
				return (!this.RespectsMaster) ? null : this.Master;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060017F7 RID: 6135 RVA: 0x000D19AC File Offset: 0x000CFDAC
		public bool UsesConfigurableHostilityResponse
		{
			get
			{
				return this.pawn.IsColonist && this.pawn.HostFaction == null;
			}
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x000D19E4 File Offset: 0x000CFDE4
		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.joinTick, "joinTick", 0, false);
			Scribe_Values.Look<bool>(ref this.animalsReleased, "animalsReleased", false, false);
			Scribe_Values.Look<MedicalCareCategory>(ref this.medCare, "medCare", MedicalCareCategory.NoCare, false);
			Scribe_References.Look<Area>(ref this.areaAllowedInt, "areaAllowed", false);
			Scribe_References.Look<Pawn>(ref this.master, "master", false);
			Scribe_Values.Look<bool>(ref this.followDrafted, "followDrafted", false, false);
			Scribe_Values.Look<bool>(ref this.followFieldwork, "followFieldwork", false, false);
			Scribe_Values.Look<HostilityResponseMode>(ref this.hostilityResponse, "hostilityResponse", HostilityResponseMode.Flee, false);
			Scribe_Values.Look<bool>(ref this.selfTend, "selfTend", false, false);
			Scribe_Values.Look<int>(ref this.displayOrder, "displayOrder", 0, false);
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x000D1AA4 File Offset: 0x000CFEA4
		public IEnumerable<Gizmo> GetGizmos()
		{
			if (this.pawn.Drafted)
			{
				if (PawnUtility.SpawnedMasteredPawns(this.pawn).Any((Pawn p) => p.training.HasLearned(TrainableDefOf.Release)))
				{
					yield return new Command_Toggle
					{
						defaultLabel = "CommandReleaseAnimalsLabel".Translate(),
						defaultDesc = "CommandReleaseAnimalsDesc".Translate(),
						icon = TexCommand.ReleaseAnimals,
						hotKey = KeyBindingDefOf.Misc7,
						isActive = (() => this.animalsReleased),
						toggleAction = delegate()
						{
							this.animalsReleased = !this.animalsReleased;
							if (this.animalsReleased)
							{
								foreach (Pawn pawn in PawnUtility.SpawnedMasteredPawns(this.pawn))
								{
									if (pawn.caller != null)
									{
										pawn.caller.Notify_Released();
									}
									pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
								}
							}
						}
					};
				}
			}
			yield break;
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x000D1ACE File Offset: 0x000CFECE
		public void Notify_FactionChanged()
		{
			this.ResetMedicalCare();
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x000D1AD7 File Offset: 0x000CFED7
		public void Notify_MadePrisoner()
		{
			this.ResetMedicalCare();
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x000D1AE0 File Offset: 0x000CFEE0
		public void ResetMedicalCare()
		{
			if (Scribe.mode != LoadSaveMode.LoadingVars)
			{
				if (this.pawn.Faction == Faction.OfPlayer)
				{
					if (!this.pawn.RaceProps.Animal)
					{
						if (!this.pawn.IsPrisoner)
						{
							this.medCare = Find.PlaySettings.defaultCareForColonyHumanlike;
						}
						else
						{
							this.medCare = Find.PlaySettings.defaultCareForColonyPrisoner;
						}
					}
					else
					{
						this.medCare = Find.PlaySettings.defaultCareForColonyAnimal;
					}
				}
				else if (this.pawn.Faction == null && this.pawn.RaceProps.Animal)
				{
					this.medCare = Find.PlaySettings.defaultCareForNeutralAnimal;
				}
				else if (this.pawn.Faction == null || !this.pawn.Faction.HostileTo(Faction.OfPlayer))
				{
					this.medCare = Find.PlaySettings.defaultCareForNeutralFaction;
				}
				else
				{
					this.medCare = Find.PlaySettings.defaultCareForHostileFaction;
				}
			}
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x000D1C07 File Offset: 0x000D0007
		public void Notify_AreaRemoved(Area area)
		{
			if (this.areaAllowedInt == area)
			{
				this.areaAllowedInt = null;
			}
		}
	}
}
