using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models;

public class AssistantPromptExecutionSettings
{
    /// <summary>
    /// Temperature controls the randomness of the completion.
    /// The higher the temperature, the more random the completion.
    /// Default is 1.0.
    /// </summary>
    [YamlMember(Alias = "temperature")]
    public double Temperature { get; set; } = 1;

    /// <summary>
    /// TopP controls the diversity of the completion.
    /// The higher the TopP, the more diverse the completion.
    /// Default is 1.0.
    /// </summary>
    [YamlMember(Alias = "top_p")]
    public double TopP { get; set; } = 1;

    /// <summary>
    /// Number between -2.0 and 2.0. Positive values penalize new tokens
    /// based on whether they appear in the text so far, increasing the
    /// model's likelihood to talk about new topics.
    /// </summary>
    [YamlMember(Alias = "presence_penalty")]
    public double PresencePenalty { get; set; }

    /// <summary>
    /// Number between -2.0 and 2.0. Positive values penalize new tokens
    /// based on their existing frequency in the text so far, decreasing
    /// the model's likelihood to repeat the same line verbatim.
    /// </summary>
    [YamlMember(Alias = "frequency_penalty")]
    public double FrequencyPenalty { get; set; }

    /// <summary>
    /// The maximum number of tokens to generate in the completion.
    /// </summary>
    [YamlMember(Alias = "max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Sequences where the completion will stop generating further tokens.
    /// </summary>
    [YamlMember(Alias = "stop_sequences")]
    public IList<string>? StopSequences { get; set; }
}
