package dk.sdu.orchestration;

import dk.sdu.core.api.IAgvService;
import dk.sdu.core.api.IAssemblyService;
import dk.sdu.core.api.IProductionOrchestrator;
import dk.sdu.core.api.IWarehouseService;
import dk.sdu.core.model.ProductionStatus;

public class ProductionOrchestrator implements IProductionOrchestrator {

    // Discovered via ServiceLoader — see ServiceLoaderConfig in web module
    private final IAgvService agvService;
    private final IWarehouseService warehouseService;
    private final IAssemblyService assemblyService;

    public ProductionOrchestrator(IAgvService agvService,
                                   IWarehouseService warehouseService,
                                   IAssemblyService assemblyService) {
        this.agvService = agvService;
        this.warehouseService = warehouseService;
        this.assemblyService = assemblyService;
    }

    @Override
    public void runProductionCycle() {
        // TODO: Implement 16-step production sequence
    }

    @Override
    public void stopProductionCycle() {
        // TODO: Implement
    }

    @Override
    public ProductionStatus getStatus() {
        // TODO: Implement
        return null;
    }
}
