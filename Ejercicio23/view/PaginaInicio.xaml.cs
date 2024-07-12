using Ejercicio23.Models;
using Plugin.Maui.Audio;
using System.Diagnostics;
using Ejercicio23.Request;
using Ejercicio23.Services;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Ejercicio23.view
{
    public partial class PaginaInicio : ContentPage
    {
        IAudioManager audioManager;
        readonly IDispatcher dispatcher;
        IAudioRecorder audioRecorder;
        AsyncAudioPlayer audioPlayer;
        IAudioSource audioSource = null;
        readonly Stopwatch recordingStopwatch = new Stopwatch();
        bool isPlaying;


        private RecordedAudioHttpService _service;
        public PaginaInicio()
        {
            InitializeComponent();
            _service = new RecordedAudioHttpService();
        }

        public double RecordingTime
        {
            get => recordingStopwatch.ElapsedMilliseconds / 1000;
        }

        public bool IsPlaying
        {
            get => isPlaying;
            set => isPlaying = value;
        }

        private async void Start(object sender, EventArgs e)
        {
            if (await ComprobarPermisos<Microphone>())
            {
                if (audioManager == null)
                {
                    audioManager = AudioManager.Current;
                }

                audioRecorder = audioManager.CreateRecorder();

                await audioRecorder.StartAsync();

                img.Source = "stop.png";
            }

            btnStop.IsEnabled = true;
            btnStart.IsEnabled = false;
        }
        
        private async void Guardar(object sender, EventArgs e)
        {
            if (audioSource is null)
            {
                return;
            }
            
            Stream stream = ((FileAudioSource)audioSource).GetAudioStream();
            byte[] audioBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                audioBytes = ms.ToArray();
            }

            var audio = new Audios
            {
                fecha = ""+DateTime.Now.ToLocalTime(),
                audio = audioBytes
            };
                    
            string appDataPath = FileSystem.Current.AppDataDirectory;
            string filename = $"{DateTime.Now:yyyyMMddHHmmss}.wav";
            string filePath = Path.Combine(appDataPath, filename);
            await File.WriteAllBytesAsync(filePath, audioBytes);
            string base64 = Convert.ToBase64String(audioBytes);
               
            bool isCorrectOperation = await App.Instance.AddAudio(audio) > 0;
            if (isCorrectOperation)
            {
                audioBytes = new byte[0];
                audioSource = null;
                btnGuardar.IsEnabled = false;
           
                var mysqlRecord = new AudioRecordedReq()
                {
                    Description = txtDescription.Text,
                    AudioRecorded = base64,
                    AudioPath = filePath
                };
                await _service.AddRecordingAsync(mysqlRecord);

                await DisplayAlert("Aviso", "Audio guardado correctamente", "Ok");
                txtDescription.Text = "";
            }
        }

        private async void Lista(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Listado());
        }

        private async void Play(object sender, EventArgs e)
        {
            if (audioSource != null)
            {
                Stream d = ((FileAudioSource)audioSource).GetAudioStream();
                audioPlayer = this.audioManager.CreateAsyncPlayer(d);

                isPlaying = true;
                await audioPlayer.PlayAsync(CancellationToken.None);
                isPlaying = false;
            }
        }

        private async void Stop(object sender, EventArgs e)
        {
            audioSource = await audioRecorder.StopAsync();
            recordingStopwatch.Stop();

            img.Source = "play.png";

            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;
            btnGuardar.IsEnabled = true;
        }

        public static async Task<bool> ComprobarPermisos<TPermission>() where TPermission : BasePermission, new()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<TPermission>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }


            status = await Permissions.RequestAsync<TPermission>();

            return status == PermissionStatus.Granted;
        }
    }
}
