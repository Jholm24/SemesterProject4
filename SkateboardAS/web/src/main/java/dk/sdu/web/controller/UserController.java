package dk.sdu.web.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/users")
@PreAuthorize("hasRole('MANAGER')")
public class UserController {

    @GetMapping
    public ResponseEntity<?> getAll() {
        // TODO: Implement
        return null;
    }

    @PostMapping("/{userId}/assign/{lineId}")
    public ResponseEntity<Void> assignToLine(@PathVariable Long userId, @PathVariable Long lineId) {
        // TODO: Implement
        return null;
    }

    @DeleteMapping("/{userId}/assign/{lineId}")
    public ResponseEntity<Void> removeFromLine(@PathVariable Long userId, @PathVariable Long lineId) {
        // TODO: Implement
        return null;
    }
}
