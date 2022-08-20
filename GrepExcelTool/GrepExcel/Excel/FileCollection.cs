using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GrepExcel.Excel
{
    public class FileCollection : IEnumerable
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        private List<string> files_ = new List<string>();
        private TypeMethod typeMode_;
        private int maxFile_ = int.Parse(Config.ReadSetting("MAX_FILE"));
        public FileCollection(string path, TypeMethod method)
        {
            typeMode_ = method;
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                GetFiles(path);
        }

        private void GetFiles(string path)
        {
            if (File.Exists(path))
            {
                // This path is a file
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                // This path is a directory
                ProcessDirectory(path);
            }
            else
            {
               log_.DebugFormat("{0} is not a valid file or directory.", path);
            }
        }

        private void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            if (TypeMethod.SubFolder == typeMode_)
            {
                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectory(subdirectory);
            }
        }

        private void ProcessFile(string path)
        {
            // Show.Msg(F.FLMD(), "Processed file '{0}'.", path);
            string ext = Path.GetExtension(path);
            if (ext == Define.EXTENSION_FILE_XLSM ||
                ext == Define.EXTENSION_FILE_XLSX ||
                ext == Define.EXTENSION_FILE_XLS)
            {
                //kiem soat so luong file them vao
                if (maxFile_ > files_.Count)
                {
                    files_.Add(path);
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return files_.GetEnumerator();
        }
    }
}
