﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x020004E3 RID: 1251
	public class SolidBioDatabase
	{
		// Token: 0x04000D06 RID: 3334
		public static List<PawnBio> allBios = new List<PawnBio>();

		// Token: 0x0600164F RID: 5711 RVA: 0x000C6480 File Offset: 0x000C4880
		public static void Clear()
		{
			SolidBioDatabase.allBios.Clear();
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x000C6490 File Offset: 0x000C4890
		public static void LoadAllBios()
		{
			foreach (PawnBio pawnBio in DirectXmlLoader.LoadXmlDataInResourcesFolder<PawnBio>("Backstories/Solid"))
			{
				pawnBio.name.ResolveMissingPieces(null);
				if (pawnBio.childhood == null || pawnBio.adulthood == null)
				{
					PawnNameDatabaseSolid.AddPlayerContentName(pawnBio.name, pawnBio.gender);
				}
				else
				{
					pawnBio.PostLoad();
					pawnBio.ResolveReferences();
					foreach (string text in pawnBio.ConfigErrors())
					{
						Log.Error(text, false);
					}
					SolidBioDatabase.allBios.Add(pawnBio);
					pawnBio.childhood.shuffleable = false;
					pawnBio.childhood.slot = BackstorySlot.Childhood;
					pawnBio.adulthood.shuffleable = false;
					pawnBio.adulthood.slot = BackstorySlot.Adulthood;
					BackstoryHardcodedData.InjectHardcodedData(pawnBio);
					BackstoryDatabase.AddBackstory(pawnBio.childhood);
					BackstoryDatabase.AddBackstory(pawnBio.adulthood);
				}
			}
		}
	}
}
