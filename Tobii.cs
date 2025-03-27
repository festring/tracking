using UnityEngine;
using UnityEngine.UI;
using Tobii.GameIntegration.Net;

public class TobiiIntegrationExample : MonoBehaviour
{
    private bool _isApiInitialized = false;
    private GazePoint _gazePoint;
    public Image gazePointImage; // �ü� ����Ʈ�� �ð�ȭ�� Image UI ���

    void Start()
    {
        InitializeTobiiApi();
    }

    private void InitializeTobiiApi()
    {
        try
        {
            // Tobii API �ʱ�ȭ �� Ʈ��Ŀ ����
            TobiiGameIntegrationApi.SetApplicationName("YourGameName");
            TobiiGameIntegrationApi.PrelinkAll();
            TobiiGameIntegrationApi.TrackTracker("tobii-prp://IS5FF-100212145514"); //���� Ʈ��Ŀ url �Է�
            _isApiInitialized = true;
            Debug.Log("Tobii Game Integration API initialized successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception during API initialization: " + ex.Message);
        }
    }

    void Update()
    {
        if (_isApiInitialized)
        {
            TobiiGameIntegrationApi.Update();
            Debug.Log("API updated, trying to get latest gaze point...");

            // �ֽ� �ü� ��ǥ �����͸� �������� �õ�
            if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out _gazePoint))
            {
                Debug.Log("Gaze Point - X: " + _gazePoint.X + ", Y: " + _gazePoint.Y);
                UpdateGazePointVisualization(_gazePoint);
            }
            else
            {
                Debug.LogWarning("-");
            }
        }
        else
        {
            Debug.LogWarning("Tobii API is not initialized.");
        }
    }

    // �ü� ����Ʈ�� ȭ�鿡 �ð�ȭ
    private void UpdateGazePointVisualization(GazePoint gazePoint)
    {
        // �ü� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
        float normalizedX = (gazePoint.X + 1f) / 2f * Screen.width;
        float normalizedY = (gazePoint.Y + 1f) / 2f * Screen.height;
        Vector2 screenPoint = new Vector2(normalizedX, normalizedY);

        if (gazePointImage != null && gazePointImage.rectTransform.parent is RectTransform parentRect)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                screenPoint,
                null,
                out Vector2 localPoint))
            {
                gazePointImage.rectTransform.localPosition = localPoint;
            }
            else
            {
                Debug.LogWarning("Failed to convert screen point to local point.");
            }
        }
        else
        {
            Debug.LogWarning("Gaze point image is not assigned or its parent is not a RectTransform.");
        }
    }

    // API ���� ó��
    void OnDestroy()
    {
        if (_isApiInitialized)
        {
            TobiiGameIntegrationApi.Shutdown();
            Debug.Log("Tobii Game Integration API shut down.");
        }
    }
}
