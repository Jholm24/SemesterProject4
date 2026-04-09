using System.Xml.Linq;
using Core.Models;

namespace Infrastructure.Warehouse.Internal;

internal class InventoryMapper
{
    private static readonly XNamespace Ns = "http://tempuri.org/";

    public Inventory Map(XDocument soapResponse)
    {
        var inventory = new Inventory();

        var body = soapResponse.Descendants(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/")).FirstOrDefault();
        if (body is null)
            return inventory;

        var result = body.Descendants(XName.Get("GetInventoryResult", Ns.NamespaceName)).FirstOrDefault()
                  ?? body.Descendants("GetInventoryResult").FirstOrDefault();

        if (result is null)
            return inventory;

        var items = result.Elements(XName.Get("InventoryItem", Ns.NamespaceName))
                          .Concat(result.Elements("InventoryItem"));

        foreach (var item in items)
        {
            var trayId = item.Element(XName.Get("TrayId", Ns.NamespaceName))?.Value
                      ?? item.Element("TrayId")?.Value
                      ?? string.Empty;

            var name = item.Element(XName.Get("Name", Ns.NamespaceName))?.Value
                    ?? item.Element("Name")?.Value
                    ?? string.Empty;

            var isOccupiedStr = item.Element(XName.Get("IsOccupied", Ns.NamespaceName))?.Value
                             ?? item.Element("IsOccupied")?.Value
                             ?? "false";

            inventory.Items.Add(new InventoryItem
            {
                TrayId = trayId,
                Name = name,
                IsOccupied = bool.TryParse(isOccupiedStr, out var occupied) && occupied
            });
        }

        return inventory;
    }
}
