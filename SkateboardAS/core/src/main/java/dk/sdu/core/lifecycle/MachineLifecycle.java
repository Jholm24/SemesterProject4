package dk.sdu.core.lifecycle;

public interface MachineLifecycle {
    ComponentState getState();
    // Named initialize/shutdown to avoid clash with IMachineComponent.start()/stop() (CommandResult return type)
    void initialize() throws Exception;
    void shutdown() throws Exception;
}
