using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldable
{
    void PickUp();

    void Drop();

    bool PlaceIntoWorldSlot(ProductWorldSlot slot);

    void ToggleHighlight(bool on = true);

    ProductSize GetProductSize();

    GenericPositionData GetSlotPositionData();

    GenericPositionData GetHandPositionData();

    bool CanBeSlottedVertically();

    void ToggleWorldspaceUI(bool on);

    bool CanBeDropped();
}
