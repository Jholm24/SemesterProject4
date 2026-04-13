package dk.sdu.core.event;

import dk.sdu.core.enums.MachineStatus;
import java.time.Instant;

public record MachineStatusChangedEvent(String id, MachineStatus newStatus, Instant timestamp) {}
