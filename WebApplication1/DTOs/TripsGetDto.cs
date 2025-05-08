namespace WebApplication1.DTOs;

public class TripsGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxPeople { get; set; }
    public List<TripsCountryDto> Countries { get; set; }
}