using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeAnalysis
{
	class Solution
	{
			public string Name = "";
			public List<string> ProjectFiles = new List<string>();
			public List<FileInfo> CodeFiles = new List<FileInfo>();
			public override string ToString ()
			{
				return Name;
			}

			public int UniqueLinesOfCode
			{
				get
				{
					return (from f in CodeFiles
						where f.Solutions.Count == 1
						select f.LinesOfCode).Sum();
				}
			}

			public int SharedLinesOfCode
			{
				get
				{
					return (from f in CodeFiles
						where f.Solutions.Count > 1
						select f.LinesOfCode).Sum();
				}
			}

			public int TotalLinesOfCode
			{
				get
				{
					return (from f in CodeFiles
						select f.LinesOfCode).Sum();
				}
			}
		}
	}
}
