﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x02000576 RID: 1398
	public class WorldFeatures : IExposable
	{
		// Token: 0x04000F7F RID: 3967
		public List<WorldFeature> features = new List<WorldFeature>();

		// Token: 0x04000F80 RID: 3968
		public bool textsCreated;

		// Token: 0x04000F81 RID: 3969
		private static List<WorldFeatureTextMesh> texts = new List<WorldFeatureTextMesh>();

		// Token: 0x04000F82 RID: 3970
		private const float BaseAlpha = 0.3f;

		// Token: 0x04000F83 RID: 3971
		private const float AlphaChangeSpeed = 5f;

		// Token: 0x04000F84 RID: 3972
		[TweakValue("Interface", 0f, 300f)]
		private static float TextWrapThreshold = 150f;

		// Token: 0x04000F85 RID: 3973
		[TweakValue("Interface.World", 0f, 100f)]
		protected static bool ForceLegacyText = false;

		// Token: 0x04000F86 RID: 3974
		[TweakValue("Interface.World", 1f, 150f)]
		protected static float AlphaScale = 30f;

		// Token: 0x04000F87 RID: 3975
		[TweakValue("Interface.World", 0f, 1f)]
		protected static float VisibleMinimumSize = 0.04f;

		// Token: 0x04000F88 RID: 3976
		[TweakValue("Interface.World", 0f, 5f)]
		protected static float VisibleMaximumSize = 1f;

		// Token: 0x06001AA6 RID: 6822 RVA: 0x000E58BD File Offset: 0x000E3CBD
		private static void TextWrapThreshold_Changed()
		{
			Find.WorldFeatures.textsCreated = false;
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x000E58CB File Offset: 0x000E3CCB
		protected static void ForceLegacyText_Changed()
		{
			Find.WorldFeatures.textsCreated = false;
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x000E58DC File Offset: 0x000E3CDC
		public void ExposeData()
		{
			Scribe_Collections.Look<WorldFeature>(ref this.features, "features", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				WorldGrid grid = Find.WorldGrid;
				if (grid.tileFeature != null && grid.tileFeature.Length != 0)
				{
					DataSerializeUtility.LoadUshort(grid.tileFeature, grid.TilesCount, delegate(int i, ushort data)
					{
						grid[i].feature = ((data != ushort.MaxValue) ? this.GetFeatureWithID((int)data) : null);
					});
				}
				this.textsCreated = false;
			}
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x000E5978 File Offset: 0x000E3D78
		public void UpdateFeatures()
		{
			if (!this.textsCreated)
			{
				this.textsCreated = true;
				this.CreateTextsAndSetPosition();
			}
			bool showWorldFeatures = Find.PlaySettings.showWorldFeatures;
			for (int i = 0; i < this.features.Count; i++)
			{
				Vector3 position = WorldFeatures.texts[i].Position;
				bool flag = showWorldFeatures && !WorldRendererUtility.HiddenBehindTerrainNow(position);
				if (flag != WorldFeatures.texts[i].Active)
				{
					WorldFeatures.texts[i].SetActive(flag);
					WorldFeatures.texts[i].WrapAroundPlanetSurface();
				}
				if (flag)
				{
					this.UpdateAlpha(WorldFeatures.texts[i], this.features[i]);
				}
			}
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x000E5A4C File Offset: 0x000E3E4C
		public WorldFeature GetFeatureWithID(int uniqueID)
		{
			for (int i = 0; i < this.features.Count; i++)
			{
				if (this.features[i].uniqueID == uniqueID)
				{
					return this.features[i];
				}
			}
			return null;
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x000E5AAC File Offset: 0x000E3EAC
		private void UpdateAlpha(WorldFeatureTextMesh text, WorldFeature feature)
		{
			float num = 0.3f * feature.alpha;
			if (text.Color.a != num)
			{
				text.Color = new Color(1f, 1f, 1f, num);
				text.WrapAroundPlanetSurface();
			}
			float num2 = Time.deltaTime * 5f;
			if (this.GoodCameraAltitudeFor(feature))
			{
				feature.alpha += num2;
			}
			else
			{
				feature.alpha -= num2;
			}
			feature.alpha = Mathf.Clamp01(feature.alpha);
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x000E5B48 File Offset: 0x000E3F48
		private bool GoodCameraAltitudeFor(WorldFeature feature)
		{
			float num = feature.EffectiveDrawSize;
			float altitude = Find.WorldCameraDriver.altitude;
			float num2 = 1f / (altitude / WorldFeatures.AlphaScale * (altitude / WorldFeatures.AlphaScale));
			num *= num2;
			bool result;
			if (Find.WorldCameraDriver.CurrentZoom <= WorldCameraZoomRange.VeryClose && num >= 0.56f)
			{
				result = false;
			}
			else if (num < WorldFeatures.VisibleMinimumSize)
			{
				result = (Find.WorldCameraDriver.AltitudePercent <= 0.07f);
			}
			else
			{
				result = (num <= WorldFeatures.VisibleMaximumSize || Find.WorldCameraDriver.AltitudePercent >= 0.35f);
			}
			return result;
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x000E5BF8 File Offset: 0x000E3FF8
		private void CreateTextsAndSetPosition()
		{
			this.CreateOrDestroyTexts();
			WorldGrid worldGrid = Find.WorldGrid;
			float averageTileSize = worldGrid.averageTileSize;
			for (int i = 0; i < this.features.Count; i++)
			{
				WorldFeatures.texts[i].Text = this.features[i].name.WordWrapAt(WorldFeatures.TextWrapThreshold);
				WorldFeatures.texts[i].Size = this.features[i].EffectiveDrawSize * averageTileSize;
				Vector3 normalized = this.features[i].drawCenter.normalized;
				Quaternion quaternion = Quaternion.LookRotation(Vector3.Cross(normalized, Vector3.up), normalized);
				quaternion *= Quaternion.Euler(Vector3.right * 90f);
				quaternion *= Quaternion.Euler(Vector3.forward * (90f - this.features[i].drawAngle));
				WorldFeatures.texts[i].Rotation = quaternion;
				WorldFeatures.texts[i].LocalPosition = this.features[i].drawCenter;
				WorldFeatures.texts[i].WrapAroundPlanetSurface();
				WorldFeatures.texts[i].SetActive(false);
			}
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x000E5D50 File Offset: 0x000E4150
		private void CreateOrDestroyTexts()
		{
			for (int i = 0; i < WorldFeatures.texts.Count; i++)
			{
				WorldFeatures.texts[i].Destroy();
			}
			WorldFeatures.texts.Clear();
			bool flag = LanguageDatabase.activeLanguage == LanguageDatabase.defaultLanguage;
			for (int j = 0; j < this.features.Count; j++)
			{
				WorldFeatureTextMesh worldFeatureTextMesh;
				if (WorldFeatures.ForceLegacyText || (!flag && this.HasCharactersUnsupportedByTextMeshPro(this.features[j].name)))
				{
					worldFeatureTextMesh = new WorldFeatureTextMesh_Legacy();
				}
				else
				{
					worldFeatureTextMesh = new WorldFeatureTextMesh_TextMeshPro();
				}
				worldFeatureTextMesh.Init();
				WorldFeatures.texts.Add(worldFeatureTextMesh);
			}
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x000E5E10 File Offset: 0x000E4210
		private bool HasCharactersUnsupportedByTextMeshPro(string str)
		{
			TMP_FontAsset font = WorldFeatureTextMesh_TextMeshPro.WorldTextPrefab.GetComponent<TextMeshPro>().font;
			for (int i = 0; i < str.Length; i++)
			{
				if (!this.HasCharacter(font, str[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x000E5E6C File Offset: 0x000E426C
		private bool HasCharacter(TMP_FontAsset font, char character)
		{
			string characters = TMP_FontAsset.GetCharacters(font);
			bool result;
			if (characters.IndexOf(character) >= 0)
			{
				result = true;
			}
			else
			{
				List<TMP_FontAsset> fallbackFontAssets = font.fallbackFontAssets;
				for (int i = 0; i < fallbackFontAssets.Count; i++)
				{
					characters = TMP_FontAsset.GetCharacters(fallbackFontAssets[i]);
					if (characters.IndexOf(character) >= 0)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}
	}
}
