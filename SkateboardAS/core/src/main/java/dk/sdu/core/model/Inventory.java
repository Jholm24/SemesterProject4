package dk.sdu.core.model;

import java.util.List;
import java.util.Map;

public record Inventory(List<String> trays, Map<String, Boolean> slotOccupancy) {}
