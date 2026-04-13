package dk.sdu.shared.entity;

import dk.sdu.core.enums.UserRole;
import jakarta.persistence.*;
import java.util.List;

@Entity
@Table(name = "app_users")
public class AppUser {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String username;
    private String passwordHash;

    @Enumerated(EnumType.STRING)
    private UserRole role;

    @ElementCollection
    private List<Long> assignedLineIds;

    // TODO: Implement getters/setters
}
