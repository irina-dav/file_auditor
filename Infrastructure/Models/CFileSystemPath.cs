using System;
using System.IO;

namespace Infrastructure.Models
{
    public class CFileSystemPath
    {
        public CFileSystemPath(string path)
        {
            SourcePath = path;
        }

        public string SourcePath { get; }

        public string CleanedPath
        {
            get
            {
                if (SourcePath != null)
                    return Path.GetFullPath(SourcePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return "";
            }
        }

        public override string ToString()
        {
            return CleanedPath;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            CFileSystemPath fsPath = (CFileSystemPath) obj;
            return string.Equals(CleanedPath, fsPath.CleanedPath, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => new { SourcePath, CleanedPath }.GetHashCode();
    }
}
