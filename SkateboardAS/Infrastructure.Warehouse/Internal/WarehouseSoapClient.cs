using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Core.Models;

namespace Infrastructure.Warehouse.Internal;

internal class WarehouseSoapClient
{
    private const string Endpoint = "http://localhost:8081/Service.asmx";
    private const string Namespace = "http://tempuri.org/";
    private readonly HttpClient _http = new();
    private readonly InventoryMapper _mapper = new();

    private async Task<XDocument> SendSoapAsync(string soapAction, string bodyContent, CancellationToken ct)
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

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var xml = await response.Content.ReadAsStringAsync(ct);
        return XDocument.Parse(xml);
    }

    public async Task VerifyConnectionAsync(CancellationToken ct = default)
    {
        await GetInventoryAsync(ct);
    }

    public async Task<CommandResult> PickItemAsync(string trayId, CancellationToken ct = default)
    {
        var body = $"""
            <tns:PickItem>
              <tns:trayId>{trayId}</tns:trayId>
            </tns:PickItem>
            """;

        var doc = await SendSoapAsync($"{Namespace}PickItem", body, ct);

        var resultText = doc.Descendants("PickItemResult").FirstOrDefault()?.Value ?? "false";
        return bool.TryParse(resultText, out var success) && success
            ? CommandResult.Ok()
            : CommandResult.Fail($"PickItem failed for tray {trayId}");
    }

    public async Task<CommandResult> InsertItemAsync(string trayId, string name, CancellationToken ct = default)
    {
        var body = $"""
            <tns:InsertItem>
              <tns:trayId>{trayId}</tns:trayId>
              <tns:name>{name}</tns:name>
            </tns:InsertItem>
            """;

        var doc = await SendSoapAsync($"{Namespace}InsertItem", body, ct);

        var resultText = doc.Descendants("InsertItemResult").FirstOrDefault()?.Value ?? "false";
        return bool.TryParse(resultText, out var success) && success
            ? CommandResult.Ok()
            : CommandResult.Fail($"InsertItem failed for tray {trayId}");
    }

    public async Task<Inventory> GetInventoryAsync(CancellationToken ct = default)
    {
        var body = "<tns:GetInventory />";
        var doc = await SendSoapAsync($"{Namespace}GetInventory", body, ct);
        return _mapper.Map(doc);
    }
}
