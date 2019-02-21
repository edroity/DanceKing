using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NativeBuilder
{

	public class IOSWindow :  EditorWindow
	{
		string[] mod_list = null;
		string selected_mod = null;

		bool build_xCode_project = true;
		bool apply_native_builder = true;
		bool build_ipa = false;

		string[] backup_list = null;

		string[] option_list = null;
		string selected_option = null;

		Dictionary<string, string> option_backup_mapping = new Dictionary<string, string>();

		public static void Show()
		{
			var window = EditorWindow.GetWindow<IOSWindow>("Build iOS", true) as IOSWindow;
			window.Show(true);
		}

		private void ReadInfo()
		{
			Configuration.Gloable.Repaire();
			/*
			string projectPath = Configuration.Gloable.XCode_Project_Backup_Home;
		
			if(Directory.Exists(projectPath))
			{
				this.backup_project = new XCProject(projectPath);
			}
			else
			{
				backup_project = null;
			}
			*/

			// mod
            /*
			mod_list = new string[0];
			DirectoryInfo modDir = new DirectoryInfo(Configuration.Gloable.IOSSrcDir);
			if(modDir.Exists)
			{
				var xupe_dirs = modDir.GetDirectories("*.xupe", SearchOption.TopDirectoryOnly);
				mod_list = (from eupe in xupe_dirs select eupe.Name).ToArray();
				if(string.IsNullOrEmpty(selected_mod) && mod_list.Length > 0)
				{
					selected_mod = mod_list[0];
				}
			}
			*/


			// backup
			backup_list = new string[0];
			DirectoryInfo backupHome = new DirectoryInfo(Configuration.Gloable.XCode_Project_Backup_Home);
			if(backupHome.Exists)
			{
				backup_list = (from d in backupHome.GetDirectories("*", SearchOption.TopDirectoryOnly) select d.Name).ToArray();
			}

			// option
			option_backup_mapping.Clear();
			option_backup_mapping.Add("Rebuild", null);
			foreach(string backup in backup_list)
			{
				if(backup == "autosave") option_backup_mapping.Add("Use Last Version (autosave)", backup);
				else option_backup_mapping.Add("Use '" + backup + "'", backup);
			}
			option_list = option_backup_mapping.Keys.ToArray();
			if(selected_option == null || !option_list.Contains(selected_option)) selected_option = option_list[0];

		}

		public void OnFocus()
		{
			ReadInfo();
		}

		public void OnGUI()
		{

			// build xCode
//			build_xCode_project = EditorGUILayout.Toggle("Build xCode Project", build_xCode_project);

			// option
//			if(build_xCode_project)
			{
				//build_option = (IOSBuildOption)EditorGUILayout.EnumPopup("Option", build_option);
				int selected = Array.IndexOf(option_list, selected_option);
				selected = EditorGUILayout.Popup("Use Backuped ?", selected, option_list);
				selected_option = option_list[selected];
			}

			// player name
			{
				var productName = PlayerSettings.productName;
				productName = EditorGUILayout.TextField("Product Name", productName);
				PlayerSettings.productName = productName;
			}

			// package name
			{
				var a = PlayerSettings.applicationIdentifier;
				var b = EditorGUILayout.TextField("Identifier", a);
				//PlayerSettings.applicationIdentifier = b;
				PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, b);
			}

			{
				var list = EditorBuildSettings.scenes;
				foreach (var s in list)
				{
					var enable = GUILayout.Toggle(s.enabled, s.path);
					s.enabled = enable;
				}
				EditorBuildSettings.scenes = list;
			}

			

			// backup info
			//if(this.Backup != null)
			//{
			//	// if backup_project is not current selected project, reload it.
			//	if(backup_project == null || backup_project.path != Configuration.Gloable.XCode_Project_Backup_Home + "/" + this.Backup)
			//	{
			//		backup_project = new XCProject(Configuration.Gloable.XCode_Project_Backup_Home + "/" + this.Backup);
			//	}
			//	EditorGUILayout.LabelField("Backup Built At: " + this.backup_project.BuildTime);
			//}


			//if(!build_xCode_project)
			//{
			//	apply_native_builder = false;
			//}
			
			
			//apply_native_builder = EditorGUILayout.Toggle("Apply NativeBuilder", apply_native_builder);
			//{
			//	if(apply_native_builder)
			//	{
			//		if(mod_list.Length > 0)
			//		{
			//			int selectedIndex = Array.IndexOf(mod_list, selected_mod);
			//			selectedIndex = EditorGUILayout.Popup("Xupe package", selectedIndex, mod_list);
			//			this.selected_mod = mod_list[selectedIndex];
			//		}
			//		else
			//		{
			//			EditorGUILayout.LabelField("None of .xupe package found, Can't Build.");
			//		}
			//	}
			//}

			//build_ipa = EditorGUILayout.Toggle("Build Ipa", build_ipa);
			//if(build_ipa)
			//{
			//	build_xCode_project = true;
			//	apply_native_builder = true;
			//}

			
			if( !string.IsNullOrEmpty(this.selected_option))
			{
				if(GUILayout.Button("Build"))
				{
					var task = new BuildTask_iOS(this.Backup);
					task.Build();
				}
			}

			if(GUILayout.Button("Open ProductDirectory"))
			{
				OpenProductDir();
			}
		}

		public void OpenProductDir()
		{
			//string unityProjectPath =  Application.dataPath.Remove(Application.dataPath.LastIndexOf('/'));
			var path = "NativeBuilderProduct/xCode_project";
			Debug.Log(path);
			EditorUtility.OpenWithDefaultApp(path);
		}
	
		public string Backup
		{
			get
			{
				return option_backup_mapping[selected_option];
			}
		}
	}


}

