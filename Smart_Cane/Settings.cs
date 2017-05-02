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

namespace Smart_Cane
{
    [Activity(Label = "Settings")]
    public class Settings : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
           // Global global;
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings);
            int amount;
            TextView text = (TextView)FindViewById(Resource.Id.CurrentDistance);
            text.Text = "The current search radius is: " + Global.range.ToString() + " meters.";
            EditText amount_text = (EditText)FindViewById(Resource.Id.SetDistance);

            amount_text.TextChanged += delegate
            {
                int.TryParse(amount_text.Text.ToString(), out amount);
                if (amount == 0)
                {
                    Global.range = 250;
                    text.Text = "The current search radius is: " + Global.range.ToString() + " meters.";
                }
                else
                {
                    Global.range = amount;
                    text.Text = "The current search radius is: " + Global.range.ToString() + " meters.";
                }

            };
           


        }
    }
}