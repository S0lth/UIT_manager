using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace UITManagerAgent;

/// <summary>
/// This class communicates with a remote API by sending machine information via HTTP POST requests.
/// </summary>
public class ApiCommunicator {
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiCommunicator"/> class.
    /// </summary>
    public ApiCommunicator(string apiUrl,TokenResponse? token, HttpClient? httpClient = null) {
        _apiUrl = apiUrl;
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5014");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.value);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Sends machine information to the API endpoint asynchronously.
    /// </summary>
    /// <param name="machineInformation">The machine information to send.</param>
    /// <returns>
    /// A task representing the asynchronous operation. 
    /// Returns <c>true</c> if the request succeeds; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> SendMachineInformationAsync(MachineInformation machineInformation) {
        try {

            StringContent jsonContent = new StringContent(machineInformation.ToJson(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl,jsonContent);

            if (response.IsSuccessStatusCode) {
                Console.WriteLine("=> Machine's Information received successfully.");
                return true;
            }
            else {
                Console.WriteLine($"=> Machine's Information could not be received.\nERROR HTTP : {response.StatusCode} \n{await response.Content.ReadAsStringAsync()}");
                return false;
            }
        }
        catch (HttpRequestException ex) {
            Console.WriteLine($"=> ERROR network or HTTP detected : {ex.Message}");
            return false;
        }
        catch (Exception ex) {
            Console.WriteLine($"=> Unexpected ERROR : {ex.Message}");
            return false;
        }
        finally {
            Console.WriteLine("=> End of Machine Information operation");
        }
    }

    public static async Task<TokenResponse> generateTokenAsync() {
        string endpointUrl = "http://localhost:5014/api/v1.0/Auth";
        string user = "uitmanager";
        string password = "StrongerPassword!1";

        using (HttpClient httpClient = new HttpClient()) {
            var requestData = new { Name = user, Serial = password };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );
            
            try
            {
                // Envoyer la requête POST
                HttpResponseMessage response = await httpClient.PostAsync(endpointUrl, requestContent);

                // Vérifier si la requête a réussi
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    
                    // Désérialiser la réponse JSON
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody);
                    return tokenResponse;
                }
                else
                {
                    Console.WriteLine($"Erreur: {response.StatusCode}");
                    string errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Détails de l'erreur: {errorBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
        return null;
    }
}
