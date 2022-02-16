using System;

namespace EasySave_Interface.Model
{
	class LogFile
	{
		public DateTime timestamp { get; set; }
		public string backupName { get; set; }
		public string sourcePath { get; set; }
		public string destPath { get; set; }
		public long fileSize { get; set; }
		public long transferTime { get; set; }

		public LogFile( string backupName, string sourcePath, string destPath, long fileSize, long transferTime): base()
        {
			this.timestamp =  DateTime.Now;
			this.backupName = backupName;
			this.sourcePath = sourcePath;
			this.destPath = destPath;
			this.fileSize = fileSize;
			this.transferTime = transferTime;

		}
	}
}
