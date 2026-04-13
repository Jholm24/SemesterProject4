module dk.sdu.infrastructure.warehouse {
    requires dk.sdu.core;
    requires java.xml;
    requires jakarta.xml.ws;

    // ENCAPSULATION: only this package is visible; internal/ is NOT exported
    exports dk.sdu.infrastructure.warehouse;

    // EXPLICIT CONTRACTS: what this module provides
    provides dk.sdu.core.api.IWarehouseService
        with dk.sdu.infrastructure.warehouse.WarehouseService;
    provides dk.sdu.core.api.IMachineComponent
        with dk.sdu.infrastructure.warehouse.WarehouseService;
    provides dk.sdu.core.api.IComponentUIDescriptor
        with dk.sdu.infrastructure.warehouse.WarehouseUIDescriptor;
}
