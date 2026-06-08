using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ClipboardAI.Models;
using ClipboardAI.Services.Settings;

namespace ClipboardAI.Services.AI;

public class AIService : IAIService
{
    private readonly SettingsManager _settingsManager;
    private readonly HttpClient _httpClient;

    public AIService(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _httpClient = new HttpClient();
    }

    public async Task<List<ExtractedField>> ExtractDataAsync(string text, CancellationToken cancellationToken = default)
    {
        var settings = _settingsManager.CurrentSettings;
        if (string.IsNullOrWhiteSpace(settings.AIToken))
        {
            throw new Exception("API Key is missing. Please configure it in Settings.");
        }

        string endpoint = settings.AIEndpoint?.TrimEnd('/') ?? "";
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new Exception("API Endpoint is missing. Please configure it in Settings.");
        }

        string modelName = settings.AIModelName ?? "gpt-4.1-mini";
        
        // Setup request payload for Azure OpenAI Chat Completions
        // If the user's URI is https://aistudio1303-resource.services.ai.azure.com/openai/v1/responses
        // Wait, standard Azure OpenAI endpoint for Chat Completion is:
        // {endpoint}/openai/deployments/{deployment-id}/chat/completions?api-version=2024-02-15-preview
        // But some users use standard OpenAI compatibility mode if supported, or pass the full URL.
        // For standard OpenAI compatibility on Azure, we can send to chat/completions.
        
        // Let's assume the endpoint provided is the base URL.
        string requestUrl = $"{endpoint}/openai/deployments/{modelName}/chat/completions?api-version=2024-02-15-preview";
        
        // If the user's endpoint already contains 'chat/completions', use it directly.
        if (endpoint.Contains("chat/completions"))
        {
            requestUrl = endpoint;
        }
        else if (endpoint.Contains("/openai/v1/responses"))
        {
            // Just a fallback if they pasted exactly what was in their UI for prompt flow.
            // Azure OpenAI Chat Completions usually requires deployment path.
            requestUrl = $"{endpoint.Replace("/openai/v1/responses", "")}/openai/deployments/{modelName}/chat/completions?api-version=2024-02-15-preview";
        }

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are an intelligent data extractor. Analyze the user's unstructured text and extract key-value pairs representing the structured data. Return the extracted data by calling the 'extract_structured_data' function." },
                new { role = "user", content = text }
            },
            tools = new[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "extract_structured_data",
                        description = "Extract structured key-value fields from text.",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                fields = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            key = new { type = "string", description = "The name of the field (e.g. 'Model name', 'Tokens per Minute')" },
                                            value = new { type = "string", description = "The extracted value for the field." }
                                        },
                                        required = new[] { "key", "value" }
                                    }
                                }
                            },
                            required = new[] { "fields" }
                        }
                    }
                }
            },
            tool_choice = new
            {
                type = "function",
                function = new { name = "extract_structured_data" }
            }
        };

        var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Content = jsonContent;
        // Azure OpenAI uses api-key header
        request.Headers.Add("api-key", settings.AIToken);
        
        // Also add Authorization Bearer just in case it's standard OpenAI disguised
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.AIToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"AI Request Failed: {response.StatusCode}\n{error}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseString);
        
        var choices = jsonDoc.RootElement.GetProperty("choices");
        if (choices.GetArrayLength() == 0) return new List<ExtractedField>();

        var message = choices[0].GetProperty("message");
        if (message.TryGetProperty("tool_calls", out var toolCalls) && toolCalls.GetArrayLength() > 0)
        {
            var functionCall = toolCalls[0].GetProperty("function");
            var argumentsJson = functionCall.GetProperty("arguments").GetString();
            
            if (!string.IsNullOrEmpty(argumentsJson))
            {
                using var argsDoc = JsonDocument.Parse(argumentsJson);
                if (argsDoc.RootElement.TryGetProperty("fields", out var fieldsArray))
                {
                    var result = new List<ExtractedField>();
                    foreach (var item in fieldsArray.EnumerateArray())
                    {
                        result.Add(new ExtractedField
                        {
                            Key = item.GetProperty("key").GetString() ?? "",
                            Value = item.GetProperty("value").GetString() ?? ""
                        });
                    }
                    return result;
                }
            }
        }

        return new List<ExtractedField>();
    }

    public async Task<List<ExtractedField>> SummarizeAndTranslateAsync(string text, CancellationToken cancellationToken = default)
    {
        var settings = _settingsManager.CurrentSettings;
        if (string.IsNullOrWhiteSpace(settings.AIToken)) throw new Exception("API Key is missing. Please configure it in Settings.");

        string endpoint = settings.AIEndpoint?.TrimEnd('/') ?? "";
        if (string.IsNullOrWhiteSpace(endpoint)) throw new Exception("API Endpoint is missing. Please configure it in Settings.");

        string modelName = settings.AIModelName ?? "gpt-4.1-mini";
        
        string requestUrl = $"{endpoint}/openai/deployments/{modelName}/chat/completions?api-version=2024-02-15-preview";
        if (endpoint.Contains("chat/completions")) requestUrl = endpoint;
        else if (endpoint.Contains("/openai/v1/responses")) requestUrl = $"{endpoint.Replace("/openai/v1/responses", "")}/openai/deployments/{modelName}/chat/completions?api-version=2024-02-15-preview";

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are an AI assistant. Your task is to provide a concise summary of the provided text, and also translate the ENTIRE text into Vietnamese (if the source is not Vietnamese) or English (if the source is Vietnamese). Use the tool call to return the structured results." },
                new { role = "user", content = text }
            },
            tools = new[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "summarize_and_translate",
                        description = "Return the summary and translation of the text.",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                summary = new { type = "string", description = "A concise summary of the text." },
                                translation = new { type = "string", description = "The full translation of the text." }
                            },
                            required = new[] { "summary", "translation" }
                        }
                    }
                }
            },
            tool_choice = new
            {
                type = "function",
                function = new { name = "summarize_and_translate" }
            }
        };

        var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Content = jsonContent;
        request.Headers.Add("api-key", settings.AIToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.AIToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"AI Request Failed: {response.StatusCode}\n{error}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseString);
        
        var choices = jsonDoc.RootElement.GetProperty("choices");
        if (choices.GetArrayLength() == 0) return new List<ExtractedField>();

        var message = choices[0].GetProperty("message");
        if (message.TryGetProperty("tool_calls", out var toolCalls) && toolCalls.GetArrayLength() > 0)
        {
            var functionCall = toolCalls[0].GetProperty("function");
            var argumentsJson = functionCall.GetProperty("arguments").GetString();
            
            if (!string.IsNullOrEmpty(argumentsJson))
            {
                using var argsDoc = JsonDocument.Parse(argumentsJson);
                var result = new List<ExtractedField>();
                
                if (argsDoc.RootElement.TryGetProperty("summary", out var summaryElement))
                {
                    result.Add(new ExtractedField { Key = "Tóm tắt (Summary)", Value = summaryElement.GetString() ?? "" });
                }
                if (argsDoc.RootElement.TryGetProperty("translation", out var translationElement))
                {
                    result.Add(new ExtractedField { Key = "Bản dịch (Translation)", Value = translationElement.GetString() ?? "" });
                }
                
                return result;
            }
        }

        return new List<ExtractedField>();
    }
}
