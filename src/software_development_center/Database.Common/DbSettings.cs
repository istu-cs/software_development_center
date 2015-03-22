using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Database.Common
{
	public class DbSettings
	{
		private const string DefaultCfgFileName = "Database.cfg.xml";

		public string Server { get; set; }

		public string Database { get; set; }

		public string UserId { get; set; }

		public string Password { get; set; }

		private static string GetFilePath(string fileName)
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			// Note RelativeSearchPath can be null even if the doc say something else; don't remove the check
			var searchPath = AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty;
			string relativeSearchPath = searchPath.Split(';').First();
			string binPath = Path.Combine(baseDir, relativeSearchPath);
			return Path.Combine(binPath, fileName);
		}

		public void Serialize()
		{
			Serialize(DefaultCfgFileName);
		}

		public void Serialize(string cfgFileName)
		{
			var xml = new XmlSerializer(typeof(DbSettings));
			using (var sw = new StreamWriter(GetFilePath(cfgFileName)))
			{
				xml.Serialize(sw, this);
			}
		}

		public static DbSettings Deserialize()
		{
			return Deserialize(DefaultCfgFileName);
		}

		public static DbSettings Deserialize(string cfgFileName)
		{
			var xml = new XmlSerializer(typeof(DbSettings));
			using (var sr = new StreamReader(GetFilePath(cfgFileName)))
			{
				return (DbSettings)xml.Deserialize(sr);
			}
		}

		public string GetConnectionString()
		{
			const string pattern = "Server={0};Database={1};Uid={2};Pwd={3};";
			return string.Format(pattern, Server, Database, UserId, Password);
		}
	}
}
