using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JSBuild
{
	class CssFile : SourceFile
	{
		public CssFile(FileInfo file, string pathInfo) : base(file, pathInfo) { }

		public override string OutputFilename
		{
			//TODO: This is temporary -- even though CSS files do not currently get parsed,
			// we still want to return the filename with the -min suffix so that once css parsing
			// is implemented the output file handling will not have to change.  To be removed once
			// source parsing is implemented.
			get
			{
				return this.file.Name.Replace(base.file.Extension,
					Options.GetInstance().OutputSuffix + base.file.Extension);
			}
		}
	}
}
