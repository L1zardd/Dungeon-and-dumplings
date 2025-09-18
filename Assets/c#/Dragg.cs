using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoord;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;

        // �������� ���������� Z ������� � ������� ������������
        zCoord = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;

        // ��������� �������� ����� �������� ������� � �������� ���� � ������� �����������
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    // �������� ������� ���� � ������� ����������� � ������ ������� �������
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;

        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
