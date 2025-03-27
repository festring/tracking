using UnityEngine;
using UnityEngine.Video;
using Tobii.GameIntegration.Net;
using System.IO;
using System;

public class TobiiIntegrationExample : MonoBehaviour
{
    private bool _isApiInitialized = false;
    private GazePoint _gazePoint;
    public VideoPlayer videoPlayer;
    private StreamWriter _csvWriter;

    void Start()
    {
        InitializeTobiiApi();

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filePath = Path.Combine(Application.dataPath, $"{timestamp}.csv");
        _csvWriter = new StreamWriter(filePath, true);
        _csvWriter.WriteLine("Timestamp (Current Time), X, Y");
        Debug.Log($"CSV file initialized at path: {filePath}");

        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogWarning("VideoPlayer is not assigned.");
        }
    }

    private void InitializeTobiiApi()
    {
        try
        {
            TobiiGameIntegrationApi.SetApplicationName("TobiiEyeTracking");
            TobiiGameIntegrationApi.PrelinkAll();
            TobiiGameIntegrationApi.TrackTracker("tobii-prp://IS5FF-100203207464");//100212145514
            _isApiInitialized = true;
            Debug.Log("Tobii Game Integration API initialized successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception during Tobii API initialization: " + ex.Message);
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
        Debug.Log("Video is now playing.");
    }

    void Update()
    {
        if (_isApiInitialized && videoPlayer != null && videoPlayer.isPlaying)
        {
            TobiiGameIntegrationApi.Update();

            if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out _gazePoint))
            {
                LogGazeDataToCsv(_gazePoint);
            }
            else
            {
                Debug.Log($"Gaze Point - X: {_gazePoint.X}, Y: {_gazePoint.Y}, Time: {_gazePoint.TimeStampMicroSeconds}");
            }
        }
        else if (!_isApiInitialized)
        {
            Debug.LogWarning("Tobii API is not initialized.");
        }
        else if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            Debug.Log("Video is not playing; gaze data is not recorded.");
        }
    }

    private void LogGazeDataToCsv(GazePoint gazePoint)
    {
        if (_csvWriter != null)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss.ffff");
            string log = $"{formattedTime}, {gazePoint.X}, {gazePoint.Y}";
            _csvWriter.WriteLine(log);
        }
    }

    void OnDestroy()
    {
        if (_isApiInitialized)
        {
            TobiiGameIntegrationApi.Shutdown();
            Debug.Log("Tobii Game Integration API shut down.");
        }

        if (_csvWriter != null)
        {
            _csvWriter.Close();
            Debug.Log("Gaze data CSV file saved and closed.");
        }
    }
}
