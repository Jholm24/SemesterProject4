using Core.Models;

namespace Core.Interfaces;

public interface IComponentUIDescriptor
{
    string ComponentType { get; }
    ComponentCardModel GetCardModel();
    IEnumerable<string> GetAvailableActions();
    IEnumerable<string> GetDisplayedStatusFields();
}
