module dk.sdu.orchestration {
    requires dk.sdu.core;

    // LATE BINDING (Pillar 4): declares intent to discover via ServiceLoader
    uses dk.sdu.core.api.IMachineComponent;
    uses dk.sdu.core.api.IAgvService;
    uses dk.sdu.core.api.IWarehouseService;
    uses dk.sdu.core.api.IAssemblyService;

    exports dk.sdu.orchestration;
    exports dk.sdu.orchestration.lifecycle;
}
