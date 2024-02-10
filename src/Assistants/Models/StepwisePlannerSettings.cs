using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models
{
    internal class StepwisePlannerSettings
    {
        [YamlMember(Alias = "max_iterations")]
        public int MaxIterations { get; set; } = Defaults.StepwisePlannerMaxIterations;

        [YamlMember(Alias = "max_tokens")]
        public int MaxTokens { get; set; } = Defaults.StepwisePlannerMaxTokens;
    }
}
