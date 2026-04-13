package dk.sdu.web.controller;

import dk.sdu.core.api.IMachineComponent;
import dk.sdu.core.metadata.ComponentMetadata;
import org.springframework.web.bind.annotation.*;
import java.util.*;

@RestController
@RequestMapping("/api/components")
public class ComponentDiscoveryController {

    private final ServiceLoader<IMachineComponent> components;

    public ComponentDiscoveryController(ServiceLoader<IMachineComponent> components) {
        this.components = components;
    }

    @GetMapping
    public List<Map<String, Object>> getAll() {
        // TODO: Implement — return metadata from ServiceLoader.stream()
        return null;
    }
}
