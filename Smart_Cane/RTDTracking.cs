using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Locations;
using Plugin.Geolocator;
using System.IO;
using Android.Content.Res;
using System.Net;

using transit_realtime;
using ProtoBuf;


namespace Smart_Cane
{
    [Activity(Label = "RTD",ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class RTDTracking : Activity
    {

        Uri myUri = new Uri("http://www.rtd-denver.com/google_sync/TripUpdate.pb");
        //Uri myUri = new Uri("http://www.rtd-denver.com/google_sync/VehiclePosition.pb");


        
          
        
        int minuteA,minuteD;
        
        public int elocation = 0;
        public static List<string> mItems = new List<string>();
        mylocation my_loc = new mylocation("");
        LocationManager locMgr;
        static double longitude = new double();
        static double latitude = new double();
        List<mylocation> near_stops = new List<mylocation>();

        public FeedMessage feed = new FeedMessage();
        protected override void OnCreate(Bundle savedInstanceState)
        {

            WebRequest myWebRequest = HttpWebRequest.Create(myUri);
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;
            NetworkCredential myNetworkCredential = new NetworkCredential(GetString(Resource.String.User),GetString(Resource.String.Pass)); // insert credentials
            CredentialCache myCredentialCache = new CredentialCache();
            myCredentialCache.Add(myUri, "Basic", myNetworkCredential);
            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.Credentials = myCredentialCache;
            feed = Serializer.Deserialize < FeedMessage>(myWebRequest.GetResponse().GetResponseStream());

            
            AssetManager assets = Assets;
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 100;
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RTDTRACKING);
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            var locationbutton = FindViewById<Button>(Resource.Id.getNearest);

            locationbutton.Click += async (sender, e) =>
                {

                        var position = await locator.GetPositionAsync(20000);
                        longitude = position.Longitude;
                        latitude = position.Latitude;
                        my_loc.Latitude = position.Latitude;
                        my_loc.Longitude = position.Longitude;
                        Stops_Near_Me();


                    };



        }
        public async void Stops_Near_Me()
        {
            // Read each line of the file into a string array. Each element
            // of the array is one line of the file
            
            StreamReader stream = new StreamReader(Assets.Open("stops.txt"));

            while (!stream.EndOfStream)
            {
                string line = await stream.ReadLineAsync();

                string[] stop_values = line.Split(',');

                string lat = stop_values[3];
                string lon = stop_values[4];
                double lats, lons;
                double.TryParse(lon, out lons);
                double.TryParse(lat, out lats);

                mylocation targetLocation = new mylocation("blah blah");
                float acc;
                targetLocation.Latitude = lats;
                targetLocation.Longitude = lons;
                targetLocation.Provider = stop_values[0] + ',' + stop_values[1] + ',' + stop_values[2];
                float.TryParse(stop_values[5], out acc);
                targetLocation.Accuracy = acc;
                targetLocation.Stop_Id = stop_values[5].Remove(0,1);
                targetLocation.Route_Id = stop_values[0].Remove(0, 6);
                targetLocation.Last_Stop = stop_values[6].Remove(0,1);
                Distance_Between(targetLocation);

            }

            near_stops.Sort();

            foreach (mylocation near in near_stops)
            {
                string temp = near.Provider;
                mItems.Add(temp);
            }

            var mylistview = new ListView(this);
            mylistview = FindViewById<ListView>(Resource.Id.Locations);
            mylistview.Clickable.Equals(true);
            ArrayAdapter<string> adaptor = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            mylistview.Adapter = adaptor;
            mylistview.ItemClick += listView_ItemClick;




        }
        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            Console.WriteLine("This is a test" + mItems[e.Position]);
            var alertbuild = new AlertDialog.Builder(this);
            {
                for (int x = 0; x<feed.entity.Count; x++)
                {
                    if (near_stops[e.Position].Route_Id == feed.entity[x].trip_update.trip.route_id && near_stops[e.Position].Last_Stop == feed.entity[x].trip_update.stop_time_update[feed.entity[x].trip_update.stop_time_update.Count -1].stop_id)
                    {
                        for (int y = 0; y < feed.entity[x].trip_update.stop_time_update.Count; y++)
                        {
                            if (feed.entity[x].trip_update.stop_time_update[y].stop_id == near_stops[e.Position].Stop_Id)
                            {
                                long nextA = 0,nextD = 0;
                                nextD = feed.entity[x].trip_update.stop_time_update[y].departure.time;
                                nextA = feed.entity[x].trip_update.stop_time_update[y].arrival.time;
                                long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                                nextA = nextA - currentTime;
                                nextD = nextD - currentTime;
                                minuteD = (int)(nextD / 60);
                                minuteA = (int) (nextA / 60);
                                break;
                            }
                        }
                    }      
                }
                alertbuild.SetTitle(" Do you want to go here?");
                alertbuild.SetMessage(mItems[e.Position] + "\nNext Arrival in: " + minuteA + " minutes\nNext Departure in: " + minuteD +" minutes");
                alertbuild.SetPositiveButton("YES", OkAction);
                alertbuild.SetNegativeButton("NO", CancelAction);
                var customAlert = alertbuild.Create();
                elocation = e.Position;
                customAlert.Show();
            }
        }

        private void OkAction(object sender, DialogClickEventArgs e)
        {

            string loca = "google.navigation:q=";
            loca = loca + near_stops[elocation].Latitude + ',' + near_stops[elocation].Longitude + "&mode=w";
            var geoUri = Android.Net.Uri.Parse(loca);
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            StartActivity(mapIntent);

        }
        private void CancelAction(object sender, DialogClickEventArgs e)
        {
        }

        public void Distance_Between(mylocation RTD_Stop)
        {
            double distance_betn = my_loc.DistanceTo(RTD_Stop);

            if (distance_betn <= 500)
            {
                RTD_Stop.Dist_Between = distance_betn;
               
                near_stops.Add(RTD_Stop);
            }
        }

    }
}

