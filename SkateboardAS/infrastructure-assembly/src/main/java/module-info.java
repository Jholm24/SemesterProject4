module dk.sdu.infrastructure.assembly {
    requires dk.sdu.core;
    requires com.hivemq.client.mqtt;

    // ENCAPSULATION: only this package is visible; internal/ is NOT exported
    exports dk.sdu.infrastructure.assembly;

    // EXPLICIT CONTRACTS: what this module provides
    provides dk.sdu.core.api.IAssemblyService
        with dk.sdu.infrastructure.assembly.AssemblyService;
    provides dk.sdu.core.api.IMachineComponent
        with dk.sdu.infrastructure.assembly.AssemblyService;
    provides dk.sdu.core.api.IComponentUIDescriptor
        with dk.sdu.infrastructure.assembly.AssemblyUIDescriptor;
}
