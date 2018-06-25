﻿using System;
using System.Linq;

namespace Verse
{
	// Token: 0x02000D4B RID: 3403
	public class NameTriple : Name
	{
		// Token: 0x04003287 RID: 12935
		[LoadAlias("first")]
		private string firstInt;

		// Token: 0x04003288 RID: 12936
		[LoadAlias("nick")]
		private string nickInt;

		// Token: 0x04003289 RID: 12937
		[LoadAlias("last")]
		private string lastInt;

		// Token: 0x0400328A RID: 12938
		private static NameTriple invalidInt = new NameTriple("Invalid", "Invalid", "Invalid");

		// Token: 0x06004B07 RID: 19207 RVA: 0x002722E2 File Offset: 0x002706E2
		public NameTriple()
		{
		}

		// Token: 0x06004B08 RID: 19208 RVA: 0x002722EB File Offset: 0x002706EB
		public NameTriple(string first, string nick, string last)
		{
			this.firstInt = first.Trim();
			this.nickInt = nick.Trim();
			this.lastInt = last.Trim();
		}

		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x06004B09 RID: 19209 RVA: 0x00272318 File Offset: 0x00270718
		public string First
		{
			get
			{
				return this.firstInt;
			}
		}

		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x06004B0A RID: 19210 RVA: 0x00272334 File Offset: 0x00270734
		public string Nick
		{
			get
			{
				return this.nickInt;
			}
		}

		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x06004B0B RID: 19211 RVA: 0x00272350 File Offset: 0x00270750
		public string Last
		{
			get
			{
				return this.lastInt;
			}
		}

		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x06004B0C RID: 19212 RVA: 0x0027236C File Offset: 0x0027076C
		public override string ToStringFull
		{
			get
			{
				string result;
				if (this.First == this.Nick || this.Last == this.Nick)
				{
					result = this.First + " " + this.Last;
				}
				else
				{
					result = string.Concat(new string[]
					{
						this.First,
						" '",
						this.Nick,
						"' ",
						this.Last
					});
				}
				return result;
			}
		}

		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x06004B0D RID: 19213 RVA: 0x00272400 File Offset: 0x00270800
		public override string ToStringShort
		{
			get
			{
				return this.nickInt;
			}
		}

		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x06004B0E RID: 19214 RVA: 0x0027241C File Offset: 0x0027081C
		public override bool IsValid
		{
			get
			{
				return !this.First.NullOrEmpty() && !this.Last.NullOrEmpty();
			}
		}

		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x06004B0F RID: 19215 RVA: 0x00272454 File Offset: 0x00270854
		public override bool Numerical
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x06004B10 RID: 19216 RVA: 0x0027246C File Offset: 0x0027086C
		public static NameTriple Invalid
		{
			get
			{
				return NameTriple.invalidInt;
			}
		}

		// Token: 0x06004B11 RID: 19217 RVA: 0x00272486 File Offset: 0x00270886
		public override void ExposeData()
		{
			Scribe_Values.Look<string>(ref this.firstInt, "first", null, false);
			Scribe_Values.Look<string>(ref this.nickInt, "nick", null, false);
			Scribe_Values.Look<string>(ref this.lastInt, "last", null, false);
		}

		// Token: 0x06004B12 RID: 19218 RVA: 0x002724C0 File Offset: 0x002708C0
		public void PostLoad()
		{
			if (this.firstInt != null)
			{
				this.firstInt = this.firstInt.Trim();
			}
			if (this.nickInt != null)
			{
				this.nickInt = this.nickInt.Trim();
			}
			if (this.lastInt != null)
			{
				this.lastInt = this.lastInt.Trim();
			}
		}

		// Token: 0x06004B13 RID: 19219 RVA: 0x00272524 File Offset: 0x00270924
		public void ResolveMissingPieces(string overrideLastName = null)
		{
			if (this.First.NullOrEmpty() && this.Nick.NullOrEmpty() && this.Last.NullOrEmpty())
			{
				Log.Error("Cannot resolve misssing pieces in PawnName: No name data.", false);
				this.firstInt = (this.nickInt = (this.lastInt = "Empty"));
			}
			else
			{
				if (this.First == null)
				{
					this.firstInt = "";
				}
				if (this.Last == null)
				{
					this.lastInt = "";
				}
				if (overrideLastName != null)
				{
					this.lastInt = overrideLastName;
				}
				if (this.Nick.NullOrEmpty())
				{
					if (this.Last == "")
					{
						this.nickInt = this.First;
					}
					else
					{
						if (Rand.ValueSeeded(Gen.HashCombine<string>(this.First.GetHashCode(), this.Last)) < 0.5f)
						{
							this.nickInt = this.First;
						}
						else
						{
							this.nickInt = this.Last;
						}
						this.CapitalizeNick();
					}
				}
			}
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x0027264C File Offset: 0x00270A4C
		public override bool ConfusinglySimilarTo(Name other)
		{
			NameTriple nameTriple = other as NameTriple;
			if (nameTriple != null)
			{
				if (this.Nick != null && this.Nick == nameTriple.Nick)
				{
					return true;
				}
				if (this.First == nameTriple.First && this.Last == nameTriple.Last)
				{
					return true;
				}
			}
			NameSingle nameSingle = other as NameSingle;
			return nameSingle != null && nameSingle.Name == this.Nick;
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x002726F8 File Offset: 0x00270AF8
		public static NameTriple FromString(string rawName)
		{
			NameTriple result;
			if (rawName.Trim().Length == 0)
			{
				Log.Error("Tried to parse PawnName from empty or whitespace string.", false);
				result = NameTriple.Invalid;
			}
			else
			{
				NameTriple nameTriple = new NameTriple();
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < rawName.Length - 1; i++)
				{
					if (rawName[i] == ' ' && rawName[i + 1] == '\'' && num == -1)
					{
						num = i;
					}
					if (rawName[i] == '\'' && rawName[i + 1] == ' ')
					{
						num2 = i;
					}
				}
				if (num == -1 || num2 == -1)
				{
					if (!rawName.Contains(' '))
					{
						nameTriple.nickInt = rawName.Trim();
					}
					else
					{
						string[] array = rawName.Split(new char[]
						{
							' '
						});
						if (array.Length == 1)
						{
							nameTriple.nickInt = array[0].Trim();
						}
						else if (array.Length == 2)
						{
							nameTriple.firstInt = array[0].Trim();
							nameTriple.lastInt = array[1].Trim();
						}
						else
						{
							nameTriple.firstInt = array[0].Trim();
							nameTriple.lastInt = "";
							for (int j = 1; j < array.Length; j++)
							{
								NameTriple nameTriple2 = nameTriple;
								nameTriple2.lastInt += array[j];
								if (j < array.Length - 1)
								{
									NameTriple nameTriple3 = nameTriple;
									nameTriple3.lastInt += " ";
								}
							}
						}
					}
				}
				else
				{
					nameTriple.firstInt = rawName.Substring(0, num).Trim();
					nameTriple.nickInt = rawName.Substring(num + 2, num2 - num - 2).Trim();
					nameTriple.lastInt = ((num2 >= rawName.Length - 2) ? "" : rawName.Substring(num2 + 2).Trim());
				}
				result = nameTriple;
			}
			return result;
		}

		// Token: 0x06004B16 RID: 19222 RVA: 0x0027290C File Offset: 0x00270D0C
		public void CapitalizeNick()
		{
			if (!this.nickInt.NullOrEmpty())
			{
				this.nickInt = char.ToUpper(this.Nick[0]) + this.Nick.Substring(1);
			}
		}

		// Token: 0x06004B17 RID: 19223 RVA: 0x0027294C File Offset: 0x00270D4C
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.First,
				" '",
				this.Nick,
				"' ",
				this.Last
			});
		}

		// Token: 0x06004B18 RID: 19224 RVA: 0x00272998 File Offset: 0x00270D98
		public override bool Equals(object obj)
		{
			bool result;
			if (obj == null)
			{
				result = false;
			}
			else if (!(obj is NameTriple))
			{
				result = false;
			}
			else
			{
				NameTriple nameTriple = (NameTriple)obj;
				result = (this.First == nameTriple.First && this.Last == nameTriple.Last && this.Nick == nameTriple.Nick);
			}
			return result;
		}

		// Token: 0x06004B19 RID: 19225 RVA: 0x00272A14 File Offset: 0x00270E14
		public override int GetHashCode()
		{
			int seed = 0;
			seed = Gen.HashCombine<string>(seed, this.First);
			seed = Gen.HashCombine<string>(seed, this.Last);
			return Gen.HashCombine<string>(seed, this.Nick);
		}
	}
}
