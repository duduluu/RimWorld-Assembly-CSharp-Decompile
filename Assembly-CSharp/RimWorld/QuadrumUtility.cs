﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x0200090D RID: 2317
	public static class QuadrumUtility
	{
		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x060035F9 RID: 13817 RVA: 0x001CFD10 File Offset: 0x001CE110
		public static Quadrum FirstQuadrum
		{
			get
			{
				return Quadrum.Aprimay;
			}
		}

		// Token: 0x060035FA RID: 13818 RVA: 0x001CFD28 File Offset: 0x001CE128
		public static Twelfth GetFirstTwelfth(this Quadrum quadrum)
		{
			Twelfth result;
			switch (quadrum)
			{
			case Quadrum.Aprimay:
				result = Twelfth.First;
				break;
			case Quadrum.Jugust:
				result = Twelfth.Fourth;
				break;
			case Quadrum.Septober:
				result = Twelfth.Seventh;
				break;
			case Quadrum.Decembary:
				result = Twelfth.Tenth;
				break;
			default:
				result = Twelfth.Undefined;
				break;
			}
			return result;
		}

		// Token: 0x060035FB RID: 13819 RVA: 0x001CFD78 File Offset: 0x001CE178
		public static Twelfth GetMiddleTwelfth(this Quadrum quadrum)
		{
			Twelfth result;
			switch (quadrum)
			{
			case Quadrum.Aprimay:
				result = Twelfth.Second;
				break;
			case Quadrum.Jugust:
				result = Twelfth.Fifth;
				break;
			case Quadrum.Septober:
				result = Twelfth.Eighth;
				break;
			case Quadrum.Decembary:
				result = Twelfth.Eleventh;
				break;
			default:
				result = Twelfth.Undefined;
				break;
			}
			return result;
		}

		// Token: 0x060035FC RID: 13820 RVA: 0x001CFDC8 File Offset: 0x001CE1C8
		public static float GetMiddleYearPct(this Quadrum quadrum)
		{
			return quadrum.GetMiddleTwelfth().GetMiddleYearPct();
		}

		// Token: 0x060035FD RID: 13821 RVA: 0x001CFDE8 File Offset: 0x001CE1E8
		public static string Label(this Quadrum quadrum)
		{
			string result;
			switch (quadrum)
			{
			case Quadrum.Aprimay:
				result = "QuadrumAprimay".Translate();
				break;
			case Quadrum.Jugust:
				result = "QuadrumJugust".Translate();
				break;
			case Quadrum.Septober:
				result = "QuadrumSeptober".Translate();
				break;
			case Quadrum.Decembary:
				result = "QuadrumDecembary".Translate();
				break;
			default:
				result = "Unknown quadrum";
				break;
			}
			return result;
		}

		// Token: 0x060035FE RID: 13822 RVA: 0x001CFE60 File Offset: 0x001CE260
		public static Season GetSeason(this Quadrum q, float latitude)
		{
			float middleYearPct = q.GetMiddleYearPct();
			return SeasonUtility.GetReportedSeason(middleYearPct, latitude);
		}

		// Token: 0x060035FF RID: 13823 RVA: 0x001CFE84 File Offset: 0x001CE284
		public static string QuadrumsRangeLabel(List<Twelfth> twelfths)
		{
			string result;
			if (twelfths.Count == 0)
			{
				result = "";
			}
			else if (twelfths.Count == 12)
			{
				result = "WholeYear".Translate();
			}
			else
			{
				string text = "";
				for (int i = 0; i < 12; i++)
				{
					Twelfth twelfth = (Twelfth)i;
					if (twelfths.Contains(twelfth))
					{
						if (!text.NullOrEmpty())
						{
							text += ", ";
						}
						text += QuadrumUtility.QuadrumsContinuousRangeLabel(twelfths, twelfth);
					}
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06003600 RID: 13824 RVA: 0x001CFF20 File Offset: 0x001CE320
		private static string QuadrumsContinuousRangeLabel(List<Twelfth> twelfths, Twelfth rootTwelfth)
		{
			Twelfth leftMostTwelfth = TwelfthUtility.GetLeftMostTwelfth(twelfths, rootTwelfth);
			Twelfth rightMostTwelfth = TwelfthUtility.GetRightMostTwelfth(twelfths, rootTwelfth);
			for (Twelfth twelfth = leftMostTwelfth; twelfth != rightMostTwelfth; twelfth = TwelfthUtility.TwelfthAfter(twelfth))
			{
				if (!twelfths.Contains(twelfth))
				{
					Log.Error(string.Concat(new object[]
					{
						"Twelfths doesn't contain ",
						twelfth,
						" (",
						leftMostTwelfth,
						"..",
						rightMostTwelfth,
						")"
					}), false);
					break;
				}
				twelfths.Remove(twelfth);
			}
			twelfths.Remove(rightMostTwelfth);
			return GenDate.QuadrumDateStringAt(leftMostTwelfth) + " - " + GenDate.QuadrumDateStringAt(rightMostTwelfth);
		}
	}
}
