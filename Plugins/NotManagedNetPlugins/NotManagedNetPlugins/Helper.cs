/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace NotManagedNetPlugins
{
    public static class Helper
    {
        /// <summary>
        /// Get a "sFolder" variable with all files and folders from the main folder path.
        /// </summary>
        /// <param name="folderPath">Folder to read</param>
        /// <param name="currFolder">Empty folder</param>
        /// <returns></returns>
        public static sFolder Recursive_GetDirectories(string folderPath, sFolder currFolder)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                sFile newFile = new sFile();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;

                if (!(currFolder.files is List<sFile>))
                    currFolder.files = new List<sFile>();
                currFolder.files.Add(newFile);
            }

            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                sFolder newFolder = new sFolder();
                newFolder.name = new DirectoryInfo(folder).Name;
                newFolder = Recursive_GetDirectories(folder, newFolder);

                if (!(currFolder.folders is List<sFolder>))
                    currFolder.folders = new List<sFolder>();
                currFolder.folders.Add(newFolder);
            }

            return currFolder;
        }
        /// <summary>
        /// Include a file in a "sFolder" variable.
        /// </summary>
        /// <param name="file">Path of the file to include</param>
        /// <param name="currFolder">Root "sFolder" variable to include it</param>
        /// <param name="pathFolder">Empty string</param>
        /// <param name="relativePath">Path of the first folder where all files and folders are.</param>
        /// <returns></returns>
        public static sFolder Recursive_GetDirectories(string file, sFolder currFolder, string pathFolder, string relativePath)
        {
            String directoryPath = new FileInfo(file).DirectoryName;
            if (directoryPath.Replace(relativePath, "") == pathFolder)
            {
                sFile newFile = new sFile();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;

                if (!(currFolder.files is List<sFile>))
                    currFolder.files = new List<sFile>();
                currFolder.files.Add(newFile);
                return currFolder;
            }
            else
            {
                sFolder newFolder = new sFolder(); ;
                if (currFolder.folders is List<sFolder>)
                {
                    string folderName = file.Replace(relativePath + pathFolder, "");
                    folderName = folderName.Substring(1, folderName.Substring(2).IndexOf(Path.DirectorySeparatorChar) + 1);

                    int i;
                    for (i = 0; i < currFolder.folders.Count; i++)
                    {
                        if (currFolder.folders[i].name == folderName)
                        {
                            newFolder = currFolder.folders[i];
                            break;
                        }
                    }

                    if (!(newFolder.name is String))
                        goto Create_Folder;

                    pathFolder += Path.DirectorySeparatorChar + newFolder.name;
                    newFolder = Recursive_GetDirectories(file, newFolder, pathFolder, relativePath);
                    currFolder.folders[i] = newFolder;

                    return currFolder;
                }

            Create_Folder:
                newFolder.name = file.Replace(relativePath + pathFolder, "");
                newFolder.name = newFolder.name.Substring(1, newFolder.name.Substring(2).IndexOf(Path.DirectorySeparatorChar) + 1);
                if (!(currFolder.folders is List<sFolder>))
                    currFolder.folders = new List<sFolder>();

                pathFolder += Path.DirectorySeparatorChar + newFolder.name;
                newFolder = Recursive_GetDirectories(file, newFolder, pathFolder, relativePath);
                currFolder.folders.Add(newFolder);
                return currFolder;
            }
        }

        public static Format StringToFormat(string format)
        {
            switch (format)
            {
                case "Palette":
                    return Format.Palette;
                case "Tile":
                    return Format.Tile;
                case "Map":
                    return Format.Map;
                case "Cell":
                    return Format.Cell;
                case "Animation":
                    return Format.Animation;
                case "FullImage":
                    return Format.FullImage;
                case "Text":
                    return Format.Text;
                case "Video":
                    return Format.Video;
                case "Sound":
                    return Format.Sound;
                case "Font":
                    return Format.Font;
                case "Compress":
                    return Format.Compressed;
                case "Script":
                    return Format.Script;
                case "System":
                    return Format.System;
                case "Pack":
                    return Format.Pack;
                case "Unknown":
                    return Format.Unknown;
                default:
                    return Format.Unknown;
            }
        }
        public static sFolder Get_DecompressedFiles(string txtFile, int num, IPluginHost pluginHost)
        {
            String[] files = File.ReadAllLines(txtFile);
            sFolder decompressed = new sFolder();
            decompressed.files = new List<sFile>();

            String currFolder = files[0].Substring((pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar).Length);
            currFolder = currFolder.Substring(0, currFolder.IndexOf(Path.DirectorySeparatorChar));
            String relativePath = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + currFolder;
            currFolder = "";

            decompressed = Recursive_GetDirectories(relativePath, decompressed);

            return decompressed;
        }
    }
}
