using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using Android.Views;



namespace Smart_Cane
{
    [Activity(Label = "Smart Cane", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //set view
            SetContentView(Resource.Layout.Main);

            //create button for test button (first one)
            Button test;
            test = FindViewById<Button>(Resource.Id.button3);
            test.Click += dosomething;

            //create button voice
            Button Voice;
            Voice = FindViewById<Button>(Resource.Id.voice);
            Voice.Click += gotovoice;



            //create Settings button
            Button Settings;
            Settings = FindViewById<Button>(Resource.Id.settings);
            Settings.Click += gotoSettings;

            //******all buttons created******\\

       

        }

        void gotoSettings(object sender, EventArgs args)
        {
            StartActivity(typeof(Settings));
        }

        void gotovoice(object sender, EventArgs args)
        {
            StartActivity(typeof(Voice_Rec));
        }

        void dosomething(object sender, EventArgs args)
        {
            StartActivity(typeof(RTDTracking));
        }
   
    }

 
}


