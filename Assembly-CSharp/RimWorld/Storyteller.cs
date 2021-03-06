﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Storyteller : IExposable
	{
		public StorytellerDef def;

		public DifficultyDef difficulty;

		public List<StorytellerComp> storytellerComps;

		public IncidentQueue incidentQueue = new IncidentQueue();

		public static readonly Vector2 PortraitSizeTiny = new Vector2(116f, 124f);

		public static readonly Vector2 PortraitSizeLarge = new Vector2(580f, 620f);

		public const int IntervalsPerDay = 60;

		public const int CheckInterval = 1000;

		private static List<IIncidentTarget> tmpAllIncidentTargets = new List<IIncidentTarget>();

		private string debugStringCached = "Generating data...";

		[CompilerGenerated]
		private static Func<Pawn, bool> <>f__am$cache0;

		[CompilerGenerated]
		private static Func<Pawn, bool> <>f__am$cache1;

		public Storyteller()
		{
		}

		public Storyteller(StorytellerDef def, DifficultyDef difficulty)
		{
			this.def = def;
			this.difficulty = difficulty;
			this.InitializeStorytellerComps();
		}

		public List<IIncidentTarget> AllIncidentTargets
		{
			get
			{
				Storyteller.tmpAllIncidentTargets.Clear();
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					Storyteller.tmpAllIncidentTargets.Add(maps[i]);
				}
				List<Caravan> caravans = Find.WorldObjects.Caravans;
				for (int j = 0; j < caravans.Count; j++)
				{
					if (caravans[j].IsPlayerControlled)
					{
						Storyteller.tmpAllIncidentTargets.Add(caravans[j]);
					}
				}
				Storyteller.tmpAllIncidentTargets.Add(Find.World);
				return Storyteller.tmpAllIncidentTargets;
			}
		}

		public static void StorytellerStaticUpdate()
		{
			Storyteller.tmpAllIncidentTargets.Clear();
		}

		private void InitializeStorytellerComps()
		{
			this.storytellerComps = new List<StorytellerComp>();
			for (int i = 0; i < this.def.comps.Count; i++)
			{
				StorytellerComp storytellerComp = (StorytellerComp)Activator.CreateInstance(this.def.comps[i].compClass);
				storytellerComp.props = this.def.comps[i];
				this.storytellerComps.Add(storytellerComp);
			}
		}

		public void ExposeData()
		{
			Scribe_Defs.Look<StorytellerDef>(ref this.def, "def");
			Scribe_Defs.Look<DifficultyDef>(ref this.difficulty, "difficulty");
			Scribe_Deep.Look<IncidentQueue>(ref this.incidentQueue, "incidentQueue", new object[0]);
			if (this.difficulty == null)
			{
				Log.Error("Loaded storyteller without difficulty", false);
				this.difficulty = DefDatabase<DifficultyDef>.AllDefsListForReading[3];
			}
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				this.InitializeStorytellerComps();
			}
		}

		public void StorytellerTick()
		{
			this.incidentQueue.IncidentQueueTick();
			if (Find.TickManager.TicksGame % 1000 == 0)
			{
				if (!DebugSettings.enableStoryteller)
				{
					return;
				}
				foreach (FiringIncident fi in this.MakeIncidentsForInterval())
				{
					this.TryFire(fi);
				}
			}
		}

		public bool TryFire(FiringIncident fi)
		{
			if (fi.def.Worker.CanFireNow(fi.parms, false) && fi.def.Worker.TryExecute(fi.parms))
			{
				fi.parms.target.StoryState.Notify_IncidentFired(fi);
				return true;
			}
			return false;
		}

		public IEnumerable<FiringIncident> MakeIncidentsForInterval()
		{
			List<IIncidentTarget> targets = this.AllIncidentTargets;
			for (int i = 0; i < this.storytellerComps.Count; i++)
			{
				foreach (FiringIncident incident in this.MakeIncidentsForInterval(this.storytellerComps[i], targets))
				{
					yield return incident;
				}
			}
			yield break;
		}

		public IEnumerable<FiringIncident> MakeIncidentsForInterval(StorytellerComp comp, List<IIncidentTarget> targets)
		{
			if (GenDate.DaysPassedFloat <= comp.props.minDaysPassed)
			{
				yield break;
			}
			for (int i = 0; i < targets.Count; i++)
			{
				IIncidentTarget targ = targets[i];
				bool flag = false;
				bool flag2 = comp.props.allowedTargetTags.NullOrEmpty<IncidentTargetTagDef>();
				foreach (IncidentTargetTagDef item in targ.IncidentTargetTags())
				{
					if (!comp.props.disallowedTargetTags.NullOrEmpty<IncidentTargetTagDef>() && comp.props.disallowedTargetTags.Contains(item))
					{
						flag = true;
						break;
					}
					if (!flag2 && comp.props.allowedTargetTags.Contains(item))
					{
						flag2 = true;
					}
				}
				if (!flag && flag2)
				{
					foreach (FiringIncident fi in comp.MakeIntervalIncidents(targ))
					{
						if (Find.Storyteller.difficulty.allowBigThreats || (fi.def.category != IncidentCategoryDefOf.ThreatBig && fi.def.category != IncidentCategoryDefOf.RaidBeacon))
						{
							yield return fi;
						}
					}
				}
			}
			yield break;
		}

		public void Notify_PawnEvent(Pawn pawn, AdaptationEvent ev, DamageInfo? dinfo = null)
		{
			Find.StoryWatcher.watcherAdaptation.Notify_PawnEvent(pawn, ev, dinfo);
			for (int i = 0; i < this.storytellerComps.Count; i++)
			{
				StorytellerComp storytellerComp = this.storytellerComps[i];
				storytellerComp.Notify_PawnEvent(pawn, ev, dinfo);
			}
		}

		public void Notify_DefChanged()
		{
			this.InitializeStorytellerComps();
		}

		public string DebugString()
		{
			if (Time.frameCount % 60 == 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Storyteller : " + this.def.label);
				stringBuilder.AppendLine("------------- Global threats data ---------------");
				stringBuilder.AppendLine("   Adaptation days: " + Find.StoryWatcher.watcherAdaptation.AdaptDays.ToString("F1"));
				stringBuilder.AppendLine("   Adapt points factor: " + Find.StoryWatcher.watcherAdaptation.TotalThreatPointsFactor.ToString("F2"));
				stringBuilder.AppendLine("   Time points factor: " + Find.Storyteller.def.pointsFactorFromDaysPassed.Evaluate((float)GenDate.DaysPassed).ToString("F2"));
				stringBuilder.AppendLine("   Num raids enemy: " + Find.StoryWatcher.statsRecord.numRaidsEnemy);
				stringBuilder.AppendLine("   Ally incident fraction (neutral or ally): " + StorytellerUtility.AllyIncidentFraction(false).ToString("F2"));
				stringBuilder.AppendLine("   Ally incident fraction (ally only): " + StorytellerUtility.AllyIncidentFraction(true).ToString("F2"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("-------------- Global population data --------------");
				stringBuilder.AppendLine(StorytellerUtilityPopulation.DebugReadout().TrimEndNewlines());
				stringBuilder.AppendLine("   Greatest population: " + Find.StoryWatcher.statsRecord.greatestPopulation);
				stringBuilder.AppendLine("------------- All incident targets --------------");
				for (int i = 0; i < this.AllIncidentTargets.Count; i++)
				{
					stringBuilder.AppendLine("   " + this.AllIncidentTargets[i].ToString());
				}
				IIncidentTarget incidentTarget = Find.WorldSelector.SingleSelectedObject as IIncidentTarget;
				if (incidentTarget == null)
				{
					incidentTarget = Find.CurrentMap;
				}
				if (incidentTarget != null)
				{
					Map map = incidentTarget as Map;
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("---------- Selected: " + incidentTarget + " --------");
					stringBuilder.AppendLine("   Wealth: " + incidentTarget.PlayerWealthForStoryteller.ToString("F0"));
					if (map != null)
					{
						stringBuilder.AppendLine(string.Concat(new string[]
						{
							"   (Items: ",
							map.wealthWatcher.WealthItems.ToString("F0"),
							" Buildings: ",
							map.wealthWatcher.WealthBuildings.ToString("F0"),
							" (Floors: ",
							map.wealthWatcher.WealthFloorsOnly.ToString("F0"),
							") Pawns: ",
							map.wealthWatcher.WealthPawns.ToString("F0"),
							")"
						}));
					}
					stringBuilder.AppendLine("   IncidentPointsRandomFactorRange: " + incidentTarget.IncidentPointsRandomFactorRange);
					stringBuilder.AppendLine("   Pawns-Humanlikes: " + (from p in incidentTarget.PlayerPawnsForStoryteller
					where p.def.race.Humanlike
					select p).Count<Pawn>());
					stringBuilder.AppendLine("   Pawns-Animals: " + (from p in incidentTarget.PlayerPawnsForStoryteller
					where p.def.race.Animal
					select p).Count<Pawn>());
					if (map != null)
					{
						stringBuilder.AppendLine("   StoryDanger: " + map.dangerWatcher.DangerRating);
						stringBuilder.AppendLine("   FireDanger: " + map.fireWatcher.FireDanger.ToString("F2"));
						stringBuilder.AppendLine("   LastThreatBigTick days ago: " + (Find.TickManager.TicksGame - map.storyState.LastThreatBigTick).ToStringTicksToDays("F1"));
					}
					stringBuilder.AppendLine("   Current points (ignoring early raid factors): " + StorytellerUtility.DefaultThreatPointsNow(incidentTarget).ToString("F0"));
					stringBuilder.AppendLine("   Current points for specific IncidentMakers:");
					for (int j = 0; j < this.storytellerComps.Count; j++)
					{
						IncidentParms incidentParms = this.storytellerComps[j].GenerateParms(IncidentCategoryDefOf.ThreatBig, incidentTarget);
						stringBuilder.AppendLine("      " + this.storytellerComps[j].GetType().ToString().Substring(23) + ": " + incidentParms.points.ToString("F0"));
					}
				}
				this.debugStringCached = stringBuilder.ToString();
			}
			return this.debugStringCached;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Storyteller()
		{
		}

		[CompilerGenerated]
		private static bool <DebugString>m__0(Pawn p)
		{
			return p.def.race.Humanlike;
		}

		[CompilerGenerated]
		private static bool <DebugString>m__1(Pawn p)
		{
			return p.def.race.Animal;
		}

		[CompilerGenerated]
		private sealed class <MakeIncidentsForInterval>c__Iterator0 : IEnumerable, IEnumerable<FiringIncident>, IEnumerator, IDisposable, IEnumerator<FiringIncident>
		{
			internal List<IIncidentTarget> <targets>__0;

			internal int <i>__1;

			internal IEnumerator<FiringIncident> $locvar0;

			internal FiringIncident <incident>__2;

			internal Storyteller $this;

			internal FiringIncident $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <MakeIncidentsForInterval>c__Iterator0()
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
					targets = base.AllIncidentTargets;
					i = 0;
					break;
				case 1u:
					Block_2:
					try
					{
						switch (num)
						{
						}
						if (enumerator.MoveNext())
						{
							incident = enumerator.Current;
							this.$current = incident;
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
					i++;
					break;
				default:
					return false;
				}
				if (i < this.storytellerComps.Count)
				{
					enumerator = base.MakeIncidentsForInterval(this.storytellerComps[i], targets).GetEnumerator();
					num = 4294967293u;
					goto Block_2;
				}
				this.$PC = -1;
				return false;
			}

			FiringIncident IEnumerator<FiringIncident>.Current
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
				return this.System.Collections.Generic.IEnumerable<RimWorld.FiringIncident>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<FiringIncident> IEnumerable<FiringIncident>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				Storyteller.<MakeIncidentsForInterval>c__Iterator0 <MakeIncidentsForInterval>c__Iterator = new Storyteller.<MakeIncidentsForInterval>c__Iterator0();
				<MakeIncidentsForInterval>c__Iterator.$this = this;
				return <MakeIncidentsForInterval>c__Iterator;
			}
		}

		[CompilerGenerated]
		private sealed class <MakeIncidentsForInterval>c__Iterator1 : IEnumerable, IEnumerable<FiringIncident>, IEnumerator, IDisposable, IEnumerator<FiringIncident>
		{
			internal StorytellerComp comp;

			internal int <i>__1;

			internal List<IIncidentTarget> targets;

			internal IIncidentTarget <targ>__2;

			internal IEnumerator<FiringIncident> $locvar1;

			internal FiringIncident <fi>__3;

			internal FiringIncident $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <MakeIncidentsForInterval>c__Iterator1()
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
					if (GenDate.DaysPassedFloat <= comp.props.minDaysPassed)
					{
						return false;
					}
					i = 0;
					goto IL_21D;
				case 1u:
					Block_5:
					try
					{
						switch (num)
						{
						}
						while (enumerator2.MoveNext())
						{
							fi = enumerator2.Current;
							if (Find.Storyteller.difficulty.allowBigThreats || (fi.def.category != IncidentCategoryDefOf.ThreatBig && fi.def.category != IncidentCategoryDefOf.RaidBeacon))
							{
								this.$current = fi;
								if (!this.$disposing)
								{
									this.$PC = 1;
								}
								flag = true;
								return true;
							}
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator2 != null)
							{
								enumerator2.Dispose();
							}
						}
					}
					break;
				default:
					return false;
				}
				IL_20F:
				i++;
				IL_21D:
				if (i >= targets.Count)
				{
					this.$PC = -1;
				}
				else
				{
					targ = targets[i];
					bool flag2 = false;
					bool flag3 = comp.props.allowedTargetTags.NullOrEmpty<IncidentTargetTagDef>();
					foreach (IncidentTargetTagDef item in targ.IncidentTargetTags())
					{
						if (!comp.props.disallowedTargetTags.NullOrEmpty<IncidentTargetTagDef>() && comp.props.disallowedTargetTags.Contains(item))
						{
							flag2 = true;
							break;
						}
						if (!flag3 && comp.props.allowedTargetTags.Contains(item))
						{
							flag3 = true;
						}
					}
					if (flag2 || !flag3)
					{
						goto IL_20F;
					}
					enumerator2 = comp.MakeIntervalIncidents(targ).GetEnumerator();
					num = 4294967293u;
					goto Block_5;
				}
				return false;
			}

			FiringIncident IEnumerator<FiringIncident>.Current
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
						if (enumerator2 != null)
						{
							enumerator2.Dispose();
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
				return this.System.Collections.Generic.IEnumerable<RimWorld.FiringIncident>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<FiringIncident> IEnumerable<FiringIncident>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				Storyteller.<MakeIncidentsForInterval>c__Iterator1 <MakeIncidentsForInterval>c__Iterator = new Storyteller.<MakeIncidentsForInterval>c__Iterator1();
				<MakeIncidentsForInterval>c__Iterator.comp = comp;
				<MakeIncidentsForInterval>c__Iterator.targets = targets;
				return <MakeIncidentsForInterval>c__Iterator;
			}
		}
	}
}
