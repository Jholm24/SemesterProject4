package dk.sdu.core.api;

import dk.sdu.core.model.CommandResult;
import dk.sdu.core.model.Inventory;

public interface IWarehouseService {
    CommandResult pickItem(String trayId);
    CommandResult insertItem(String trayId, String name);
    Inventory getWarehouseInventory();
}
