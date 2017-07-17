using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderMove
{
    internal class Program
    {
        public static void MoveFolder(string folder, string path)
        {
            var FolderName = System.IO.Path.GetFileName(folder);
            var FolderSpaceIndex = FolderName.LastIndexOf(' ');
            if (FolderSpaceIndex != -1)
            {
                var Rest = FolderName.Substring(Math.Min(FolderName.Length, FolderSpaceIndex + 1));
                int test;
                if (int.TryParse(Rest, out test))
                    FolderName = FolderName.Substring(0, FolderSpaceIndex);
            }

            var FolderNumbers = System.IO.Directory.EnumerateDirectories(path)
                .Select(x => System.IO.Path.GetFileName(x).ToLower())
            .Where(x => x.Contains(FolderName.ToLower()) && x.Replace(FolderName.ToLower(), "").AsEnumerable().All(y => Char.IsDigit(y) || y == ' '))
            .Select(x =>
            {
                var SpaceIndex = x.LastIndexOf(' ');
                if (SpaceIndex == -1)
                    return 0;
                int result;
                int.TryParse(x.Substring(SpaceIndex), out result);
                return result;
            })
            .ToArray();

            var NextNumber = Enumerable.Range(1, FolderNumbers.Length + 1).Except(FolderNumbers).Min();

            var Suffix = FolderNumbers.Any() ? " " + NextNumber.ToString().PadLeft(3, '0') : "";

            Directory.Move(folder, path + "\\" + FolderName + Suffix);
        }

        private static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine(@"foldermove ""source path"" ""destination parent path"" ""hide""");
                return;
            }

            var Hide = args.Length == 3 && args[2] == "hide";

            var SourcePath = args[0].TrimEnd(new[] { '\\' });
            var DestinationPath = args[1].TrimEnd(new[] { '\\' });

            var Folder = Directory.GetDirectories(SourcePath)
                .OrderBy(x => Directory.GetLastWriteTime(x)).FirstOrDefault();

            if (Folder != null)
            {
                var di = new DirectoryInfo(Folder);
                di.Attributes &= Hide ? FileAttributes.Hidden : ~FileAttributes.Hidden;

                MoveFolder(Folder, DestinationPath);
            }
            else
                Console.WriteLine("There was no subfolder found in " + SourcePath);
        }
    }
}