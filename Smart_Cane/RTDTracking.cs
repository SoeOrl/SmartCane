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
using Android.Locations;
using System.Threading;
using Plugin.Geolocator;
using System.IO;
using System.Reflection;
using Android.Content.Res;
using System.Collections.ObjectModel;
using System.Net;

using transit_realtime;
using ProtoBuf;

// Give Rapsberry a boolean to start a scan of t sourroundings, and will receive a boolean in return. I will send a scan request every x seconds. If i get true then 
//trigger message to warn user. Dynamic data. 
namespace Smart_Cane
{
    [Activity(Label = "Test")]
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
        //ObservableCollection<mylocation> near_stops = new ObservableCollection<mylocation>();
        public FeedMessage feed = new FeedMessage();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            WebRequest myWebRequest = HttpWebRequest.Create(myUri);

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;

            // This username nad password is issued to John Bennett for the IWKS 4120 class
            // Please DO NOT redistribute
            NetworkCredential myNetworkCredential = new NetworkCredential(); // insert credentials

            CredentialCache myCredentialCache = new CredentialCache();
            myCredentialCache.Add(myUri, "Basic", myNetworkCredential);

            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.Credentials = myCredentialCache;
            
          feed = Serializer.Deserialize < FeedMessage>(myWebRequest.GetResponse().GetResponseStream());

            
            AssetManager assets = this.Assets;
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 100;
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RTDTRACKING);
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            var locationbutton = FindViewById<Button>(Resource.Id.getNearest);

            locationbutton.Click += async (sender, e) =>
                {

                        var position = await locator.GetPositionAsync(20000);
                        Console.WriteLine("Position Status: {0}", position.Timestamp);
                        Console.WriteLine("Position Latitude: {0}", position.Latitude);
                        Console.WriteLine("Position Longitude: {0}", position.Longitude);
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
            // of the array is one line of the file.


            
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
                //    targetLocation.Latitude = double.Parse(lat);
                //   targetLocation.Longitude = double.Parse(lon);
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


                //add form for user to fill out what they know. Bus #, beginning loc, end loc, general area, 
                //integrate feed
                //voice saving
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
            var test = new AlertDialog.Builder(this);
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
                    
                 //   if (near_stops[elocation].Accuracy.ToString() == feed.entity[x].trip_update.stop_time_update.)
                }
                test.SetTitle(" Do you want to go here?");
                test.SetMessage(mItems[e.Position] + "\nNext Arrival in: " + minuteA + " minutes\nNext Departure in: " + minuteD +" minutes");
                test.SetPositiveButton("YES", OkAction);
                test.SetNegativeButton("NO", CancelAction);
                var customAlert = test.Create();
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
            //do something on cancel selected
            Console.WriteLine("in cancel");
        }

    
    
        
        // determine whether the stops are within one mile of my current location, and if so, add to array
        public void Distance_Between(mylocation RTD_Stop)
        {
            double distance_betn = my_loc.DistanceTo(RTD_Stop);

            if (distance_betn <= 500)
            {
                RTD_Stop.Dist_Between = distance_betn;
               
                near_stops.Add(RTD_Stop);
                Console.WriteLine("Found a position wihtin 1 mile");

            }


        }


    }
}

