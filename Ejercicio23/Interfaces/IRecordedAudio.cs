namespace Ejercicio23.Interfaces;

public interface IRecordedAudio
{
    public string RecordId { get; set; } 

    public DateTime RecordDate { get; set; } 

    public string Description { get; set; } 

    public string AudioRecorded { get; set; } 

    public string AudioPath { get; set; }  
}
