package dk.sdu.core.api;

import dk.sdu.core.enums.MachineStatus;
import dk.sdu.core.enums.MachineType;
import dk.sdu.core.model.CommandResult;
import dk.sdu.core.model.MachineStatusModel;

public interface IMachineComponent {
    String getId();
    String getName();
    MachineType getType();
    MachineStatus getStatus();
    CommandResult start();
    CommandResult stop();
    CommandResult reset();
    MachineStatusModel getStatusDetails();
}
