using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace LLMPlayground;

public class NpcLlm(String name) {
    private static readonly String _systemMessage;

    private readonly ILlmConnector _connector = new GroqConnector();
    private readonly List<Message> _history = new();
    private readonly string _mySystemMessage = _systemMessage.Replace("<npc_name>", name);

    static NpcLlm() {
        // var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        var stream = Assembly.GetCallingAssembly()
            .GetManifestResourceStream("LLMPlayground.SystemMessage.txt")!;
        var stream_reader = new StreamReader(stream);
        _systemMessage = stream_reader.ReadToEnd();
    }

    public async Task<string> GetCompletion(string input) {
        this._history.Add(new() {
            role = "user",
            content = input
        });

        var messages = new List<Message> {
            new() {
                role = "system",
                content = _mySystemMessage
            }
        };
        messages.AddRange(this._history);

        var output = await _connector.GetCompletionAsync(messages);

        this._history.Add(new() {
            role = "assistant",
            content = output
        });

        return output;
    }
}