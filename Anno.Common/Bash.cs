using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Anno.Common.Util;

namespace Anno.Common
{
    /// <summary>Handles boilerplate for Bash commands and stores output information.</summary>
    public class Bash
    {
        private static bool _linux { get; }
        private static bool _mac { get; }
        private static bool _windows { get; }
        private static string _bashPath { get; }

        /// <summary>Determines whether bash is running in a native OS (Linux/MacOS).</summary>
        /// <returns>True if in *nix, else false.</returns>
        public static bool Native { get; }

        /// <summary>Determines if using Windows and if Linux subsystem is installed.</summary>
        /// <returns>True if in Windows and bash detected.</returns>
        public static bool Subsystem => _windows && File.Exists(@"C:\Windows\System32\bash.exe");

        /// <summary>Stores output of the previous command if redirected.</summary>
        public string Output { get; private set; }

        /// <summary>
        /// Gets an array of the command output split by newline characters if redirected. </summary>
        public string[] Lines => Output?.Split(Environment.NewLine.ToCharArray());

        /// <summary>Stores the exit code of the previous command.</summary>
        public int ExitCode { get; private set; }

        /// <summary>Stores the error message of the previous command if redirected.</summary>
        public string ErrorMsg { get; private set; }

        static Bash()
        {
            _linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            _mac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            Native = _linux || _mac ? true : false;
            _bashPath = Native ? "/usr/bin/sh" : "bash.exe";
        }
        /// <summary>
        /// ��ִ���ļ�����·��
        /// </summary>
        public string CurrentExePath {
            get
            {
                var path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                return path.Substring(0, path.LastIndexOf("/"));
            }
        }

        /// <summary>Execute a new Bash command.</summary>
        /// <param name="input">The command to execute.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Command(string input, bool redirect = true)
        {
            if (!Native && !Subsystem)
                throw new PlatformNotSupportedException();

            using (var bash = new Process { StartInfo = BashInfo(input, redirect) })
            {
                bash.Start();

                if (redirect)
                {
                    Output = bash.StandardOutput.ReadToEnd()
                        .TrimEnd(Environment.NewLine.ToCharArray());
                    ErrorMsg = bash.StandardError.ReadToEnd()
                        .TrimEnd(Environment.NewLine.ToCharArray());
                }
                else
                {
                    Output = null;
                    ErrorMsg = null;
                }

                bash.WaitForExit();
                ExitCode = bash.ExitCode;
                bash.Close();
            }

            if (redirect)
                return new BashResult(Output, ErrorMsg, ExitCode);
            else
                return new BashResult(null, null, ExitCode);
        }

        private ProcessStartInfo BashInfo(string input, bool redirectOutput)
        {
            return new ProcessStartInfo
            {
                FileName = _bashPath,
                Arguments = $"-c \"{input}\"",
                RedirectStandardInput = false,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = redirectOutput,
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false
            };
        }

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Echo(string input, bool redirect = false) =>
            Command($"echo {input}", redirect: redirect);

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="flags">Optional `echo` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Echo(string input, string flags, bool redirect = false) =>
            Command($"echo {flags} {input}", redirect: redirect);

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Echo(object input, bool redirect = false) =>
            Command($"echo {input}", redirect: redirect);

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="flags">Optional `echo` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Echo(object input, string flags, bool redirect = false) =>
            Command($"echo {flags} {input}", redirect: redirect);

        /// <summary>Search for `pattern` in each file in `location`.</summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="location">The files or directory to search.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Grep(string pattern, string location, bool redirect = true) =>
            Command($"grep {pattern} {location}", redirect: redirect);

        /// <summary>Search for `pattern` in each file in `location`.</summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="location">The files or directory to search.</param>
        /// <param name="flags">Optional `grep` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        public BashResult Grep(string pattern, string location, string flags, bool redirect = true) =>
            Command($"grep {pattern} {flags} {location}", redirect: redirect);

        /// <summary>List information about files in the current directory.</summary>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Ls(bool redirect = true) =>
            Command("ls", redirect: redirect);

        /// <summary>List information about files in the current directory.</summary>
        /// <param name="flags">Optional `ls` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Ls(string flags, bool redirect = true) =>
            Command($"ls {flags}", redirect: redirect);

        /// <summary>List information about the given files.</summary>
        /// <param name="flags">Optional `ls` arguments.</param>
        /// <param name="files">Files or directory to search.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Ls(string flags, string files, bool redirect = true) =>
            Command($"ls {flags} {files}", redirect: redirect);

        /// <summary>Move `source` to `directory`.</summary>
        /// <param name="source">The file to be moved.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Mv(string source, string directory, bool redirect = true) =>
            Command($"mv {source} {directory}", redirect: redirect);

        /// <summary>Move `source` to `directory`.</summary>
        /// <param name="source">The file to be moved.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="flags">Optional `mv` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Mv(string source, string directory, string flags, bool redirect = true) =>
            Command($"mv {flags} {source} {directory}", redirect: redirect);

        /// <summary>Copy `source` to `directory`.</summary>
        /// <param name="source">The file to be copied.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cp(string source, string directory, bool redirect = true) =>
            Command($"cp {source} {directory}", redirect: redirect);

        /// <summary>Copy `source` to `directory`.</summary>
        /// <param name="source">The file to be copied.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="flags">Optional `cp` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cp(string source, string directory, string flags, bool redirect = true) =>
            Command($"cp {flags} {source} {directory}", redirect: redirect);

        /// <summary>Remove or unlink the given file.</summary>
        /// <param name="file">The file(s) to be removed.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Rm(string file, bool redirect = true) =>
            Command($"rm {file}", redirect: redirect);

        /// <summary>Remove or unlink the given file.</summary>
        /// <param name="file">The file(s) to be removed.</param>
        /// <param name="flags">Optional `rm` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Rm(string file, string flags, bool redirect = true) =>
            Command($"rm {flags} {file}", redirect: redirect);

        /// <summary>Concatenate `file` to standard input.</summary>
        /// <param name="file">The source file.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cat(string file, bool redirect = true) =>
            Command($"cat {file}", redirect: redirect);

        /// <summary>Concatenate `file` to standard input.</summary>
        /// <param name="file">The source file.</param>
        /// <param name="flags">Optional `cat` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cat(string file, string flags, bool redirect = true) =>
            Command($"cat {flags} {file}", redirect: redirect);
    }
}
