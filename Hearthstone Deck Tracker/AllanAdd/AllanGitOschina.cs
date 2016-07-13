using Hearthstone_Deck_Tracker.Utility.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Hearthstone_Deck_Tracker.Utility.GitHub;

namespace Hearthstone_Deck_Tracker.AllanAdd
{
    class AllanGitOschina
    {
        public const string AllanVersion = "0.8.8"; //每次更新版本都需要修改@！！！
        public const bool DEBUG_FORCE_UPDATE = false;
        public const string DEBUG_FORCE_UPDATE_TO_VERSION = "0.8.8";

        //public static bool isNeedUpdateConfig() {
        //    Log.Info("00nowVers ");
        //    Version nowVersion = new Version(AllanVersion + ".0");
        //    Log.Info("11nowVers " + nowVersion + " config ");
        //    Version configVersion = string.IsNullOrEmpty(Config.Instance.CreatedByVersion) ? null : new Version(Config.Instance.CreatedAllanByVersion);
        //    Log.Info("22nowVers " + nowVersion + " config " + configVersion);
        //    if (configVersion != null && nowVersion > configVersion)
        //    {
        //        Log.Info("nowVerstrue");
        //        Config.Instance.CreatedAllanByVersion = AllanVersion + ".0";
        //        return true;
        //    }
        //    else {
        //        Log.Info("nowVersfalse");
        //        return false;
        //    }
        //}

        public static async Task<Release> CheckForUpdate(Version currentVersion)
        {
            if (DEBUG_FORCE_UPDATE) {
                Release rel = new Release();
                rel.Tag = "v" + DEBUG_FORCE_UPDATE_TO_VERSION;
                Release.Asset ass = new Release.Asset();
                ass.Url = "http://git.oschina.net/allan.jiang/HDTCN_release/raw/master/HDThanhua_super_" + rel.Tag + ".zip";
                ass.Name = "HDThanhua_super_" + rel.Tag + ".zip";
                rel.Assets = new List<Release.Asset>();
                rel.Assets.Add(ass);
                Utility.Updater.Cleanup();
                return rel;
            }
            try
            {
                currentVersion = Version.Parse(AllanVersion);
                Log.Info("AllanLog:AllanGitOschina " + $"Checking for updates (current={currentVersion})");
                var latest = await GetLatestRelease();
                Log.Info("AllanLog:JSON version " + latest.GetVersion() + " tag " + latest.Tag + " assert url " + latest.Assets[0].Url
                    + " assert name " + latest.Assets[0].Name);
                if (latest.Assets.Count > 0)
                {
                    if (latest.GetVersion()?.CompareTo(currentVersion) > 0)
                    {
                        Log.Info("AllanLog:AllanGitOschina " + $"A new version is available (latest={latest.Tag})");
                        Utility.Updater.Cleanup();
                        return latest;
                    }
                    Log.Info("AllanLog:AllanGitOschina " + $"We are up-to-date (latest={latest.Tag})");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return null;
        }

        private static async Task<Release> GetLatestRelease()
        {
            try
            {
                string json;
                string webVersion = "";
                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "allan.jiang");
                    wc.Encoding = Encoding.GetEncoding("GB2312");
                    var url = "http://git.oschina.net/allan.jiang/HDTCN_release/releases";
                    await Task.Delay(10);
                    json = Encoding.UTF8.GetString(wc.DownloadData(url));
                    //Log.Info("json = " + json);
                    string[] ss = json.Split('\n');
                    //获取最后的方式
                    int si = 0;
                    foreach (string s in ss)
                    {
                        if (s != null && s.Contains("/allan.jiang/HDTCN_release/releases/v") && s.Contains("ui header"))
                        {
                            char[] chs = s.Substring(s.IndexOf("releases/v") + 10).ToArray();
                            foreach (char ch in chs)
                            {
                                if (ch == '"')
                                {
                                    break;
                                }
                                webVersion += ch;
                            }
                            Log.Info("AllanLog:AllanGitOschina webversion " + webVersion);
                            break;
                        }
                        si++;
                    }
                }
                Release rel = new Release();
                rel.Tag = "v" + webVersion;
                Release.Asset ass = new Release.Asset();
                ass.Url = "http://git.oschina.net/allan.jiang/HDTCN_release/raw/master/HDThanhua_super_" + rel.Tag + ".zip";
                ass.Name = "HDThanhua_super_" + rel.Tag + ".zip";
                rel.Assets = new List<Release.Asset>();
                rel.Assets.Add(ass);
                return rel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> GetLatestReleaseTitleAndBody()
        {
            try
            {
                string json;
                string webTitle = "";
                string webBody = "";
                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "allan.jiang");
                    wc.Encoding = Encoding.GetEncoding("GB2312");
                    var url = "http://git.oschina.net/allan.jiang/HDTCN_release/releases";
                    await Task.Delay(10);
                    json = Encoding.UTF8.GetString(wc.DownloadData(url));
                    //Log.Info("json = " + json);
                    string[] ss = json.Split('\n');
                    //获取最后的方式
                    int si = 0;
                    foreach (string s in ss) {
                        if (s != null && s.Contains("/allan.jiang/HDTCN_release/releases/v") && s.Contains("ui header")) {
                            Regex reg = new Regex(".*>(?<result>.*)</a>");
                            webTitle = reg.Replace(s, "${result}");
                            Log.Info("AllanLog:AllanGitOschina title " + webTitle);
                        }
                        if (s != null && s.Contains("markdown-body release-author")) {
                            string temp = ss.ElementAt(si + 1);
                            for (int u =1;u<10;u++) {
                                if (ss.ElementAt(si+u).Contains("auto-desc release-author")) {
                                    break;
                                }
                                temp += ss.ElementAt(u+1) +"\n";
                            }
                            webBody = temp.Replace("<p>", "").Replace("&#x000A;", "\r\n").Replace("</div><head>", "").Replace("</p>", "").Replace("<meta charset='utf-8'>", "");
                            Log.Info("webBody = " + webBody);
                            break;
                        }
                        si++;
                    }
                }
                return "===新版本说明:===\r\n\r\n          " + webTitle + "\r\n\r\n" + webBody;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return "连接更新服务器失败，稍后重试。";
            }
        }

        public static async Task<string> DownloadRelease(Release release, string downloadDirectory)
        {
            try
            {
                var path = Path.Combine(downloadDirectory, release.Assets[0].Name);
                using (var wc = new WebClient())
                    await wc.DownloadFileTaskAsync(release.Assets[0].Url, path);
                return path;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
    }
}
