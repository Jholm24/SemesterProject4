package dk.sdu.web.dto;

public record AgvStatusDto(String position, int battery, String currentProgram, String state) {}
