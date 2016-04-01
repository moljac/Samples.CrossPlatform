﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeAnalysis
{
	class Program
	{
		static void Main (string[] args)
		{
			var path = Environment.CurrentDirectory;
			for (int i = 0; i < 3; i++)
			{
				path = Path.Combine(Path.GetDirectoryName(path), string.Empty);
			}
			var projects = new List<Solution> {


				new Solution {
					Name = "Android",
					ProjectFiles = new List<string> {
						Path.Combine(path, "MeetupManager.Droid/MeetupManager.Droid.csproj"),
						Path.Combine(path, "MeetupManager.Portable/MeetupManager.Portable.csproj")
					},
				},

				new Solution {
					Name = "iOS",
					ProjectFiles = new List<string> {
						Path.Combine(path, "MeetupManager.iOS/MeetupManager.iOS.csproj"),
						Path.Combine(path, "MeetupManager.Portable/MeetupManager.Portable.csproj")
					},
				},

                new Solution {
					Name = "WP",
					ProjectFiles = new List<string> {
						Path.Combine(path, "MeetupManager.WP8/MeetupManager.WP8.csproj"),
						Path.Combine(path, "MeetupManager.Portable/MeetupManager.Portable.csproj")
					},
				},

			};


			new App().Run(projects);
		}

		class Solution
		{

		class FileInfo
		{
			public string Path = "";
			public List<Solution> Solutions = new List<Solution>();
			public int LinesOfCode = 0;
			public override string ToString ()
			{
				return Path;
			}
		}

		Dictionary<string, FileInfo> _files = new Dictionary<string, FileInfo>();

		void AddRef (string path, Solution sln)
		{

			if (_files.ContainsKey(path)) {
				_files[path].Solutions.Add(sln);
				sln.CodeFiles.Add(_files[path]);
			}
			else {
				var info = new FileInfo { Path = path, };
				info.Solutions.Add(sln);
				_files[path] = info;
				sln.CodeFiles.Add(info);
			}
		}

		void Run (List<Solution> solutions)
		{
			//
			// Find all the files
			//
			foreach (var sln in solutions) {
				foreach (var projectFile in sln.ProjectFiles) {
					var dir = Path.GetDirectoryName(projectFile);
					var projectName = Path.GetFileNameWithoutExtension(projectFile);
					var doc = XDocument.Load(projectFile);
					var q = from x in doc.Descendants()
					        let e = x as XElement
						        where e != null
						        where e.Name.LocalName == "Compile"
						        where e.Attributes().Any(a => a.Name.LocalName == "Include")
					        select e.Attribute("Include").Value;
					foreach (var inc in q) {
						//skip over some things that are added automatically
						if (inc.Contains ("Resource.designer.cs") ||
						    inc.Contains ("DebugTrace.cs") ||
						    inc.Contains ("LinkerPleaseInclude.cs") ||
						    inc.Contains ("AssemblyInfo.cs") ||
							inc.Contains ("Bootstrap.cs") ||
							inc.Contains(".designer.cs") ||
                            inc.EndsWith(".xaml") ||
                            inc.EndsWith(".xml") ||
                            inc.EndsWith(".axml")) {
							continue;
						}

						var inc2 = inc.Replace ("\\", Path.DirectorySeparatorChar.ToString());
						AddRef(Path.GetFullPath(Path.Combine(dir, inc2)), sln);
					}
				}
			}

			//
			// Get the lines of code
			//
			foreach (var f in _files.Values) {
				try
				{
				    var lines = File.ReadAllLines(f.Path).ToList();

					f.LinesOfCode = lines.Count;
				}
				catch (Exception ex) {
				}                
			}

			//
			// Output
			//
			Console.WriteLine("app\tt\tu\ts\tu%\ts%");
			foreach (var sln in solutions) {

				Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4:p}\t{5:p}",
					sln.Name,
					sln.TotalLinesOfCode,
					sln.UniqueLinesOfCode,
					sln.SharedLinesOfCode,
					sln.UniqueLinesOfCode / (double)sln.TotalLinesOfCode,
					sln.SharedLinesOfCode / (double)sln.TotalLinesOfCode);
			}

			Console.WriteLine("DONE");
		}
	}
}
