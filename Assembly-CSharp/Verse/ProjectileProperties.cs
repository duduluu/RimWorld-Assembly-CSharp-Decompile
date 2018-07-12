﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using RimWorld;
using UnityEngine;

namespace Verse
{
	public class ProjectileProperties
	{
		public float speed = 5f;

		public bool flyOverhead = false;

		public bool alwaysFreeIntercept = false;

		public DamageDef damageDef = null;

		private int damageAmountBase = -1;

		private float armorPenetrationBase = -1f;

		public float stoppingPower = 0.5f;

		public SoundDef soundHitThickRoof = null;

		public SoundDef soundExplode = null;

		public SoundDef soundImpactAnticipate = null;

		public SoundDef soundAmbient = null;

		public float explosionRadius = 0f;

		public int explosionDelay = 0;

		public ThingDef preExplosionSpawnThingDef = null;

		public float preExplosionSpawnChance = 1f;

		public int preExplosionSpawnThingCount = 1;

		public ThingDef postExplosionSpawnThingDef = null;

		public float postExplosionSpawnChance = 1f;

		public int postExplosionSpawnThingCount = 1;

		public bool applyDamageToExplosionCellsNeighbors;

		public float explosionChanceToStartFire;

		public bool explosionDamageFalloff;

		public EffecterDef explosionEffect;

		public bool ai_IsIncendiary = false;

		public ProjectileProperties()
		{
		}

		public float StoppingPower
		{
			get
			{
				float result;
				if (this.stoppingPower != 0f)
				{
					result = this.stoppingPower;
				}
				else if (this.damageDef != null)
				{
					result = this.damageDef.defaultStoppingPower;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		public int GetDamageAmount(Thing weapon, StringBuilder explanation = null)
		{
			float weaponDamageMultiplier = (weapon == null) ? 1f : weapon.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier, true);
			return this.GetDamageAmount(weaponDamageMultiplier, explanation);
		}

		public int GetDamageAmount(float weaponDamageMultiplier, StringBuilder explanation = null)
		{
			int num;
			if (this.damageAmountBase != -1)
			{
				num = this.damageAmountBase;
			}
			else
			{
				if (this.damageDef == null)
				{
					Log.ErrorOnce("Failed to find sane damage amount", 91094882, false);
					return 1;
				}
				num = this.damageDef.defaultDamage;
			}
			if (explanation != null)
			{
				explanation.Append("StatsReport_BaseValue".Translate() + "\n    " + num);
				explanation.AppendLine();
				explanation.AppendLine();
				explanation.Append("StatsReport_QualityMultiplier".Translate() + ": " + weaponDamageMultiplier.ToStringPercent());
			}
			num = Mathf.RoundToInt((float)num * weaponDamageMultiplier);
			if (explanation != null)
			{
				explanation.AppendLine();
				explanation.AppendLine();
				explanation.Append("StatsReport_FinalValue".Translate() + ": " + num);
			}
			return num;
		}

		public float GetArmorPenetration(Thing weapon, StringBuilder explanation = null)
		{
			float weaponDamageMultiplier = (weapon == null) ? 1f : weapon.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier, true);
			return this.GetArmorPenetration(weaponDamageMultiplier, explanation);
		}

		public float GetArmorPenetration(float weaponDamageMultiplier, StringBuilder explanation = null)
		{
			float result;
			if (this.damageDef.armorCategory == null)
			{
				result = 0f;
			}
			else
			{
				float num;
				if (this.damageAmountBase != -1 || this.armorPenetrationBase >= 0f)
				{
					num = this.armorPenetrationBase;
				}
				else
				{
					if (this.damageDef == null)
					{
						return 0f;
					}
					num = this.damageDef.defaultArmorPenetration;
				}
				if (num < 0f)
				{
					int damageAmount = this.GetDamageAmount(null, null);
					num = (float)damageAmount * 0.015f;
				}
				if (explanation != null)
				{
					explanation.Append("StatsReport_BaseValue".Translate() + "\n    " + num.ToStringPercent());
					explanation.AppendLine();
					explanation.AppendLine();
					explanation.Append("StatsReport_QualityMultiplier".Translate() + ": " + weaponDamageMultiplier.ToStringPercent());
				}
				num *= weaponDamageMultiplier;
				if (explanation != null)
				{
					explanation.AppendLine();
					explanation.AppendLine();
					explanation.Append("StatsReport_FinalValue".Translate() + ": " + num.ToStringPercent());
				}
				result = num;
			}
			return result;
		}

		public IEnumerable<string> ConfigErrors(ThingDef parent)
		{
			if (this.alwaysFreeIntercept && this.flyOverhead)
			{
				yield return "alwaysFreeIntercept and flyOverhead are both true";
			}
			if (this.damageAmountBase == -1 && this.damageDef != null && this.damageDef.defaultDamage == -1)
			{
				yield return "no damage amount specified for projectile";
			}
			yield break;
		}

		[CompilerGenerated]
		private sealed class <ConfigErrors>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
		{
			internal ProjectileProperties $this;

			internal string $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <ConfigErrors>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					if (this.alwaysFreeIntercept && this.flyOverhead)
					{
						this.$current = "alwaysFreeIntercept and flyOverhead are both true";
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						return true;
					}
					break;
				case 1u:
					break;
				case 2u:
					goto IL_BB;
				default:
					return false;
				}
				if (this.damageAmountBase == -1 && this.damageDef != null && this.damageDef.defaultDamage == -1)
				{
					this.$current = "no damage amount specified for projectile";
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				IL_BB:
				this.$PC = -1;
				return false;
			}

			string IEnumerator<string>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$disposing = true;
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				ProjectileProperties.<ConfigErrors>c__Iterator0 <ConfigErrors>c__Iterator = new ProjectileProperties.<ConfigErrors>c__Iterator0();
				<ConfigErrors>c__Iterator.$this = this;
				return <ConfigErrors>c__Iterator;
			}
		}
	}
}
