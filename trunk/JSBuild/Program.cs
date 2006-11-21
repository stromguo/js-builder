using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JSBuild
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            String startProject = null;

			for (int i = 0; i < args.Length; i++)
			{
				Util.CommandLine.Arg arg = Util.CommandLine.ParseArg(args[i]);
				
				switch (arg.Name)
				{
					case Util.CommandLine.AvailableArgs.ProjectPath:
						startProject = arg.Value;
						break;
				}
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm(startProject));
        }
    }
}