using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_autogen.Exceptions
{
    public class CodeInterpreterException : Exception
    {
        public CodeInterpreterException(string message, params string[] warnings)
            : base(message)
        {
            this.Warnings = warnings;
        }

        public string[] Warnings { get; }
    }
}
