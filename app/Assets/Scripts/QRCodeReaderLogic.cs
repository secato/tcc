using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using QRCodeReaderAndGenerator;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class QRCodeReaderLogic : MonoBehaviour
{

	[SerializeField]
	RawImage rawImage;

	[SerializeField]
	Text txtResult;

	GameObject previewObject;

	// Use this for initialization
	IEnumerator Start ()
	{
		yield return Application.RequestUserAuthorization (UserAuthorization.WebCam);

		if (StaticData.objeto) {
			PreviewModel ();
		} else {
			ScanQRCode ();
			//HandleOnQRCodeFound (ZXing.BarcodeFormat.AZTEC, "http://speedtest.ftp.otenet.gr/files/test1Gb.db");
		}

	}

	void OnEnable ()
	{
		// adicionando handlers
		QRCodeManager.onError += HandleOnError;
		QRCodeManager.onQrCodeFound += HandleOnQRCodeFound;
	}

	void HandleOnQRCodeFound (ZXing.BarcodeFormat barCodeType, string barCodeValue)
	{
		Debug.Log (barCodeType + " __ " + barCodeValue);
		txtResult.text = barCodeValue;
		StartCoroutine (AsyncDownloadModel (barCodeValue));
	}

	void HandleOnError (string err)
	{
		Debug.LogError (err);
	}

	public void ScanQRCode ()
	{
		if (previewObject) {
			rawImage.enabled = true;
			txtResult.text = "";
			Destroy (previewObject);
		}

		QRCodeManager.CameraSettings camSettings = new QRCodeManager.CameraSettings ();
		string rearCamName = GetRearCamName ();
		if (rearCamName != null) {
			camSettings.deviceName = rearCamName;
			camSettings.maintainAspectRatio = true;
			camSettings.scanType = ScanType.ONCE;
			QRCodeManager.Instance.ScanQRCode (camSettings, rawImage, 1f);
		}
		
	}

	// this function is require to call to stop scanning when camSettings.scanType = ScanType.CONTINUOUS;
	// no need to call when camSettings.scanType = ScanType.ONCE;
	public void StopScanning ()
	{
		QRCodeManager.Instance.StopScanning ();
	}

	string GetRearCamName ()
	{
		foreach (WebCamDevice device in WebCamTexture.devices) {
			if (!device.isFrontFacing) {
				
				return device.name;
			}
		}
		return null;
	}


	#region MEUS METODOS

	IEnumerator AsyncDownloadModel (string url)
	{
		UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.GetAssetBundle (url, 0);
		yield return request.Send ();

		//Debug.Log (request.downloadProgress);
//		while (!request.isDone) {
//			txtResult.text = "Realizando o download...";
//		}
		if (request.isError) {
			Debug.Log (request.error);
			txtResult.text = request.error;
		} else {

			while (!request.isDone) {
				txtResult.text = request.downloadProgress.ToString();
			}
			txtResult.text = "Download concluído...";
			AssetBundle bundle = DownloadHandlerAssetBundle.GetContent (request);


			StaticData.objeto = bundle.LoadAsset<GameObject> ("modelo");
			PreviewModel ();
			bundle.Unload (false);
		}
			
	}

	void PreviewModel ()
	{
		txtResult.text = "Pré-Visualização";
		rawImage.enabled = false;
		previewObject = StaticData.objeto;
		previewObject.transform.localScale = new Vector3 (25, 25, 25);
		previewObject = Instantiate (previewObject, new Vector3 (0, -100, 250), Quaternion.identity);

	}

	public void VoltarPrincipal ()
	{
		Initiate.Fade ("Main", Color.gray, 1.5f);
		//SceneManager.LoadScene ("Main");
	}

	void Update ()
	{

		if (previewObject) {
			if (Input.touchCount == 1) {
				Touch touch0 = Input.GetTouch (0);

				if (touch0.phase == TouchPhase.Moved) {
					previewObject.transform.Rotate (0f, touch0.deltaPosition.x, 0f);
				}
			}

			if (Input.touchCount == 2) {
				//txtResult.text = "2 toque";
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);

				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				if (deltaMagnitudeDiff > 1.5f || deltaMagnitudeDiff < -1.5f) {
					deltaMagnitudeDiff -= 1.5f;
					Vector3 newScale = previewObject.transform.localScale - new Vector3 (deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff);
					previewObject.transform.localScale = newScale;
				}
			}
		}
	}

	#endregion
}
