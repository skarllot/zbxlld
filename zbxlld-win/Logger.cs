using System;
using System.IO;
using System.Text;

namespace zbxlld.Windows
{
    internal class Logger : IDisposable
    {
        private StreamWriter writer;
        private static readonly string SEPARATOR = new string('=', 80);

        public Logger(string filepath)
        {
            this.writer = new StreamWriter(filepath, true, Encoding.UTF8);
        }

        private string GetDateNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void WriteFooter()
        {
            this.writer.WriteLine(SEPARATOR);
            this.writer.WriteLine(SEPARATOR);
            this.writer.WriteLine();
            this.writer.Flush();
        }

        public void WriteHeader(string programName, string programVersion)
        {
            this.writer.WriteLine(SEPARATOR);
            this.writer.WriteLine(string.Format("Program Info: {0} {1}", programName, programVersion));
            this.writer.WriteLine(string.Format("OS: {0}", Environment.OSVersion.VersionString));
            this.writer.WriteLine(string.Format("Date: {0}", this.GetDateNow()));
            this.writer.WriteLine(string.Format("Command Line: {0}", Environment.CommandLine));
            this.writer.WriteLine(SEPARATOR);
            this.writer.Flush();
        }
        public void WriteEntry(string s)
        {
            this.writer.WriteLine(string.Format("[{0}] {1}", this.GetDateNow(), s));
            this.writer.Flush();
        }

        public void Dispose()
        {
            this.writer.Flush();
            this.writer.Close();
            this.writer.Dispose();
        }
    }
}