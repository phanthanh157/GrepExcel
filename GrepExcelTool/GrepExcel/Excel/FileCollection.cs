using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GrepExcel.Excel
{
    public class FileCollection : IEnumerable
    {
        private List<string> _files = new List<string>();
        private TypeMethod _typeMethod;
        private int _maxFile = int.Parse(Config.ReadSetting("MAX_FILE"));
        public FileCollection(string path, TypeMethod method)
        {
            _typeMethod = method;
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                GetFiles(path);
        }

        private void GetFiles(string path)
        {
            ShowDebug.Msg(F.FLMD(), " Get List File");

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
                ShowDebug.Msg(F.FLMD(), "{0} is not a valid file or directory.", path);
            }

        }

        private void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            if (TypeMethod.SubFolder == _typeMethod)
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
            if (ext == Define.EXTENSION_FILE_XLSM || ext == Define.EXTENSION_FILE_XLSX || ext == Define.EXTENSION_FILE_XLS)
            {
                //kiem soat so luong file them vao
                if (_maxFile > _files.Count)
                {
                    _files.Add(path);
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _files.GetEnumerator();
        }
    }
}
