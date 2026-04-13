package dk.sdu.web.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/auth")
public class AuthController {

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody Object request) {
        // TODO: Implement — authenticate and issue JWT
        return null;
    }

    @PostMapping("/register")
    public ResponseEntity<?> register(@RequestBody Object request) {
        // TODO: Implement — register new user (MANAGER only)
        return null;
    }
}
