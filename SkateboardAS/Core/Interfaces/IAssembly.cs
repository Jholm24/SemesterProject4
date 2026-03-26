namespace SemesterProjekt4.SkateboardAS.Core.Interfaces;
public interface IAssembly
{
    int State { get; set; }
    bool IsHealthy { get; set; }
    int OperationId { get; set; }
    int LastOperationId { get; set; }
    
    
    // remember to change the data types in the class diagram if the 
    // following method datatypes is correct 
    
    int GetStatus();
    bool GetHealth();
    int GetOperation();
    int GetLastOperation();
}