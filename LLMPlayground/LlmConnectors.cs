using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LLMPlayground;

public interface ILlmConnector {
    Task<string> GetCompletionAsync(IEnumerable<Message> messages);
}

public struct Message {
    public Message() {
    }

    public string role { get; set; } = "";
    public string content { get; set; } = "";
}

class ResponseFormat {
    public string type { get; set; } = "json_object";
}

class RequestBody {
    public string model { get; set; } = "";
    public int max_tokens { get; set; } = 0;
    public float temperature { get; set; } = 1.0f;
    public List<Message> messages { get; set; } = new();
    public ResponseFormat response_format { get; set; } = new();
}

class Choice {
    public Int32 index { get; set; } = 0;
    public Message message { get; set; } = new();
}

class ResponseBody {
    public List<Choice> choices { get; set; } = new();
}

public class OpenAIConnector : ILlmConnector {
    private const String RequestUrl = "https://api.openai.com/v1/chat/completions";

    // private const String Model = "gpt-3.5-turbo";
    private const String Model = "gpt-4-turbo";
    private const Int32 MaxTokens = 128;

    private static readonly String _apiKey;


    static OpenAIConnector() {
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
        Debug.Assert(_apiKey != null);
    }

    public async Task<string> GetCompletionAsync(IEnumerable<Message> messages) {
        var requestBody = new RequestBody {
            model = Model,
            max_tokens = MaxTokens,
            response_format = new ResponseFormat { },
            messages = messages.ToList(),
        };
        var body = JsonContent.Create(requestBody);
        var request = new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri(RequestUrl),
            Headers = {
                { "Authorization", "Bearer " + _apiKey },
            },
            Content = body,
        };

        var response = await new HttpClient().SendAsync(request);
        var contentJson = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != HttpStatusCode.OK) {
            throw new Exception("Failed to get completion: " + response.StatusCode + " " + contentJson);
        }

        var responseBody = JsonSerializer.Deserialize<ResponseBody>(contentJson);
        var output = responseBody.choices[0].message.content;

        return output;
    }
}

public class GroqConnector : ILlmConnector {
    private const String RequestUrl = "https://api.groq.com/openai/v1/chat/completions";

    // private const String Model = "llama3-8b-8192";
    private const String Model = "llama3-70b-8192";
    private const Int32 MaxTokens = 128;

    private static readonly String _apiKey;


    static GroqConnector() {
        _apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY")!;
        Debug.Assert(_apiKey != null);
    }

    public async Task<string> GetCompletionAsync(IEnumerable<Message> messages) {
        var requestBody = new RequestBody {
            model = Model,
            max_tokens = MaxTokens,
            response_format = new(),
            messages = messages.ToList(),
        };
        var body = JsonContent.Create(requestBody);
        var request = new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri(RequestUrl),
            Headers = {
                { "Authorization", "Bearer " + _apiKey },
            },
            Content = body,
        };

        var response = await new HttpClient().SendAsync(request);
        var contentJson = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != HttpStatusCode.OK) {
            throw new Exception("Failed to get completion: " + response.StatusCode + " " + contentJson);
        }

        var responseBody = JsonSerializer.Deserialize<ResponseBody>(contentJson);
        var output = responseBody.choices[0].message.content;

        return output;
    }
}