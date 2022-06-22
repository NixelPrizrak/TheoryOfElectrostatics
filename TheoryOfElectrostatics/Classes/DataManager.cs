using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace TheoryOfElectrostatics.Classes
{
    static class DataManager
    {
        static public Frame MainFrame { get; set; }
        static public Frame DataFrame { get; set; }
        static public Frame HelpFrame { get; set; }
        static public Frame AdministrationFrame { get; set; }
        static public string CurrentTheme { get; set; }
        static public int Section { get; set; }
        static public string DataPath { get => $"Data/Data.zip"; }
        static public string ImagesPath { get => $"{CurrentTheme}/Images"; }

        static public void CreateTempFolder()
        {
            string filePath = $"{Path.GetTempPath()}{Path.GetRandomFileName()}";
            while (File.Exists(filePath) || Directory.Exists(filePath))
            {
                filePath = $"{Path.GetTempPath()}{Path.GetRandomFileName()}";
            }
            Directory.CreateDirectory(filePath);

            Properties.Settings.Default.TempPath = filePath;
            Properties.Settings.Default.Save();
        }

        static public void CheckTempFolder()
        {
            if (Properties.Settings.Default.TempPath == null || Properties.Settings.Default.TempPath == "")
            {
                CreateTempFolder();
            }
            else
            {
                if (!Directory.Exists(Properties.Settings.Default.TempPath))
                {
                    if (!File.Exists(Properties.Settings.Default.TempPath))
                    {
                        Directory.CreateDirectory(Properties.Settings.Default.TempPath);
                    }
                    else
                    {
                        CreateTempFolder();
                    }
                }
            }
        }

        static public ZipFile OpenZip(string zipPath)
        {
            if (!File.Exists(zipPath))
            {
                using (ZipFile zipFile = new ZipFile())
                {
                    zipFile.Encryption = EncryptionAlgorithm.WinZipAes256;
                    zipFile.Password = "123";
                    zipFile.TempFileFolder = Properties.Settings.Default.TempPath;
                    zipFile.Save(zipPath);
                }
            }

            ZipFile zip = ZipFile.Read(zipPath, new ReadOptions { Encoding = Encoding.GetEncoding(866) });
            zip.Encryption = EncryptionAlgorithm.WinZipAes256;
            zip.Password = "123";
            CheckTempFolder();
            zip.TempFileFolder = Properties.Settings.Default.TempPath;

            return zip;
        }

        static public string SelectFolder()
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                return folder.SelectedPath;
            }
            else
            {
                return null;
            }
        }

        static public List<T> ShuffleList<T>(List<T> list)
        {
            Random rand = new Random();

            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = list[j];
                list[j] = list[i];
                list[i] = tmp;
            }

            return list;
        }

        static public BitmapImage GetImage(string path)
        {
            BitmapImage image = null;

            using (ZipFile zip = OpenZip(DataPath))
            {
                if (zip[path] != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        zip[path].Extract(ms);
                        ms.Position = 0;
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = null;
                        image.StreamSource = ms;
                        image.EndInit();
                    }
                }
            }

            return image;
        }

        static public string LoadImage(string fileName, string folder)
        {
            string name = "";
            string extension = new FileInfo(fileName).Extension;

            using (ZipFile zip = OpenZip(DataPath))
            {
                List<string> names = zip.EntryFileNames.ToList();

                do
                {
                    name = $"{Guid.NewGuid()}{extension}";
                }
                while (names.Contains($"{folder}/{name}"));

                zip.AddEntry($"{folder}/{name}", File.ReadAllBytes(fileName));
                zip.Save();
            }

            return name;
        }

        static public void RemoveImage(string fileName)
        {
            using (ZipFile zip = OpenZip(DataPath))
            {
                string entryName = $"{ImagesPath}/{fileName}";

                if (zip.EntryFileNames.Contains(entryName))
                {
                    zip.RemoveEntry(entryName);
                    zip.Save();
                }
            }
        }
    }
}
