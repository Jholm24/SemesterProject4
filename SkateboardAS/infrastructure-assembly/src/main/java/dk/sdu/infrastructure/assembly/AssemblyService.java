package dk.sdu.infrastructure.assembly;

import dk.sdu.core.api.IAssemblyService;
import dk.sdu.core.api.IMachineComponent;
import dk.sdu.core.enums.MachineStatus;
import dk.sdu.core.enums.MachineType;
import dk.sdu.core.lifecycle.ComponentState;
import dk.sdu.core.lifecycle.MachineLifecycle;
import dk.sdu.core.metadata.ComponentMetadata;
import dk.sdu.core.model.AssemblyStatus;
import dk.sdu.core.model.CommandResult;
import dk.sdu.core.model.MachineStatusModel;

@ComponentMetadata(
    name = "Assembly Station",
    protocol = "MQTT",
    machineType = MachineType.ASSEMBLY_STATION,
    description = "Assembly station (UR) for part assembly",
    icon = "cog",
    priority = 0
)
public class AssemblyService implements IAssemblyService, IMachineComponent, MachineLifecycle {

    // IAssemblyService
    @Override
    public CommandResult startOperation(String processId) {
        // TODO: Implement
        return null;
    }

    @Override
    public boolean checkHealth() {
        // TODO: Implement
        return false;
    }

    @Override
    public AssemblyStatus getAssemblyStatus() {
        // TODO: Implement
        return null;
    }

    // IMachineComponent
    @Override
    public String getId() {
        // TODO: Implement
        return null;
    }

    @Override
    public String getName() {
        // TODO: Implement
        return null;
    }

    @Override
    public MachineType getType() {
        // TODO: Implement
        return null;
    }

    @Override
    public MachineStatus getStatus() {
        // TODO: Implement
        return null;
    }

    @Override
    public CommandResult start() {
        // TODO: Implement
        return null;
    }

    @Override
    public CommandResult stop() {
        // TODO: Implement
        return null;
    }

    @Override
    public CommandResult reset() {
        // TODO: Implement
        return null;
    }

    @Override
    public MachineStatusModel getStatusDetails() {
        // TODO: Implement
        return null;
    }

    // MachineLifecycle
    @Override
    public ComponentState getState() {
        // TODO: Implement
        return null;
    }

    @Override
    public void initialize() throws Exception {
        // TODO: Implement
    }

    @Override
    public void shutdown() throws Exception {
        // TODO: Implement
    }
}
