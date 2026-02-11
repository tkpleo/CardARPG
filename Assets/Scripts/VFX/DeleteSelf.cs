using UnityEngine;

public class DeleteSelf : MonoBehaviour
{
    GameObject VFXToDelete;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VFXToDelete = this.gameObject;
        // Destroy with a 1 second delay
        Destroy(VFXToDelete, 1f);
    }

}
