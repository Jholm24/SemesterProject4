package dk.sdu.web.dto;

public record UserDto(Long id, String username, String role, java.util.List<Long> assignedLineIds) {}
