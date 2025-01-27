using FlightInfo;
using static System.Runtime.InteropServices.JavaScript.JSType;

Dictionary<string, Airline> airlines = new Dictionary<string, Airline>();
Dictionary<string, BoardingGate> boardingGates = new Dictionary<string, BoardingGate>();
Dictionary<string, Flight> flights = new Dictionary<string, Flight>();
Dictionary<string, string> gateAssignments = new Dictionary<string, string>();

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
sr.Close();

//2
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
    Airline airline = airlines[airlineCode];
    airline.AddFlight(f);
}

Terminal terminal = new Terminal("Terminal 5",airlines,flights,boardingGates);

//assignToAirline(flights,airlines);
//ListAllBoardingGates(boardingGates, gateAssignments);
while (true) {
    //assignboardingGateToflight(flights, boardingGates, gateAssignments);
    CreateNewFlight(flights);
}


//3
//assign flight to airline
void assignToAirline(Dictionary<string, Flight> flights,Dictionary<string,BoardingGate> boardingGates, Dictionary<string, Airline> airlines)
{
    Console.WriteLine($"{"Flight Number",-14} {"Airline Name",-20} {"Origin",-22} {"Destination",-18} {"Expected Departure/Arrival Time",-40}");

    foreach (var flight in flights.Values)
    {
        string airlineCode = flight.FlightNumber.Substring(0, 2);
        if (airlines.ContainsKey(airlineCode))
        {
            Console.WriteLine($"{flight.FlightNumber,-14} {airlines[airlineCode].Name,-20} {flight.Origin,-22} {flight.Destination,-18} {flight.ExpectedDateTime,-40}");
        }
        else
        {
            Console.WriteLine($"Error: Airline code {airlineCode} not found for flight {flight.FlightNumber}.");
        }
    }
}
  
//4
//displayBoardingGate
void ListAllBoardingGates(Dictionary<string, BoardingGate> boardingGates, Dictionary<string, string> gateAssignments)
{
    Console.WriteLine($"{"Gate Name",-10} {"Supports DDJB",-15} {"Supports CFFT",-15} {"Supports LWTT",-15}");

    foreach (var gate in boardingGates.Values)
    {
        string assignedFlight = gateAssignments.ContainsKey(gate.GateName) ? gateAssignments[gate.GateName] : "None";
        Console.WriteLine($"{gate.GateName,-10} {gate.SupportDDJB,-15} {gate.SupportCFFT,-15} {gate.SupportLWTT,-15}");
    }
}

//5
void assignboardingGateToflight(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates, Dictionary<string, string> gateAssignments)
{
    Console.Write("Enter Flight Number to assign a boarding gate: ");
    string flightNo = Console.ReadLine();

    if (!flights.ContainsKey(flightNo))
    {
        Console.WriteLine($"{flightNo} does not exist.");
        return;
    }

    Flight fl = flights[flightNo];

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
            continue;
        }

        if (gateAssignments.ContainsKey(boardingGateName))
        {
            Console.WriteLine($"Boarding Gate {boardingGateName} is already assigned to Flight {gateAssignments[boardingGateName]}. Please choose another gate.");
            continue;
        }
        BoardingGate gate = boardingGates[boardingGateName];
        Console.WriteLine($"Boarding Gate Information:\nName: {gate.GateName}\nSupports DDJB: {gate.SupportDDJB}\nSupports CFFT: {gate.SupportCFFT}\nSupports LWTT: {gate.SupportLWTT}");
        if (!checkSRC(gate, fl))
        {
            Console.WriteLine("Error: This gate does not support the flight's special request. Please choose another gate.");
            continue;
        }

        // If all checks pass, assign the gate
        gateAssignments[boardingGateName] = flightNo;
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
                case 3: default: fl.Status = "On Time"; break;
            }

        }
        Console.WriteLine($"Flight {fl.FlightNumber} updated successfully.");
        Console.WriteLine($"Flight Status: {fl.Status}");
        Console.WriteLine($"Assigned Gate: {boardingGateName}");
        
    
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
        string flightNum=Console.ReadLine();

        if (flights.ContainsKey(flightNum)) 
        {
            Console.WriteLine($"{flightNum} already exists.");
            continue;
        }

        Console.Write("Enter Origin: ");
        string origin=Console.ReadLine();

        Console.Write("Enter destination: ");
        string destination=Console.ReadLine();

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






