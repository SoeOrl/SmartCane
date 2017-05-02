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
      
        public mylocation(string i) : base(i)
        {
            Provider = i;
        }

        public string Stop_Id { get; set; }
        
        public string Route_Id { get; set; }

        public string Last_Stop { get; set; }

        public double Dist_Between { get; set; }
      
        public string Train_Or_Not { get; set; }

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