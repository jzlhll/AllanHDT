using Hearthstone_Deck_Tracker.Utility.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Hearthstone_Deck_Tracker.Utility.GitHub;

namespace Hearthstone_Deck_Tracker.AllanAdd
{
    class AllanGitOschina
    {
        public const string AllanVersion = "0.8.3"; //每次更新版本都需要修改@！！！
        public static async Task<Release> CheckForUpdate(Version currentVersion)
        {
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
                    var url = "http://git.oschina.net/allan.jiang/HDTCN_release/releases";
                    json = await wc.DownloadStringTaskAsync(url);
                    string[] ss = json.Split('\n');
                    
                    foreach (string s in ss) {
                        if (s != null && s.Contains("/allan.jiang/HDTCN_release/releases/v")) {
                            Log.Info("AllanLog:AllanGitOschinacontaio111 " + s + "index " + s.IndexOf("releases/v"));
                            Log.Info("AllanLog:AllanGitOschinacontaio222 " + s.Substring(s.IndexOf("releases/v") + 10));
                            char[] chs = s.Substring(s.IndexOf("releases/v") + 10).ToArray();
                            foreach (char ch in chs) {
                                if (ch == '"') {
                                    break;
                                }
                                webVersion += ch;
                            }
                            Log.Info("AllanLog:AllanGitOschina webversion " + webVersion);
                            break;
                        }
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
