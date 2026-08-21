using System.Text.RegularExpressions;

namespace PromptVault.App.Services;

public sealed partial class VariableResolver : IVariableResolver
{
    public IReadOnlyList<string> ExtractVariables(string content) => VariablePattern()
        .Matches(content ?? string.Empty)
        .Select(match => match.Groups[1].Value)
        .Distinct(StringComparer.CurrentCultureIgnoreCase)
        .OrderBy(value => value)
        .ToList();

    public string Resolve(string content, IReadOnlyDictionary<string, string> values)
    {
        return VariablePattern().Replace(content ?? string.Empty, match =>
        {
            var key = match.Groups[1].Value;
            return values.TryGetValue(key, out var value) ? value : match.Value;
        });
    }

    [GeneratedRegex(@"\{\{\s*([a-zA-Z0-9_.-]+)\s*\}\}")]
    private static partial Regex VariablePattern();
}
