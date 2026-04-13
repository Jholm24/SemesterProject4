package dk.sdu.core.model;

import dk.sdu.core.enums.MachineStatus;

public record AgvStatus(String position, int battery, String currentProgram, MachineStatus state) {}
