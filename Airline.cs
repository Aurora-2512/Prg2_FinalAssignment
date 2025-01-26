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
            double fees = 0;
            foreach (var flight in Flights.Values)
            {
                fees += flight.CalculateFees();
            }
            return fees;
        }

        public override string? ToString()
        {
            return $"{Code,-15} {Name,-30}";
        }
    }
}
