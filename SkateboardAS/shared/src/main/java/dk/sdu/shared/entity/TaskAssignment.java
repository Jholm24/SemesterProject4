package dk.sdu.shared.entity;

import jakarta.persistence.*;

@Entity
@Table(name = "task_assignments")
public class TaskAssignment {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private Long operatorId;
    private Long productionLineId;

    // TODO: Implement getters/setters
}
