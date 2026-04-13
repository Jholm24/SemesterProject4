package dk.sdu.core.api;

import dk.sdu.core.model.ProductionStatus;

public interface IProductionOrchestrator {
    void runProductionCycle();
    void stopProductionCycle();
    ProductionStatus getStatus();
}
