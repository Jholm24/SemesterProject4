package dk.sdu.web.dto;

import java.util.List;

public record ProductionStatusDto(String lineId, String overallState, List<String> componentStatuses) {}
