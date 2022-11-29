using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField]
    private Structure[] structures;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject buildUI;

    [SerializeField]
    private Material greenMaterial;

    [SerializeField]
    private Material redMaterial;

    private bool inPlace;
    private ItemData currentStructureItem;
    private bool canBuild;
    private Vector3 finalPosition;

    private void FixedUpdate()
    {
        if (currentStructureItem == null)
        {
            return;
        }
        canBuild = GetStructureOfType(currentStructureItem).placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().CheckConnection();
        finalPosition = GetNearestPoint(transform.position);
        CheckPosition();
        UpdatePlacementStructureMaterial();
    }

    private void Update()
    {
        if (currentStructureItem == null)
        {
            if (buildUI.activeSelf)
            {
                buildUI.SetActive(false);
            }
            return;
        }
        if (!buildUI.activeSelf)
        {
            buildUI.SetActive(true);

        }
        if (Input.GetKey(KeyCode.A))
        {
            GetStructureOfType(currentStructureItem).placementPrefab.transform.Rotate(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            GetStructureOfType(currentStructureItem).placementPrefab.transform.Rotate(0, -1, 0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Inventory.Instance.AddItem(currentStructureItem);
            currentStructureItem = null;
            ChangeStructureItem(currentStructureItem);
            Inventory.Instance.OpenInventory();
        }
        if (Input.GetMouseButtonDown(0) && canBuild)
        {
            Instantiate(GetStructureOfType(currentStructureItem).instatiatedPrefab, GetStructureOfType(currentStructureItem).placementPrefab.transform.position, GetStructureOfType(currentStructureItem).placementPrefab.transform.rotation);
            currentStructureItem = null;
            ChangeStructureItem(currentStructureItem);

        }
    }


    void UpdatePlacementStructureMaterial()
    {
        MeshRenderer placementPrefabRenderer = GetStructureOfType(currentStructureItem).placementPrefab.GetComponent<CollisionDetectionEdge>().meshRenderer;

        if (canBuild)
        {
            placementPrefabRenderer.material = greenMaterial;
        }else
        {
            placementPrefabRenderer.material = redMaterial;
        }
    }

    void CheckPosition()
    {
        inPlace = GetStructureOfType(currentStructureItem).placementPrefab.transform.position == finalPosition;

        if (!inPlace)
        {
            SetPosition(finalPosition);
        }
    }

    void SetPosition(Vector3 targetPosition)
    {
        Transform placementPrefabTransform = GetStructureOfType(currentStructureItem).placementPrefab.transform;
        Vector3 positionVelocity = Vector3.zero;

        if (Vector3.Distance(placementPrefabTransform.position, targetPosition) > 10)
        {
            placementPrefabTransform.position = targetPosition;
            return;
        }

        Vector3 newTargetPosition = Vector3.SmoothDamp(placementPrefabTransform.position, targetPosition, ref positionVelocity, 0, 15000);
        placementPrefabTransform.position = newTargetPosition;
    }

    private Vector3 GetNearestPoint(Vector3 reference)
    {
        return grid.GetNearestPointOnGrid(reference);
    }
    //58:19
    public void ChangeStructureItem(ItemData newItemData)
    {

        currentStructureItem = newItemData;
        if (currentStructureItem == null)
        {
            foreach (var structure in structures)
            {
                structure.placementPrefab.SetActive(false);

            }
        }

        foreach (var structure in structures)
        {
            structure.placementPrefab.SetActive(structure.itemData == currentStructureItem);
        }
    }

    private Structure GetStructureOfType(ItemData StructureitemData)
    {
        return structures.Where(elem => elem.itemData == StructureitemData).FirstOrDefault();    
    }
}



[System.Serializable]
public class Structure
{
    public GameObject placementPrefab;
    public GameObject instatiatedPrefab;
    public ItemData itemData;
}

