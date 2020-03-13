using NuGet.Common;
using System;
using System.IO;
using System.Linq;

namespace BrainfuckNET
{
	static class NuGetPackageResolver
	{
		private static readonly string nuGetHome = Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome), "packages");

		public static string GetLatestsPath(string packageId)
		{
			try
			{
				string packageHome = Path.Combine(nuGetHome, packageId);
				foreach (string versionPath in Directory.GetDirectories(packageHome).OrderByDescending(x => x))
				{
					try
					{
						string @ref = Path.Combine(versionPath, "ref");
						foreach (string framworkVersion in Directory.GetDirectories(@ref).Where(p => Path.GetFileName(p).StartsWith("netstandard")).OrderByDescending(x => x))
						{
							string libPath = Path.Combine(framworkVersion, packageId + ".dll");
							if (File.Exists(libPath))
							{
								return libPath;
							}
						}
					}
					catch (DirectoryNotFoundException) { }
				}
			}
			catch (DirectoryNotFoundException) { }

			throw new Exception($"Could not find a netstandard version of {packageId}, please download it with NuGet.");
		}
	}
}