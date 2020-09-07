using System;

namespace Anno.Common.Util
{
    /// <summary>
    /// Simple container for the results of a Bash command.</summary>
    public class BashResult
    {
        /// <summary>
        /// The command's standard output as a string. (if redirected)</summary>
        public string Output { get; private set; }

        /// <summary>
        /// The command's error output as a string. (if redirected)</summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// The command's exit code as an integer.</summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// An array of the command's output split by newline characters. (if redirected)</summary>
        public string[] Lines => Output?.Split(Environment.NewLine.ToCharArray());

        internal BashResult(string output, string errorMsg, int exitCode)
        {
            Output = output?.TrimEnd(Environment.NewLine.ToCharArray());
            ErrorMsg = errorMsg?.TrimEnd(Environment.NewLine.ToCharArray());
            ExitCode = exitCode;
        }
    }
}