using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace JSBuild
{
	class Program
	{
		static void Main(string[] args)
		{
			String projectPath = null;
			bool displayHelp = false;

			for (int i = 0; i < args.Length; i++)
			{
				Util.CommandLine.Arg arg = Util.CommandLine.ParseArg(args[i]);

				switch (arg.Name)
				{
					case Util.CommandLine.AvailableArgs.ProjectToBuild:
						projectPath = arg.Value;
						break;

					case Util.CommandLine.AvailableArgs.DisplayHelp:
					case Util.CommandLine.AvailableArgs.DisplayHelpShort:
						displayHelp = true;
						break;
				}
			}

			if (args.Length == 0 || displayHelp)
			{
				// If the help param was supplied, ignore other params
				DisplayHelp();
			}
			else if (projectPath != null && projectPath.Length > 0)
			{
				try
				{
					Build(projectPath);
				}
				catch (Exception ex)
				{
					Console.Out.WriteLine("\nBUILD FAILED!\n" + ex.ToString());
					Wait();
				}
			}
			else
			{
				// No valid params were supplied, so show the help
				DisplayHelp();
			}
		}

		static void Build(string projectPath)
		{
			Console.Out.WriteLine("\nBuilding: " + projectPath);
			Console.Out.WriteLine();

			string appExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			System.IO.FileInfo fi = new System.IO.FileInfo(appExePath);
			appExePath = Util.FixPath(fi.DirectoryName);

			//NOTE: In order to load an existing settings.xml file for debugging, you must first
			//create one using the GUI then copy it into the directory from which this assembly
			//is running (normally jsbuildconsole\bin\debug).  Otherwise it will simply use defaults.
			Options.GetInstance().Load(appExePath);

			ProjectBuilder.MessageAvailable += new MessageDelegate(ProjectBuilder_MessageAvailable);
			ProjectBuilder.ProgressUpdate += new ProgressDelegate(ProjectBuilder_ProgressUpdate);

			Project project = Project.GetInstance();
			project.Load(appExePath, projectPath);

			ProjectBuilder.Build(project);
			Wait();
		}

		static void ProjectBuilder_ProgressUpdate(ProgressInfo progressInfo)
		{
			Console.Out.WriteLine(progressInfo.Message);
		}

		static void ProjectBuilder_MessageAvailable(Message message)
		{
			switch (message.Type)
			{
				case MessageTypes.Info:
					Console.Out.WriteLine("INFO: " + message.Text);
					break;

				case MessageTypes.Error:
					Console.Out.WriteLine("\nBUILD FAILED!\n" + message.Text);
					break;

				case MessageTypes.Status:
					Console.Out.WriteLine(message.Text);
					break;
			}
		}

		static void DisplayHelp()
		{
			Console.Out.WriteLine("\nJS Builder Console " + 
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
				" by Jack Slocum (www.jackslocum.com)");

			Console.Out.WriteLine("\nThe JS Builder Console performs silent builds of JS Builder project files.  ");
			Console.Out.WriteLine("You must supply the path to an existing .jsb file created with the JS Builder GUI application (JSBuilder.exe).");
			Console.Out.WriteLine("\nUsage: JSBuildConsole /p=[full path to .jsb project file]\n");
			Console.Out.WriteLine(@"Ex:    JSBuildConsole /p=C:\projectdir\myproject.jsb");
			Wait();
		}

		static void Wait()
		{
			#if DEBUG
			// Pause here so we can verify the output.  Only pause in debug mode
			// so we don't interfere with automated builds.
			Console.Out.WriteLine("\nPress ENTER to continue...");
			Console.ReadLine();
			#endif
		}
	}
}
