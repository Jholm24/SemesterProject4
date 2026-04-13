package dk.sdu.core.model;

import dk.sdu.core.enums.MachineStatus;
import java.util.List;

public record ProductionStatus(String lineId, MachineStatus overallState, List<MachineStatusModel> componentStatuses) {}
