package dk.sdu.shared.service;

import dk.sdu.shared.entity.AppUser;
import dk.sdu.shared.repository.UserRepository;

public class UserService {

    private final UserRepository repository;

    public UserService(UserRepository repository) {
        this.repository = repository;
    }

    public AppUser register(AppUser user) {
        // TODO: Implement
        return null;
    }

    public void assignToLine(Long userId, Long lineId) {
        // TODO: Implement
    }

    public void removeFromLine(Long userId, Long lineId) {
        // TODO: Implement
    }
}
