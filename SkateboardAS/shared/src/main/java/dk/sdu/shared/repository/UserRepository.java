package dk.sdu.shared.repository;

import dk.sdu.shared.entity.AppUser;
import org.springframework.data.jpa.repository.JpaRepository;

public interface UserRepository extends JpaRepository<AppUser, Long> {
    // TODO: Add query methods
}
