﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x02000326 RID: 806
	public class IncidentWorker_AnimalInsanityMass : IncidentWorker
	{
		// Token: 0x06000DC0 RID: 3520 RVA: 0x000759D0 File Offset: 0x00073DD0
		public static bool AnimalUsable(Pawn p)
		{
			return p.Spawned && !p.Position.Fogged(p.Map) && (!p.InMentalState || !p.MentalStateDef.IsAggro) && !p.Downed && p.Faction == null;
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x00075A38 File Offset: 0x00073E38
		public static void DriveInsane(Pawn p)
		{
			p.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, true, false, null, false);
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x00075A58 File Offset: 0x00073E58
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (parms.points <= 0f)
			{
				Log.Error("AnimalInsanity running without points.", false);
				parms.points = (float)((int)(map.strengthWatcher.StrengthRating * 50f));
			}
			float adjustedPoints = parms.points;
			if (adjustedPoints > 250f)
			{
				adjustedPoints -= 250f;
				adjustedPoints *= 0.5f;
				adjustedPoints += 250f;
			}
			IEnumerable<PawnKindDef> source = from def in DefDatabase<PawnKindDef>.AllDefs
			where def.RaceProps.Animal && def.combatPower <= adjustedPoints && (from p in map.mapPawns.AllPawnsSpawned
			where p.kindDef == def && IncidentWorker_AnimalInsanityMass.AnimalUsable(p)
			select p).Count<Pawn>() >= 3
			select def;
			PawnKindDef animalDef;
			bool result;
			if (!source.TryRandomElement(out animalDef))
			{
				result = false;
			}
			else
			{
				List<Pawn> list = (from p in map.mapPawns.AllPawnsSpawned
				where p.kindDef == animalDef && IncidentWorker_AnimalInsanityMass.AnimalUsable(p)
				select p).ToList<Pawn>();
				float combatPower = animalDef.combatPower;
				float num = 0f;
				int num2 = 0;
				Pawn pawn = null;
				list.Shuffle<Pawn>();
				foreach (Pawn pawn2 in list)
				{
					if (num + combatPower > adjustedPoints)
					{
						break;
					}
					IncidentWorker_AnimalInsanityMass.DriveInsane(pawn2);
					num += combatPower;
					num2++;
					pawn = pawn2;
				}
				if (num == 0f)
				{
					result = false;
				}
				else
				{
					string label;
					string text;
					LetterDef textLetterDef;
					if (num2 == 1)
					{
						label = "LetterLabelAnimalInsanitySingle".Translate() + ": " + pawn.LabelCap;
						text = "AnimalInsanitySingle".Translate(new object[]
						{
							pawn.LabelShort
						});
						textLetterDef = LetterDefOf.ThreatSmall;
					}
					else
					{
						label = "LetterLabelAnimalInsanityMultiple".Translate() + ": " + animalDef.LabelCap;
						text = "AnimalInsanityMultiple".Translate(new object[]
						{
							animalDef.GetLabelPlural(-1)
						});
						textLetterDef = LetterDefOf.ThreatBig;
					}
					Find.LetterStack.ReceiveLetter(label, text, textLetterDef, pawn, null, null);
					SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(map);
					if (map == Find.CurrentMap)
					{
						Find.CameraDriver.shaker.DoShake(1f);
					}
					result = true;
				}
			}
			return result;
		}
	}
}
