package dk.sdu.infrastructure.warehouse;

import dk.sdu.core.api.IMachineComponent;
import dk.sdu.core.api.IWarehouseService;
import dk.sdu.core.enums.MachineStatus;
import dk.sdu.core.enums.MachineType;
import dk.sdu.core.lifecycle.ComponentState;
import dk.sdu.core.lifecycle.MachineLifecycle;
import dk.sdu.core.metadata.ComponentMetadata;
import dk.sdu.core.model.CommandResult;
import dk.sdu.core.model.Inventory;
import dk.sdu.core.model.MachineStatusModel;

@ComponentMetadata(
    name = "Warehouse Service",
    protocol = "SOAP",
    machineType = MachineType.WAREHOUSE,
    description = "Automated warehouse (Effimat) for part storage and retrieval",
    icon = "warehouse",
    priority = 0
)
public class WarehouseService implements IWarehouseService, IMachineComponent, MachineLifecycle {

    // IWarehouseService
    @Override
    public CommandResult pickItem(String trayId) {
        // TODO: Implement
        return null;
    }

    @Override
    public CommandResult insertItem(String trayId, String name) {
        // TODO: Implement
        return null;
    }

    @Override
    public Inventory getWarehouseInventory() {
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
