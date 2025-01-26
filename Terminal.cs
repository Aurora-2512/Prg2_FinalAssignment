using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FlightInfo
{
    internal class Terminal
    {
        private string terminalName;
        private Dictionary<string, Airline> airlines;
        private Dictionary<string, Flight> flights;
        private Dictionary<string,BoardingGate> boardingGates;
        private Dictionary<string,double> gates;

        public Terminal(string terminalName, Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates, Dictionary<string, double> gates)
        {
            this.terminalName = terminalName;
            this.airlines = airlines;
            this.flights = flights;
            this.boardingGates = boardingGates;
            this.gates = gates;
        }

        public bool AddAirline(Airline airline)
        {
            if (airlines.ContainsKey(airline.Code))
            {
                Console.WriteLine($"Airline {airline.Code} already exists.");
                return false;
            }

            airlines[airline.Code] = airline;
            return true;
        }

        public bool AddBoardingGate(BoardingGate boardingGate) 
        {
            if (boardingGates.ContainsKey(boardingGate.GateName))
            {
                Console.WriteLine($"Boarding Gate {boardingGate.GateName} already exists.");
                return false;
            }

            boardingGates[boardingGate.GateName] = boardingGate;
            return true;
        }

        public Airline GetAirlineFromFlight(string fNumber)
        {
            if (flights.ContainsKey(fNumber))
            {
                foreach (var airline in airlines.Values)
                {
                    if (airline.Flights.ContainsKey(fNumber))
                    {
                        return airline;
                    }
                }
            }

            Console.WriteLine($"No airline found for Flight {fNumber}.");
            return null;
        }

        public void printAirlineFees()
        {
            foreach (var airline in airlines.Values)
            {
                double totalFees = airline.calculateFees();
                Console.WriteLine($"Airline {airline.Name} (Code: {airline.Code}) - Total Fees: ${totalFees}");
            }
        }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
