namespace SemesterProjekt4.SkateboardAS.Core.Interfaces;

public interface IConnect
    {
    public int MachineId { get; set; }
    
    public string MachineType { get; set; }

    public void AddMachine(int machineId, string machineType);

    public void RemoveMachine(int machineId);

    public Task ConnectMachine(int machineId);

    public void DisconnectMachine(int machineId);

    public bool IsConnected(int machineId);
    }