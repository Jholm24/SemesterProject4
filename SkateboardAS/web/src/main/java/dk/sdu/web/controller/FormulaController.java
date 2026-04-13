package dk.sdu.web.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/formulas")
public class FormulaController {

    @GetMapping
    public ResponseEntity<?> getAll() {
        // TODO: Implement
        return null;
    }

    @PostMapping
    public ResponseEntity<?> create(@RequestBody Object request) {
        // TODO: Implement
        return null;
    }

    @PutMapping("/{id}")
    public ResponseEntity<?> update(@PathVariable Long id, @RequestBody Object request) {
        // TODO: Implement
        return null;
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        // TODO: Implement
        return null;
    }
}
