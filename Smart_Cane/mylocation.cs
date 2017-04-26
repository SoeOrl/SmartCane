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

namespace Smart_Cane
{
    public class mylocation : Location, IComparable<mylocation>
    {
        private string stop_id;
        private string route_id;
        private string last_stop;
        private double dist_between;
        public mylocation(string i) : base(i)
        {
            Provider = i;
        }

        public string Stop_Id
        {
            get
            {
                return stop_id;
            }
            set
            {
                stop_id = value;
            }

        }
        public string Route_Id
        {
            get
            {
                return route_id;
            }
            set
            {
                route_id = value;
            }
        }
        public string Last_Stop
        {
            get
            {
                return last_stop;
            }
            set
            {
                last_stop = value;
            }
        }
        public double Dist_Between
        {
            get
            {
                return dist_between;
            }
            set
            {
                dist_between = value;
            }

        }


        public static bool operator <(mylocation lhs, mylocation rhs)
        {
            if (lhs.Dist_Between < rhs.Dist_Between)
            {
                return true;
            }
            else
                return false;
        }

        public static bool operator >(mylocation lhs, mylocation rhs)
        {
            if (lhs.Dist_Between > rhs.Dist_Between)
            {
                return true;
            }
            else
                return false;
        }

        public int CompareTo(mylocation other)
        {
            return Dist_Between.CompareTo(other.Dist_Between);
        }
    }
}