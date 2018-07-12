﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace RimWorld
{
	public class Bombardment : OrbitalStrike
	{
		private const int ImpactAreaRadius = 15;

		private const int ExplosionRadiusMin = 6;

		private const int ExplosionRadiusMax = 8;

		public const int EffectiveRadius = 23;

		public const int RandomFireRadius = 25;

		private const int BombIntervalTicks = 18;

		private const int StartRandomFireEveryTicks = 20;

		public Bombardment()
		{
		}

		public override void StartStrike()
		{
			base.StartStrike();
			MoteMaker.MakeBombardmentMote(base.Position, base.Map);
		}

		public override void Tick()
		{
			base.Tick();
			if (!base.Destroyed)
			{
				if (Find.TickManager.TicksGame % 18 == 0)
				{
					this.CreateRandomExplosion();
				}
				if (Find.TickManager.TicksGame % 20 == 0)
				{
					this.StartRandomFire();
				}
			}
		}

		private void CreateRandomExplosion()
		{
			IntVec3 intVec = (from x in GenRadial.RadialCellsAround(base.Position, 15f, true)
			where x.InBounds(base.Map)
			select x).RandomElement<IntVec3>();
			float num = (float)Rand.Range(6, 8);
			IntVec3 center = intVec;
			Map map = base.Map;
			float radius = num;
			DamageDef bomb = DamageDefOf.Bomb;
			Thing instigator = this.instigator;
			ThingDef def = this.def;
			ThingDef weaponDef = this.weaponDef;
			GenExplosion.DoExplosion(center, map, radius, bomb, instigator, -1, -1f, null, weaponDef, def, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
		}

		private void StartRandomFire()
		{
			IntVec3 c = (from x in GenRadial.RadialCellsAround(base.Position, 25f, true)
			where x.InBounds(base.Map)
			select x).RandomElement<IntVec3>();
			FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
		}

		[CompilerGenerated]
		private bool <CreateRandomExplosion>m__0(IntVec3 x)
		{
			return x.InBounds(base.Map);
		}

		[CompilerGenerated]
		private bool <StartRandomFire>m__1(IntVec3 x)
		{
			return x.InBounds(base.Map);
		}
	}
}
