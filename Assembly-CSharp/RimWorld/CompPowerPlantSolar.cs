﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x0200041C RID: 1052
	[StaticConstructorOnStartup]
	public class CompPowerPlantSolar : CompPowerPlant
	{
		// Token: 0x04000B0C RID: 2828
		private const float FullSunPower = 1700f;

		// Token: 0x04000B0D RID: 2829
		private const float NightPower = 0f;

		// Token: 0x04000B0E RID: 2830
		private static readonly Vector2 BarSize = new Vector2(2.3f, 0.14f);

		// Token: 0x04000B0F RID: 2831
		private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f), false);

		// Token: 0x04000B10 RID: 2832
		private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f), false);

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x0009E45C File Offset: 0x0009C85C
		protected override float DesiredPowerOutput
		{
			get
			{
				return Mathf.Lerp(0f, 1700f, this.parent.Map.skyManager.CurSkyGlow) * this.RoofedPowerOutputFactor;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x0009E49C File Offset: 0x0009C89C
		private float RoofedPowerOutputFactor
		{
			get
			{
				int num = 0;
				int num2 = 0;
				foreach (IntVec3 c in this.parent.OccupiedRect())
				{
					num++;
					if (this.parent.Map.roofGrid.Roofed(c))
					{
						num2++;
					}
				}
				return (float)(num - num2) / (float)num;
			}
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x0009E534 File Offset: 0x0009C934
		public override void PostDraw()
		{
			base.PostDraw();
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
			r.center = this.parent.DrawPos + Vector3.up * 0.1f;
			r.size = CompPowerPlantSolar.BarSize;
			r.fillPercent = base.PowerOutput / 1700f;
			r.filledMat = CompPowerPlantSolar.PowerPlantSolarBarFilledMat;
			r.unfilledMat = CompPowerPlantSolar.PowerPlantSolarBarUnfilledMat;
			r.margin = 0.15f;
			Rot4 rotation = this.parent.Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
		}
	}
}
