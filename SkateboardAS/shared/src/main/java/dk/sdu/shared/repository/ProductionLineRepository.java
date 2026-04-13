package dk.sdu.shared.repository;

import dk.sdu.shared.entity.ProductionLine;
import org.springframework.data.jpa.repository.JpaRepository;

public interface ProductionLineRepository extends JpaRepository<ProductionLine, Long> {
    // TODO: Add query methods
}
