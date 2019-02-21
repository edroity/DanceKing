using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using NativeBuilder;

namespace NativeBuilder
{
	public class BuildTask_iOS : BuildTask 
	{
		private enum TaskList
		{
			BuildXCodeProject,
			ApplyNativeBuilder,
			BuildIpa
		}

		string BackupName;
		string modName;

		public BuildTask_iOS(string backupName)
		{
			this.BackupName = backupName;
		}

		private bool UseBackup
		{
			get
			{
				return !string.IsNullOrEmpty(this.BackupName);
			}
		}
		

		Conf_Gloable global = null;
		string modPath = null;
		string xCodePath = null;

		public override void OnPreBuild ()
		{
			this.global = Configuration.Gloable;
			this.global.Repaire();
			// get my xupe file path
			//this.modPath = gloabal["ios.conf"];
            if (!string.IsNullOrEmpty(this.modName))
            {
                this.modPath = Path.Combine(this.global.IOSSrcDir, this.modName);
            }
			// set a target xCode path 
			this.xCodePath = global["ios.project"];
			// export!
			//NativeBuilderCore.ExportIOS (xCodePath, modPath);
			
			if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
			{
				throw new Exception("Current platform must be iOS! (now is " + EditorUserBuildSettings.activeBuildTarget + ")");
			}
			//check mod exist
			{
                if (!string.IsNullOrEmpty(this.modName))
                {
                    if (!(new DirectoryInfo(modPath).Exists))
                    {
                        throw new Exception("xupe package not exists! (" + modPath + ")");
                    }
                }
			}
		}
		
		public override void OnBuild ()
		{
			Debug.Log("Unity building xCode..."); 

			// check parent dir exsits
			DirectoryInfo directory =  new DirectoryInfo(xCodePath);
			var parent = directory.Parent;
			if(!parent.Exists){
				parent.Create();
			}
			if(directory.Exists)
			{
				directory.Delete(true);
			}

		
			if(!this.UseBackup)
			{
				// build
				NativeBuilderUtility.Build(this.xCodePath, UnityEditor.BuildTarget.iOS, UnityEditor.BuildOptions.None);

				//back up pure xCode Project
				PShellUtil.CopyTo(this.xCodePath, this.global.XCode_Project_Backup_Home + "/autosave", PShellUtil.FileExsitsOption.Override, PShellUtil.DirectoryExsitsOption.Override);
			}
			else
			{
				// use last version

				PShellUtil.CopyTo(Path.Combine(this.global.XCode_Project_Backup_Home, this.BackupName), this.xCodePath, PShellUtil.FileExsitsOption.Override, PShellUtil.DirectoryExsitsOption.Override);
			}	
		}
	}
}

