package dk.sdu.shared.service;

import dk.sdu.shared.entity.ProductionLine;
import dk.sdu.shared.repository.ProductionLineRepository;
import java.util.List;

public class ProductionLineService {

    private final ProductionLineRepository repository;

    public ProductionLineService(ProductionLineRepository repository) {
        this.repository = repository;
    }

    public ProductionLine compose(ProductionLine line) {
        // TODO: Implement
        return null;
    }

    public void start(Long lineId) {
        // TODO: Implement
    }

    public void stop(Long lineId) {
        // TODO: Implement
    }

    public List<ProductionLine> findAll() {
        // TODO: Implement
        return null;
    }
}
