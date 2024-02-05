using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_autogen.Plugins
{
    internal class CodeInterpretionPluginOptions
    {
        public string DockerEndpoint { get; set; } = string.Empty;

        public string DockerImage { get; set; } = "python:3-alpine";
    }
}
