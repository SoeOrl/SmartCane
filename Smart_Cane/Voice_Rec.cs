using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;

namespace Smart_Cane
{
    [Activity(Label = "Activity1")]
    public class Voice_Rec : Activity
    {
        MediaRecorder _recorder;
        MediaPlayer _player;
        Button _start;
        Button _stop;
        string path = "/sdcard/test.3gpp";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.VoiceRec);
            _start = FindViewById<Button>(Resource.Id.start);
            _stop = FindViewById<Button>(Resource.Id.stop);
          

            _start.Click += startRecording;


            _stop.Click += stopRecording; 
          
                
         }

        private void stopRecording(object sender, EventArgs e)
        {
            _stop.Enabled = !_stop.Enabled;

            _recorder.Stop();
            _recorder.Reset();

            _player.SetDataSource(path);
            _player.Prepare();
            _player.Start();
        }

        private void startRecording(object sender, EventArgs e)
        {
            _stop.Enabled = !_stop.Enabled;
            _start.Enabled = !_start.Enabled;

            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            _recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            _recorder.SetOutputFile(path);
            _recorder.Prepare();
            _recorder.Start();
        }
        protected override void OnResume()
        {
            base.OnResume();

            _recorder = new MediaRecorder();
            _player = new MediaPlayer();

            _player.Completion += (sender, e) => {
                _player.Reset();
                _start.Enabled = !_start.Enabled;
            };

        }
        protected override void OnPause()
        {
            base.OnPause();

            _player.Release();
            _recorder.Release();
            _player.Dispose();
            _recorder.Dispose();
            _player = null;
            _recorder = null;
        }
    }
    
}