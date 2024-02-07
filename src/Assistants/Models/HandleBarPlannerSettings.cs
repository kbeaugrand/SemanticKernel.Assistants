using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models
{
    internal class HandleBarPlannerSettings
    {
        [YamlMember(Alias = "fixed_plan")]
        public string? FixedPlan { get; set; } = null;
    }
}
