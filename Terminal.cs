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
        private Dictionary<string,double> gateFees;

        public Terminal(string terminalName, Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
        {
            TerminalName = terminalName;
            Airlines = airlines;
            Flights = flights;
            BoardingGates = boardingGates;
            this.gateFees = gateFees;
        }

        public string TerminalName { get => terminalName; set => terminalName = value; }
        internal Dictionary<string, Airline> Airlines { get => airlines; set => airlines = value; }
        internal Dictionary<string, Flight> Flights { get => flights; set => flights = value; }
        internal Dictionary<string, BoardingGate> BoardingGates { get => boardingGates; set => boardingGates = value; }

        public bool AddAirline(Airline airline)
        {
            if (Airlines.ContainsKey(airline.Code))
            {
                Console.WriteLine($"Airline {airline.Code} already exists.");
                return false;
            }

            Airlines[airline.Code] = airline;
            return true;
        }

        public bool AddBoardingGate(BoardingGate boardingGate) 
        {
            if (BoardingGates.ContainsKey(boardingGate.GateName))
            {
                Console.WriteLine($"Boarding Gate {boardingGate.GateName} already exists.");
                return false;
            }

            BoardingGates[boardingGate.GateName] = boardingGate;
            return true;
        }

        public Airline GetAirlineFromFlight(string fNumber)
        {
            if (Flights.ContainsKey(fNumber))
            {
                foreach (var airline in Airlines.Values)
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
            foreach (var airline in Airlines.Values)
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
