﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000BF6 RID: 3062
	public class LoadedLanguage
	{
		// Token: 0x060042E0 RID: 17120 RVA: 0x002364E0 File Offset: 0x002348E0
		public LoadedLanguage(string folderPath)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
			this.folderName = directoryInfo.Name;
		}

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x060042E1 RID: 17121 RVA: 0x00236564 File Offset: 0x00234964
		public string FriendlyNameNative
		{
			get
			{
				string friendlyNameNative;
				if (this.info == null || this.info.friendlyNameNative.NullOrEmpty())
				{
					friendlyNameNative = this.folderName;
				}
				else
				{
					friendlyNameNative = this.info.friendlyNameNative;
				}
				return friendlyNameNative;
			}
		}

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x060042E2 RID: 17122 RVA: 0x002365B0 File Offset: 0x002349B0
		public string FriendlyNameEnglish
		{
			get
			{
				string friendlyNameEnglish;
				if (this.info == null || this.info.friendlyNameEnglish.NullOrEmpty())
				{
					friendlyNameEnglish = this.folderName;
				}
				else
				{
					friendlyNameEnglish = this.info.friendlyNameEnglish;
				}
				return friendlyNameEnglish;
			}
		}

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x060042E3 RID: 17123 RVA: 0x002365FC File Offset: 0x002349FC
		public IEnumerable<string> FolderPaths
		{
			get
			{
				foreach (ModContentPack mod in LoadedModManager.RunningMods)
				{
					string langDirPath = Path.Combine(mod.RootDir, "Languages");
					string myDirPath = Path.Combine(langDirPath, this.folderName);
					DirectoryInfo myDir = new DirectoryInfo(myDirPath);
					if (myDir.Exists)
					{
						yield return myDirPath;
					}
				}
				yield break;
			}
		}

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x060042E4 RID: 17124 RVA: 0x00236628 File Offset: 0x00234A28
		public LanguageWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (LanguageWorker)Activator.CreateInstance(this.info.languageWorkerClass);
				}
				return this.workerInt;
			}
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x0023666C File Offset: 0x00234A6C
		public void TryLoadMetadataFrom(string folderPath)
		{
			if (this.info == null)
			{
				string filePath = Path.Combine(folderPath.ToString(), "LanguageInfo.xml");
				this.info = DirectXmlLoader.ItemFromXmlFile<LanguageInfo>(filePath, false);
				if (this.info.friendlyNameNative.NullOrEmpty())
				{
					FileInfo fileInfo = new FileInfo(Path.Combine(folderPath.ToString(), "FriendlyName.txt"));
					if (fileInfo.Exists)
					{
						this.info.friendlyNameNative = GenFile.TextFromRawFile(fileInfo.ToString());
					}
				}
				if (this.info.friendlyNameNative.NullOrEmpty())
				{
					this.info.friendlyNameNative = this.folderName;
				}
				if (this.info.friendlyNameEnglish.NullOrEmpty())
				{
					this.info.friendlyNameEnglish = this.folderName;
				}
			}
		}

		// Token: 0x060042E6 RID: 17126 RVA: 0x00236740 File Offset: 0x00234B40
		public void LoadData()
		{
			if (!this.dataIsLoaded)
			{
				this.dataIsLoaded = true;
				DeepProfiler.Start("Loading language data: " + this.folderName);
				foreach (string text in this.FolderPaths)
				{
					string localFolderPath = text;
					LongEventHandler.ExecuteWhenFinished(delegate
					{
						if (this.icon == BaseContent.BadTex)
						{
							FileInfo fileInfo = new FileInfo(Path.Combine(localFolderPath.ToString(), "LangIcon.png"));
							if (fileInfo.Exists)
							{
								this.icon = ModContentLoader<Texture2D>.LoadItem(fileInfo.FullName, null).contentItem;
							}
						}
					});
					DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(text.ToString(), "CodeLinked"));
					if (directoryInfo.Exists)
					{
						this.loadErrors.Add("Translations aren't called CodeLinked any more. Please rename to Keyed: " + directoryInfo);
					}
					else
					{
						directoryInfo = new DirectoryInfo(Path.Combine(text.ToString(), "Keyed"));
					}
					if (directoryInfo.Exists)
					{
						foreach (FileInfo file in directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories))
						{
							this.LoadFromFile_Keyed(file);
						}
					}
					DirectoryInfo directoryInfo2 = new DirectoryInfo(Path.Combine(text.ToString(), "DefLinked"));
					if (directoryInfo2.Exists)
					{
						this.loadErrors.Add("Translations aren't called DefLinked any more. Please rename to DefInjected: " + directoryInfo2);
					}
					else
					{
						directoryInfo2 = new DirectoryInfo(Path.Combine(text.ToString(), "DefInjected"));
					}
					if (directoryInfo2.Exists)
					{
						foreach (DirectoryInfo directoryInfo3 in directoryInfo2.GetDirectories("*", SearchOption.TopDirectoryOnly))
						{
							string name = directoryInfo3.Name;
							Type typeInAnyAssembly = GenTypes.GetTypeInAnyAssembly(name);
							if (typeInAnyAssembly == null && name.Length > 3)
							{
								typeInAnyAssembly = GenTypes.GetTypeInAnyAssembly(name.Substring(0, name.Length - 1));
							}
							if (typeInAnyAssembly == null)
							{
								this.loadErrors.Add(string.Concat(new string[]
								{
									"Error loading language from ",
									text,
									": dir ",
									directoryInfo3.Name,
									" doesn't correspond to any def type. Skipping..."
								}));
							}
							else
							{
								foreach (FileInfo file2 in directoryInfo3.GetFiles("*.xml", SearchOption.AllDirectories))
								{
									this.LoadFromFile_DefInject(file2, typeInAnyAssembly);
								}
							}
						}
					}
					DirectoryInfo directoryInfo4 = new DirectoryInfo(Path.Combine(text.ToString(), "Strings"));
					if (directoryInfo4.Exists)
					{
						foreach (DirectoryInfo directoryInfo5 in directoryInfo4.GetDirectories("*", SearchOption.TopDirectoryOnly))
						{
							foreach (FileInfo file3 in directoryInfo5.GetFiles("*.txt", SearchOption.AllDirectories))
							{
								this.LoadFromFile_Strings(file3, directoryInfo4);
							}
						}
					}
				}
				DeepProfiler.End();
			}
		}

		// Token: 0x060042E7 RID: 17127 RVA: 0x00236A84 File Offset: 0x00234E84
		private void LoadFromFile_Strings(FileInfo file, DirectoryInfo stringsTopDir)
		{
			string text;
			try
			{
				text = GenFile.TextFromRawFile(file.FullName);
			}
			catch (Exception ex)
			{
				this.loadErrors.Add(string.Concat(new object[]
				{
					"Exception loading from strings file ",
					file,
					": ",
					ex
				}));
				return;
			}
			string text2 = file.FullName;
			if (stringsTopDir != null)
			{
				text2 = text2.Substring(stringsTopDir.FullName.Length + 1);
			}
			text2 = text2.Substring(0, text2.Length - Path.GetExtension(text2).Length);
			text2 = text2.Replace('\\', '/');
			List<string> list = new List<string>();
			foreach (string item in GenText.LinesFromString(text))
			{
				list.Add(item);
			}
			List<string> list2;
			if (this.stringFiles.TryGetValue(text2, out list2))
			{
				foreach (string item2 in list)
				{
					list2.Add(item2);
				}
			}
			else
			{
				this.stringFiles.Add(text2, list);
			}
		}

		// Token: 0x060042E8 RID: 17128 RVA: 0x00236C00 File Offset: 0x00235000
		private void LoadFromFile_Keyed(FileInfo file)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			try
			{
				foreach (DirectXmlLoaderSimple.XmlKeyValuePair xmlKeyValuePair in DirectXmlLoaderSimple.ValuesFromXmlFile(file))
				{
					if (this.keyedReplacements.ContainsKey(xmlKeyValuePair.key) || dictionary.ContainsKey(xmlKeyValuePair.key))
					{
						this.loadErrors.Add("Duplicate keyed translation key: " + xmlKeyValuePair.key + " in language " + this.folderName);
					}
					else
					{
						dictionary.Add(xmlKeyValuePair.key, xmlKeyValuePair.value);
						dictionary2.Add(xmlKeyValuePair.key, xmlKeyValuePair.lineNumber);
					}
				}
			}
			catch (Exception ex)
			{
				this.loadErrors.Add(string.Concat(new object[]
				{
					"Exception loading from translation file ",
					file,
					": ",
					ex
				}));
				dictionary.Clear();
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				this.keyedReplacements.Add(keyValuePair.Key, keyValuePair.Value);
				this.keyedReplacementsFileSource.Add(keyValuePair.Key, new Pair<string, int>(file.Name, dictionary2[keyValuePair.Key]));
			}
		}

		// Token: 0x060042E9 RID: 17129 RVA: 0x00236DB8 File Offset: 0x002351B8
		public void LoadFromFile_DefInject(FileInfo file, Type defType)
		{
			DefInjectionPackage defInjectionPackage = (from di in this.defInjections
			where di.defType == defType
			select di).FirstOrDefault<DefInjectionPackage>();
			if (defInjectionPackage == null)
			{
				defInjectionPackage = new DefInjectionPackage(defType);
				this.defInjections.Add(defInjectionPackage);
			}
			defInjectionPackage.AddDataFromFile(file);
		}

		// Token: 0x060042EA RID: 17130 RVA: 0x00236E18 File Offset: 0x00235218
		public bool HaveTextForKey(string key)
		{
			if (!this.dataIsLoaded)
			{
				this.LoadData();
			}
			return this.keyedReplacements.ContainsKey(key);
		}

		// Token: 0x060042EB RID: 17131 RVA: 0x00236E4C File Offset: 0x0023524C
		public bool TryGetTextFromKey(string key, out string translated)
		{
			if (!this.dataIsLoaded)
			{
				this.LoadData();
			}
			bool result;
			if (!this.keyedReplacements.TryGetValue(key, out translated))
			{
				translated = key;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060042EC RID: 17132 RVA: 0x00236E90 File Offset: 0x00235290
		public bool TryGetStringsFromFile(string fileName, out List<string> stringsList)
		{
			if (!this.dataIsLoaded)
			{
				this.LoadData();
			}
			bool result;
			if (!this.stringFiles.TryGetValue(fileName, out stringsList))
			{
				stringsList = null;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060042ED RID: 17133 RVA: 0x00236ED4 File Offset: 0x002352D4
		public string GetKeySourceFileAndLine(string key)
		{
			Pair<string, int> pair;
			string result;
			if (!this.keyedReplacementsFileSource.TryGetValue(key, out pair))
			{
				result = "unknown";
			}
			else
			{
				result = pair.First + ":" + pair.Second;
			}
			return result;
		}

		// Token: 0x060042EE RID: 17134 RVA: 0x00236F24 File Offset: 0x00235324
		public void InjectIntoData_BeforeImpliedDefs()
		{
			if (!this.dataIsLoaded)
			{
				this.LoadData();
			}
			foreach (DefInjectionPackage defInjectionPackage in this.defInjections)
			{
				try
				{
					defInjectionPackage.InjectIntoDefs(false);
				}
				catch (Exception arg)
				{
					Log.Error("Critical error while injecting translations into defs: " + arg, false);
				}
			}
		}

		// Token: 0x060042EF RID: 17135 RVA: 0x00236FC4 File Offset: 0x002353C4
		public void InjectIntoData_AfterImpliedDefs()
		{
			if (!this.dataIsLoaded)
			{
				this.LoadData();
			}
			int num = this.loadErrors.Count;
			foreach (DefInjectionPackage defInjectionPackage in this.defInjections)
			{
				try
				{
					defInjectionPackage.InjectIntoDefs(true);
					num += defInjectionPackage.loadErrors.Count;
				}
				catch (Exception arg)
				{
					Log.Error("Critical error while injecting translations into defs: " + arg, false);
				}
			}
			BackstoryTranslationUtility.LoadAndInjectBackstoryData(this.FolderPaths, this.backstoriesLoadErrors);
			num += this.backstoriesLoadErrors.Count;
			if (num != 0)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Translation data for language ",
					LanguageDatabase.activeLanguage.FriendlyNameEnglish,
					" has ",
					num,
					" errors. Generate translation report for more info."
				}), false);
			}
		}

		// Token: 0x060042F0 RID: 17136 RVA: 0x002370E0 File Offset: 0x002354E0
		public override string ToString()
		{
			return this.info.friendlyNameEnglish;
		}

		// Token: 0x04002DAE RID: 11694
		public string folderName;

		// Token: 0x04002DAF RID: 11695
		public LanguageInfo info = null;

		// Token: 0x04002DB0 RID: 11696
		private LanguageWorker workerInt;

		// Token: 0x04002DB1 RID: 11697
		private bool dataIsLoaded = false;

		// Token: 0x04002DB2 RID: 11698
		public List<string> loadErrors = new List<string>();

		// Token: 0x04002DB3 RID: 11699
		public List<string> backstoriesLoadErrors = new List<string>();

		// Token: 0x04002DB4 RID: 11700
		public Texture2D icon = BaseContent.BadTex;

		// Token: 0x04002DB5 RID: 11701
		public Dictionary<string, string> keyedReplacements = new Dictionary<string, string>();

		// Token: 0x04002DB6 RID: 11702
		public Dictionary<string, Pair<string, int>> keyedReplacementsFileSource = new Dictionary<string, Pair<string, int>>();

		// Token: 0x04002DB7 RID: 11703
		public List<DefInjectionPackage> defInjections = new List<DefInjectionPackage>();

		// Token: 0x04002DB8 RID: 11704
		public Dictionary<string, List<string>> stringFiles = new Dictionary<string, List<string>>();
	}
}
