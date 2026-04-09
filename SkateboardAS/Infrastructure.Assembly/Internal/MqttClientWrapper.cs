using MQTTnet;
using MQTTnet.Client;
using Core.Models;

namespace Infrastructure.Assembly.Internal;

internal class MqttClientWrapper
{
    private const string BrokerHost = "localhost";
    private const int BrokerPort = 1883;
    private const string OperationTopic = "emulator/operation";
    private const string StatusTopic = "emulator/status";
    private const string HealthTopic = "emulator/checkhealth";

    private IMqttClient? _client;
    private readonly AssemblyStatusMapper _mapper = new();
    private TaskCompletionSource<bool>? _healthTcs;
    private TaskCompletionSource<AssemblyStatus>? _statusTcs;

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(BrokerHost, BrokerPort)
            .WithCleanSession()
            .Build();

        _client.ApplicationMessageReceivedAsync += HandleMessageAsync;

        await _client.ConnectAsync(options, ct);

        await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(StatusTopic)
            .WithTopicFilter(HealthTopic)
            .Build(), ct);
    }

    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        if (_client is { IsConnected: true })
        {
            await _client.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().Build(), ct);
        }
    }

    public async Task<CommandResult> PublishOperationAsync(string processId, CancellationToken ct = default)
    {
        if (_client is null || !_client.IsConnected)
            return CommandResult.Fail("MQTT client is not connected.");

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(OperationTopic)
            .WithPayload(processId)
            .Build();

        await _client.PublishAsync(message, ct);
        return CommandResult.Ok();
    }

    public async Task<bool> CheckHealthAsync(CancellationToken ct = default)
    {
        if (_client is null || !_client.IsConnected)
            return false;

        _healthTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(60));
        cts.Token.Register(() => _healthTcs?.TrySetResult(false));

        try
        {
            return await _healthTcs.Task;
        }
        finally
        {
            _healthTcs = null;
        }
    }

    public async Task<AssemblyStatus> GetStatusAsync(CancellationToken ct = default)
    {
        if (_client is null || !_client.IsConnected)
            return new AssemblyStatus();

        _statusTcs = new TaskCompletionSource<AssemblyStatus>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(10));
        cts.Token.Register(() => _statusTcs?.TrySetResult(new AssemblyStatus()));

        try
        {
            return await _statusTcs.Task;
        }
        finally
        {
            _statusTcs = null;
        }
    }

    private Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var topic = e.ApplicationMessage.Topic;
        var payload = e.ApplicationMessage.ConvertPayloadToString();

        if (topic == HealthTopic)
        {
            var isHealthy = string.Equals(payload, "true", StringComparison.OrdinalIgnoreCase);
            _healthTcs?.TrySetResult(isHealthy);
        }
        else if (topic == StatusTopic)
        {
            var status = _mapper.Map(payload);
            _statusTcs?.TrySetResult(status);
        }

        return Task.CompletedTask;
    }
}
