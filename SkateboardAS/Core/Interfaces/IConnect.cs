namespace SemesterProjekt4.SkateboardAS.Core.Interfaces;

public interface IConnect
    {
    public Task<int> MachineId { get; set; }
    
    public Task<string> MachineType { get; set; }

    public Task AddMachine(int machineId, string machineType);

    public Task RemoveMachine(int machineId);

    public Task ConnectMachine(int machineId);

    public Task DisconnectMachine(int machineId);

    public Task<bool> IsConnected(int machineId);
    }