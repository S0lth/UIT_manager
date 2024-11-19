using System.Net.Http.Headers;
using System.Text;

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
    public ApiCommunicator(string apiUrl, HttpClient? httpClient = null) {
        _apiUrl = apiUrl;
        _httpClient = httpClient ?? new HttpClient();
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

            _httpClient.BaseAddress = new Uri("http://localhost:5014/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            StringContent jsonContent = new StringContent(machineInformation.ToJson(), Encoding.UTF8);

            HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl,jsonContent);

            if (response.IsSuccessStatusCode) {
                Console.WriteLine("Données envoyées avec succès.");
                return true;
            }
            else {
                Console.WriteLine($"Erreur HTTP : {response.StatusCode}");
                Console.WriteLine($"Message : {await response.Content.ReadAsStringAsync()}");
                return false;
            }
        }
        catch (HttpRequestException ex) {
            Console.WriteLine("Erreur réseau ou HTTP détectée :");
            Console.WriteLine($"Message : {ex.Message}");
            return false;
        }
        catch (Exception ex) {
            Console.WriteLine("Une erreur imprévue est survenue :");
            Console.WriteLine($"Message : {ex.Message}");
            return false;
        }
        finally {
            Console.WriteLine("Fin de l'opération d'envoi des données.");
        }
    }
}
