using Ejercicio23.Models;
using Plugin.Maui.Audio;
namespace Ejercicio23.view;

public partial class Listado : ContentPage
{

    private Audios audioSeleccionado;
    private bool isPlaying;
    AsyncAudioPlayer audioPlayer;

    public Listado()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        carouselView.ItemsSource = await App.Instancia.ListAudios();

    }

    private async void onPlay(object sender, TappedEventArgs e)
    {
        if (audioSeleccionado != null)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                btnPlay.Source = "stop.png";
                MemoryStream memoryStream = new MemoryStream(audioSeleccionado.audio);

                audioPlayer = AudioManager.Current.CreateAsyncPlayer(memoryStream);

                await audioPlayer.PlayAsync(CancellationToken.None);


            }
            else
            {
                isPlaying = false;
                btnPlay.Source = "play.png";
                audioPlayer.Stop();

            }

        }
    }


    private async void carouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        Audios audio = e.CurrentItem as Audios;
        audioSeleccionado = audio;

        if (isPlaying)
        {
            MemoryStream memoryStream = new MemoryStream(audioSeleccionado.audio);

            audioPlayer = AudioManager.Current.CreateAsyncPlayer(memoryStream);

            await audioPlayer.PlayAsync(CancellationToken.None);
        }

    }

}