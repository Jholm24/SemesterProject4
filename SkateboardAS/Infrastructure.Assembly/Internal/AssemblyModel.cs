namespace SemesterProjekt4.SkateboardAS.Infrastructure.Assembly;

public class AssemblyModel
{
    public string broker;
    public int machineId;
    private string clientID;
    private string topic;

    public AssemblyModel( int machineId)
    {
        this.machineId = machineId;
    }


    public int State;
    public bool IsHealthy;
    public int OperationId;
    public int LastOperationId;

    
}