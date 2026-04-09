namespace SemesterProjekt4.SkateboardAS.Core.Interfaces;
public interface IAssembly
{
    Task State { get; set; }
    Task<bool> IsHealthy { get; set; }
    Task<int> OperationId { get; set; }
    Task<int> LastOperationId { get; set; }
    
    
    // remember to change the data types in the class diagram if the 
    // following method datatypes is correct 
    
    Task<int> GetStatus();
    Task<bool> GetHealth();
    Task<int> GetOperation();
    Task<int> GetLastOperation();
}