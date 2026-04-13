module dk.sdu.infrastructure.agv {
    requires dk.sdu.core;
    requires java.net.http;

    // ENCAPSULATION: only this package is visible; internal/ is NOT exported
    exports dk.sdu.infrastructure.agv;

    // EXPLICIT CONTRACTS: what this module provides
    provides dk.sdu.core.api.IAgvService
        with dk.sdu.infrastructure.agv.AgvService;
    provides dk.sdu.core.api.IMachineComponent
        with dk.sdu.infrastructure.agv.AgvService;
    provides dk.sdu.core.api.IComponentUIDescriptor
        with dk.sdu.infrastructure.agv.AgvUIDescriptor;
}
