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

    ProductPositionData GetSlotPositionData();

    ProductPositionData GetHandPositionData();

    bool CanBeSlottedVertically();

    void ToggleWorldspaceUI(bool on);
}
