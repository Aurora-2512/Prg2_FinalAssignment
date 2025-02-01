using System.Collections.Immutable;
using System.Net.NetworkInformation;
using System.Numerics;
using FlightInfo;
using static System.Runtime.InteropServices.JavaScript.JSType;

Dictionary<string, Airline> airlines = new Dictionary<string, Airline>();
Dictionary<string, BoardingGate> boardingGates = new Dictionary<string, BoardingGate>();
Dictionary<string, Flight> flights = new Dictionary<string, Flight>();


//1
//Load Airline.csv
StreamReader sr = new StreamReader("airlines.csv");
sr.ReadLine();
while (!(sr.EndOfStream))
{
    string[] arr = sr.ReadLine().Split(',');
    airlines.Add(arr[1], new Airline(arr[0], arr[1]));
}
sr.Close();

//Load Boardinggate.csv
StreamReader bg = new StreamReader("boardinggates.csv");
bg.ReadLine();
while (!(bg.EndOfStream))
{
    string[] arr = bg.ReadLine().Split(',');
    boardingGates.Add(arr[0], new BoardingGate(arr[0], bool.Parse(arr[1]), bool.Parse(arr[2]), bool.Parse(arr[3])));
}
bg.Close();

//2
//load flights
StreamReader fl = new StreamReader("flights.csv");
fl.ReadLine();
while (!(fl.EndOfStream))
{
    string[] arr = fl.ReadLine().Split(',');
    Flight f;
    if (arr[4].Equals("DDJB"))
    {
        f = new DDJBFlight(arr[0], arr[1], arr[2], DateTime.Parse(arr[3]));
    }
    else if (arr[4].Equals("CFFT"))
    {
        f = new CFFTFlight(arr[0], arr[1], arr[2], DateTime.Parse(arr[3]));
    }
    else if (arr[4].Equals("LWTT"))
    {
        f = new LWTTFlight(arr[0], arr[1], arr[2], DateTime.Parse(arr[3]));
    }
    else
    {
        f = new NORMFlight(arr[0], arr[1], arr[2], DateTime.Parse(arr[3]));
    }
    flights.Add(f.FlightNumber, f);
    string airlineCode = arr[0].Substring(0, 2);
    if (airlines.ContainsKey(airlineCode))
    {
        airlines[airlineCode].AddFlight(f);
    }
}
fl.Close();

Terminal terminal = new Terminal("Terminal 5", airlines, flights, boardingGates);

while (true)
{
    string menu = @"=============================================
Welcome to Changi Airport Terminal 5
=============================================
1. List All Flights
2. List Boarding Gates
3. Assign a Boarding Gate to a Flight
4. Create Flight
5. Display Airline Flights
6. Modify Flight Details
7. Display Flight Schedule
8. Process Unassigned Flight to Gate 
0. Exit";

    try
    {
        Console.Write($"{menu}\nEnter your option: ");
        if (!int.TryParse(Console.ReadLine(), out int opt) || opt < 0 || opt > 8)
        {
            Console.WriteLine("Invalid input. Please enter a number between 0 and 8.");
            continue; // Loop again if input is invalid
        }
        switch (opt)
        {
            case 1:
                displayFlightInfo(); break;
            case 2:
                ListAllBoardingGates(boardingGates); break;
            case 3:
                assignboardingGateToflight(flights, boardingGates); break;
            case 4:
                CreateNewFlight(flights); break;
            case 5:
                displayAirlineFlight(airlines, flights); break;
            case 6:
                modifyFlightDetails(flights, airlines); break;
            case 7:
                displayscheduledFlight(flights); break;
            case 8:
                processingUnassignedFlight(flights, boardingGates); break;
            case 0:
                Console.WriteLine("Bye!!"); break;

        }
        if (opt == 0)
        { break; }
    }

    catch (FormatException ex)
    {
        Console.WriteLine("Invalid input. Please enter a valid numeric option.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }

}

void displayFlightInfo()
{
    Console.WriteLine($"{"Flight Number",-14} {"Airline Name",-20} {"Origin",-22} {"Destination",-18} {"Expected Departure/Arrival Time",-40}");

    foreach (var flight in flights.Values)
    {
        Airline airline = terminal.GetAirlineFromFlight(flight);
        string airlineName = airline != null ? airline.Name : "Unknown Airline";
        Console.WriteLine($"{flight.FlightNumber,-14} {airlineName,-20} {flight.Origin,-22} {flight.Destination,-18} {flight.ExpectedDateTime,-40}");

    }
}

//4
//displayBoardingGate
void ListAllBoardingGates(Dictionary<string, BoardingGate> boardingGates)
{
    Console.WriteLine($"{"Gate Name",-10} {"Supports DDJB",-15} {"Supports CFFT",-15} {"Supports LWTT",-15}");

    foreach (var gate in boardingGates.Values)
    {
        Console.WriteLine($"{gate.GateName,-10} {gate.SupportDDJB,-15} {gate.SupportCFFT,-15} {gate.SupportLWTT,-15}");
    }
}

//5
//assign boardinggate to flight
void assignboardingGateToflight(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
{
    Flight fl = null;
    while (true)
    {
        Console.Write("Enter Flight Number to assign a boarding gate: ");
        string flightNo = Console.ReadLine();

        if (!flights.ContainsKey(flightNo))
        {
            Console.WriteLine($"Error: Flight {flightNo} does not exist. Please try again.");
            break; // Prompt the user again for a valid flight number
        }

        fl = flights[flightNo];


        string currentGate = getBoardingGateFromFlight(fl);
        if (currentGate != "Unassigned")
        {
            Console.WriteLine($"Error: Flight {fl.FlightNumber} is already assigned to Gate {currentGate}. Reassignment is not allowed.");
            return; // Exit function to prevent reassignment
        }
        break; // Exit the loop once a valid flight is found
    }
    // Display basic flight information
    Console.WriteLine($"Flight Information: ");
    Console.WriteLine($"Flight Number: {fl.FlightNumber}");
    Console.WriteLine($"Origin: {fl.Origin}");
    Console.WriteLine($"Destination: {fl.Destination}");
    Console.WriteLine($"Expected Time: {fl.ExpectedDateTime}");
    string specialRequest = fl is DDJBFlight ? "DDJB" :
                            fl is CFFTFlight ? "CFFT" :
                            fl is LWTTFlight ? "LWTT" :
                            "None";
    Console.WriteLine($"Special Request: {specialRequest}");

    BoardingGate Gate;
    string boardingGateName;
    while (true)
    {
        Console.Write("Enter Boarding Gate Name: ");
        boardingGateName = Console.ReadLine();

        if (!boardingGates.ContainsKey(boardingGateName))
        {
            Console.WriteLine($"Boarding Gate {boardingGateName} does not exist.");
            break;
        }
        Gate = boardingGates[boardingGateName];

        if (Gate.F != null)
        {
            Console.WriteLine($"Boarding Gate {boardingGateName} is already assigned to Flight {Gate.F.FlightNumber}. Please choose another gate.");
            continue;
        }
        BoardingGate gate = boardingGates[boardingGateName];
        Console.WriteLine($"Boarding Gate Information:\nName: {gate.GateName}\nSupports DDJB: {gate.SupportDDJB}\nSupports CFFT: {gate.SupportCFFT}\nSupports LWTT: {gate.SupportLWTT}");
        if (!checkSRC(gate, fl))
        {
            Console.WriteLine("This gate does not support the flight's special request. Please choose another gate.");
            continue;
        }


        // If all checks pass, assign the gate
        gate.F = fl;
        Console.WriteLine($"Success: Flight {fl.FlightNumber} successfully assigned to Gate {boardingGateName}.");
        break;

    }

    Console.WriteLine($"Flight {fl.FlightNumber} successfully assigned to Gate {boardingGateName}.");

    Console.WriteLine("Would you like to update the status of the flight? (Y / N)");
    string updateStatus = Console.ReadLine().Trim().ToUpper();
    if (updateStatus == "Y")
    {
        Console.Write("1. Delayed\n2. Boarding\n3. On Time\nPlease select the new status of the flight: ");
        int statusNo = int.Parse(Console.ReadLine());
        switch (statusNo)
        {
            case 1: fl.Status = "Delayed"; break;
            case 2: fl.Status = "Boarding"; break;
            case 3: fl.Status = "On Time"; break;
        }
        
    }
    else
    {
        fl.Status = "On Time";
    }
    Console.WriteLine($"Flight {fl.FlightNumber} updated successfully.");

}

//check whether boarding gate support Special Request
bool checkSRC(BoardingGate bg, Flight f)
{
    if (f is NORMFlight)
    {
        return true;
    }
    else if (f is DDJBFlight)
    {
        if (bg.SupportDDJB)
            return true;
    }
    else if (f is LWTTFlight)
    {
        if (bg.SupportLWTT)
            return true;
    }
    else if (f is CFFTFlight)
    {
        if (bg.SupportCFFT)
            return true;
    }
    return false;
}

//6
//Create New Flight
void CreateNewFlight(Dictionary<string, Flight> flights)
{
    bool addAnotherFlight = true;

    while (addAnotherFlight)
    {
        Console.WriteLine("Create a New Flight");

        Console.Write("Enter Flight Number: ");
        string flightNum = Console.ReadLine();

        if (flights.ContainsKey(flightNum))
        {
            Console.WriteLine($"{flightNum} already exists.");
            continue;
        }

        Console.Write("Enter Origin: ");
        string origin = Console.ReadLine();

        Console.Write("Enter destination: ");
        string destination = Console.ReadLine();

        Console.Write("Enter Expected Departure/Arrival Time (HH:mm tt): ");
        DateTime expectedTime;

        while (!DateTime.TryParse(Console.ReadLine(), out expectedTime))
        {
            Console.Write("Invalid date format. Please enter again (HH:mm tt): ");
        }

        Console.Write("Would you like to add a Special Request Code? (Y/N): ");
        string specialRequestChoice = Console.ReadLine().Trim().ToUpper();
        string specialRequest = "None";


        if (specialRequestChoice == "Y")
        {
            Console.WriteLine("Available Special Request Codes:");
            Console.WriteLine("1. DDJB");
            Console.WriteLine("2. CFFT");
            Console.WriteLine("3. LWTT");
            Console.Write("Enter the corresponding number (1/2/3): ");

            string requestCodeChoice = Console.ReadLine();
            specialRequest = requestCodeChoice switch
            {
                "1" => "DDJB",
                "2" => "CFFT",
                "3" => "LWTT",
                _ => "None"
            };
        }

        Flight newFlight;
        switch (specialRequest)
        {
            case "DDJB":
                newFlight = new DDJBFlight(flightNum, origin, destination, expectedTime);
                break;
            case "CFFT":
                newFlight = new CFFTFlight(flightNum, origin, destination, expectedTime);
                break;
            case "LWTT":
                newFlight = new LWTTFlight(flightNum, origin, destination, expectedTime);
                break;
            default:
                newFlight = new NORMFlight(flightNum, origin, destination, expectedTime);
                break;
        }

        flights.Add(flightNum, newFlight);

        Airline airline = terminal.GetAirlineFromFlight(newFlight);
        if (airline != null)
        {
            airline.AddFlight(newFlight);
            Console.WriteLine($"Flight {flightNum} added under {airline.Name}.");
        }
        else
        {
            Console.WriteLine($"Error: Airline for flight {flightNum} not found.");
        }

        using (StreamWriter sw = new StreamWriter("flight.csv", append: true))
        {
            sw.WriteLine($"{flightNum},{origin},{destination},{expectedTime:HH:mm tt},{specialRequest}");
        }

        Console.WriteLine($"Flight {flightNum} successfully added.");

        // Prompt to add another flight
        Console.Write("Would you like to add another flight? (Y/N): ");
        string addAnotherChoice = Console.ReadLine().Trim().ToUpper();
        addAnotherFlight = addAnotherChoice == "Y";
    }
    Console.WriteLine("All new flights have been successfully added.");
}


//7 display airline Flight
void displayAirlineFlight(Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights)
{

    Airline selectedAirline = availableAirline(airlines);
    if (selectedAirline == null) return;

    Console.Write("Enter Flight Number to see details: ");
    string fNumber = Console.ReadLine();
    if (!selectedAirline.Flights.ContainsKey(fNumber))
    {
        Console.WriteLine($"Error: Flight {fNumber} not found.");
        return;
    }
    Flight selectedFlight = flights[fNumber];


    string specialRequest = selectedFlight is DDJBFlight ? "DDJB" :
                            selectedFlight is CFFTFlight ? "CFFT" :
                            selectedFlight is LWTTFlight ? "LWTT" : "None";

    string bg = getBoardingGateFromFlight(selectedFlight);

    Console.WriteLine("\nFlight Details:");
    Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
    Console.WriteLine($"Airline Name: {selectedAirline.Name}");
    Console.WriteLine($"Origin: {selectedFlight.Origin}");
    Console.WriteLine($"Destination: {selectedFlight.Destination}");
    Console.WriteLine($"Expected Time: {selectedFlight.ExpectedDateTime:yyyy-MM-dd hh:mm tt}");
    Console.WriteLine($"Special Request Code: {specialRequest}");
    Console.WriteLine($"Boarding Gate: {bg}");
}

//8 ModifyFLight

void modifyFlightDetails(Dictionary<string, Flight> flights, Dictionary<string, Airline> airlines)
{
    Airline selectedAirline = availableAirline(airlines);
    if (selectedAirline == null) return;

    Console.Write("\nEnter the Flight Number to Modify/Delete: ");
    string fNumber = Console.ReadLine();

    if (!selectedAirline.Flights.ContainsKey(fNumber))
    {
        Console.WriteLine("Flight number not found. Please try again.");
        return;
    }

    Flight selectedFlight = selectedAirline.Flights[fNumber];

    Console.WriteLine("\n[1] Modify Flight\n[2] Delete Flight");
    Console.Write("Choose an Option: ");
    string choice = Console.ReadLine();
    if (choice == "1")
    {
        modifyExistingFlight(selectedFlight, boardingGates);
    }
    else if (choice == "2")
    {
        deleteFlight(flights, boardingGates, fNumber);
    }
    else
    {
        Console.WriteLine("Invalid option! Returning to menu...");
    }
}


//9
//Sorting
void displayscheduledFlight(Dictionary<string, Flight> flights)
{
    var sortedFlights = flights.Values.OrderBy(f => f.ExpectedDateTime);
    Console.WriteLine($"{"Flight Number",-14} {"Airline Name",-20} {"Origin",-22} {"Destination",-18} {"Expected Departure/Arrival Time",-35} {"Status",-10} {"Boarding Gate",-15}");
    foreach (var flight in sortedFlights)
    {
        Airline airline = terminal.GetAirlineFromFlight(flight);
        string bg = getBoardingGateFromFlight(flight);
        Console.WriteLine($"{flight.FlightNumber,-14} {airline?.Name ?? "Unknown Airline",-20} {flight.Origin,-22} {flight.Destination,-18} {flight.ExpectedDateTime,-35} {flight.Status,-10} {getBoardingGateFromFlight(flight),-15}");
    }
}

string getBoardingGateFromFlight(Flight flight)
{
    string gateName = "";
    bool flag = false;
    foreach (var bg in boardingGates.Values)
    {
        if (bg.F != null)
        {
            if (bg.F.FlightNumber == flight.FlightNumber)
            {
                gateName = bg.GateName;
                flag = true;
                break;
            }
        }

    }
    if (!flag)
    {
        gateName = "Unassigned";
    }
    return gateName;
}


Airline availableAirline(Dictionary<string, Airline> airlines)
{
    Console.WriteLine("\n=========================");
    Console.WriteLine("  Available Airlines ");
    Console.WriteLine("=========================");
    Console.WriteLine($"{"Airline Code",-15} {"Airline Name",-30}");
    foreach (var airline in airlines.Values)
    {
        Console.WriteLine($"{airline.Code,-15} {airline.Name,-30}");
    }

    Console.Write("\nEnter the 2-Letter Airline Code: ");
    string airlinecode = Console.ReadLine().ToUpper();

    if (!airlines.ContainsKey(airlinecode))
    {
        Console.WriteLine("Error: Airline code not found. Please try again.");
        return null;
    }

    Airline selectedAirline = airlines[airlinecode];
    Console.WriteLine($"\nFlights for {selectedAirline.Name}:");
    Console.WriteLine($"{"Flight Number",-14} {"Origin",-22} {"Destination",-18}");
    foreach (var flight in selectedAirline.Flights.Values)
    {
        Console.WriteLine($"{flight.FlightNumber,-14} {flight.Origin,-22} {flight.Destination,-18}");
    }
    return selectedAirline;
}

void modifyExistingFlight(Flight flight, Dictionary<string, BoardingGate> boardingGate)
{
    Console.WriteLine("\nModify Flight Details:");
    Console.WriteLine("[1] Modify Basic Information\n[2] Modify Status\n[3] Modify Special Request\n[4] Modify Boarding Gate");
    int opt = int.Parse(Console.ReadLine());
    switch (opt)
    {
        case 1:
            {
                Console.Write("Enter new Origin: ");
                flight.Origin = Console.ReadLine();
                Console.Write("Enter new Destination: ");
                flight.Destination = Console.ReadLine();
                Console.Write("Enter New Expected Time(yyyy-MM-dd HH:mm): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime newTime))
                {
                    Console.WriteLine("Invalid Format. Try again!!!");
                }
                flight.ExpectedDateTime = newTime;

                break;
            }
        case 2:
            {
                Console.Write("1. Delayed\n2. Boarding\n3. On Time\nPlease select the new status of the flight: ");
                int statusNo = int.Parse(Console.ReadLine());
                switch (statusNo)
                {
                    case 1: flight.Status = "Delayed"; break;
                    case 2: flight.Status = "Boarding"; break;
                    case 3: default: flight.Status = "On Time"; break;
                }
                break;
            }
        case 3:
            {
                Console.WriteLine("Available Special Request Codes:\n1. DDJB\n2. CFFT\n3. LWTT\n4. None");
                Console.Write("Enter your choice: ");
                string spChoice = Console.ReadLine();
                flight = spChoice switch
                {
                    "1" => new DDJBFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedDateTime),
                    "2" => new CFFTFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedDateTime),
                    "3" => new LWTTFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedDateTime),
                    _ => new NORMFlight(flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedDateTime),
                };
                break;
            }

        case 4:
            {
                BoardingGate Gate;
                Console.Write("Enter Boarding Gate Name: ");
                string boardingGateName = Console.ReadLine();
                if (!boardingGates.ContainsKey(boardingGateName))
                {
                    Console.WriteLine($"Boarding Gate {boardingGateName} does not exist.");
                }
                Gate = boardingGates[boardingGateName];

                if (Gate.F != null)
                {
                    Console.WriteLine($"Boarding Gate {boardingGateName} is already assigned to Flight {Gate.F.FlightNumber}. Please choose another gate.");
                }
                break;
            }
        default:
            Console.WriteLine("Invalid option!.No changes made.");
            break;
    }
}

void deleteFlight(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates, string flightNumber)
{
    if (!flights.ContainsKey(flightNumber))
    {
        Console.WriteLine($"Error: Flight {flightNumber} not found.");
        return;
    }

    Flight flight = flights[flightNumber];
    Airline airline = terminal.GetAirlineFromFlight(flight);
    if (airline == null)
    {
        Console.WriteLine($"Error: Airline for flight {flightNumber} not found.");
        return;
    }

    Console.Write("\nAre you sure you want to delete this flight? (Y/N): ");
    string confirm = Console.ReadLine().ToUpper();

    if (confirm == "Y")
    {
        flights.Remove(flightNumber);

        // Remove the flight from the airline
        if (airline.Flights.ContainsKey(flightNumber))
        {
            airline.Flights.Remove(flightNumber);
        }

        // Check and remove the flight from boarding gate
        foreach (var gate in boardingGates.Values)
        {
            if (gate.F != null && gate.F.FlightNumber == flight.FlightNumber)
            {
                gate.F = null;
                Console.WriteLine($"Flight {flightNumber} removed from Boarding Gate {gate.GateName}.");
                break;
            }
        }

        Console.WriteLine($"Flight {flightNumber} has been successfully deleted.");
    }
    else
    {
        Console.WriteLine($"Flight {flightNumber} was not deleted.");
    }
}
void processingUnassignedFlight(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
{
    Queue<Flight> unassignedFlightQueue = new Queue<Flight>();
    foreach (var flight in flights.Values)
    {
        if (getBoardingGateFromFlight(flight) == "Unassigned")
        {
            unassignedFlightQueue.Enqueue(flight);
        }
    }
    Console.WriteLine($"Total number of unassigned flight: {unassignedFlightQueue.Count}");

    List<BoardingGate> unassignedGates = new List<BoardingGate>();
    foreach (var boardingGate in boardingGates.Values)
    {
        if (boardingGate.F == null)
        {
            unassignedGates.Add(boardingGate);
        }
    }

    Console.WriteLine($"Free boarding gate count is: {unassignedGates.Count}");

    int processedFlights = 0;
    int processedGates = 0;

    while (unassignedFlightQueue.Count > 0 && unassignedGates.Count > 0)
    {
        Flight flight = unassignedFlightQueue.Dequeue();
        BoardingGate assignedGate = null;

        string specialRequest = flight is DDJBFlight ? "DDJB" :
                                flight is CFFTFlight ? "CFFT" :
                                flight is LWTTFlight ? "LWTT" : "None";

        foreach (var gate in unassignedGates)
        {
            if ((specialRequest == "DDJB" && gate.SupportDDJB) ||
                (specialRequest == "CFFT" && gate.SupportCFFT) ||
                (specialRequest == "LWTT" && gate.SupportLWTT) ||
                (specialRequest == "None" && !gate.SupportLWTT && !gate.SupportDDJB && !gate.SupportCFFT))
            {
                assignedGate = gate;
                break;
            }
        }

        if (assignedGate == null)
        {
            foreach (var gate in unassignedGates)
            {
                if (gate.F == null)
                {
                    break;
                }
            }
        }

        if (assignedGate != null)
        {
            Console.WriteLine("Would you like to update the status of the flight? (Y / N)");
            string opt = Console.ReadLine();
            string flightStatus = "";
            if (opt.ToUpper() == "Y")
            {
                Console.WriteLine("1. Delayed\n2. Boarding\n3. On Time\nPlease select the new status of the flight:\n");
                int statusNo = int.Parse(Console.ReadLine());
                switch (statusNo)
                {
                    case 1: flightStatus = "Delayed"; break;
                    case 2: flightStatus = "Boarding"; break;
                    case 3: flightStatus = "On Time"; break;
                }
            }
            else
            {
                flightStatus = "On Time";
            }
            flight.Status = flightStatus;
            assignedGate.F = flight;
            processedFlights++;
            processedGates++;
            unassignedGates.Remove(assignedGate);
            Console.WriteLine($"Flight {flight.FlightNumber} assigned to Gate {assignedGate.GateName}");
        }


    }
    int totalFlights = flights.Count;
    int totalGates = boardingGates.Count;

    double percentageFlightProcessed = (processedFlights / (double)totalFlights) * 100;
    double percentageGateProcessed = (processedGates / (double)totalGates) * 100;

    Console.WriteLine($"Percentage of flight processed: {percentageFlightProcessed:F0}%");
    Console.WriteLine($"Percentage of flight processed: {percentageGateProcessed:F0}%");
    displayscheduledFlight(flights);
}