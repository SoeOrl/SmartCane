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
    class Global : Application
    {
        public static int range {get;set;} = 250;
    }
}