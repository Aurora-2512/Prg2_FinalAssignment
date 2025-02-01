using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInfo
{
    internal class Airline
    {
        private string name;
        private string code;
        private Dictionary<string, Flight> flights;

        public string Name { get => name; set => name = value; }
        public string Code { get => code; set => code = value; }
        internal Dictionary<string, Flight> Flights { get => flights; set => flights = value; }

        public Airline(string name, string code)
        {
            this.Name = name;
            this.Code = code;
            flights = new Dictionary<string, Flight>();
        }

        public bool AddFlight(Flight f)
        {
            if (Flights.ContainsKey(f.FlightNumber))
            {
                Console.WriteLine($"Flight {f.FlightNumber} already exists for airline {Code}.");
                return false;
            }
            Flights[f.FlightNumber] = f;
            return true;

        }

        public bool RemoveFlight(string f)
        {
            if (!Flights.ContainsKey(f))
            {
                Console.WriteLine($"Flight {f} does not exist for airline {Code}.");
                return false;
            }
            Flights.Remove(f);
            return true;
        }

        public double calculateFees()
        {
            double totalFee = 0;
            int totalFlights = Flights.Count;
            int discountFlights = 0;
            int earlyLateFlights = 0;
            int originDiscountFlights = 0;
            int normalFlights = 0;

            foreach (var flight in Flights.Values)
            {
                totalFee += flight.CalculateFees();

                if (flight.ExpectedDateTime.Hour < 11 || flight.ExpectedDateTime.Hour >= 21)
                    earlyLateFlights++;

               
                if (flight.Origin== "Dubai (DXB)" || flight.Origin == "Bangkok (BKK)" || flight.Origin == "Tokyo (NRT)")
                    originDiscountFlights++;

                if (flight is NORMFlight)
                    normalFlights++;
            }

            discountFlights = totalFlights / 3;
            double discount = (discountFlights * 350) + (earlyLateFlights * 110) + (originDiscountFlights * 25) + (normalFlights * 50);

            if (totalFlights > 5)
            {
                discount += totalFee * 0.03;
            }

            return totalFee - discount;
        }

        public override string? ToString()
        {
            return $"{Code,-15} {Name,-30}";
        }
    }
}
