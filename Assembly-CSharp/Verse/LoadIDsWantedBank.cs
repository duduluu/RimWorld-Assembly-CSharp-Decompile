﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Verse
{
	// Token: 0x02000D98 RID: 3480
	public class LoadIDsWantedBank
	{
		// Token: 0x040033ED RID: 13293
		private List<LoadIDsWantedBank.IdRecord> idsRead = new List<LoadIDsWantedBank.IdRecord>();

		// Token: 0x040033EE RID: 13294
		private List<LoadIDsWantedBank.IdListRecord> idListsRead = new List<LoadIDsWantedBank.IdListRecord>();

		// Token: 0x06004DCA RID: 19914 RVA: 0x0028A208 File Offset: 0x00288608
		public void ConfirmClear()
		{
			if (this.idsRead.Count > 0 || this.idListsRead.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Not all loadIDs which were read were consumed.");
				if (this.idsRead.Count > 0)
				{
					stringBuilder.AppendLine("Singles:");
					for (int i = 0; i < this.idsRead.Count; i++)
					{
						stringBuilder.AppendLine(string.Concat(new object[]
						{
							"  ",
							this.idsRead[i].targetLoadID.ToStringSafe<string>(),
							" of type ",
							this.idsRead[i].targetType,
							". pathRelToParent=",
							this.idsRead[i].pathRelToParent,
							", parent=",
							this.idsRead[i].parent.ToStringSafe<IExposable>()
						}));
					}
				}
				if (this.idListsRead.Count > 0)
				{
					stringBuilder.AppendLine("Lists:");
					for (int j = 0; j < this.idListsRead.Count; j++)
					{
						stringBuilder.AppendLine(string.Concat(new object[]
						{
							"  List with ",
							(this.idListsRead[j].targetLoadIDs == null) ? 0 : this.idListsRead[j].targetLoadIDs.Count,
							" elements. pathRelToParent=",
							this.idListsRead[j].pathRelToParent,
							", parent=",
							this.idListsRead[j].parent.ToStringSafe<IExposable>()
						}));
					}
				}
				Log.Warning(stringBuilder.ToString().TrimEndNewlines(), false);
			}
			this.Clear();
		}

		// Token: 0x06004DCB RID: 19915 RVA: 0x0028A420 File Offset: 0x00288820
		public void Clear()
		{
			this.idsRead.Clear();
			this.idListsRead.Clear();
		}

		// Token: 0x06004DCC RID: 19916 RVA: 0x0028A43C File Offset: 0x0028883C
		public void RegisterLoadIDReadFromXml(string targetLoadID, Type targetType, string pathRelToParent, IExposable parent)
		{
			for (int i = 0; i < this.idsRead.Count; i++)
			{
				if (this.idsRead[i].parent == parent && this.idsRead[i].pathRelToParent == pathRelToParent)
				{
					Log.Error(string.Concat(new string[]
					{
						"Tried to register the same load ID twice: ",
						targetLoadID,
						", pathRelToParent=",
						pathRelToParent,
						", parent=",
						parent.ToStringSafe<IExposable>()
					}), false);
					return;
				}
			}
			this.idsRead.Add(new LoadIDsWantedBank.IdRecord(targetLoadID, targetType, pathRelToParent, parent));
		}

		// Token: 0x06004DCD RID: 19917 RVA: 0x0028A4F8 File Offset: 0x002888F8
		public void RegisterLoadIDReadFromXml(string targetLoadID, Type targetType, string toAppendToPathRelToParent)
		{
			string text = Scribe.loader.curPathRelToParent;
			if (!toAppendToPathRelToParent.NullOrEmpty())
			{
				text = text + '/' + toAppendToPathRelToParent;
			}
			this.RegisterLoadIDReadFromXml(targetLoadID, targetType, text, Scribe.loader.curParent);
		}

		// Token: 0x06004DCE RID: 19918 RVA: 0x0028A540 File Offset: 0x00288940
		public void RegisterLoadIDListReadFromXml(List<string> targetLoadIDList, string pathRelToParent, IExposable parent)
		{
			for (int i = 0; i < this.idListsRead.Count; i++)
			{
				if (this.idListsRead[i].parent == parent && this.idListsRead[i].pathRelToParent == pathRelToParent)
				{
					Log.Error("Tried to register the same list of load IDs twice. pathRelToParent=" + pathRelToParent + ", parent=" + parent.ToStringSafe<IExposable>(), false);
					return;
				}
			}
			this.idListsRead.Add(new LoadIDsWantedBank.IdListRecord(targetLoadIDList, pathRelToParent, parent));
		}

		// Token: 0x06004DCF RID: 19919 RVA: 0x0028A5DC File Offset: 0x002889DC
		public void RegisterLoadIDListReadFromXml(List<string> targetLoadIDList, string toAppendToPathRelToParent)
		{
			string text = Scribe.loader.curPathRelToParent;
			if (!toAppendToPathRelToParent.NullOrEmpty())
			{
				text = text + '/' + toAppendToPathRelToParent;
			}
			this.RegisterLoadIDListReadFromXml(targetLoadIDList, text, Scribe.loader.curParent);
		}

		// Token: 0x06004DD0 RID: 19920 RVA: 0x0028A624 File Offset: 0x00288A24
		public string Take<T>(string pathRelToParent, IExposable parent)
		{
			for (int i = 0; i < this.idsRead.Count; i++)
			{
				if (this.idsRead[i].parent == parent && this.idsRead[i].pathRelToParent == pathRelToParent)
				{
					string targetLoadID = this.idsRead[i].targetLoadID;
					if (typeof(T) != this.idsRead[i].targetType)
					{
						Log.Error(string.Concat(new object[]
						{
							"Trying to get load ID of object of type ",
							typeof(T),
							", but it was registered as ",
							this.idsRead[i].targetType,
							". pathRelToParent=",
							pathRelToParent,
							", parent=",
							parent.ToStringSafe<IExposable>()
						}), false);
					}
					this.idsRead.RemoveAt(i);
					return targetLoadID;
				}
			}
			Log.Error("Could not get load ID. We're asking for something which was never added during LoadingVars. pathRelToParent=" + pathRelToParent + ", parent=" + parent.ToStringSafe<IExposable>(), false);
			return null;
		}

		// Token: 0x06004DD1 RID: 19921 RVA: 0x0028A764 File Offset: 0x00288B64
		public List<string> TakeList(string pathRelToParent, IExposable parent)
		{
			for (int i = 0; i < this.idListsRead.Count; i++)
			{
				if (this.idListsRead[i].parent == parent && this.idListsRead[i].pathRelToParent == pathRelToParent)
				{
					List<string> targetLoadIDs = this.idListsRead[i].targetLoadIDs;
					this.idListsRead.RemoveAt(i);
					return targetLoadIDs;
				}
			}
			Log.Error("Could not get load IDs list. We're asking for something which was never added during LoadingVars. pathRelToParent=" + pathRelToParent + ", parent=" + parent.ToStringSafe<IExposable>(), false);
			return new List<string>();
		}

		// Token: 0x02000D99 RID: 3481
		private struct IdRecord
		{
			// Token: 0x040033EF RID: 13295
			public string targetLoadID;

			// Token: 0x040033F0 RID: 13296
			public Type targetType;

			// Token: 0x040033F1 RID: 13297
			public string pathRelToParent;

			// Token: 0x040033F2 RID: 13298
			public IExposable parent;

			// Token: 0x06004DD2 RID: 19922 RVA: 0x0028A81E File Offset: 0x00288C1E
			public IdRecord(string targetLoadID, Type targetType, string pathRelToParent, IExposable parent)
			{
				this.targetLoadID = targetLoadID;
				this.targetType = targetType;
				this.pathRelToParent = pathRelToParent;
				this.parent = parent;
			}
		}

		// Token: 0x02000D9A RID: 3482
		private struct IdListRecord
		{
			// Token: 0x040033F3 RID: 13299
			public List<string> targetLoadIDs;

			// Token: 0x040033F4 RID: 13300
			public string pathRelToParent;

			// Token: 0x040033F5 RID: 13301
			public IExposable parent;

			// Token: 0x06004DD3 RID: 19923 RVA: 0x0028A83E File Offset: 0x00288C3E
			public IdListRecord(List<string> targetLoadIDs, string pathRelToParent, IExposable parent)
			{
				this.targetLoadIDs = targetLoadIDs;
				this.pathRelToParent = pathRelToParent;
				this.parent = parent;
			}
		}
	}
}
