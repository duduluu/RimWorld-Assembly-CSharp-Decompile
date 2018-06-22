﻿using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Grammar;

namespace Verse
{
	// Token: 0x02000BCD RID: 3021
	[HasDebugOutput]
	public class PlayLogEntry_Interaction : LogEntry
	{
		// Token: 0x060041DB RID: 16859 RVA: 0x0022B4D4 File Offset: 0x002298D4
		public PlayLogEntry_Interaction() : base(null)
		{
		}

		// Token: 0x060041DC RID: 16860 RVA: 0x0022B4E5 File Offset: 0x002298E5
		public PlayLogEntry_Interaction(InteractionDef intDef, Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks) : base(null)
		{
			this.intDef = intDef;
			this.initiator = initiator;
			this.recipient = recipient;
			this.extraSentencePacks = extraSentencePacks;
		}

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x060041DD RID: 16861 RVA: 0x0022B514 File Offset: 0x00229914
		private string InitiatorName
		{
			get
			{
				return (this.initiator == null) ? "null" : this.initiator.LabelShort;
			}
		}

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x060041DE RID: 16862 RVA: 0x0022B54C File Offset: 0x0022994C
		private string RecipientName
		{
			get
			{
				return (this.recipient == null) ? "null" : this.recipient.LabelShort;
			}
		}

		// Token: 0x060041DF RID: 16863 RVA: 0x0022B584 File Offset: 0x00229984
		public override bool Concerns(Thing t)
		{
			return t == this.initiator || t == this.recipient;
		}

		// Token: 0x060041E0 RID: 16864 RVA: 0x0022B5B4 File Offset: 0x002299B4
		public override IEnumerable<Thing> GetConcerns()
		{
			if (this.initiator != null)
			{
				yield return this.initiator;
			}
			if (this.recipient != null)
			{
				yield return this.recipient;
			}
			yield break;
		}

		// Token: 0x060041E1 RID: 16865 RVA: 0x0022B5E0 File Offset: 0x002299E0
		public override void ClickedFromPOV(Thing pov)
		{
			if (pov == this.initiator)
			{
				CameraJumper.TryJumpAndSelect(this.recipient);
			}
			else
			{
				if (pov != this.recipient)
				{
					throw new NotImplementedException();
				}
				CameraJumper.TryJumpAndSelect(this.initiator);
			}
		}

		// Token: 0x060041E2 RID: 16866 RVA: 0x0022B638 File Offset: 0x00229A38
		public override Texture2D IconFromPOV(Thing pov)
		{
			return this.intDef.Symbol;
		}

		// Token: 0x060041E3 RID: 16867 RVA: 0x0022B658 File Offset: 0x00229A58
		public override string GetTipString()
		{
			return this.intDef.LabelCap + "\n" + base.GetTipString();
		}

		// Token: 0x060041E4 RID: 16868 RVA: 0x0022B688 File Offset: 0x00229A88
		protected override string ToGameStringFromPOV_Worker(Thing pov, bool forceLog)
		{
			string result;
			if (this.initiator == null || this.recipient == null)
			{
				Log.ErrorOnce("PlayLogEntry_Interaction has a null pawn reference.", 34422, false);
				result = "[" + this.intDef.label + " error: null pawn reference]";
			}
			else
			{
				Rand.PushState();
				Rand.Seed = this.logID;
				GrammarRequest request = base.GenerateGrammarRequest();
				string text;
				if (pov == this.initiator)
				{
					request.IncludesBare.Add(this.intDef.logRulesInitiator);
					request.Rules.AddRange(GrammarUtility.RulesForPawn("ME", this.initiator, request.Constants));
					request.Rules.AddRange(GrammarUtility.RulesForPawn("OTHER", this.recipient, request.Constants));
					request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, request.Constants));
					request.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, request.Constants));
					text = GrammarResolver.Resolve("r_logentry", request, "interaction from initiator", forceLog);
				}
				else if (pov == this.recipient)
				{
					if (this.intDef.logRulesRecipient != null)
					{
						request.IncludesBare.Add(this.intDef.logRulesRecipient);
					}
					else
					{
						request.IncludesBare.Add(this.intDef.logRulesInitiator);
					}
					request.Rules.AddRange(GrammarUtility.RulesForPawn("ME", this.recipient, request.Constants));
					request.Rules.AddRange(GrammarUtility.RulesForPawn("OTHER", this.initiator, request.Constants));
					request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, request.Constants));
					request.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, request.Constants));
					text = GrammarResolver.Resolve("r_logentry", request, "interaction from recipient", forceLog);
				}
				else
				{
					Log.ErrorOnce("Cannot display PlayLogEntry_Interaction from POV who isn't initiator or recipient.", 51251, false);
					text = this.ToString();
				}
				if (this.extraSentencePacks != null)
				{
					for (int i = 0; i < this.extraSentencePacks.Count; i++)
					{
						request.Clear();
						request.Includes.Add(this.extraSentencePacks[i]);
						request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiator, request.Constants));
						request.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipient, request.Constants));
						text = text + " " + GrammarResolver.Resolve(this.extraSentencePacks[i].RulesPlusIncludes[0].keyword, request, "extraSentencePack", forceLog);
					}
				}
				Rand.PopState();
				result = text;
			}
			return result;
		}

		// Token: 0x060041E5 RID: 16869 RVA: 0x0022B99C File Offset: 0x00229D9C
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look<InteractionDef>(ref this.intDef, "intDef");
			Scribe_References.Look<Pawn>(ref this.initiator, "initiator", true);
			Scribe_References.Look<Pawn>(ref this.recipient, "recipient", true);
			Scribe_Collections.Look<RulePackDef>(ref this.extraSentencePacks, "extras", LookMode.Undefined, new object[0]);
		}

		// Token: 0x060041E6 RID: 16870 RVA: 0x0022B9FC File Offset: 0x00229DFC
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.intDef.label,
				": ",
				this.InitiatorName,
				"->",
				this.RecipientName
			});
		}

		// Token: 0x04002CF7 RID: 11511
		private InteractionDef intDef;

		// Token: 0x04002CF8 RID: 11512
		private Pawn initiator;

		// Token: 0x04002CF9 RID: 11513
		private Pawn recipient;

		// Token: 0x04002CFA RID: 11514
		private List<RulePackDef> extraSentencePacks = null;
	}
}
