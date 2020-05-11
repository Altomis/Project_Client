using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DeamonClient
{
    public class Backup
    {
        string sourcePath = @"Z:\TestSource\text.txt";
        string desinationPath = @"Z:\TestDestination\DestText.txt";
        

        public void CopyFile()
        {           
            FileInfo fi = new FileInfo(sourcePath);
            string fileName = System.IO.Path.GetFileName(sourcePath);
            fi.CopyTo(@"Z:\TestDestination\" + fileName);
        }

        public void CopyFolder(string source, string destination, bool copySubdirs)
        {
            DirectoryInfo dir = new DirectoryInfo(source);

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination); 
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = System.IO.Path.Combine(destination, file.Name);
                file.CopyTo(tempPath, true);
            }

            if (copySubdirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = System.IO.Path.Combine(destination, subdir.Name);
                    CopyFolder(subdir.FullName, tempPath, copySubdirs);
                }
            }            
        }
    }
}

