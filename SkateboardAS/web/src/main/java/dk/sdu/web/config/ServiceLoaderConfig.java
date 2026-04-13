package dk.sdu.web.config;

import dk.sdu.core.api.IAgvService;
import dk.sdu.core.api.IAssemblyService;
import dk.sdu.core.api.IComponentUIDescriptor;
import dk.sdu.core.api.IMachineComponent;
import dk.sdu.core.api.IWarehouseService;
import dk.sdu.core.metadata.ComponentMetadata;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import java.util.List;
import java.util.Objects;
import java.util.ServiceLoader;

@Configuration
public class ServiceLoaderConfig {

    @Bean
    public ServiceLoader<IMachineComponent> machineComponents() {
        return ServiceLoader.load(IMachineComponent.class);
    }

    @Bean
    public List<ComponentMetadata> componentMetadataList() {
        return ServiceLoader.load(IMachineComponent.class)
            .stream()
            .map(p -> p.type().getAnnotation(ComponentMetadata.class))
            .filter(Objects::nonNull)
            .toList();
    }

    @Bean
    public IAgvService agvService() {
        return ServiceLoader.load(IAgvService.class).findFirst()
            .orElseThrow(() -> new IllegalStateException("AGV module not on module path"));
    }

    @Bean
    public IWarehouseService warehouseService() {
        return ServiceLoader.load(IWarehouseService.class).findFirst()
            .orElseThrow(() -> new IllegalStateException("Warehouse module not on module path"));
    }

    @Bean
    public IAssemblyService assemblyService() {
        return ServiceLoader.load(IAssemblyService.class).findFirst()
            .orElseThrow(() -> new IllegalStateException("Assembly module not on module path"));
    }
}
