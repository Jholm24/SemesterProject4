package dk.sdu.web.dto;

public record AssemblyStatusDto(String operation, int progress, boolean health, String state) {}
