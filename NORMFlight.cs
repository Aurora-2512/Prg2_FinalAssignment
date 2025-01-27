using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInfo
{
    internal class NORMFlight : Flight
    {
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedDateTime) : base(flightNumber, origin, destination, expectedDateTime)
        {
        }

        public double CalculateFees()
        {
            return base.CalculateFees();
        }

        public override string ToString() 
        {
            return base.ToString()+$"Special Request Code: None";
        }
    }
}
