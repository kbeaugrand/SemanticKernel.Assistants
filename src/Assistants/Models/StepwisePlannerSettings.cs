using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models
{
    internal class StepwisePlannerSettings
    {
        [YamlMember(Alias = "max_iterations")]
        public int MaxIterations { get; set; } = 15;

        [YamlMember(Alias = "max_tokens")]
        public int MaxTokens { get; set; } = 4000;
    }
}
