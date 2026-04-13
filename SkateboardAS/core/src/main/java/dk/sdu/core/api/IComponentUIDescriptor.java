package dk.sdu.core.api;

import dk.sdu.core.model.ComponentCardModel;
import java.util.List;

public interface IComponentUIDescriptor {
    String getComponentType();
    ComponentCardModel getCardModel();
    List<String> getAvailableActions();
    List<String> getDisplayedStatusFields();
}
