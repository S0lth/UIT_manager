using System.Net;
using System.Runtime.Versioning;
using Moq;
using Moq.Protected;

namespace UITManagerAgent.Tests;

/// <summary>
/// Unit tests for the <see cref="ApiCommunicator"/> class.
/// </summary>
[TestClass]
public class ApiCommunicatorTest {

    /// <summary>
    /// Tests the <see cref="ApiCommunicator.SendMachineInformationAsync"/> method.
    /// Ensures that it returns true when the API responds with a successful HTTP status code (200 OK).
    /// </summary>
    /// <returns>A task representing the asynchronous operation of this test.</returns>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public async Task SendMachineInformationAsync_ShouldReturnTrue_OnSuccess() {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"success\": true}")
            });

        var httpClient = new HttpClient(mockHandler.Object) {
            BaseAddress = new Uri("http://localhost:5014/")
        };

        TokenResponse tokenResponse= await ApiCommunicator.generateTokenAsync();
        var apiCommunicator = new ApiCommunicator("api/agent", tokenResponse,httpClient);

        var machineInfo = new MachineInformation();

        var result = await apiCommunicator.SendMachineInformationAsync(machineInfo);

        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests the <see cref="ApiCommunicator.SendMachineInformationAsync"/> method.
    /// Ensures that it returns false when the API responds with a server error (HTTP 500).
    /// </summary>
    /// <returns>A task representing the asynchronous operation of this test.</returns>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public async Task SendMachineInformationAsync_ShouldReturnFalse_OnServerError() {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal Server Error")
            });

        var httpClient = new HttpClient(mockHandler.Object) {
            BaseAddress = new Uri("http://localhost:5014/")
        };
        TokenResponse tokenResponse= await ApiCommunicator.generateTokenAsync();
        var apiCommunicator = new ApiCommunicator("api/agent", tokenResponse,httpClient);

        var machineInfo = new MachineInformation();

        var result = await apiCommunicator.SendMachineInformationAsync(machineInfo);

        Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests the <see cref="ApiCommunicator.SendMachineInformationAsync"/> method.
    /// Ensures that it returns false when a network error occurs, throwing an <see cref="HttpRequestException"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous operation of this test.</returns>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public async Task SendMachineInformationAsync_ShouldReturnFalse_OnHttpRequestException() {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHandler.Object) {
            BaseAddress = new Uri("http://localhost:5014/")
        };
        TokenResponse tokenResponse= await ApiCommunicator.generateTokenAsync();
        var apiCommunicator = new ApiCommunicator("api/agent", tokenResponse,httpClient);
        var machineInfo = new MachineInformation();

        var result = await apiCommunicator.SendMachineInformationAsync(machineInfo);

        Assert.IsFalse(result);
    }

    // <summary>
    /// Tests the <see cref="ApiCommunicator.SendMachineInformationAsync"/> method.
    /// Ensures that it returns false when a general exception is thrown.
    /// </summary>
    /// <returns>A task representing the asynchronous operation of this test.</returns>
    [TestMethod]
    [SupportedOSPlatform("windows")]
    public async Task SendMachineInformationAsync_ShouldReturnFalse_OnGeneralException() {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new Exception("Unexpected error"));

        var httpClient = new HttpClient(mockHandler.Object) {
            BaseAddress = new Uri("http://localhost:5014/")
        };
        TokenResponse tokenResponse= await ApiCommunicator.generateTokenAsync();
        var apiCommunicator = new ApiCommunicator("api/agent", tokenResponse,httpClient);

        var machineInfo = new MachineInformation();

        var result = await apiCommunicator.SendMachineInformationAsync(machineInfo);

        Assert.IsFalse(result);
    }
}
