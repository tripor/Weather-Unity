using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerController : MonoBehaviour
{

    public VideoPlayer videoPlayer2D;
    public VideoPlayer videoPlayer3D;
    public GameObject CubeVideoPlayer;
    public Button startButton;
    public TextMeshProUGUI startButtonText;
    public RawImage staticImage;
    public GameObject loadingText;
    public GameObject errorText;
    public TMP_InputField videoInput;
    public TMP_InputField imageInput;

    [SerializeField]
    private float speedRotation = 1f;
    private string defaultVideoLink = "https://file-examples-com.github.io/uploads/2017/04/file_example_MP4_480_1_5MG.mp4";
    private string defaultImageLink = "https://images.squarespace-cdn.com/content/v1/56d8ba4ab654f9a47f6d39fa/1480447528456-YVOR69SOKYNTMBV9WXMZ/glartek_logo.png";

    private bool playingVideo;
    private int loading;

    private string videoLink;
    private string imageLink;


    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        videoPlayer2D.errorReceived += VideoPlayerError;
        videoPlayer3D.errorReceived += VideoPlayerError;
        videoPlayer2D.prepareCompleted += VideoPrepared;
        videoPlayer3D.prepareCompleted += VideoPrepared;
        playingVideo = false;
        videoPlayer2D.Stop();
        videoPlayer3D.Stop();
        loadingText.SetActive(false);
        errorText.SetActive(false);
        videoLink = "";
        imageLink = "";
        startButton.interactable = true;
        loading = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playingVideo)
        {
            CubeVideoPlayer.transform.rotation *= Quaternion.Euler(Time.deltaTime * speedRotation, Time.deltaTime * speedRotation, Time.deltaTime * speedRotation * 2);
        }
    }

    public void StartVideoPlayer()
    {
        if (playingVideo)
        {
            errorText.SetActive(false);
            loadingText.SetActive(false);
            startButtonText.text = "Start";
            playingVideo = false;
            videoPlayer2D.Stop();
            videoPlayer3D.Stop();
        }
        else
        {
            startButtonText.text = "Stop";
            if (string.IsNullOrWhiteSpace(videoLink))
            {
                videoLink = defaultVideoLink;
            }
            if (string.IsNullOrWhiteSpace(imageLink))
            {
                imageLink = defaultImageLink;
            }
            videoPlayer2D.url = videoLink;
            videoPlayer3D.url = videoLink;
            videoPlayer2D.Prepare();
            videoPlayer3D.Prepare();
            loadingText.SetActive(true);
            startButton.interactable = false;
            videoInput.interactable = false;
            imageInput.interactable = false;
            StartCoroutine(GetImage(imageLink));
        }

    }

    public void VideoLinkChanged(string value)
    {
        videoLink = value;
    }

    public void ImageLinkChanged(string value)
    {
        imageLink = value;
    }

    private IEnumerator GetImage(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                errorText.GetComponent<TextMeshProUGUI>().text = "Error: could not load image";
                errorText.SetActive(true);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                staticImage.texture = texture;
            }
        }
        CheckIfLoading();
    }
    private void VideoPlayerError(VideoPlayer source, string message)
    {
        errorText.GetComponent<TextMeshProUGUI>().text = "Error: could not load video";
        errorText.SetActive(true);
    }

    private void VideoPrepared(VideoPlayer source)
    {
        CheckIfLoading();
    }
    private void OnSceneUnloaded(Scene current)
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        videoPlayer2D.errorReceived -= VideoPlayerError;
        videoPlayer3D.errorReceived -= VideoPlayerError;
        videoPlayer2D.prepareCompleted -= VideoPrepared;
        videoPlayer3D.prepareCompleted -= VideoPrepared;
    }
    private void CheckIfLoading()
    {
        loading++;
        if (loading >= 3)
        {
            loading = 0;
            playingVideo = true;
            loadingText.SetActive(false);
            videoPlayer2D.Play();
            videoPlayer3D.Play();
            startButton.interactable = true;
            videoInput.interactable = true;
            imageInput.interactable = true;
        }
    }
}
