using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    private Material _sphereMaterial;

    private Dictionary<int, bool> _selectedSpheres = new Dictionary<int, bool>();

    void Start()
    {
        var size = 1;
        var halfSize = (float)size / 2f;
        for (int z = 0; z <= size; z++) {
            for (int y = 0; y <= size; y++) {
                for (int x = 0; x <= size; x++) {
                    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    sphere.transform.position = new Vector3(x - halfSize, y - halfSize, z - halfSize);
                    sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

                    _selectedSpheres.Add(sphere.GetInstanceID(), false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f)) {
                Debug.Log($"HIT {hit.transform.position}");
            }
        }
    }

    
}
