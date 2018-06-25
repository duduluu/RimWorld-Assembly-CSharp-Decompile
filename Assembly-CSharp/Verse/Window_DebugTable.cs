﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse
{
	// Token: 0x02000E51 RID: 3665
	public class Window_DebugTable : Window
	{
		// Token: 0x0400391F RID: 14623
		private string[,] tableRaw;

		// Token: 0x04003920 RID: 14624
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x04003921 RID: 14625
		private string[,] tableSorted;

		// Token: 0x04003922 RID: 14626
		private List<float> colWidths = new List<float>();

		// Token: 0x04003923 RID: 14627
		private List<float> rowHeights = new List<float>();

		// Token: 0x04003924 RID: 14628
		private int sortColumn = -1;

		// Token: 0x04003925 RID: 14629
		private Window_DebugTable.SortMode sortMode = Window_DebugTable.SortMode.Off;

		// Token: 0x04003926 RID: 14630
		private bool[] colVisible;

		// Token: 0x04003927 RID: 14631
		private const float ColExtraWidth = 2f;

		// Token: 0x04003928 RID: 14632
		private const float RowExtraHeight = 2f;

		// Token: 0x04003929 RID: 14633
		private const float HiddenColumnWidth = 10f;

		// Token: 0x06005667 RID: 22119 RVA: 0x002C8CE8 File Offset: 0x002C70E8
		public Window_DebugTable(string[,] tables)
		{
			this.tableRaw = tables;
			this.colVisible = new bool[this.tableRaw.GetLength(0)];
			for (int i = 0; i < this.colVisible.Length; i++)
			{
				this.colVisible[i] = true;
			}
			this.doCloseButton = true;
			this.doCloseX = true;
			Text.Font = GameFont.Tiny;
			this.BuildTableSorted();
		}

		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x06005668 RID: 22120 RVA: 0x002C8D88 File Offset: 0x002C7188
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2((float)UI.screenWidth, (float)UI.screenHeight);
			}
		}

		// Token: 0x06005669 RID: 22121 RVA: 0x002C8DB0 File Offset: 0x002C71B0
		private void BuildTableSorted()
		{
			if (this.sortMode == Window_DebugTable.SortMode.Off)
			{
				this.tableSorted = this.tableRaw;
			}
			else
			{
				List<List<string>> list = new List<List<string>>();
				for (int i = 1; i < this.tableRaw.GetLength(1); i++)
				{
					list.Add(new List<string>());
					for (int j = 0; j < this.tableRaw.GetLength(0); j++)
					{
						list[i - 1].Add(this.tableRaw[j, i]);
					}
				}
				Window_DebugTable.NumericStringComparer comparer = new Window_DebugTable.NumericStringComparer();
				Window_DebugTable.SortMode sortMode = this.sortMode;
				if (sortMode != Window_DebugTable.SortMode.Ascending)
				{
					if (sortMode != Window_DebugTable.SortMode.Descending)
					{
						if (sortMode == Window_DebugTable.SortMode.Off)
						{
							throw new Exception();
						}
					}
					else
					{
						list = list.OrderByDescending((List<string> x) => x[this.sortColumn], comparer).ToList<List<string>>();
					}
				}
				else
				{
					list = list.OrderBy((List<string> x) => x[this.sortColumn], comparer).ToList<List<string>>();
				}
				this.tableSorted = new string[this.tableRaw.GetLength(0), this.tableRaw.GetLength(1)];
				for (int k = 0; k < this.tableRaw.GetLength(1); k++)
				{
					for (int l = 0; l < this.tableRaw.GetLength(0); l++)
					{
						if (k == 0)
						{
							this.tableSorted[l, k] = this.tableRaw[l, k];
						}
						else
						{
							this.tableSorted[l, k] = list[k - 1][l];
						}
					}
				}
			}
			this.colWidths.Clear();
			for (int m = 0; m < this.tableRaw.GetLength(0); m++)
			{
				float item;
				if (this.colVisible[m])
				{
					float num = 0f;
					for (int n = 0; n < this.tableRaw.GetLength(1); n++)
					{
						string text = this.tableRaw[m, n];
						float x2 = Text.CalcSize(text).x;
						if (x2 > num)
						{
							num = x2;
						}
					}
					item = num + 2f;
				}
				else
				{
					item = 10f;
				}
				this.colWidths.Add(item);
			}
			this.rowHeights.Clear();
			for (int num2 = 0; num2 < this.tableSorted.GetLength(1); num2++)
			{
				float num3 = 0f;
				for (int num4 = 0; num4 < this.tableSorted.GetLength(0); num4++)
				{
					string text2 = this.tableSorted[num4, num2];
					float y = Text.CalcSize(text2).y;
					if (y > num3)
					{
						num3 = y;
					}
				}
				this.rowHeights.Add(num3 + 2f);
			}
		}

		// Token: 0x0600566A RID: 22122 RVA: 0x002C90C4 File Offset: 0x002C74C4
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Tiny;
			inRect.yMax -= 40f;
			Rect viewRect = new Rect(0f, 0f, this.colWidths.Sum(), this.rowHeights.Sum());
			Widgets.BeginScrollView(inRect, ref this.scrollPosition, viewRect, true);
			float num = 0f;
			for (int i = 0; i < this.tableSorted.GetLength(0); i++)
			{
				float num2 = 0f;
				for (int j = 0; j < this.tableSorted.GetLength(1); j++)
				{
					Rect rect = new Rect(num, num2, this.colWidths[i], this.rowHeights[j]);
					Rect rect2 = rect;
					rect2.xMin -= 999f;
					rect2.xMax += 999f;
					if (Mouse.IsOver(rect2) || i % 2 == 0)
					{
						Widgets.DrawHighlight(rect);
					}
					if (j == 0 && Mouse.IsOver(rect))
					{
						rect.x += 2f;
						rect.y += 2f;
					}
					if (i == 0 || this.colVisible[i])
					{
						Widgets.Label(rect, this.tableSorted[i, j]);
					}
					if (j == 0)
					{
						MouseoverSounds.DoRegion(rect);
						if (Mouse.IsOver(rect) && Event.current.type == EventType.MouseDown)
						{
							if (Event.current.button == 0)
							{
								if (i != this.sortColumn)
								{
									this.sortMode = Window_DebugTable.SortMode.Off;
								}
								Window_DebugTable.SortMode sortMode = this.sortMode;
								if (sortMode != Window_DebugTable.SortMode.Off)
								{
									if (sortMode != Window_DebugTable.SortMode.Descending)
									{
										if (sortMode == Window_DebugTable.SortMode.Ascending)
										{
											this.sortMode = Window_DebugTable.SortMode.Off;
											this.sortColumn = -1;
											SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
										}
									}
									else
									{
										this.sortMode = Window_DebugTable.SortMode.Ascending;
										this.sortColumn = i;
										SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
									}
								}
								else
								{
									this.sortMode = Window_DebugTable.SortMode.Descending;
									this.sortColumn = i;
									SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
								}
								this.BuildTableSorted();
							}
							else if (Event.current.button == 1)
							{
								this.colVisible[i] = !this.colVisible[i];
								SoundDefOf.Crunch.PlayOneShotOnCamera(null);
								this.BuildTableSorted();
							}
							Event.current.Use();
						}
					}
					num2 += this.rowHeights[j];
				}
				num += this.colWidths[i];
			}
			Widgets.EndScrollView();
			Rect butRect = new Rect(inRect.x + inRect.width - 44f, inRect.y + 4f, 18f, 24f);
			if (Widgets.ButtonImage(butRect, TexButton.Copy))
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int k = 0; k < this.tableSorted.GetLength(1); k++)
				{
					for (int l = 0; l < this.tableSorted.GetLength(0); l++)
					{
						if (l != 0)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(this.tableSorted[l, k]);
					}
					stringBuilder.Append("\n");
				}
				GUIUtility.systemCopyBuffer = stringBuilder.ToString();
			}
		}

		// Token: 0x02000E52 RID: 3666
		private enum SortMode
		{
			// Token: 0x0400392B RID: 14635
			Off,
			// Token: 0x0400392C RID: 14636
			Ascending,
			// Token: 0x0400392D RID: 14637
			Descending
		}

		// Token: 0x02000E53 RID: 3667
		public class NumericStringComparer : IComparer<string>
		{
			// Token: 0x0600566E RID: 22126 RVA: 0x002C949C File Offset: 0x002C789C
			public int Compare(string x, string y)
			{
				if (x.Contains("~"))
				{
					string[] array = x.Split(new char[]
					{
						'~'
					});
					if (array.Length == 2)
					{
						x = array[0];
					}
				}
				if (y.Contains("~"))
				{
					string[] array2 = y.Split(new char[]
					{
						'~'
					});
					if (array2.Length == 2)
					{
						y = array2[0];
					}
				}
				if ((x.EndsWith("%") && y.EndsWith("%")) || (x.EndsWith("C") && y.EndsWith("C")))
				{
					x = x.Substring(0, x.Length - 1);
					y = y.Substring(0, y.Length - 1);
				}
				float num;
				float value;
				int result;
				if (float.TryParse(x, out num) && float.TryParse(y, out value))
				{
					result = num.CompareTo(value);
				}
				else
				{
					result = x.CompareTo(y);
				}
				return result;
			}
		}
	}
}
