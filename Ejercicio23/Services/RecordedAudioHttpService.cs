using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Ejercicio23.Interfaces;
using Ejercicio23.Models;
using Ejercicio23.Request;

namespace Ejercicio23.Services;

public class RecordedAudioHttpService
{
    HttpClient _client;
    JsonSerializerOptions _serializerOptions;
    private readonly string ip = "192.168.100.5";
    private readonly int port = 5003;
    public RecordedAudioHttpService()
    {
        _client = new HttpClient();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };
    }

    public List<MySqlAudioRecordedModel> Items { get; private set; }
    

    public async Task AddRecordingAsync(AudioRecordedReq item, bool isNewItem = false)
    {
   
        string ur = $"http://{ip}:{port}/api/recordinz";
        Uri uri = new Uri(string.Format(ur, string.Empty));

        try
        {
            string json = JsonSerializer.Serialize(item);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }
    
    public async Task<List<MySqlAudioRecordedModel>> GetRecordingsAsync()
    {
        Items = new List<MySqlAudioRecordedModel>();
        string ur = $"http://{ip}:{port}/api/recordinz";
        Uri uri = new Uri(string.Format(ur, string.Empty));
        HttpResponseMessage response = await _client.GetAsync(uri);
        
        bool isOk = response.IsSuccessStatusCode;
        if (isOk)
        {
            string content = await response.Content.ReadAsStringAsync();
          
            Items = JsonSerializer.Deserialize<List<MySqlAudioRecordedModel>>(content);
        }

        return Items;
    }
}