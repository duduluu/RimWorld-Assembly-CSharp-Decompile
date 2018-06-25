﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000589 RID: 1417
	[StaticConstructorOnStartup]
	public static class WorldDebugMatsSpectrum
	{
		// Token: 0x04000FF4 RID: 4084
		private static readonly Material[] spectrumMats = new Material[100];

		// Token: 0x04000FF5 RID: 4085
		public const int MaterialCount = 100;

		// Token: 0x04000FF6 RID: 4086
		private const float Opacity = 0.25f;

		// Token: 0x04000FF7 RID: 4087
		private static readonly Color[] DebugSpectrum = DebugMatsSpectrum.DebugSpectrum;

		// Token: 0x06001B0C RID: 6924 RVA: 0x000E89B8 File Offset: 0x000E6DB8
		static WorldDebugMatsSpectrum()
		{
			for (int i = 0; i < 100; i++)
			{
				WorldDebugMatsSpectrum.spectrumMats[i] = MatsFromSpectrum.Get(WorldDebugMatsSpectrum.DebugSpectrum, (float)i / 100f, ShaderDatabase.WorldOverlayTransparent);
				WorldDebugMatsSpectrum.spectrumMats[i].renderQueue = WorldMaterials.DebugTileRenderQueue;
			}
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x000E8A20 File Offset: 0x000E6E20
		public static Material Mat(int ind)
		{
			ind = Mathf.Clamp(ind, 0, 99);
			return WorldDebugMatsSpectrum.spectrumMats[ind];
		}
	}
}
