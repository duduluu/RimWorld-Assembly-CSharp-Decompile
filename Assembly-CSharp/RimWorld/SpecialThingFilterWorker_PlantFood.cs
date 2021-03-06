﻿using System;
using Verse;

namespace RimWorld
{
	public class SpecialThingFilterWorker_PlantFood : SpecialThingFilterWorker
	{
		public SpecialThingFilterWorker_PlantFood()
		{
		}

		public override bool Matches(Thing t)
		{
			return this.AlwaysMatches(t.def);
		}

		public override bool AlwaysMatches(ThingDef def)
		{
			return def.ingestible != null && (def.ingestible.foodType & FoodTypeFlags.Plant) != FoodTypeFlags.None;
		}

		public override bool CanEverMatch(ThingDef def)
		{
			return this.AlwaysMatches(def);
		}
	}
}
