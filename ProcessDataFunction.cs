using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Text.Json;

public class ProcessDataFunction
{
 [Function("ProcessDataFunction")]
 public async Task<HttpResponseData> Run(
 [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
 {
 // Read request body
 var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
 var data = JsonSerializer.Deserialize<IncomingData>(requestBody);

 // Use provided Azure SQL connection string
 var connectionString = "Server=tcp:st10497450severclvdpracticum.database.windows.net,1433;Initial Catalog=realtimeprossecing;Persist Security Info=False;User ID=st10497450;Password=Uefa2021;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

 using (var connection = new SqlConnection(connectionString))
 {
 await connection.OpenAsync();
 var command = new SqlCommand("INSERT INTO ProcessedData (Value, Timestamp) VALUES (@Value, @Timestamp)", connection);
 command.Parameters.AddWithValue("@Value", data.Value);
 command.Parameters.AddWithValue("@Timestamp", data.Timestamp);
 await command.ExecuteNonQueryAsync();
 }

 var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
 await response.WriteStringAsync("Data processed and stored successfully.");
 return response;
 }
}

public class IncomingData
{
 public string Value { get; set; }
 public string Timestamp { get; set; }
}
