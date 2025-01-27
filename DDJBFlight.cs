using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInfo
{
    internal class DDJBFlight:Flight
    {
        private double requestFee;

        public double RequestFee { get => requestFee; set => requestFee = value; }

        public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedDateTime) : base(flightNumber, origin, destination, expectedDateTime)
        {
            
        }

        public override double CalculateFees()
        {
            return base.CalculateFees()+300;
        }

        public override string? ToString()
        {
            return base.ToString() + $"Special Request Code: DDJB";
        }
    }
}
