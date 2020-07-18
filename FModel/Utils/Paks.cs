﻿using FModel.Grabber.Paks;
using FModel.Logger;
using Newtonsoft.Json;
using PakReader.Parsers.Objects;
using System.Collections.Generic;
using System.IO;
using System;

namespace FModel.Utils
{
    static class Paks
    {
        /// <summary>
        /// 1. AppName
        /// 2. AppVersion
        /// 3. AppFilesPath
        /// </summary>
        /// <returns></returns>
        public static (string, string, string) GetUEGameFilesPath(string game)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                string launcher = $"{drive.Name}ProgramData\\Epic\\UnrealEngineLauncher\\LauncherInstalled.dat";
                if (File.Exists(launcher))
                {
                    DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[LauncherInstalled.dat]", launcher);
                    LauncherDat launcherDat = JsonConvert.DeserializeObject<LauncherDat>(File.ReadAllText(launcher));
                    if (launcherDat?.InstallationList != null)
                    {
                        foreach (InstallationList installationList in launcherDat.InstallationList)
                        {
                            if (installationList.AppName.Equals(game))
                                return (installationList.AppName, installationList.AppVersion,
                                    installationList.InstallLocation);
                        }

                        DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[LauncherInstalled.dat]",
                            $"{game} not found");
                        return (string.Empty, string.Empty, string.Empty);
                    }
                }
            }

            DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[LauncherInstalled.dat]", "File not found");
            return (string.Empty, string.Empty, string.Empty);
        }

        // This method is in testing and is not recommended to be used other than for development purposes
        public static (string, string) GetUWPPakFilesPath(string game)
        {
            var dir = "C:\\Program Files\\WindowsApps\\";
            try
            {
                if (Directory.Exists(dir))
                {
                    DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[WindowsApps]", "Folder as been found");
                    foreach (var directory in Directory.GetDirectories(dir))
                        if (directory.Equals(game))
                            return (game, directory);

                    DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[WindowsApps]", $"{game} not found");
                    return (string.Empty, string.Empty);
                }
            }
            catch (UnauthorizedAccessException)
            {
                DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[WindowsApps]",
                    $"{dir} can't be accessed without permission changes to the folder.");
            }

            DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[WindowsApps]", "Folder not found");
            return (string.Empty, string.Empty);
        }

        public static string GetFortnitePakFilesPath()
        {
            (_, string _, string fortniteFilesPath) = GetUEGameFilesPath("Fortnite");
            if (!string.IsNullOrEmpty(fortniteFilesPath))
                return $"{fortniteFilesPath}\\FortniteGame\\Content\\Paks";
            else
                return string.Empty;
        }

        public static string GetValorantPakFilesPath()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                string installs = $"{drive.Name}ProgramData\\Riot Games\\RiotClientInstalls.json";
                if (File.Exists(installs))
                {
                    DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[RiotClientInstalls.json]", installs);
                    InstallsJson installsJson = JsonConvert.DeserializeObject<InstallsJson>(File.ReadAllText(installs));
                    if (installsJson?.AssociatedClient.Count > 0)
                    {
                        foreach (var KvP in installsJson.AssociatedClient)
                            if (KvP.Key.Contains("VALORANT/live/"))
                                return $"{KvP.Key.Replace("/", "\\")}ShooterGame\\Content\\Paks";

                        DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[RiotClientInstalls.json]",
                            "Valorant not found");
                    }
                }
            }

            DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[RiotClientInstalls.json]", "File not found");
            return string.Empty;
        }

        public static string GetStateOfDecay2PakFilesPath()
        {
            // WIP
            (_, string sod2UWPPakFilesPath) = (string.Empty, string.Empty);
            (_, string _, string sod2PakFilesPath) = (string.Empty, string.Empty, string.Empty);
            if (!GetUWPPakFilesPath("Microsoft.Dayton_1.3544.68.2_x64__8wekyb3d8bbwe").Equals(null))
                (_, sod2UWPPakFilesPath) = GetUWPPakFilesPath("Microsoft.Dayton_1.3544.68.2_x64__8wekyb3d8bbwe");
            else
                (_, _, sod2PakFilesPath) = GetUEGameFilesPath("<Unknown Epic Games Name>");

            if (!string.IsNullOrEmpty(sod2PakFilesPath))
                return $"{sod2PakFilesPath}\\StateOfDecay2\\Content\\Paks";
            else if (!string.IsNullOrEmpty(sod2UWPPakFilesPath))
                return $"{sod2UWPPakFilesPath}\\StateOfDecay2\\Content\\Paks";
            return string.Empty;
        }

        public static string GetBorderlands3PakFilesPath()
        {
            (_, string _, string borderlands3FilesPath) = GetUEGameFilesPath("Catnip");
            if (!string.IsNullOrEmpty(borderlands3FilesPath))
                return $"{borderlands3FilesPath}\\OakGame\\Content\\Paks";
            else
                return string.Empty;
        }

        public static string GetTheCyclePakFilesPath()
        {
            (_, string _, string theCycleFilesPath) = GetUEGameFilesPath("AzaleaAlpha"); // TODO: Change when out of alpha
            if (!string.IsNullOrEmpty(theCycleFilesPath))
                return $"{theCycleFilesPath}\\Prospect\\Content\\Paks";
            else
                return string.Empty;
        }

        public static string GetMinecraftDungeonsPakFilesPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var install = $"{appData}/.minecraft_dungeons/launcher_settings.json";
            if (File.Exists(install))
            {
                DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[launcher_settings.json]", install);
                var launcherSettings = JsonConvert.DeserializeObject<LauncherSettings>(File.ReadAllText(install));

                if (launcherSettings.productLibraryDir != null &&
                    !string.IsNullOrEmpty(launcherSettings.productLibraryDir))
                    return $"{launcherSettings.productLibraryDir}\\dungeons\\dungeons\\Dungeons\\Content\\Paks";

                DebugHelper.WriteLine("{0} {1} {2}", "[FModel]", "[launcher_settings.json]",
                    "Minecraft Dungeons not found");
            }

            return string.Empty;
        }

        public static string GetBattleBreakersPakFilesPath()
        {
            (_, string _, string battlebreakersFilesPath) = GetUEGameFilesPath("WorldExplorersLive");
            if (!string.IsNullOrEmpty(battlebreakersFilesPath))
                return $"{battlebreakersFilesPath}\\WorldExplorers\\Content\\Paks";
            else
                return string.Empty;
        }

        public static string GetSpellbreakPakFilesPath()
        {
            (_, string _, string spellbreakFilesPath) = GetUEGameFilesPath("Newt");
            if (!string.IsNullOrEmpty(spellbreakFilesPath))
                return $"{spellbreakFilesPath}\\g3\\Content\\Paks";
            else
                return string.Empty;
        }

        public static void Merge(Dictionary<string, FPakEntry> tempFiles, out Dictionary<string, FPakEntry> files,
            string mount)
        {
            files = new Dictionary<string, FPakEntry>();
            foreach (FPakEntry entry in tempFiles.Values)
            {
                if (files.ContainsKey(mount + entry.GetPathWithoutExtension()) ||
                    entry.GetExtension().Equals(".uptnl") ||
                    entry.GetExtension().Equals(".uexp") ||
                    entry.GetExtension().Equals(".ubulk"))
                    continue;

                if (entry.IsUE4Package()) // if .uasset
                {
                    if (!tempFiles.ContainsKey(Path.ChangeExtension(entry.Name, ".umap"))) // but not including a .umap
                    {
                        string e = Path.ChangeExtension(entry.Name, ".uexp");
                        FPakEntry uexp = tempFiles.ContainsKey(e) ? tempFiles[e] : null; // add its uexp
                        if (uexp != null)
                            entry.Uexp = uexp;

                        string u = Path.ChangeExtension(entry.Name, ".ubulk");
                        FPakEntry ubulk = tempFiles.ContainsKey(u) ? tempFiles[u] : null; // add its ubulk
                        if (ubulk != null)
                            entry.Ubulk = ubulk;
                        else
                        {
                            string f = Path.ChangeExtension(entry.Name, ".ufont");
                            FPakEntry ufont = tempFiles.ContainsKey(f) ? tempFiles[f] : null; // add its ufont
                            if (ufont != null)
                                entry.Ubulk = ufont;
                        }
                    }
                }
                else if (entry.IsUE4Map()) // if .umap
                {
                    string e = Path.ChangeExtension(entry.Name, ".uexp");
                    string u = Path.ChangeExtension(entry.Name, ".ubulk");
                    FPakEntry uexp = tempFiles.ContainsKey(e) ? tempFiles[e] : null; // add its uexp
                    if (uexp != null)
                        entry.Uexp = uexp;
                    FPakEntry ubulk = tempFiles.ContainsKey(u) ? tempFiles[u] : null; // add its ubulk
                    if (ubulk != null)
                        entry.Ubulk = ubulk;
                }

                files[mount + entry.GetPathWithoutExtension()] = entry;
            }
        }

        public static bool IsFileReadLocked(FileInfo PakFileInfo)
        {
            FileStream stream = null;
            try
            {
                stream = PakFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        public static bool IsFileWriteLocked(FileInfo PakFileInfo)
        {
            FileStream stream = null;
            try
            {
                stream = PakFileInfo.Open(FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }
    }
}