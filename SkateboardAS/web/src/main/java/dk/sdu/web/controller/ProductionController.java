package dk.sdu.web.controller;

import dk.sdu.core.api.IProductionOrchestrator;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/production")
public class ProductionController {

    private final IProductionOrchestrator orchestrator;

    public ProductionController(IProductionOrchestrator orchestrator) {
        this.orchestrator = orchestrator;
    }

    @PostMapping("/start")
    @PreAuthorize("hasAnyRole('OPERATOR', 'MANAGER')")
    public ResponseEntity<Void> start() {
        // TODO: Implement
        return ResponseEntity.ok().build();
    }

    @PostMapping("/stop")
    @PreAuthorize("hasAnyRole('OPERATOR', 'MANAGER')")
    public ResponseEntity<Void> stop() {
        // TODO: Implement
        return ResponseEntity.ok().build();
    }
}
