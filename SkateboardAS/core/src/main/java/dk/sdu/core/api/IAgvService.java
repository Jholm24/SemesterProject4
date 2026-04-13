package dk.sdu.core.api;

import dk.sdu.core.model.AgvStatus;
import dk.sdu.core.model.CommandResult;

public interface IAgvService {
    CommandResult loadProgram(String programName);
    CommandResult executeProgram();
    AgvStatus getAgvStatus();
}
