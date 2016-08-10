#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Newtonsoft.Json;
using System.Text;

#endregion

namespace Hearthstone_Deck_Tracker.Utility
{
	public class GitHub
	{
		public static async Task<Release> CheckForUpdate(string user, string repo, Version version, bool preRelease = false)
		{
			try
			{
				Log.Info($"{user}/{repo}: Checking for updates (current={version}, pre-release={preRelease})");
                Release latest = null;//= await GetLatestRelease(user, repo, preRelease);
                if (user.Equals("jzlhll"))
                {
                    latest = await GetLatestReleaseByAllan(preRelease);
                }
                else {
                    latest = await GetLatestRelease(user, repo, preRelease);
                }
				if(latest != null && latest.Assets.Count > 0)
				{
					if(latest.GetVersion()?.CompareTo(version) > 0)
					{
						Log.Info($"{user}/{repo}: A new version is available (latest={latest.Tag}, pre-release={preRelease})");
						return latest;
					}
					Log.Info($"{user}/{repo}: We are up-to-date (latest={latest.Tag}, pre-release={preRelease})") ;
				}
			}
			catch(Exception e)
			{
				Log.Error(e);
			}
			return null;
		}

        private static async Task<Release> GetLatestReleaseByAllan(bool preRelease)
        {
            try
            {
                string json;
                string webVersion = "";
                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "jzlhll");
                    wc.Encoding = Encoding.GetEncoding("GB2312");
                    var url = "https://api.github.com/repos/jzlhll/AllanHDT/releases";
                    await Task.Delay(10);
                    json = Encoding.UTF8.GetString(wc.DownloadData(url));
                    //Log.Info("json = " + json);
                    string[] ss = json.Split('\n');
                    //获取最后的方式
                    int si = 0;
                    foreach (string s in ss)
                    {
                        if (s != null && s.Contains("tag_name"))
                        {

                            string chs = s.Trim().Replace("\"tag_name\": \"v", "").Replace("\",", "");
                            Log.Info("AllanLog:AllanGitOschina webversion " + webVersion);
                            break;
                        }
                        si++;
                    }
                }
                Release rel = new Release();
                rel.Tag = "v" + webVersion;
                Release.Asset ass = new Release.Asset();
                //TODO 
                /* http://pan.plyz.net/d.asp?u=337122900&p=HDThanhua_super_v0.9.5.zip  */
                /* http://d.139.sh/337122900/HDThanhua_super_v0.9.4.zip  */
                /* https://github.com/jzlhll/AllanHDT/releases/download/v0.9.5/HDThanhua_super_v0.9.5.zip  */
                ///TODO
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

        private static async Task<Release> GetLatestRelease(string user, string repo, bool preRelease)
		{
			try
			{
				string json;
				using(var wc = new WebClient())
				{
					wc.Headers.Add(HttpRequestHeader.UserAgent, user);
					var url = $"https://api.github.com/repos/{user}/{repo}/releases";
					if(!preRelease)
						url += "/latest";
					json = await wc.DownloadStringTaskAsync(url);
				}
				return preRelease ? JsonConvert.DeserializeObject<Release[]>(json).FirstOrDefault() 
								  : JsonConvert.DeserializeObject<Release>(json);
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}

		public static async Task<string> DownloadRelease(Release release, string downloadDirectory)
		{
			try
			{
				var path = Path.Combine(downloadDirectory, release.Assets[0].Name);
				using(var wc = new WebClient())
					await wc.DownloadFileTaskAsync(release.Assets[0].Url, path);
				return path;
			}
			catch(Exception e)
			{
				Log.Error(e);
				return null;
			}
		}

		public class Release
		{
			[JsonProperty("tag_name")]
			public string Tag { get; set; }

			[JsonProperty("assets")]
			public List<Asset> Assets { get; set; }

			public class Asset
			{
				[JsonProperty("browser_download_url")]
				public string Url { get; set; }

				[JsonProperty("name")]
				public string Name { get; set; }
			}

			public Version GetVersion()
			{
				Version v;
				return Version.TryParse(Tag.Replace("v", ""), out v) ? v : null;
			}
		}
	}
}