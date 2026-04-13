package dk.sdu.core.model;

import dk.sdu.core.enums.MachineStatus;
import java.time.Instant;

public record MachineStatusModel(String id, String name, MachineStatus status, Instant timestamp) {}
