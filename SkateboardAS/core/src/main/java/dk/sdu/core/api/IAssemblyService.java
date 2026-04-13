package dk.sdu.core.api;

import dk.sdu.core.model.AssemblyStatus;
import dk.sdu.core.model.CommandResult;

public interface IAssemblyService {
    CommandResult startOperation(String processId);
    boolean checkHealth();
    AssemblyStatus getAssemblyStatus();
}
