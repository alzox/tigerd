using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Logging;

namespace tiger.helpers

{
	public class Message
	{
	    // Change fields to properties with public getters and setters
	    public string Subject { get; set; }
	    public string Body { get; set; }

	    // Constructor using properties (this will be automatically bound by JsonSerializer)
	    public Message(string subject, string body)
	    {
		this.Subject = subject;
		this.Body = body;
	    }
	}

	public class MessageJson 
    {
        public Message[] messages;
	private readonly ILogger _logger;
        public MessageJson(ILogger logger, string file_arg)
        {
            _logger = logger;
            string contextPath = AppContext.BaseDirectory;
            int index = contextPath.IndexOf("tiger\\");
	    string slash = "";

	    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
		index = contextPath.IndexOf("tiger\\");
		slash = "\\";
	    }
	    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
		index = contextPath.IndexOf("tiger/");
		slash = "/";
	    }

	    string tigerPath = contextPath.Substring(0, index + 6);
            string processPath = tigerPath + "messages" + slash + file_arg + ".json";

	    messages = ProcessJson(processPath);
	}

	
        private Message[] ProcessJson(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
		var options = new JsonSerializerOptions
		{
		    PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // This ensures camelCase JSON is correctly mapped
		};
                var jsonArray = JsonSerializer.Deserialize<Message[]>(jsonString, options); // or another object type
                foreach (Message msg in jsonArray)
		{
			_logger.LogInformation(msg.Subject + ":" + msg.Body);
		}
		if (jsonArray != null && jsonArray.Length > 0)
		{
		    return jsonArray;
		}
		else
                {
                    _logger.LogError("Failed to deserialize JSON or the JSON is empty.");
                }
            }
            catch (FileNotFoundException fnfEx)
            {
                _logger.LogError($"File not found: {filePath}. Exception: {fnfEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"Error deserializing JSON: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred: {ex.Message}");
            }
	    return null;

        }
    }
}
