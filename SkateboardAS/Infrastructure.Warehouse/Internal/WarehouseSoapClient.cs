using System.Text;
using System.Xml.Linq;
using Core.Models;

namespace Infrastructure.Warehouse.Internal;

internal class WarehouseSoapClient
{
    private const string Endpoint = "http://localhost:8081/Service.asmx";
    private const string Namespace = "http://tempuri.org/";
    private readonly HttpClient _http = new();


    public async Task<XDocument> InitializeAsync(string soapAction, string bodyContent, CancellationToken ct = default)
    {
        var envelope = $"""
                        <?xml version="1.0" encoding="utf-8"?>
                        <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
                                       xmlns:tns="{Namespace}">
                          <soap:Body>
                            {bodyContent}
                          </soap:Body>
                        </soap:Envelope>
                        """;
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = new StringContent(envelope, Encoding.UTF8, "text/xml")
        };
        request.Headers.Add("SOAPAction", soapAction);
        
        var response = await _http.PostAsync(Endpoint, new StringContent(envelope), ct);
        
        var responseContent = await response.Content.ReadAsStringAsync(ct);
        return XDocument.Parse(responseContent);
    }

    public async Task VerifyConnectionAsync(CancellationToken ct = default)
    {
        
        
        await Task.CompletedTask;
    }

    public async Task<CommandResult> PickItemAsync(string trayId, CancellationToken ct = default)
    {
        var bodyContent = $"""
                    <tns:PickItem>
                      <tns:trayId>{trayId}</tns:trayId>
                    </tns:PickItem>
                    """;
        
        var soapAction = $"{Namespace}PickItem";
        
        // TODO: Implement - SOAP PickItem(trayId)
        
        await Task.CompletedTask;
        return CommandResult.Ok();
    }

    public async Task<CommandResult> InsertItemAsync(string trayId, string name, CancellationToken ct = default)
    {
        // TODO: Implement - SOAP InsertItem(trayId, name)
        await Task.CompletedTask;
        return CommandResult.Ok();
    }

    public async Task<Inventory> GetInventoryAsync(CancellationToken ct = default)
    {
        // TODO: Implement - SOAP GetInventory()
        await Task.CompletedTask;
        return new Inventory();
    }
}
