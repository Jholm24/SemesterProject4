using SemesterProjekt4.SkateboardAS.Core.Interfaces;
using MQTTnet; 
using MQTTnet.Client;

namespace SemesterProjekt4.SkateboardAS.Assembly.Internal;


public class AssemblyController : IConnect, IAssembly
{
    // IAssembly properties
    public int State { get; set; }
    public bool IsHealthy { get; set; }
    public int OperationId { get; set; }
    public int LastOperationId { get; set; }
    
    public string broker = "localhost";
    public int port = 1883;
    private string clientID = Guid.NewGuid().ToString(); 
    private string topic = "Csharp/Mqtt";
    

    private readonly IMqttClient _mqttClient = new MqttFactory().CreateMqttClient();

    public async Task ConnectMachine(int machineId)
    {


        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, port) 
            .WithClientId(clientID)
            .WithCleanSession()
            .Build();
        
        await _mqttClient.ConnectAsync(options);
        Console.WriteLine($"Connected to {broker}:{port}");
    }

    // IConnect properties
    public int MachineId { get; set; }
    public string MachineType { get; set; }

    public int GetStatus()      { return State; }
    public bool GetHealth()     { return IsHealthy; }
    public int GetOperation()   { return OperationId; }
    public int GetLastOperation() { return LastOperationId; }

    // IConnect methods
    public void AddMachine(int machineId, string machineType) { }
    public void RemoveMachine(int machineId) { }
    public void DisconnectMachine(int machineId) { }
    public bool IsConnected(int machineId) { return false; }
}