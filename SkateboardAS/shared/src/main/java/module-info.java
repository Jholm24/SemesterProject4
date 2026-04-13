module dk.sdu.shared {
    requires dk.sdu.core;
    requires spring.data.jpa;
    requires jakarta.persistence;

    exports dk.sdu.shared.entity;
    exports dk.sdu.shared.repository;
    exports dk.sdu.shared.service;

    // JPA/Hibernate needs reflection access to entities
    opens dk.sdu.shared.entity to org.hibernate.orm.core, spring.core;
}
