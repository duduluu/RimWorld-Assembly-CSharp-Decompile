﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000434 RID: 1076
	public class RoomRoleWorker_Kitchen : RoomRoleWorker
	{
		// Token: 0x060012C8 RID: 4808 RVA: 0x000A2C04 File Offset: 0x000A1004
		public override float GetScore(Room room)
		{
			int num = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			for (int i = 0; i < containedAndAdjacentThings.Count; i++)
			{
				Thing thing = containedAndAdjacentThings[i];
				if (thing.def.designationCategory == DesignationCategoryDefOf.Production)
				{
					for (int j = 0; j < thing.def.AllRecipes.Count; j++)
					{
						RecipeDef recipeDef = thing.def.AllRecipes[j];
						for (int k = 0; k < recipeDef.products.Count; k++)
						{
							ThingDef thingDef = recipeDef.products[k].thingDef;
							if (thingDef.IsNutritionGivingIngestible && thingDef.ingestible.HumanEdible)
							{
								num++;
								goto IL_CE;
							}
						}
					}
					IL_CE:;
				}
			}
			return (float)num * 14f;
		}
	}
}
