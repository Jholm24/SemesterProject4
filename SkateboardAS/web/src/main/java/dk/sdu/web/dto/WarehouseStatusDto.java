package dk.sdu.web.dto;

import java.util.List;
import java.util.Map;

public record WarehouseStatusDto(List<String> trays, Map<String, Boolean> slotOccupancy, String state) {}
