namespace DefaultNamespace;

public interface IWareHouse
{
     //Methods 
     Task<string> PickItemAsync(int trayId);
     Task<string> InsertItemAsync(int trayId, string name);
     Task<string> GetInventoryAsync();
}