package dk.sdu.orchestration.lifecycle;

import dk.sdu.core.lifecycle.MachineLifecycle;
import java.util.List;

// Spring SmartLifecycle: iterates all MachineLifecycle instances and drives initialize/shutdown
public class ComponentLifecycleManager {

    private final List<MachineLifecycle> components;

    public ComponentLifecycleManager(List<MachineLifecycle> components) {
        this.components = components;
    }

    public void startAll() {
        // TODO: Implement — call initialize() on all components in priority order
    }

    public void stopAll() {
        // TODO: Implement — call shutdown() on all components in reverse order
    }
}
