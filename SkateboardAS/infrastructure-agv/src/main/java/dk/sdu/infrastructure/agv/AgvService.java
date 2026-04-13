package dk.sdu.infrastructure.agv;

import dk.sdu.core.api.IAgvService;
import dk.sdu.core.api.IMachineComponent;
import dk.sdu.core.enums.MachineStatus;
import dk.sdu.core.enums.MachineType;
import dk.sdu.core.lifecycle.ComponentState;
import dk.sdu.core.lifecycle.MachineLifecycle;
import dk.sdu.core.metadata.ComponentMetadata;
import dk.sdu.core.model.AgvStatus;
import dk.sdu.core.model.CommandResult;
import dk.sdu.core.model.MachineStatusModel;

@ComponentMetadata(
    name = "AGV Service",
    protocol = "REST",
    machineType = MachineType.AGV,
    description = "Automated Guided Vehicle for part transport",
    icon = "truck",
    priority = 0
)
public class AgvService implements IAgvService, IMachineComponent, MachineLifecycle {

    // IAgvService
    @Override
    public CommandResult loadProgram(String programName) {
        // TODO: Implement
        return null;
    }

    @Override
    public CommandResult executeProgram() {
        // TODO: Implement
        return null;
    }

    @Override
    public AgvStatus getAgvStatus() {
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
