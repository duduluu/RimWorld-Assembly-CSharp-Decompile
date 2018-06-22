﻿using System;
using System.Text.RegularExpressions;

namespace Verse.Grammar
{
	// Token: 0x02000BE7 RID: 3047
	public class Rule_String : Rule
	{
		// Token: 0x06004285 RID: 17029 RVA: 0x00230D4C File Offset: 0x0022F14C
		public Rule_String(string keyword, string output)
		{
			this.keyword = keyword;
			this.output = output;
		}

		// Token: 0x06004286 RID: 17030 RVA: 0x00230D70 File Offset: 0x0022F170
		public Rule_String(string rawString)
		{
			Match match = Rule_String.pattern.Match(rawString);
			if (!match.Success)
			{
				Log.Error(string.Format("Bad string pass when reading rule {0}", rawString), false);
			}
			else
			{
				this.keyword = match.Groups["keyword"].Value;
				this.output = match.Groups["output"].Value;
				for (int i = 0; i < match.Groups["paramname"].Captures.Count; i++)
				{
					string value = match.Groups["paramname"].Captures[i].Value;
					string value2 = match.Groups["paramoperator"].Captures[i].Value;
					string value3 = match.Groups["paramvalue"].Captures[i].Value;
					if (value == "p")
					{
						if (value2 != "=")
						{
							Log.Error(string.Format("Attempt to compare p instead of assigning in rule {0}", rawString), false);
						}
						this.weight = float.Parse(value3);
					}
					else if (value == "debug")
					{
						Log.Error(string.Format("Rule {0} contains debug flag; fix before commit", rawString), false);
					}
					else if (value2 == "==" || value2 == "!=")
					{
						base.AddConstantConstraint(value, value3, value2 == "==");
					}
					else
					{
						Log.Error(string.Format("Unknown parameter {0} in rule {1}", value, rawString), false);
					}
				}
			}
		}

		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06004287 RID: 17031 RVA: 0x00230F3C File Offset: 0x0022F33C
		public override float BaseSelectionWeight
		{
			get
			{
				return this.weight;
			}
		}

		// Token: 0x06004288 RID: 17032 RVA: 0x00230F58 File Offset: 0x0022F358
		public override string Generate()
		{
			return this.output;
		}

		// Token: 0x06004289 RID: 17033 RVA: 0x00230F74 File Offset: 0x0022F374
		public override string ToString()
		{
			return this.keyword + "->" + this.output;
		}

		// Token: 0x04002D82 RID: 11650
		[MustTranslate]
		private string output;

		// Token: 0x04002D83 RID: 11651
		private float weight = 1f;

		// Token: 0x04002D84 RID: 11652
		private static Regex pattern = new Regex("\r\n\t\t# hold on to your butts, this is gonna get weird\r\n\r\n\t\t^\r\n\t\t(?<keyword>[a-zA-Z0-9_]+)\t\t\t\t\t# keyword; roughly limited to standard C# identifier rules\r\n\t\t(\t\t\t\t\t\t\t\t\t\t\t# parameter list is optional, open the capture group so we can keep it or ignore it\r\n\t\t\t\\(\t\t\t\t\t\t\t\t\t\t# this is the actual parameter list opening\r\n\t\t\t\t(\t\t\t\t\t\t\t\t\t# unlimited number of parameter groups\r\n\t\t\t\t\t(?<paramname>[a-zA-Z0-9_]+)\t# parameter name is similar\r\n\t\t\t\t\t(?<paramoperator>==|=|!=|)\t\t# operators; empty operator is allowed\r\n\t\t\t\t\t(?<paramvalue>[^\\,\\)]*)\t\t\t# parameter value, however, allows everything except comma and closeparen!\r\n\t\t\t\t\t,?\t\t\t\t\t\t\t\t# comma can be used to separate blocks; it is also silently ignored if it's a trailing comma\r\n\t\t\t\t)*\r\n\t\t\t\\)\r\n\t\t)?\r\n\t\t->(?<output>.*)\t\t\t\t\t\t\t\t# output is anything-goes\r\n\t\t$\r\n\r\n\t\t", RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
	}
}