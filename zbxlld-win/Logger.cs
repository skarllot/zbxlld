//
//  Logger.cs
//
//  Author:
//       Fabricio Godoy <skarllot@gmail.com>
//
//  Copyright (c) 2014 Fabricio Godoy
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Text;

namespace zbxlld.Windows
{
    internal class Logger : IDisposable
    {
        StreamWriter writer;
        static readonly string SEPARATOR = new string('=', 80);

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