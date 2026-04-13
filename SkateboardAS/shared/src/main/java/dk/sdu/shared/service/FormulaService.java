package dk.sdu.shared.service;

import dk.sdu.shared.entity.Formula;
import dk.sdu.shared.repository.FormulaRepository;
import java.util.List;

public class FormulaService {

    private final FormulaRepository repository;

    public FormulaService(FormulaRepository repository) {
        this.repository = repository;
    }

    public Formula create(Formula formula) {
        // TODO: Implement
        return null;
    }

    public Formula update(Long id, Formula formula) {
        // TODO: Implement
        return null;
    }

    public void delete(Long id) {
        // TODO: Implement
    }

    public List<Formula> findAll() {
        // TODO: Implement
        return null;
    }
}
