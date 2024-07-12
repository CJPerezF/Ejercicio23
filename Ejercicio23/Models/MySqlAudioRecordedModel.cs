using System.Reflection.Metadata;
using Ejercicio23.Interfaces;

namespace Ejercicio23.Models;

public class MySqlAudioRecordedModel 
{
    public string RecordId { get; set; }
    public DateTime RecordDate { get; set; }
    public string Description { get; set; }
    public string AudioRecorded { get; set; }
    public string AudioPath { get; set; }
}
