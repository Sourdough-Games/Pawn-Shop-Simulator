using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldable
{
    bool PlaceIntoWorldSlot(ProductWorldSlot slot);

    ProductSize GetProductSize();

    GenericPositionData GetSlotPositionData();

    bool CanBeSlottedVertically();

    void ToggleWorldspaceUI(bool on);

    bool CanBeDropped();
}
