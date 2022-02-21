using System;
using TravelRepublic.FlightCodingTest;
class CodeTest
{
    static void Main(string[] args)
    {
        // Invoke the FlightBuilder.GetFlights method to get some test flights
        var flights = new FlightBuilder().GetFlights();

        // Apply filter rules.
        // depending on flags or args provided, we can apply different filters
        FilterOutDepartureTimeBeforeCurrentTime(ref flights);
        FilterOutArrivalTimeBeforeDepartureTime(ref flights);
        FilterOutFlightsSpentMoreThanTwoHoursOnTheGround(ref flights);

        Console.WriteLine($"The remaining flights are: {flights.Count()}");
    }

    // 1. Depart before the current date/time.
    //      This predicate is true when segment.DepartureDate < Datetime.Now
    static void FilterOutDepartureTimeBeforeCurrentTime(ref IList<Flight> flights)
    {
        flights = flights.Where(flight => flight.Segments.All(segment => segment.DepartureDate >= DateTime.Now)).ToList();
    }

    // 2. Have any segment with an arrival date before the departure date
    //      This predicate is true when segment.ArrivalDate < segment.DepartureDate
    static void FilterOutArrivalTimeBeforeDepartureTime(ref IList<Flight> flights)
    {
        flights = flights.Where(flight => flight.Segments.All(segment => segment.ArrivalDate >= segment.DepartureDate)).ToList();
    }

    // 3. Spend more than 2 hours on the ground – i.e. those with a total combined gap of over two hours between the
    // arrival date of one segment and the departure date of the next
    //      This predicate is true when segment1.ArrivalDate - segment2.DepartureDate > TimeSpan.FromHours(2)
    static void FilterOutFlightsSpentMoreThanTwoHoursOnTheGround(ref IList<Flight> flights)
    {
        flights = flights.Where((flight) =>
        {
            var prevArrivalDate = flight.Segments[0].ArrivalDate;
            for (int i = 1; i < flight.Segments.Count; i++)
            {
                var segment = flight.Segments[i];
                if (segment.DepartureDate - prevArrivalDate > TimeSpan.FromHours(2))
                    return false;
                prevArrivalDate = segment.ArrivalDate;
            }
            return true;
        }).ToList();
    }
}