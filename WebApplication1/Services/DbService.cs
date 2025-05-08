using System.Data;
using Microsoft.Data.SqlClient;
using WebApplication1.DTOs;
using WebApplication1.Exceptions;

namespace WebApplication1.Services;

public interface IDbService
{
    public Task<IEnumerable<TripsGetDto>> GetTripsAsync();
    public Task<IEnumerable<ClientTripsDto>> GetClientTripsAsync(int id);
    public Task<int> CreateClientAsync(ClientCreateDto clientCreateDto);
}

public class DbService(IConfiguration conf) : IDbService
{
    private async Task<SqlConnection> GetConnectionAsync()
    {
        var connection = new SqlConnection(conf.GetConnectionString("default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    public async Task<IEnumerable<TripsGetDto>> GetTripsAsync()
    {
        var tripsDict = new Dictionary<int, TripsGetDto>();

        await using var connection = await GetConnectionAsync();

        var sql = """
                  select T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople, C.IdCountry, C.Name
                  from Trip T
                  join Country_Trip CT on T.IdTrip = CT.IdTrip
                  join Country C on CT.IdCountry = C.IdCountry;
                  """;

        await using var command = new SqlCommand(sql, connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var tripId = reader.GetInt32(0);

            if (!tripsDict.TryGetValue(tripId, out var tripDetails))
            {
                tripDetails = new TripsGetDto
                {
                    Id = tripId,
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    StartDate = reader.GetDateTime(3),
                    EndDate = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    Countries = []
                };

                tripsDict.Add(tripId, tripDetails);
            }

            if (!await reader.IsDBNullAsync(6))
            {
                tripDetails.Countries.Add(new TripsCountryDto()
                {
                    Id = reader.GetInt32(6),
                    Name = reader.GetString(7),
                });
            }
        }

        return tripsDict.Values;
    }

    public async Task<IEnumerable<ClientTripsDto>> GetClientTripsAsync(int id)
    {
        var tripsList = new List<ClientTripsDto>();

        await using var connection = await GetConnectionAsync();

        var sql = """
                  select T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople, ClT.IdClient, ClT.RegisteredAt, ClT.PaymentDate
                  from Trip T
                  join Client_Trip ClT on T.IdTrip = ClT.IdTrip
                  join Client Cl on ClT.IdClient = Cl.IdClient
                  where Cl.IdClient = @IdClient;
                  """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdClient", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            throw new NotFoundException($"Client id {id} not found");
        }

        do
        {
            tripsList.Add(new ClientTripsDto
            {
                ClientId = reader.GetInt32(6),
                RegisteredAt = reader.GetInt32(7),
                PaymentDate = reader.IsDBNull(8) ? null : reader.GetInt32(8),
                TripId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                StartDate = reader.GetDateTime(3),
                EndDate = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5)
            });
        } while (await reader.ReadAsync());

        if (tripsList.Count == 0)
        {
            throw new NoTripsException($"Client {id} has no trips");
        }

        return tripsList;
    }

    public async Task<int> CreateClientAsync(ClientCreateDto clientCreateDto)
    {
            await using var connection = await GetConnectionAsync();
            var sql = """
                      insert into Client (FirstName,LastName,Email,Telephone,Pesel)
                      output inserted.Id
                      values (@FirstName,@LastName,@Email,@Telephone,@Pesel);
                      """;

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@FirstName", clientCreateDto.FirstName);
            command.Parameters.AddWithValue("@LastName", clientCreateDto.LastName);
            command.Parameters.AddWithValue("@Email", clientCreateDto.Email);
            command.Parameters.AddWithValue("@Telephone", clientCreateDto.Telephone);
            command.Parameters.AddWithValue("@Pesel", clientCreateDto.Pesel);

            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
    }
}