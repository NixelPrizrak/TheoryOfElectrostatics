using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace TheoryOfElectrostatics
{
    static class DataManager
    {
        static public Frame MainFrame { get; set; }
        static public Frame DataFrame { get; set; }
        static public Frame HelpFrame { get; set; }
        static public Frame AdministrationFrame { get; set; }
        static public int IdTest { get; set; }
        static public string LectionsPath { get => $"Data/Lectionss.zip"; }
        static public string TestsPath { get => $"Data/Tests.zip"; }

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

        static public void CheckZip()
        {
            if (!File.Exists(LectionsPath))
            {
                using (File.Create(LectionsPath)) { };

                using (ZipFile zip = OpenZip()) { }
            }

        }

        static public ZipFile OpenZip()
        {
            if (!File.Exists(LectionsPath))
            {
                using (ZipFile zipFile = new ZipFile())
                {
                    zipFile.Encryption = EncryptionAlgorithm.WinZipAes256;
                    zipFile.Password = "123";
                    zipFile.TempFileFolder = Properties.Settings.Default.TempPath;
                    zipFile.Save(LectionsPath);
                }
            }

            ZipFile zip = ZipFile.Read(LectionsPath, new ReadOptions { Encoding = Encoding.GetEncoding(866) });
            zip.Encryption = EncryptionAlgorithm.WinZipAes256;
            zip.Password = "123";
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

    }
}
