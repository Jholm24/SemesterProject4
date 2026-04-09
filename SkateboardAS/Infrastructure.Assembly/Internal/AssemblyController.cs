using SemesterProjekt4.SkateboardAS.Core.Interfaces;
using MQTTnet; 
using MQTTnet.Client;
using System.Text;

namespace SemesterProjekt4.SkateboardAS.Assembly.Internal;


public class AssemblyController : IConnect, IAssembly
{
    // IAssembly properties
    public Task State { get; set; }
    public bool IsHealthy { get; set; }
    public int OperationId { get; set; }
    public int LastOperationId { get; set; }
    
    public string broker = "localhost";
    public int port = 1883;
    private string clientID = Guid.NewGuid().ToString(); 
    private string topic = "Csharp/Mqtt";
    

    private readonly IMqttClient _mqttClient = new MqttFactory().CreateMqttClient();
    public AssemblyController()
    {
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            switch (topic)
            {
                case "emulator/operation":
                    OperationId = int.Parse(payload);
                    Console.WriteLine($"Operation: {payload}");
                    break;
                case "emulator/status":
                    IsHealthy = payload == "healthy";
                    Console.WriteLine($"Status: {payload}");
                    break;
                case "emulator/checkhealth":
                    Console.WriteLine($"Health: {payload}");
                    break;
            }

            return Task.CompletedTask;
        };
    }
    
    
    // subscribes to the broker, and makes the connection 

    public async Task ConnectMachine(int machineId)
    {
        
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, machineId) 
            .WithClientId(clientID)
            .WithCleanSession()
            .Build();
        
        await _mqttClient.ConnectAsync(options);
        Console.WriteLine($"Connected to {broker}:{machineId}");
    }

    // IConnect properties
    public int MachineId { get; set; }
    public string MachineType { get; set; }

    
    // This reads the data from the different topics in the MQTT 
    public async Task GetStatus()
    {
        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic("emulator/operation")
            .Build());

        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic("emulator/status")
            .Build());

        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic("emulator/checkhealth")
            .Build());

        Console.WriteLine("Subscribing to emulator/operation, emulator/status, emulator/checkhealth");
    }
    
    // This sends a specific command to the MQTT
    
    public async Task SendCommand(string command)
    {
        await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
        .WithTopic($"assembly/{MachineId}/command")
        .WithPayload(command)
        .Build());
    }
    
    
    public bool GetHealth()     { return IsHealthy; }
    public int GetOperation()   { return OperationId; }
    public int GetLastOperation() { return LastOperationId; }

    // IConnect methods
    public void AddMachine(int machineId, string machineType) { }
    public void RemoveMachine(int machineId) { }
    public void DisconnectMachine(int machineId) { }
    public bool IsConnected(int machineId) { return false; }
    
}