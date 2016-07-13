using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HDTUpdate
{
	internal class Program
	{
		private static UpdatingState _state;
		private static void Main(string[] args)
		{
			Console.Title = "HDT汉化版更新程序";
			Console.CursorVisible = false;
			if (args.Length != 2)
			{
				Console.WriteLine("参数错误!");
				return;
			}
			try
			{
				//wait for tracker to shut down
				Thread.Sleep(1000);

				int procId = int.Parse(args[0]);
				if(Process.GetProcesses().Any(p => p.Id == procId))
				{
					Process.GetProcessById(procId).Kill();
					Console.WriteLine("杀掉HDT进程");
				}
			}
			catch
			{
				return;
			}

			try
			{
				var update = Update(args[1]);
				update.Wait();
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				switch(_state)
				{
					case UpdatingState.Preparation:
						Console.WriteLine("请删除【temp】目录并且重新尝试更新. 按任意键退出.");
						Console.ReadKey();
						break;
					case UpdatingState.Downloading:
						Console.WriteLine("新的版本下载出错了！按任意键访问我的帖子，手动下载新版本。");
						Console.ReadKey();
						Process.Start(@"http://bbs.nga.cn/read.php?tid=9444162");
						break;
					case UpdatingState.Extracting:
						Console.WriteLine("安装新版本出错了！ 按任意键访问我的帖子，手动下载新版本。");
						Console.ReadKey();
						Process.Start(@"http://bbs.nga.cn/read.php?tid=9444162");
						break;
					case UpdatingState.Starting:
						Console.WriteLine("重新HDT出错了！你需要自己启动它。任意键退出.");
						Console.ReadKey();
						break;
				}
			}
			finally
			{
				try
				{
					Console.WriteLine("正在清理...");
					if(Directory.Exists("temp"))
						Directory.Delete("temp", true);
					Console.WriteLine("完成!");
				}
				catch
				{
					Console.WriteLine("删除【temp】文件夹失败！");
				}
			}
		}

		private static async Task Update(string url)
		{
			var fileName = url.Split('/').LastOrDefault() ?? "tmp.zip";
			var filePath = Path.Combine("temp", fileName);
			try
			{
				Console.WriteLine("正在创建【temp】文件");
				if(Directory.Exists("temp"))
					Directory.Delete("temp", true);
				Directory.CreateDirectory("temp");
			}
			catch(Exception e)
			{
				throw new Exception("创建/清理下载目录错误.", e);
			}
			_state = UpdatingState.Downloading;
			try
			{
				using(var wc = new WebClient())
				{
					var lockThis = new object();
					wc.DownloadProgressChanged += (sender, e) =>
					{
						lock(lockThis)
						{
							Console.CursorLeft = 0;
							Console.CursorTop = 1;
                            long prog = e.BytesReceived / (29 * 1024 * 10);
                            if (prog > 100) {
                                prog = 100;
                            }
                            Console.WriteLine("正在下载最新版本... 已下载{0}MB,总共大约30MB ({1}%)", e.BytesReceived/(1048576), prog);
						}
					};
					await wc.DownloadFileTaskAsync(url, filePath);
				}
			}
			catch(Exception e)
			{
				throw new Exception("文件下载出错.", e);
			}
			_state = UpdatingState.Extracting;
			try
			{
				//File.Move(filePath, filePath.Replace("rar", "zip"));
				Console.WriteLine("正在解压...");
				ZipFile.ExtractToDirectory(filePath, "temp");
				const string newPath = "temp\\Hearthstone Deck Tracker\\";
				CopyFiles("temp", newPath);
			}
			catch(Exception e)
			{
				throw new Exception("解压下载的文件出错了.", e);
			}
			_state = UpdatingState.Starting;
			try
			{
				Process.Start("HDT汉化高级版.exe");
			}
			catch(Exception e)
			{
				throw new Exception("重新HDT失败.", e);
			}
		}

		private static void CopyFiles(string dir, string newPath)
		{
			foreach(var subDir in Directory.GetDirectories(dir))
			{
				foreach(var file in Directory.GetFiles(subDir))
				{
					var newDir = subDir.Replace(newPath, string.Empty);
					if(!Directory.Exists(newDir))
						Directory.CreateDirectory(newDir);

					var newFilePath = file.Replace(newPath, string.Empty);
					if(file.Contains("HDTUpdate.exe"))
						File.Copy(file, newFilePath.Replace("HDTUpdate.exe", "HDTUpdate_new.exe"));
					else
						File.Copy(file, newFilePath, true);
				}
				CopyFiles(subDir, newPath);
			}
		}
	}

	public enum UpdatingState
	{
		Preparation,
		Downloading,
		Extracting,
		Starting
	}
}