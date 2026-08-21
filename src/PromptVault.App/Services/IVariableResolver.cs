namespace PromptVault.App.Services;

public interface IVariableResolver
{
    IReadOnlyList<string> ExtractVariables(string content);
    string Resolve(string content, IReadOnlyDictionary<string, string> values);
}
