using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInfo
{
    internal class Flight: IComparable<Flight>
    {
        private string flightNumber;
        private string origin;
        private string destination;
        private DateTime expectedDateTime;
        private string status;

        public string FlightNumber { get => flightNumber; set => flightNumber = value; }
        public string Origin { get => origin; set => origin = value; }
        public string Destination { get => destination; set => destination = value; }
        public DateTime ExpectedDateTime { get => expectedDateTime; set => expectedDateTime = value; }
        public string Status { get => status; set => status = value; }

        public Flight()
        {
        }

        public Flight(string flightNumber, string origin, string destination, DateTime expectedDateTime)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedDateTime = expectedDateTime;
            Status = "Scheduled";
            
        }

        public virtual double CalculateFees()
        {
            double baseFee = 300;
            if (Origin == "Singapore (SIN)")
            {
                baseFee+= 500;
            }
            if (Destination == "Singapore (SIN)")
            {
                baseFee+= 800;
            }
            return baseFee;
        }

       
        public override string? ToString()
        {
            return $"Flight Number: {FlightNumber}\nOrigin: {Origin}\nDestination: {Destination}\nExpected Time: {ExpectedDateTime}\nStatus: {Status}\n";
        }

        public int CompareTo(Flight? other)
        {
            return expectedDateTime.CompareTo(other.expectedDateTime);
        }
    }
}
