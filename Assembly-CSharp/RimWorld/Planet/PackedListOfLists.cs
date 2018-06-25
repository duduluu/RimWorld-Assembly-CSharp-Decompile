﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RimWorld.Planet
{
	// Token: 0x020005B6 RID: 1462
	public static class PackedListOfLists
	{
		// Token: 0x040010C5 RID: 4293
		private static List<int> vertAdjacentTrisCount = new List<int>();

		// Token: 0x06001C0E RID: 7182 RVA: 0x000F18DF File Offset: 0x000EFCDF
		public static void AddList<T>(List<int> offsets, List<T> values, List<T> listToAdd)
		{
			offsets.Add(values.Count);
			values.AddRange(listToAdd);
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x000F18F8 File Offset: 0x000EFCF8
		public static void GetList<T>(List<int> offsets, List<T> values, int listIndex, List<T> outList)
		{
			outList.Clear();
			int num = offsets[listIndex];
			int num2 = values.Count;
			if (listIndex + 1 < offsets.Count)
			{
				num2 = offsets[listIndex + 1];
			}
			for (int i = num; i < num2; i++)
			{
				outList.Add(values[i]);
			}
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x000F1954 File Offset: 0x000EFD54
		public static void GetListValuesIndices<T>(List<int> offsets, List<T> values, int listIndex, List<int> outList)
		{
			outList.Clear();
			int num = offsets[listIndex];
			int num2 = values.Count;
			if (listIndex + 1 < offsets.Count)
			{
				num2 = offsets[listIndex + 1];
			}
			for (int i = num; i < num2; i++)
			{
				outList.Add(i);
			}
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x000F19AC File Offset: 0x000EFDAC
		public static int GetListCount<T>(List<int> offsets, List<T> values, int listIndex)
		{
			int num = offsets[listIndex];
			int num2 = values.Count;
			if (listIndex + 1 < offsets.Count)
			{
				num2 = offsets[listIndex + 1];
			}
			return num2 - num;
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x000F19EC File Offset: 0x000EFDEC
		public static void GenerateVertToTrisPackedList(List<Vector3> verts, List<TriangleIndices> tris, List<int> outOffsets, List<int> outValues)
		{
			outOffsets.Clear();
			outValues.Clear();
			PackedListOfLists.vertAdjacentTrisCount.Clear();
			int i = 0;
			int count = verts.Count;
			while (i < count)
			{
				PackedListOfLists.vertAdjacentTrisCount.Add(0);
				i++;
			}
			int j = 0;
			int count2 = tris.Count;
			while (j < count2)
			{
				TriangleIndices triangleIndices = tris[j];
				List<int> list;
				int v;
				(list = PackedListOfLists.vertAdjacentTrisCount)[v = triangleIndices.v1] = list[v] + 1;
				int v2;
				(list = PackedListOfLists.vertAdjacentTrisCount)[v2 = triangleIndices.v2] = list[v2] + 1;
				int v3;
				(list = PackedListOfLists.vertAdjacentTrisCount)[v3 = triangleIndices.v3] = list[v3] + 1;
				j++;
			}
			int num = 0;
			int k = 0;
			int count3 = verts.Count;
			while (k < count3)
			{
				outOffsets.Add(num);
				int num2 = PackedListOfLists.vertAdjacentTrisCount[k];
				PackedListOfLists.vertAdjacentTrisCount[k] = 0;
				for (int l = 0; l < num2; l++)
				{
					outValues.Add(-1);
				}
				num += num2;
				k++;
			}
			int m = 0;
			int count4 = tris.Count;
			while (m < count4)
			{
				TriangleIndices triangleIndices2 = tris[m];
				outValues[outOffsets[triangleIndices2.v1] + PackedListOfLists.vertAdjacentTrisCount[triangleIndices2.v1]] = m;
				outValues[outOffsets[triangleIndices2.v2] + PackedListOfLists.vertAdjacentTrisCount[triangleIndices2.v2]] = m;
				outValues[outOffsets[triangleIndices2.v3] + PackedListOfLists.vertAdjacentTrisCount[triangleIndices2.v3]] = m;
				List<int> list;
				int v4;
				(list = PackedListOfLists.vertAdjacentTrisCount)[v4 = triangleIndices2.v1] = list[v4] + 1;
				int v5;
				(list = PackedListOfLists.vertAdjacentTrisCount)[v5 = triangleIndices2.v2] = list[v5] + 1;
				int v6;
				(list = PackedListOfLists.vertAdjacentTrisCount)[v6 = triangleIndices2.v3] = list[v6] + 1;
				m++;
			}
		}
	}
}
