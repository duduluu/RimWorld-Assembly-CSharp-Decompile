﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class StatWorker_MeleeArmorPenetration : StatWorker
	{
		public StatWorker_MeleeArmorPenetration()
		{
		}

		public override bool IsDisabledFor(Thing thing)
		{
			return base.IsDisabledFor(thing) || StatDefOf.MeleeHitChance.Worker.IsDisabledFor(thing);
		}

		public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
		{
			if (req.Thing == null)
			{
				Log.Error("Getting MeleeArmorPenetration stat for " + req.Def + " without concrete pawn. This always returns 0.", false);
			}
			return this.GetArmorPenetration(req, applyPostProcess);
		}

		private float GetArmorPenetration(StatRequest req, bool applyPostProcess = true)
		{
			Pawn pawn = req.Thing as Pawn;
			float result;
			if (pawn == null)
			{
				result = 0f;
			}
			else
			{
				List<VerbEntry> updatedAvailableVerbsList = pawn.meleeVerbs.GetUpdatedAvailableVerbsList(false);
				if (updatedAvailableVerbsList.Count == 0)
				{
					result = 0f;
				}
				else
				{
					float num = 0f;
					for (int i = 0; i < updatedAvailableVerbsList.Count; i++)
					{
						if (updatedAvailableVerbsList[i].IsMeleeAttack)
						{
							num += updatedAvailableVerbsList[i].GetSelectionWeight(null);
						}
					}
					if (num == 0f)
					{
						result = 0f;
					}
					else
					{
						float num2 = 0f;
						for (int j = 0; j < updatedAvailableVerbsList.Count; j++)
						{
							if (updatedAvailableVerbsList[j].IsMeleeAttack)
							{
								ThingWithComps ownerEquipment = updatedAvailableVerbsList[j].verb.ownerEquipment;
								num2 += updatedAvailableVerbsList[j].GetSelectionWeight(null) / num * updatedAvailableVerbsList[j].verb.verbProps.AdjustedArmorPenetration(updatedAvailableVerbsList[j].verb, pawn, ownerEquipment);
							}
						}
						result = num2;
					}
				}
			}
			return result;
		}
	}
}
