package dk.sdu.shared.entity;

import dk.sdu.core.enums.MachineType;
import jakarta.persistence.*;
import java.util.List;

@Entity
@Table(name = "formulas")
public class Formula {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;

    @ElementCollection
    @Enumerated(EnumType.STRING)
    private List<MachineType> requiredComponentTypes;

    // TODO: Implement getters/setters
}
