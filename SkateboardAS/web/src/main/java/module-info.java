module dk.sdu.web {
    requires dk.sdu.core;
    requires dk.sdu.orchestration;
    requires dk.sdu.shared;

    requires spring.boot;
    requires spring.boot.autoconfigure;
    requires spring.web;
    requires spring.context;
    requires spring.security.core;
    requires spring.security.config;
    requires spring.security.web;
    requires spring.websocket;
    requires spring.messaging;

    // LATE BINDING (Pillar 4): web also discovers for the component discovery API
    uses dk.sdu.core.api.IMachineComponent;
    uses dk.sdu.core.api.IComponentUIDescriptor;

    // Spring needs reflection for DI, controllers, config
    opens dk.sdu.web to spring.core, spring.beans, spring.context;
    opens dk.sdu.web.controller to spring.web, spring.beans;
    opens dk.sdu.web.config to spring.beans, spring.context;
    opens dk.sdu.web.dto to com.fasterxml.jackson.databind;
}
