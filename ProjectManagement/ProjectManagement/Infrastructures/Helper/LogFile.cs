using System.IO;
using System.Windows.Forms;

namespace ProjectManagement.Infrastructures.Helper
{
    public class LogFile
    {
        private const string FILE_NAME = "LogTextFile.txt";
        private static string ConfigFilePath
        {
            get { return Application.UserAppDataPath + FILE_NAME; }
        }
        public void WriteLogFile(string fileName, string methodName, string message)
        {
            //String FolderPath = Environment.ExpandEnvironmentVariables("C:\\User\\LogFile.txt");
            FileStream fs = null;
            if (!File.Exists(ConfigFilePath))
            {
                using (fs = File.Create(ConfigFilePath))
                {
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    using (FileStream file = new FileStream(ConfigFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        StreamWriter streamWriter = new StreamWriter(file);
                        streamWriter.WriteLine((((System.DateTime.Now + " - ") + fileName + " - ") + methodName + " - ") + message + "\r\n");
                        streamWriter.WriteLine("\n");
                        streamWriter.Close();
                    }
                }
            }
            catch
            {
            }
        }
    }
}