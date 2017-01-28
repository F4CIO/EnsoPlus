using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Common
{
    public class SyncTwoFolders
    {
        private static Dictionary<string, string> retiredFileNames = new Dictionary<string,string>();
        private static Dictionary<string, string> processedFileNames = new Dictionary<string,string>();

        public static List<Exception> Sync(string folder1, string folder1RetiredFilesFolder, string folder2, string folder2RetiredFilesFolder)
        {
            List<Exception> errors = new List<Exception>();

            try
            {
                //add retired file names to dict
                foreach (string folder1RetiredFilePath in Directory.GetFiles(folder1RetiredFilesFolder))
                {
                    try
                    {
                        retiredFileNames[Path.GetFileName(folder1RetiredFilePath)] = folder1RetiredFilePath;
                    }
                    catch (Exception exception)
                    {
                        errors.Add(exception);
                    }
                }

                foreach (string folder2RetiredFilePath in Directory.GetFiles(folder2RetiredFilesFolder))
                {
                    try
                    {
                        retiredFileNames[Path.GetFileName(folder2RetiredFilePath)] = folder2RetiredFilePath;
                    }
                    catch (Exception exception)
                    {
                        errors.Add(exception);
                    }
                }

                //process first and then second folder
                foreach (string folder1FilePath in Directory.GetFiles(folder1))
                {
                    try
                    {
                        ProcessFileName(Path.GetFileName(folder1FilePath), folder1, folder1RetiredFilesFolder, folder2, folder2RetiredFilesFolder);
                    }
                    catch (Exception exception)
                    {
                        errors.Add(exception);
                    }
                }

                foreach (string folder2FilePath in Directory.GetFiles(folder2))
                {
                    try
                    {
                        ProcessFileName(Path.GetFileName(folder2FilePath), folder1, folder1RetiredFilesFolder, folder2, folder2RetiredFilesFolder);
                    }
                    catch (Exception exception)
                    {
                        errors.Add(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                errors.Add(exception);
            }
            finally
            {
                            retiredFileNames.Clear();
            processedFileNames.Clear();
            }
            return errors;
        }

        private static void ProcessFileName(string fileName, string folder1, string folder1RetiredFilesFolder, string folder2, string folder2RetiredFilesFolder)
        {
            if (!processedFileNames.ContainsKey(fileName))
            {
                string filePath1 = Path.Combine(folder1, fileName);
                string filePath2 = Path.Combine(folder2, fileName);

                if (retiredFileNames.ContainsKey(fileName))
                {//file is retired - create empty files in retired folders if neded and delete on both sides
                    string retiredFilePath1 = Path.Combine(folder1RetiredFilesFolder, fileName);
                    string retiredFilePath2 = Path.Combine(folder2RetiredFilesFolder, fileName);
                    if (!File.Exists(retiredFilePath1)) File.Create(retiredFilePath1);
                    if (!File.Exists(retiredFilePath2)) File.Create(retiredFilePath2);

                    if (File.Exists(filePath1)) File.Delete(filePath1);
                    if (File.Exists(filePath2)) File.Delete(filePath2);
                }
                else
                {//file not retired
                    if (File.Exists(filePath1) && !File.Exists(filePath2))
                    {//only left is present - copy it to right
                        File.Copy(filePath1, filePath2);
                    }
                    else if (!File.Exists(filePath1) && File.Exists(filePath2))
                    {//only right is present - copy it to left
                        File.Copy(filePath2, filePath1);
                    }
                    else if (File.Exists(filePath1) && File.Exists(filePath2))
                    {//both present - compare modification time
                        int lastWriteTimeCompareResult = DateTime.Compare(File.GetLastWriteTimeUtc(filePath1), File.GetLastWriteTimeUtc(filePath2));
                        if (lastWriteTimeCompareResult > 0)
                        {//left is newer - overwrite right
                            File.Copy(filePath1, filePath2, true);
                        }
                        else
                        {//right is newer - overwrite left
                            File.Copy(filePath2, filePath1, true);
                        }
                    }
                }

                processedFileNames[fileName] = fileName;
            }
        }
    }
}
