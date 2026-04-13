package dk.sdu.core.model;

import java.util.List;

public record ComponentCardModel(String name, String icon, List<String> statusFields, List<String> actions) {}
