﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000441 RID: 1089
	public class RoomStatWorker_Space : RoomStatWorker
	{
		// Token: 0x060012E4 RID: 4836 RVA: 0x000A3558 File Offset: 0x000A1958
		public override float GetScore(Room room)
		{
			float result;
			if (room.PsychologicallyOutdoors)
			{
				result = 350f;
			}
			else
			{
				float num = 0f;
				foreach (IntVec3 c in room.Cells)
				{
					if (c.Standable(room.Map))
					{
						num += 1.4f;
					}
					else if (c.Walkable(room.Map))
					{
						num += 0.5f;
					}
				}
				result = Mathf.Min(num, 350f);
			}
			return result;
		}
	}
}
