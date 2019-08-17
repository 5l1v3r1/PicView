﻿using PicView.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static PicView.File_Logic.ArchiveExtraction;
using static PicView.Helpers.Variables;

namespace PicView.File_Logic
{
    internal static class FileLists
    {
        internal enum SortFilesBy
        {
            Name,
            FileSize,
            Creationtime,
            Extension,
            Lastaccesstime,
            Lastwritetime,
            Random
        }

        /// <summary>
        /// Sort and return list of supported files
        /// </summary>
        internal static List<string> FileList(string path)
        {
            switch (Properties.Settings.Default.SortPreference)
            {
                case 0:
                    return FileList(path, SortFilesBy.Name);
                case 1:
                    return FileList(path, SortFilesBy.FileSize);
                case 2:
                    return FileList(path, SortFilesBy.Creationtime);
                case 3:
                    return FileList(path, SortFilesBy.Extension);
                case 4:
                    return FileList(path, SortFilesBy.Lastaccesstime);
                case 5:
                    return FileList(path, SortFilesBy.Lastwritetime);
                case 6:
                    return FileList(path, SortFilesBy.Random);
                default:
                    return FileList(path, SortFilesBy.Name);
            }
        }

        /// <summary>
        /// Sort and return list of supported files
        /// </summary>
        internal static List<string> FileList(string path, SortFilesBy sortFilesBy)
        {
            if (!Directory.Exists(path))
                return null;

            var items = Directory.GetFiles(path)
                .AsParallel()
                .Where(file =>
                        file.ToLower().EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jpeg", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jpe", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("png", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("bmp", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("tif", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("tiff", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("gif", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ico", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("wdp", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("svg", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("psd", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("psb", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("orf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("cr2", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("crw", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dng", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("raf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("raw", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("mrw", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("nef", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("x3f", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("arw", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("webp", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("aai", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ai", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("art", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("bgra", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("bgro", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("canvas", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("cin", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("cmyk", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("cmyka", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("cur", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("cut", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dcm", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dcr", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dcx", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dds", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dfont", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dlib", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dpx", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dxt1", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("dxt5", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("emf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("epi", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("eps", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ept", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ept2", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ept3", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("exr", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("fax", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("fits", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("flif", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("g3", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("g4", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("gif87", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("gradient", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("gray", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("group4", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("hald", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("hdr", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("hrz", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("icb", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("icon", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ipl", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jc2", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("j2k", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jng", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jnx", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jpm", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jps", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("jpt", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("kdc", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("label", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("map", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("nrw", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("otb", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("otf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pbm", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pcd", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pcds", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pcl", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pct", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pcx", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pfa", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pfb", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pfm", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("picon", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pict", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pix", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pjpeg", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("png00", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("png24", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("png32", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("png48", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("png64", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("png8", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("pnm", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ppm", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ps", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("radialgradient", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ras", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("rgb", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("rgba", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("rgbo", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("rla", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("rle", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("scr", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("screenshot", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("sgi", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("srf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("sun", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("svgz", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("tiff64", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("ttf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("vda", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("vicar", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("vid", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("viff", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("vst", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("vmf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("wpg", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("xbm", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("xcf", StringComparison.OrdinalIgnoreCase)
                        || file.ToLower().EndsWith("yuv", StringComparison.OrdinalIgnoreCase)
                    );

            switch (sortFilesBy)
            {
                // Alphanumeric sort
                case SortFilesBy.Name:
                    var list = items.ToList();
                    list.Sort((x, y) => { return NativeMethods.StrCmpLogicalW(x, y); });
                    return list;
                case SortFilesBy.FileSize:
                    items = items.OrderBy(f => new FileInfo(f).Length);
                    break;
                case SortFilesBy.Extension:
                    items = items.OrderBy(f => new FileInfo(f).Extension);
                    break;
                case SortFilesBy.Creationtime:
                    items = items.OrderBy(f => new FileInfo(f).CreationTime);
                    break;
                case SortFilesBy.Lastaccesstime:
                    items = items.OrderBy(f => new FileInfo(f).LastAccessTime);
                    break;
                case SortFilesBy.Lastwritetime:
                    items = items.OrderBy(f => new FileInfo(f).LastWriteTime);
                    break;
                case SortFilesBy.Random:
                    items = items.OrderBy(f => Guid.NewGuid());
                    break;
            }
            return items.ToList();
        }

        /// <summary>
        /// Gets values and extracts archives
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static Task GetValues(string path)
        {
            return Task.Run(() =>
            {
                // Determine if archive to be extracted or not
                bool zipped = false;
                var extension = Path.GetExtension(path);
                extension = extension.ToLower();
                switch (extension)
                {
                    // Archives
                    case ".zip":
                    case ".7zip":
                    case ".7z":
                    case ".rar":
                    case ".cbr":
                    case ".cb7":
                    case ".cbt":
                    case ".cbz":
                    case ".xz":
                    case ".bzip2":
                    case ".gzip":
                    case ".tar":
                    case ".wim":
                    case ".iso":
                    case ".cab":
                        zipped = Extract(path);
                        if (!zipped)
                        {
                            Pics = new List<string>();
                            FolderIndex = -1;
                            TempZipPath = string.Empty;
                            return;
                        }
                        break;
                }

                if (zipped)
                {
                    // Make a backup of FolderIndex and PicPath
                    if (FolderIndex > -1)
                    {
                        xFolderIndex = FolderIndex;
                    }
                    if (!string.IsNullOrWhiteSpace(PicPath))
                    {
                        xPicPath = PicPath;
                    }

                    // Start at first file
                    FolderIndex = 0;

                    // Add zipped files as recent file
                    RecentFiles.SetZipped(path);

                    // Set extracted files to Pics
                    if (Directory.Exists(TempZipPath))
                    {
                        var directory = Directory.GetDirectories(TempZipPath);
                        if (directory.Length > 0)
                            TempZipPath = directory[0];

                        Pics = FileList(TempZipPath);
                    }
                }
                else
                {
                    // Set files to Pics and get index
                    Pics = FileList(Path.GetDirectoryName(path));
                    if (Pics == null)
                        return;
                    FolderIndex = Pics.IndexOf(path);
                }

                PicPath = path;
            });
        }
    }
}
