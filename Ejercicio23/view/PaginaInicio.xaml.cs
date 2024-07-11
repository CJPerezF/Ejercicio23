using Ejercicio23.Models;
using Plugin.Maui.Audio;
using System.Diagnostics;
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

        public PaginaInicio()
        {
            InitializeComponent();
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
                    audioManager = Plugin.Maui.Audio.AudioManager.Current;
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
            if (audioSource != null)
            {
                Stream stream = ((FileAudioSource)audioSource).GetAudioStream();
                byte[] audioBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    audioBytes = ms.ToArray();
                }

                var audio = new Audios
                {
                    fecha = ""+DateTime.Now.ToLocalTime().ToString(),
                    audio = audioBytes
                };

                try
                {
                    if (await App.Instancia.AddAudio(audio) > 0)
                    {
                        audioBytes = new byte[0];
                        audioSource = null;
                        btnGuardar.IsEnabled = false;

                        await DisplayAlert("Aviso", "Audio guardado correctamente", "Ok");
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "Ocurrió un error", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    // Manejar excepciones
                }
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
                audioPlayer = this.audioManager.CreateAsyncPlayer(((FileAudioSource)audioSource).GetAudioStream());

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
