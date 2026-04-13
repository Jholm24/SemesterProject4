package dk.sdu.core.event;

import dk.sdu.core.metadata.ComponentMetadata;

public record ComponentLoadedEvent(String moduleId, ComponentMetadata metadata) {}
