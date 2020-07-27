using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Archiv
{
    class Program
    {

        /// <summary>
        /// Главная функция
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            DateTime dt = DateTime.Now;
            
            //Исходная папка 
            string _globalpath = "C:\\LOG";

            //Папка для архива
            string _globalpathforarchive = "C:\\LOG1\\LOG";

            // Получаем список директорий
            foreach (var dirs in Directory.EnumerateDirectories(_globalpath))
            {
                // Получаем список директорий с датой 
                foreach (var dirswithdate in Directory.EnumerateDirectories(dirs))
                {

                    // Определяем когда создана директория
                    DateTime  datecteatedir = Directory.GetLastWriteTime(dirswithdate);
                    TimeSpan dayscteatedir = dt - datecteatedir;
                    Console.WriteLine("Дата последней записи в каталоге была " + dayscteatedir.Days);
                    if (dayscteatedir.Days > 30)
                    {
                        DirectoryInfo dirhead = new DirectoryInfo(dirs);
                        DirectoryInfo dirbody = new DirectoryInfo(dirswithdate);
                        string currentfiledir = Path.Combine(_globalpath, dirhead.Name, dirbody.Name);

                        //Получаем список файлов
                        foreach (var Getfile in Directory.GetFiles(dirswithdate))
                        {
                                string archiver = @"C:\Program Files\7-Zip\7z.exe";
                                string archiveName = Path.Combine(_globalpathforarchive, dirhead.Name, dirbody.Name) + ".7z";
                                Console.WriteLine("Текущая директория" + currentfiledir);
                                Console.WriteLine("Обработка" + Getfile);
                                AddToArchive(archiver, Getfile, archiveName);
                        }

                        // Удаляем пустую директорию 
                        if (Directory.GetFiles(currentfiledir).Length == 0)
                        {
                            Console.WriteLine("Удаляем" + currentfiledir);
                            Directory.Delete(currentfiledir);
                        }

                       
                    }

                }
               
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Архивируем файл 
        /// </summary>
        /// <param name="archiver"></param>
        /// <param name="Getfile"></param>
        /// <param name="archiveName"></param>
        public static void AddToArchive(string archiver, string Getfile, string archiveName)
        {
            try
            {
                if (!File.Exists(archiver))
                    throw new Exception("Архиватор 7z по пути \"" + archiver + "\" не найден");
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = archiver;
                startInfo.Arguments = " a -mx9 ";
                startInfo.Arguments += "\"" + archiveName + "\"";
                startInfo.Arguments += " \"" + Getfile + "\"";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                int sevenZipExitCode = 0;
                using (Process sevenZip = Process.Start(startInfo))
                {
                    sevenZip.WaitForExit();
                    sevenZipExitCode = sevenZip.ExitCode;
                    DeleteFile(Getfile);
                }
                
                if (sevenZipExitCode != 0 && sevenZipExitCode != 1)
                {
                    Thread.Sleep(1000);
                    using (Process sevenZip = Process.Start(startInfo))
                    {
                       
                        sevenZip.WaitForExit();
                        switch (sevenZip.ExitCode)
                        {
                            case 0: return; // Без ошибок и предупреждений
                            case 1: return; // Есть некритичные предупреждения
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SevenZip.AddToArchive: " + e.Message);
            }
        }

        /// <summary>
        /// Удаляем файл 
        /// </summary>
        /// <param name="Getfile"></param>
        public static void DeleteFile(string Getfile)
        {
            System.IO.File.Delete(Getfile);
        }

    }
}
