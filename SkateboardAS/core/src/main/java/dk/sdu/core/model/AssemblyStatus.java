package dk.sdu.core.model;

import dk.sdu.core.enums.MachineStatus;

public record AssemblyStatus(String operation, int progress, boolean health, MachineStatus state) {}
