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
using System.Text.RegularExpressions;

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
                    var l = await GetAllAllanRelease();
                    latest = new Release();
                    latest.Tag = "v" + l.Assets.ElementAt(0).Version;
                    //foreach (var a in l.Assets) {
                    //    Log.Info("title " + a.Title);
                    //    Log.Info("version " + a.Version);
                    //    Log.Info("Body " + a.Body);
                    //    Log.Info("urls " + a.Urls[0] + a.Urls[1]);
                    //}

                    Release.Asset ass = new Release.Asset();
                    Random ro = new Random();
                    int r = DateTime.Now.Minute % 2;
                    ass.Url = l.Assets.ElementAt(0).Urls[r].Replace("vxxx", latest.Tag);
                    //随机生成下载地址
                    /* http://pan.plyz.net/d.asp?u=337122900&p=HDThanhua_super_v0.9.5.zip  */
                    /* http://d.139.sh/337122900/HDThanhua_super_v0.9.4.zip  */
                    /* https://github.com/jzlhll/AllanHDT/releases/download/v0.9.5/HDThanhua_super_v0.9.5.zip  */
                    ass.Name = "HDThanhua_super_" + latest.Tag + ".zip";
                    latest.Assets = new List<Release.Asset>();
                    latest.Assets.Add(ass);
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
        public static async Task<AllanRelease> GetAllAllanRelease()
        {
            AllanRelease rel = new AllanRelease();
            try
            {
                string json;
                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "allan.jiang");
                    wc.Encoding = Encoding.GetEncoding("GB2312");
                    var url = "https://code.aliyun.com/allan.jiang/HDTCN/tags";
                    await Task.Delay(10);
                    json = Encoding.UTF8.GetString(wc.DownloadData(url));
                    string[] ss = json.Split('\n');
                    //获取最后的方式
                    int si = 0;
                    
                    foreach (string s in ss)
                    {
                        if (s != null && s.Contains("webversion"))
                        {
                            string webTitle = "";
                            string webBody = "";
                            string webVer = "";
                            List<string> urls = new List<string>();
                            string ns = s.Replace("&#x000A;", "\n").Replace("&amp;", "&").Replace("</p>", "").Replace("</div>", "");

                            string[] sls = ns.Split('\n');
                            for (int i = 0; i < sls.Length; i++)
                            {
                                if (sls[i].Contains("webversion"))
                                { //解析版本

                                    Regex reg = new Regex(".*webversion:v(?<ret>)");
                                    webVer = reg.Replace(sls[i], "${ret}");
                                }
                                else if (sls[i].Contains("webtitle")) //解析title
                                {
                                    Regex reg = new Regex(".*webtitle:(?<ret>)");
                                    webTitle = reg.Replace(sls[i], "${ret}");
                                }
                                else if (sls[i].Contains("href=")) //解析N个连接
                                {
                                    string u = sls[i].Replace("</a>", "").Replace("\" rel=\"nofollow\">", " ").Replace("href=\"", "");
                                    string[] us = u.Split(' ');
                                    u = us[1];

                                    urls.Add(u);
                                }
                                else if (sls[i].Contains("webbody_start")) //解析title
                                {
                                    while (!sls[++i].Contains("webbody_end"))
                                    {
                                        webBody += sls[i] + "\r\n";
                                    }
                                }
                            }
                            AllanRelease.Asset ase = new AllanRelease.Asset();
                            ase.Body = webBody;
                            ase.Title = webTitle;
                            ase.Version = webVer;
                            ase.Urls = urls.ToArray();
                            rel.Assets = new List<AllanRelease.Asset>();
                            rel.Assets.Add(ase);
                        }
                        si++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            return rel.Assets.Count == 0 ? null : rel;
        }

        public class AllanRelease
        {
            public class Asset
            {
                public string Version { get; set; }
                public string Title { get; set; }
                public string Body { get; set; }
                public string[] Urls { get; set; }
            }
            public List<Asset> Assets { get; set; }
        }
    }
}