﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using RimWorld;
using RimWorld.Planet;

namespace Verse.AI
{
	public class MentalBreaker : IExposable
	{
		private Pawn pawn;

		private int ticksBelowExtreme = 0;

		private int ticksBelowMajor = 0;

		private int ticksBelowMinor = 0;

		private const int CheckInterval = 150;

		private const float ExtremeBreakMTBDays = 0.7f;

		private const float MajorBreakMTBDays = 3f;

		private const float MinorBreakMTBDays = 10f;

		private const int MinTicksBelowToBreak = 3000;

		private const float MajorBreakMoodSpan = 0.15f;

		private const float MinorBreakMoodSpan = 0.15f;

		private static List<Thought> tmpThoughts = new List<Thought>();

		[CompilerGenerated]
		private static Func<Thought, float> <>f__am$cache0;

		public MentalBreaker()
		{
		}

		public MentalBreaker(Pawn pawn)
		{
			this.pawn = pawn;
		}

		public float BreakThresholdExtreme
		{
			get
			{
				return this.pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true);
			}
		}

		public float BreakThresholdMajor
		{
			get
			{
				return this.pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true) + 0.15f;
			}
		}

		public float BreakThresholdMinor
		{
			get
			{
				return this.pawn.GetStatValue(StatDefOf.MentalBreakThreshold, true) + 0.15f + 0.15f;
			}
		}

		private bool CanDoRandomMentalBreaks
		{
			get
			{
				return this.pawn.RaceProps.Humanlike && (this.pawn.Spawned || this.pawn.IsCaravanMember());
			}
		}

		public bool BreakExtremeIsImminent
		{
			get
			{
				return this.pawn.MentalStateDef == null && this.CurMood < this.BreakThresholdExtreme;
			}
		}

		public bool BreakMajorIsImminent
		{
			get
			{
				return this.pawn.MentalStateDef == null && !this.BreakExtremeIsImminent && this.CurMood < this.BreakThresholdMajor;
			}
		}

		public bool BreakMinorIsImminent
		{
			get
			{
				return this.pawn.MentalStateDef == null && !this.BreakExtremeIsImminent && !this.BreakMajorIsImminent && this.CurMood < this.BreakThresholdMinor;
			}
		}

		public bool BreakExtremeIsApproaching
		{
			get
			{
				return this.pawn.MentalStateDef == null && !this.BreakExtremeIsImminent && this.CurMood < this.BreakThresholdExtreme + 0.1f;
			}
		}

		private float CurMood
		{
			get
			{
				float result;
				if (this.pawn.needs.mood == null)
				{
					result = 0.5f;
				}
				else
				{
					result = this.pawn.needs.mood.CurLevel;
				}
				return result;
			}
		}

		private IEnumerable<MentalBreakDef> CurrentPossibleMoodBreaks
		{
			get
			{
				MentalBreakIntensity intensity;
				for (intensity = this.CurrentDesiredMoodBreakIntensity; intensity != MentalBreakIntensity.None; intensity = (MentalBreakIntensity)(intensity - MentalBreakIntensity.Minor))
				{
					IEnumerable<MentalBreakDef> breaks = from d in DefDatabase<MentalBreakDef>.AllDefsListForReading
					where d.intensity == intensity && d.Worker.BreakCanOccur(this.pawn)
					select d;
					bool yieldedAny = false;
					foreach (MentalBreakDef b in breaks)
					{
						yield return b;
						yieldedAny = true;
					}
					if (yieldedAny)
					{
						yield break;
					}
				}
				yield break;
			}
		}

		private MentalBreakIntensity CurrentDesiredMoodBreakIntensity
		{
			get
			{
				MentalBreakIntensity result;
				if (this.ticksBelowExtreme >= 3000)
				{
					result = MentalBreakIntensity.Extreme;
				}
				else if (this.ticksBelowMajor >= 3000)
				{
					result = MentalBreakIntensity.Major;
				}
				else if (this.ticksBelowMinor >= 3000)
				{
					result = MentalBreakIntensity.Minor;
				}
				else
				{
					result = MentalBreakIntensity.None;
				}
				return result;
			}
		}

		internal void Reset()
		{
			this.ticksBelowExtreme = 0;
			this.ticksBelowMajor = 0;
			this.ticksBelowMinor = 0;
		}

		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.ticksBelowExtreme, "ticksBelowExtreme", 0, false);
			Scribe_Values.Look<int>(ref this.ticksBelowMajor, "ticksBelowMajor", 0, false);
			Scribe_Values.Look<int>(ref this.ticksBelowMinor, "ticksBelowMinor", 0, false);
		}

		public void MentalBreakerTick()
		{
			if (this.CanDoRandomMentalBreaks && this.pawn.MentalStateDef == null && this.pawn.IsHashIntervalTick(150))
			{
				if (DebugSettings.enableRandomMentalStates)
				{
					if (this.CurMood < this.BreakThresholdExtreme)
					{
						this.ticksBelowExtreme += 150;
					}
					else
					{
						this.ticksBelowExtreme = 0;
					}
					if (this.CurMood < this.BreakThresholdMajor)
					{
						this.ticksBelowMajor += 150;
					}
					else
					{
						this.ticksBelowMajor = 0;
					}
					if (this.CurMood < this.BreakThresholdMinor)
					{
						this.ticksBelowMinor += 150;
					}
					else
					{
						this.ticksBelowMinor = 0;
					}
					if (this.TestMoodMentalBreak())
					{
						if (this.TryDoRandomMoodCausedMentalBreak())
						{
							return;
						}
					}
					if (this.pawn.story != null)
					{
						List<Trait> allTraits = this.pawn.story.traits.allTraits;
						for (int i = 0; i < allTraits.Count; i++)
						{
							TraitDegreeData currentData = allTraits[i].CurrentData;
							if (currentData.randomMentalState != null)
							{
								float mtb = currentData.randomMentalStateMtbDaysMoodCurve.Evaluate(this.CurMood);
								if (Rand.MTBEventOccurs(mtb, 60000f, 150f))
								{
									if (currentData.randomMentalState.Worker.StateCanOccur(this.pawn))
									{
										if (this.pawn.mindState.mentalStateHandler.TryStartMentalState(currentData.randomMentalState, null, false, false, null, false))
										{
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private bool TestMoodMentalBreak()
		{
			bool result;
			if (this.ticksBelowExtreme > 3000)
			{
				result = Rand.MTBEventOccurs(0.7f, 60000f, 150f);
			}
			else if (this.ticksBelowMajor > 3000)
			{
				result = Rand.MTBEventOccurs(3f, 60000f, 150f);
			}
			else
			{
				result = (this.ticksBelowMinor > 3000 && Rand.MTBEventOccurs(10f, 60000f, 150f));
			}
			return result;
		}

		public bool TryDoRandomMoodCausedMentalBreak()
		{
			bool result;
			MentalBreakDef mentalBreakDef;
			if (!this.CanDoRandomMentalBreaks || this.pawn.Downed || !this.pawn.Awake() || this.pawn.InMentalState)
			{
				result = false;
			}
			else if (this.pawn.Faction != Faction.OfPlayer && this.CurrentDesiredMoodBreakIntensity != MentalBreakIntensity.Extreme)
			{
				result = false;
			}
			else if (!this.CurrentPossibleMoodBreaks.TryRandomElementByWeight((MentalBreakDef d) => d.Worker.CommonalityFor(this.pawn), out mentalBreakDef))
			{
				result = false;
			}
			else
			{
				Thought reason = this.RandomFinalStraw();
				result = mentalBreakDef.Worker.TryStart(this.pawn, reason, true);
			}
			return result;
		}

		private Thought RandomFinalStraw()
		{
			this.pawn.needs.mood.thoughts.GetAllMoodThoughts(MentalBreaker.tmpThoughts);
			float num = 0f;
			for (int i = 0; i < MentalBreaker.tmpThoughts.Count; i++)
			{
				float num2 = MentalBreaker.tmpThoughts[i].MoodOffset();
				if (num2 < num)
				{
					num = num2;
				}
			}
			float maxMoodOffset = num * 0.5f;
			Thought result = null;
			(from x in MentalBreaker.tmpThoughts
			where x.MoodOffset() <= maxMoodOffset
			select x).TryRandomElementByWeight((Thought x) => -x.MoodOffset(), out result);
			MentalBreaker.tmpThoughts.Clear();
			return result;
		}

		public void Notify_RecoveredFromMentalState()
		{
			this.Reset();
		}

		public float MentalBreakThresholdFor(MentalBreakIntensity intensity)
		{
			float result;
			if (intensity != MentalBreakIntensity.Extreme)
			{
				if (intensity != MentalBreakIntensity.Major)
				{
					if (intensity != MentalBreakIntensity.Minor)
					{
						throw new NotImplementedException();
					}
					result = this.BreakThresholdMinor;
				}
				else
				{
					result = this.BreakThresholdMajor;
				}
			}
			else
			{
				result = this.BreakThresholdExtreme;
			}
			return result;
		}

		internal string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.pawn.ToString());
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"   ticksBelowExtreme=",
				this.ticksBelowExtreme,
				"/",
				3000
			}));
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"   ticksBelowSerious=",
				this.ticksBelowMajor,
				"/",
				3000
			}));
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"   ticksBelowMinor=",
				this.ticksBelowMinor,
				"/",
				3000
			}));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Current desired mood break intensity: " + this.CurrentDesiredMoodBreakIntensity.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Current possible mood breaks:");
			float num = (from d in this.CurrentPossibleMoodBreaks
			select d.Worker.CommonalityFor(this.pawn)).Sum();
			foreach (MentalBreakDef mentalBreakDef in this.CurrentPossibleMoodBreaks)
			{
				float num2 = mentalBreakDef.Worker.CommonalityFor(this.pawn);
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"   ",
					mentalBreakDef,
					"     ",
					(num2 / num).ToStringPercent()
				}));
			}
			return stringBuilder.ToString();
		}

		internal void LogPossibleMentalBreaks()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.pawn + " current possible mood mental breaks:");
			stringBuilder.AppendLine("CurrentDesiredMoodBreakIntensity: " + this.CurrentDesiredMoodBreakIntensity);
			foreach (MentalBreakDef arg in this.CurrentPossibleMoodBreaks)
			{
				stringBuilder.AppendLine("  " + arg);
			}
			Log.Message(stringBuilder.ToString(), false);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MentalBreaker()
		{
		}

		[CompilerGenerated]
		private float <TryDoRandomMoodCausedMentalBreak>m__0(MentalBreakDef d)
		{
			return d.Worker.CommonalityFor(this.pawn);
		}

		[CompilerGenerated]
		private static float <RandomFinalStraw>m__1(Thought x)
		{
			return -x.MoodOffset();
		}

		[CompilerGenerated]
		private float <DebugString>m__2(MentalBreakDef d)
		{
			return d.Worker.CommonalityFor(this.pawn);
		}

		[CompilerGenerated]
		private sealed class <>c__Iterator0 : IEnumerable, IEnumerable<MentalBreakDef>, IEnumerator, IDisposable, IEnumerator<MentalBreakDef>
		{
			internal IEnumerable<MentalBreakDef> <breaks>__1;

			internal bool <yieldedAny>__1;

			internal IEnumerator<MentalBreakDef> $locvar0;

			internal MentalBreakDef <b>__2;

			internal MentalBreaker $this;

			internal MentalBreakDef $current;

			internal bool $disposing;

			internal int $PC;

			private MentalBreaker.<>c__Iterator0.<>c__AnonStorey1 $locvar1;

			[DebuggerHidden]
			public <>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
				{
					MentalBreakIntensity intensity = base.CurrentDesiredMoodBreakIntensity;
					break;
				}
				case 1u:
					Block_2:
					try
					{
						switch (num)
						{
						case 1u:
							yieldedAny = true;
							break;
						}
						if (enumerator.MoveNext())
						{
							b = enumerator.Current;
							this.$current = b;
							if (!this.$disposing)
							{
								this.$PC = 1;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator != null)
							{
								enumerator.Dispose();
							}
						}
					}
					if (yieldedAny)
					{
						return false;
					}
					<>c__AnonStorey.intensity = (MentalBreakIntensity)(<>c__AnonStorey.intensity - MentalBreakIntensity.Minor);
					break;
				default:
					return false;
				}
				if (<>c__AnonStorey.intensity != MentalBreakIntensity.None)
				{
					breaks = from d in DefDatabase<MentalBreakDef>.AllDefsListForReading
					where d.intensity == <>c__AnonStorey.intensity && d.Worker.BreakCanOccur(<>c__AnonStorey.<>f__ref$0.$this.pawn)
					select d;
					yieldedAny = false;
					enumerator = breaks.GetEnumerator();
					num = 4294967293u;
					goto Block_2;
				}
				return false;
			}

			MentalBreakDef IEnumerator<MentalBreakDef>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)this.$PC;
				this.$disposing = true;
				this.$PC = -1;
				switch (num)
				{
				case 1u:
					try
					{
					}
					finally
					{
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Verse.MentalBreakDef>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<MentalBreakDef> IEnumerable<MentalBreakDef>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				MentalBreaker.<>c__Iterator0 <>c__Iterator = new MentalBreaker.<>c__Iterator0();
				<>c__Iterator.$this = this;
				return <>c__Iterator;
			}

			private sealed class <>c__AnonStorey1
			{
				internal MentalBreakIntensity intensity;

				internal MentalBreaker.<>c__Iterator0 <>f__ref$0;

				public <>c__AnonStorey1()
				{
				}

				internal bool <>m__0(MentalBreakDef d)
				{
					return d.intensity == this.intensity && d.Worker.BreakCanOccur(this.<>f__ref$0.$this.pawn);
				}
			}
		}

		[CompilerGenerated]
		private sealed class <RandomFinalStraw>c__AnonStorey2
		{
			internal float maxMoodOffset;

			public <RandomFinalStraw>c__AnonStorey2()
			{
			}

			internal bool <>m__0(Thought x)
			{
				return x.MoodOffset() <= this.maxMoodOffset;
			}
		}
	}
}
