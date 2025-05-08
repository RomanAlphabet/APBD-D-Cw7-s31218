namespace WebApplication1.DTOs;

public class ClientTripsDto
{
    public int ClientId { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
    public int TripId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxPeople { get; set; }
}