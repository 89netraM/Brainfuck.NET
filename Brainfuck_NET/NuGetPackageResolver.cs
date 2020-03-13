using NuGet.Common;
using System.IO;
using System.Linq;

namespace Brainfuck_NET
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

				return null;
			}
			catch (DirectoryNotFoundException)
			{
				return null;
			}
		}
	}
}