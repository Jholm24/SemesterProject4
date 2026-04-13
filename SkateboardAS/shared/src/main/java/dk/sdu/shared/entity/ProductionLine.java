package dk.sdu.shared.entity;

import jakarta.persistence.*;
import java.util.List;

@Entity
@Table(name = "production_lines")
public class ProductionLine {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String lineId;
    private String name;

    @ElementCollection
    private List<String> assignedComponentIds;

    private String status;

    // TODO: Implement getters/setters
}
