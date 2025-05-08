using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public class ClientCreateDto
{
    public int Id { get; set; }
    [Length(1,120)]
    public required string FirstName { get; set; }
    [Length(1,120)]
    public required string LastName { get; set; }
    [Length(6,120)]
    public required string Email { get; set; }
    [Length(9,9)]
    public required string Telephone { get; set; }
    [Length(11,11)]
    public required string Pesel { get; set; }
}