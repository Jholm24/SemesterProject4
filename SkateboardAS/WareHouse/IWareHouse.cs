namespace DefaultNamespace;

public interface IWareHouse
{
     //Properties
     public int State { get; set;}
     public int TrayID {get; set;}
     
     //Methods 
     public void PickItem (int trayID)
     public void InsertItem(int trayID , string name)
     public void GetInventory();
     
}