using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager {

    public static MouseManager Instance = new MouseManager();

    private ItemObject heldPart;

    private MouseManager() {
        heldPart = null;
    }

    public ItemObject GetPart() {
        return heldPart;
    }

    public bool SetPart(ItemObject part) {
        if ((heldPart != null) && (part != null) && (heldPart != part)) return false;

        heldPart = part;

        return true;
    }

}
